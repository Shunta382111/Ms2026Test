using Unity.VisualScripting;
using UnityEngine;

namespace Network
{
    public abstract class INetworkState
    {
        public virtual void OnEnter() { }
        public virtual void OnUpdate() { }
        public virtual void OnExit() { }
    }
}
