
using Framework.Core.State;

namespace Network
{
    public class INetworkState : IState<INetworkState>
    {
        /// <summary>
        /// [サーバー] クライアントから接続が発生
        /// </summary>
        /// <param name="id"></param>
        public virtual void OnConnectedFromClient(ClientID id) { }

        /// <summary>
        /// [サーバー] クライアントから切断が発生
        /// </summary>
        /// <param name="id"></param>
        public virtual void OnDisconnectedFromClient(ClientID id) { }

        /// <summary>
        /// [クライアント] サーバーへの接続が発生
        /// </summary>
        /// <param name="id"></param>
        public virtual void OnConnectedToServer() { }

        /// <summary>
        /// [サーバー] サーバーへの切断が発生
        /// </summary>
        /// <param name="id"></param>
        public virtual void OnDisconnectedToServer() { }
    }
}
