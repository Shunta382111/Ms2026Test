using UnityEngine;

namespace Framework.Core
{
    public class Singleton<T> : MonoBehaviour
        where T : new()
    {
        public static T instance { get; set; }

        public static void CreateInstance()
        {
            instance = new T();
        }
    }
}
