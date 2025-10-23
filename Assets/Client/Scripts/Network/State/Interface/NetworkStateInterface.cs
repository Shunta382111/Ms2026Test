using Framework;
using Framework.Core.State;
using UnityEngine;

namespace Network
{
    public abstract class INetworkState : IState
    {
        protected NetworkProvider Provider = null;
        public INetworkState(NetworkProvider provider)
        {
            DebugEx.AssertSome(provider, "NetworkProvider が null です");
            Provider = provider;
        }



        public override void OnEnter() { }
        public override void OnUpdate() { }
        public override void OnExit() { }
    }
}
