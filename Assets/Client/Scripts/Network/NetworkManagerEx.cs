using UnityEngine;
using Framework.Core;
using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using Framework;

namespace Network
{
    [RequireComponent(typeof(NetworkManager))]
    public class NetworkManagerEx : Singleton<NetworkManagerEx>, IInitializable
    {
        private NetworkProvider _provider = null;
        [SerializeField] public ushort Port { get; private set; } = 7777;
        [SerializeField] public string Ip { get; private set; } = "127.0.0.1";
        [SerializeField] public uint MemberMax { get; private set; } = 4;


        private bool _isInitialized = false;
        public bool IsInitialized => _isInitialized;

        /*--- メソッド ---*/

        public void Initialise()
        {
            _provider = new NetworkProvider(gameObject);
        }


        //public void StartAsClient()
        //{
        //    /*--- 基本設定 ---*/
        //    // クライアント
        //    _transport.SetConnectionData(_ip, _port);

        //    bool isSuccess = NetworkManager.Singleton.StartClient();
        //    if (!isSuccess)
        //    {
        //        DebugEx.LogError($"ホストへの接続に失敗しました");
        //        return;
        //    }

        //    bool isCapacityMember = NetworkManager.Singleton.ConnectedClientsIds.Count > _maxMember;
        //    if (isCapacityMember)
        //    {
        //        DebugEx.LogError($"接続人数が最大数を超えました: {_maxMember}");
        //        //NetworkManager.


        //    }
        //}

        //public void StartAsHost()
        //{
        //    /*--- 基本設定 ---*/
        //    // ホスト
        //    _transport.ConnectionData.ServerListenAddress = "0.0.0.0";
        //    _transport.ConnectionData.Port = _port;

        //    bool isSuccess = NetworkManager.Singleton.StartHost();
        //    if (!isSuccess)
        //    {
        //        DebugEx.LogError("ホストの起動に失敗しました");
        //        return;
        //    }
        //}





        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
