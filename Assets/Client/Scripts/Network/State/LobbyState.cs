//using Framework;
//using Unity.Netcode;
//using UnityEditor.Experimental.GraphView;
//using UnityEngine;

//namespace Network
//{
//    /// <summary>
//    /// [サーバー] 通信ステート: ロビー
//    /// </summary>
//    /// <remarks>
//    /// ホストを起動して、クライアントとの接続を確立させます
//    /// </remarks>
//    public class ServerLobbyState : INetworkState
//    {
//        public void StartAsClient()
//        {
//            /*--- 基本設定 ---*/
//            // クライアント
//            _transport.SetConnectionData(_ip, _port);

//            bool isSuccess = NetworkManager.Singleton.StartClient();
//            if (!isSuccess)
//            {
//                DebugEx.LogError($"ホストへの接続に失敗しました");
//                return;
//            }

//            bool isCapacityMember = NetworkManager.Singleton.ConnectedClientsIds.Count > _maxMember;
//            if (isCapacityMember)
//            {
//                DebugEx.LogError($"接続人数が最大数を超えました: {_maxMember}");
//                //NetworkManager.


//            }
//        }

//        public void StartAsHost()
//        {
//            /*--- 基本設定 ---*/
//            // ホスト
//            _transport.ConnectionData.ServerListenAddress = "0.0.0.0";
//            _transport.ConnectionData.Port = _port;

//            bool isSuccess = NetworkManager.Singleton.StartHost();
//            if (!isSuccess)
//            {
//                DebugEx.LogError("ホストの起動に失敗しました");
//                return;
//            }
//        }
//    }


//    /// <summary>
//    /// [クライアント] 通信ステート: ロビー
//    /// </summary>
//    /// <remarks>
//    /// サーバーとの接続を確立させます
//    /// </remarks>
//    public class ClientLobbyState : INetworkState
//    {

//    }


//}
