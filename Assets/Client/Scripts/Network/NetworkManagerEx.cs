using UnityEngine;
using Framework.Core;
using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using Framework;
using Network.Lobby;

namespace Network
{
    public class NetworkManagerEx : NetworkSingleton<NetworkManagerEx>, IInitializable
    {
        private NetworkProvider _provider = null;
        [SerializeField] public ushort Port = 7777;
        [SerializeField] public string Ip = "127.0.0.1";
        [SerializeField] public uint MemberMax = 4;


        private bool _isInitialized = false;
        public bool IsInitialized => _isInitialized;

        public ConnectionController _connection = null;

        [Button()]
        public void StartAsServer() => _connection.StartAsServer();

        [Button("クライアント起動")]
        public void StartAsClient() => _connection.StartAsClient();





        /*--- メソッド ---*/

        public void Initialise()
        {
            _provider = new NetworkProvider(gameObject);
            _connection = new ConnectionController(_provider, Port);

            _isInitialized = true;
        }

        public void Start()
        {
            Initialise();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
