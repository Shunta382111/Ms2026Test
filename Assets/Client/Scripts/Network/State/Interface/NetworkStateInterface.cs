using Framework.Core.State;
using UnityEngine;

namespace Network
{
    public abstract class INetworkState : IState
    {
        public override void OnEnter() { }
        public override void OnUpdate() { }
        public override void OnExit() { }
    }
}
