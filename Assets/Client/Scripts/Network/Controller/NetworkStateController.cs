
using Framework.Core.ObjectResolver;
using Framework.Core.State;
using Unity.Netcode;

namespace Network
{
    public enum NetworkState
    {
        Lobby,
    }



    public class NetworkStateController : IInjectResolver
    {



        private StateMachine<NetworkState, INetworkState> _stateMachine = new();

        public NetworkStateController()
        {
        }

        public void InjectResolver(ObjectResolver resolver)
        {
            resolver.Register<LobbyState>();

            _stateMachine.AddRoot(NetworkState.Lobby, resolver.Resolve<LobbyState>());
        }

        public void UninjectResolver(ObjectResolver resolver)
        {
            resolver.UnregisterInstance<LobbyState>();

            _stateMachine.Clear();
            _stateMachine = null;
        }

        /// <summary>
        /// [サーバー] クライアントから接続が発生
        /// </summary>
        /// <param name="id">接続先のクライアントID</param>

        public void OnConnectedFromClient(ulong id)
        {
            if (!NetworkManager.Singleton.IsServer) return; // サーバーかチェック

            _stateMachine.State?.OnConnectedFromClient(new ClientID(id));
        }


        /// <summary>
        /// [サーバー] クライアントから切断が発生
        /// </summary>
        /// <param name="id">接続先のクライアントID</param>
        public void OnDisconnectedFromClient(ulong id)
        {
            if (!NetworkManager.Singleton.IsClient) return; // サーバーかチェック

            _stateMachine.State?.OnDisconnectedFromClient(new ClientID(id));
        }

        /// <summary>
        /// [クライアント] サーバーへの接続が発生
        /// </summary>
        /// <param name="id">自身のID</param>

        public void OnConnectedToServer(ulong id)
        {
            if (!NetworkManager.Singleton.IsServer) return; // クライアントかチェック

            _stateMachine.State?.OnConnectedToServer();
        }


        /// <summary>
        /// [クライアント] サーバーへの切断が発生
        /// </summary>
        /// <param name="id">自身のID</param>
        public void OnDisconnectedToServer(ulong id)
        {
            if (!NetworkManager.Singleton.IsClient) return; // クライアントかチェック

            _stateMachine.State?.OnDisconnectedToServer();
        }
    }
}
