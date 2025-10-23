
using Framework;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEditor.PackageManager;

namespace Network
{
    public class ConnectionController 
    {
        NetworkController _controller = null;
        UnityTransport _transport = null;
        ClientContainer _clientContainer = null;


        public ConnectionController(NetworkController controller, UnityTransport transport, ClientContainer clientContainer)
        {
            _controller = controller;
            _transport = transport;
            _clientContainer = clientContainer;
        }


        #region Server (サーバー)

        /// <summary>
        /// サーバーを起動
        /// </summary>
        /// <returns>
        /// true: 起動成功<br/>
        /// false: 起動失敗
        /// </returns>
        public bool StartAsServer()
        {
            /*--- 基本設定 ---*/
            // ホスト
            _transport.ConnectionData.ServerListenAddress = "0.0.0.0";
            _transport.ConnectionData.Port = _controller.Port;

            bool isSuccess = NetworkManager.Singleton.StartHost();
            if (!isSuccess)
            {
                DebugEx.LogError("サーバーの起動に失敗しました");
                return false;
            }

            NetworkManager.Singleton.OnServerStarted += _OnStartAsServer;
            NetworkManager.Singleton.OnServerStopped += _OnStopAsServer;
            DebugEx.Log($"サーバーを起動しました");
            return true;
        }

        /// <summary>
        /// [サーバー] クライアントを切断
        /// </summary>
        /// <param name="id">切断するクライアントID</param>
        /// <returns>
        /// true: 切断成功<br/>
        /// false: 切断失敗
        /// </returns>
        public bool StopAsClient(ClientID id)
        {
            if (!NetworkManager.Singleton.IsServer) return false; // サーバーかチェック

            if (!_clientContainer.Contains(id))
            {
                DebugEx.LogError($"クライアントが切断を要求しましたが、ID が登録されていないため、失敗しました　ID: {id}");
                return false;
            }

            NetworkManager.Singleton.DisconnectClient(id.Value); // 接続をきる
            DebugEx.Log($"クライアントを切断しました　ID: {id}");
            return true;
        }

        /// <summary>
        /// [サーバー] サーバーを停止
        /// </summary>
        /// <returns>
        /// true: 停止成功<br/>
        /// false: 停止失敗
        /// </returns>
        public bool StopAsServer()
        {
            if (!NetworkManager.Singleton.IsServer) return false; // サーバーかチェック

            NetworkManager.Singleton.Shutdown(); // 接続をきる
            _clientContainer.Clear();
            DebugEx.Log($"サーバーを停止しました");
            return true;
        }

        /// <summary>
        /// クライントの接続情報をサーバーに記録する
        /// </summary>
        /// <remarks>
        /// [コール] クライアントがサーバーに接続した際
        /// </remarks>
        /// <param name="id"></param>
        private void _OnRequestConnectionFromClient(ulong id)
        {
            if (!NetworkManager.Singleton.IsServer) return; // サーバーかチェック
            ClientID clientId = new ClientID(id);

            if (_clientContainer.Contains(clientId))
            {
                DebugEx.LogError($"クライアントが接続を要求しましたが、既に ID が登録されているため、失敗しました　ID: {id}");
                NetworkManager.Singleton.DisconnectClient(id); // 接続をきる
                return;
            }

            bool isMemberMax = _clientContainer.Count >= _controller.MemberMax;
            if (isMemberMax)
            {
                DebugEx.LogError($"クライアントの接続人数が最大数を超えました　最大人数: {isMemberMax}");
                NetworkManager.Singleton.DisconnectClient(id); // 接続をきる
                return;
            }

            _clientContainer.Register(clientId);
            DebugEx.Log($"クライアントが接続しました　ID: {id}");
        }

        /// <summary>
        /// クライントの接続情報をサーバーから削除する
        /// </summary>
        /// <remarks>
        /// [コール] クライアントがサーバーから切断した際
        /// </remarks>
        /// <param name="id"></param>
        private void _OnRequestDesconnectionFromClient(ulong id)
        {
            if (!NetworkManager.Singleton.IsServer) return; // サーバーかチェック
            ClientID clientId = new ClientID(id);

            if (!_clientContainer.Contains(clientId))
            {
                return;
            }

            _clientContainer.Unregister(clientId);
            DebugEx.Log($"クライアントを切断しました　ID: {id}");
        }

        /// <summary>
        /// コールバックを追加
        /// </summary>
        /// <remarks>
        /// [コール] サーバーが起動した際
        /// </remarks>
        /// <param name="id"></param>
        private void _OnStartAsServer()
        {
            if (!NetworkManager.Singleton.IsServer) return; // サーバーかチェック


            NetworkManager.Singleton.OnClientConnectedCallback += _OnRequestConnectionFromClient;
            NetworkManager.Singleton.OnClientDisconnectCallback += _OnRequestDesconnectionFromClient;
        }

        /// <summary>
        /// コールバックを削除
        /// </summary>
        /// <remarks>
        /// [コール] サーバーが停止した際
        /// </remarks>
        private void _OnStopAsServer(bool isSuccess)
        {
            if (!NetworkManager.Singleton.IsServer) return; // サーバーかチェック


            NetworkManager.Singleton.OnClientConnectedCallback -= _OnRequestConnectionFromClient;
            NetworkManager.Singleton.OnClientDisconnectCallback -= _OnRequestDesconnectionFromClient;
        }

        #endregion


        #region Client (クライアント)

        /// <summary>
        /// クライアントの起動
        /// </summary>
        /// <returns>
        /// true: 起動成功<br/>
        /// false: 起動失敗
        /// </returns>
        public bool StartAsClient()
        {
            /*--- 基本設定 ---*/
            // クライアント
            _transport.SetConnectionData(_controller.Ip, _controller.Port);

            bool isSuccess = NetworkManager.Singleton.StartClient();
            if (!isSuccess)
            {
                DebugEx.LogError($"サーバーへの接続に失敗しました");
                return false;
            }


            var id = new ClientID(NetworkManager.Singleton.LocalClientId);
            DebugEx.Log($"サーバーへ接続しました");
            return true;
        }

        /// <summary>
        /// [クライアント] クライアントを停止
        /// </summary>
        /// <returns>
        /// true: 停止成功<br/>
        /// false: 停止失敗
        /// </returns>
        public bool StopAsClient()
        {
            if (!NetworkManager.Singleton.IsClient) return false; // クライアントかチェック

            NetworkManager.Singleton.Shutdown(); // 接続をきる
            DebugEx.Log($"サーバーから切断しました");
            return true;
        }

        #endregion
    }
}
