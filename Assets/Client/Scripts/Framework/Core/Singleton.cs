using UnityEngine;

namespace Framework.Core
{
    public class Singleton<T> : MonoBehaviour
        where T : new()
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    CreateInstance();
                }

                return _instance;
            }
        }

        public static void CreateInstance()
        {
            _instance = new T();
        }
    }
}
