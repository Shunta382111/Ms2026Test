using UnityEngine;

namespace Network
{
    public class LobbyState : INetworkState
    {
        NetworkController _controller = null;
        ServerController _server = null;

        public LobbyState(NetworkController controller, ServerController server)
        {
            _controller = controller;
            _server = server;
        }

        public override void OnEnter()
        {
            base.OnEnter();

            _server.EnableServerConnecting(); // サーバーへの接続を有効化する
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
        }

        public override void OnExit()
        {
            _server.DisableServerConnecting(); // サーバーへの接続を無効化する

            base.OnExit();
        }
    }
}
