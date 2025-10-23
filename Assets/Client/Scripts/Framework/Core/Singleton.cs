using UnityEngine;

namespace Framework.Core
{
    public class Singleton<T> : MonoBehaviour
        where T : new()
    {
        public static T Instance { get; set; }

        public static void CreateInstance()
        {
            Instance = new T();
        }
    }
}
