using UnityEngine;

namespace Framework.Core
{
    public class Singleton<T>
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


    public class SingletonBehavior<T> : SystemBehavior
        where T : MonoBehaviour
    {
        private static T _instance;

        public static T Instance => _instance;

        public void Awake()
        {
            // 既に登録されているなら削除
            if (_instance)
            {
                Destroy(this);
                return;
            }
            // シングルトンを登録
            else
            {
                _instance = this as T;
                DontDestroyOnLoad(this);
            }
        }
    }
}
