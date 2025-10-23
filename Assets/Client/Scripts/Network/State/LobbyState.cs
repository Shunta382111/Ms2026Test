using UnityEngine;

namespace Network
{
    public class LobbyState : INetworkState
    {
        NetworkController _controller = null;
        public LobbyState(NetworkController controller)
        {
            _controller = controller;
        }

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
