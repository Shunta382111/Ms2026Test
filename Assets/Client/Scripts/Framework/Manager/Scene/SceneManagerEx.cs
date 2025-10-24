using Framework.Core;
using Framework.Core.State;
using Framework.Manager.State;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Framework.Manager.Scene
{
    public class SceneManagerEx : SingletonBehavior<SceneManagerEx>
    {
        private StateMachine<string, SceneState> _machine = new();

        void Start()
        {

        }

        void Update()
        {

        }





        public void Load(string sceneName)
        {
            SceneManager.LoadSceneAsync(sceneName);
        }
    }
}
