using Network;
using Unity.Netcode;
using UnityEngine;

namespace Network.Lobby
{

    public class LobbyController
    {
        private NetworkProvider _provider = null;
        private LobbyClientProcessor _clientProcessor = new();
        private LobbyServerProcessor _serverProcessor = new();



        /*--- メソッド ---*/

        public LobbyController(NetworkProvider provider)
        {
            _provider = provider;
        }

        public void StartAsServer()
        {

        }
    }
}
