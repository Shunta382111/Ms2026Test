using Framework.Core;
using Unity.Netcode;

namespace Network
{
    public class NetworkSingleton<T> : NetworkBehaviour
        where T : NetworkBehaviour
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
