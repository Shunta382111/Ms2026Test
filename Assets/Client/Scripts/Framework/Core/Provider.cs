
using UnityEngine;

namespace Framework.Core
{
    public class Provider
    {
        protected T GetOrAddComponent<T>(GameObject owner) where T : Component
        {
            DebugEx.AssertSome(owner, "ゲームオブジェクトが null です");
            T component = null;

            // コンポーネントを取得
            if (!owner.TryGetComponent(out component))
            {
                // 失敗 → コンポーネントを追加
                DebugEx.Log($"{owner.name} にコンポーネントを追加します: {typeof(T).Name}");
                component = owner.AddComponent<T>();
            }

            return component;
        }
    }
}
