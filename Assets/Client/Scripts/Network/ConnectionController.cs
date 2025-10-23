
using Framework;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

namespace Network
{
    public class ConnectionController
    {
        NetworkProvider _provider;
        ushort _port;

        public ConnectionController(NetworkProvider provider, ushort port)
        {
            _provider = provider;
            _port = port;
        }


        #region Server (サーバー)

        /// <summary>
        /// サーバーを起動
        /// </summary>
        [ServerRpc]
        public void StartAsServer()
        {
            var transport = _provider.Transport;

            /*--- 基本設定 ---*/
            // ホスト
            transport.ConnectionData.ServerListenAddress = "0.0.0.0";
            transport.ConnectionData.Port = _port;

            bool isSuccess = NetworkManager.Singleton.StartHost();
            if (!isSuccess)
            {
                DebugEx.LogError("ホストの起動に失敗しました");
                return;
            }
        }

        [ServerRpc]
        public bool TryRegister(ClientID id)
        {
            if (_provider.ClientContainer.Contains(id))
            {
                DebugEx.LogError($"クライアントが接続を要求しましたが失敗しました　ID: {id}");
                return false;
            }

            bool isMemberMax = _provider.ClientContainer.Count >= NetworkManagerEx.Instance.MemberMax;
            if (isMemberMax)
            {
                DebugEx.LogError($"クライアントの接続人数が最大数を超えました　最大人数: {isMemberMax}");
                return false;
            }

            _provider.ClientContainer.Register(id);
            DebugEx.Log($"クライアントが接続しました　ID: {id}");
            return true;
        }

        #endregion


        #region Client
        /// <summary>
        /// クライアントの起動
        /// </summary>
        [ClientRpc]
        public void StartAsClient()
        {
            var transport = _provider.Transport;

            /*--- 基本設定 ---*/
            // クライアント
            var ip = NetworkManagerEx.Instance.Ip;
            var port = NetworkManagerEx.Instance.Port;
            transport.SetConnectionData(ip, port);

            bool isSuccess = NetworkManager.Singleton.StartClient();
            if (!isSuccess)
            {
                DebugEx.LogError($"サーバーへの接続に失敗しました");
                return;
            }


            var id = new ClientID(NetworkManager.Singleton.LocalClientId);
            if (!TryRegister(id))
            {
                NetworkManager.Singleton.Shutdown();
            }
        }

        #endregion
    }
}
