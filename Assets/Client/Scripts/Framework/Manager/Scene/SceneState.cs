
using Framework.Core.State;
using Unity.VisualScripting;

namespace Framework.Manager.State
{
    public class SceneState : IState<string, SceneState>
    {
        public virtual void OnLoad() { }

        public virtual void OnUnload() { }
    }
}
