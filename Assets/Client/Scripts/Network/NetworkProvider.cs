
using Framework.Core;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

namespace Network
{
    public class NetworkProvider : Provider
    {
        /*--- フィールド ---*/
        public ClientContainer ClientContainer { get; private set; } = new();

        public UnityTransport Transport { get; private set; } = null;

        public GameObject GameObject { get; private set; } = null;






        /*--- メソッド ---*/
        public NetworkProvider(GameObject owner, UnityTransport transport)
        {
            GameObject = owner;
            Transport = transport;
        }
    }
}
