using Framework;
using Framework.Attribute;
using Framework.Core;
using Framework.Core.ObjectResolver;
using Framework.Core.State;
using Network.Lobby;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;

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
        private ConnectionController _connection = null;
        private NetworkStateController _states = null;
        private ClientContainer _clientContainer = null;






        /*--- メソッド ---*/

        public void Initialise()
        {
            _resolver = new();
            _resolver.RegisterInstance<NetworkController>(this);
            _resolver.RegisterInstance<UnityTransport>(Transport);
            _resolver.Register<ConnectionController>();
            _resolver.Register<NetworkStateController>();
            _resolver.Register<ClientContainer>();

            _connection = _resolver.Resolve<ConnectionController>();
            _states = _resolver.Resolve<NetworkStateController>();
            _clientContainer = _resolver.Resolve<ClientContainer>();

            _isInitialized = true;
        }

        public void Release()
        {
            _resolver.UnregisterInstance<NetworkController>();
            _resolver.UnregisterInstance<UnityTransport>();
            _resolver.UnregisterInstance<ConnectionController>();
            _resolver.UnregisterInstance<NetworkStateController>();
            _resolver.UnregisterInstance<ClientContainer>();

            _connection = null;
            _states = null;
            _resolver = null;
            _clientContainer = null;

            _isInitialized = false;
        }

        [Button("サーバー起動")]
        public void StartAsServer()
        {
            bool isSuccess = _connection.StartAsServer();
            if (!isSuccess) return;

            NetworkManager.Singleton.OnClientConnectedCallback +=_states.OnConnectedFromClient;
            NetworkManager.Singleton.OnClientDisconnectCallback +=_states.OnDisconnectedFromClient;
        }

        [Button("クライアント起動")]
        public void StartAsClient()
        {
            bool isSuccess = _connection.StartAsClient();
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
