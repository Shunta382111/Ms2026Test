using Framework;
using Framework.Attribute;
using Framework.Core.ObjectResolver;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

namespace Network
{
    public class NetworkController : NetworkBehaviour, IInitializable
    {
        private bool _isInitialized = false;
        public bool IsInitialized => _isInitialized;
        private NetworkProvider _provider = null;
        [SerializeField] public UnityTransport Transport = null;
        [SerializeField] public ushort Port = 7777;
        [SerializeField] public string Ip = "127.0.0.1";
        [SerializeField] public uint MemberMax = 4;


        private ObjectResolver _resolver = null;
        private ServerController _server = null;
        private ClientController _client = null;
        private NetworkStateController _states = null;
        private ClientContainer _clientContainer = null;





        /*--- メソッド ---*/

        public void Initialise()
        {
            _resolver = new();
            _resolver.RegisterInstance<NetworkController>(this);
            _resolver.RegisterInstance<UnityTransport>(Transport);
            _resolver.Register<ServerController>();
            _resolver.Register<ClientController>();
            _resolver.Register<NetworkStateController>();
            _resolver.Register<ClientContainer>();

            _server = _resolver.Resolve<ServerController>();
            _client = _resolver.Resolve<ClientController>();
            _states = _resolver.Resolve<NetworkStateController>();
            _clientContainer = _resolver.Resolve<ClientContainer>();

            _isInitialized = true;
        }

        public void Release()
        {
            _resolver.UnregisterInstance<NetworkController>();
            _resolver.UnregisterInstance<UnityTransport>();
            _resolver.UnregisterInstance<ServerController>();
            _resolver.UnregisterInstance<ClientController>();
            _resolver.UnregisterInstance<NetworkStateController>();
            _resolver.UnregisterInstance<ClientContainer>();

            _server = null;
            _client = null;
            _states = null;
            _resolver = null;
            _clientContainer = null;

            _isInitialized = false;
        }

        [Button("サーバー起動")]
        public void StartAsServer()
        {
            bool isSuccess = _server.StartAsServer();
            if (!isSuccess) return;

            NetworkManager.Singleton.OnClientConnectedCallback +=_states.OnConnectedFromClient;
            NetworkManager.Singleton.OnClientDisconnectCallback +=_states.OnDisconnectedFromClient;
        }

        [Button("クライアント起動")]
        public void StartAsClient()
        {
            bool isSuccess = _client.StartAsClient();
            if (!isSuccess) return;

            NetworkManager.Singleton.OnClientConnectedCallback +=_states.OnConnectedToServer;
            NetworkManager.Singleton.OnClientConnectedCallback +=_states.OnDisconnectedToServer;
        }

        public void Start()
        {
            Initialise();
        }

        // Update is called once per frame
        void Update()
        {
        }

        public override void OnDestroy()
        {
            Release();

            base.OnDestroy();
        }
    }
}
