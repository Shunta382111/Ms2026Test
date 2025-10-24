
using Framework;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEditor.PackageManager;

namespace Network
{
    public class ClientController
    {
        private NetworkController _controller = null;
        private UnityTransport _transport = null;





        /*--- メソッド ---*/

        public ClientController(NetworkController controller, UnityTransport transport)
        {
            _controller = controller;
            _transport = transport;
        }


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
    }
}
