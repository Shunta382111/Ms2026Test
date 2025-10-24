
using UnityEngine;

namespace Framework.Core
{
    public class SystemBehavior : MonoBehaviour, IInitializable
    {
        protected bool isInitialized = false;
        public bool IsInitialized => isInitialized;

        public virtual void Initialise() { }
        public virtual void Activate() { }
        public virtual void Deactivate() { }
    }
}
