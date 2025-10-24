
using System;
using UnityEngine.InputSystem;
namespace Framework.Core.Event
{
    /// <summary>
    /// イベント基底。必要に応じて継承して振る舞いを定義。
    /// </summary>
    public abstract class IEvent<TKey, TEvent> : IEnter, IUpdate, IExit
        where TEvent : IEvent<TKey, TEvent>
    {
        private EventContext<TKey, TEvent> _ctx;

        internal void __Bind(EventContext<TKey, TEvent> ctx) => _ctx = ctx;

        /// <summary>
        /// 生成直後に一度だけ呼ばれる
        /// </summary>
        public virtual void OnInitialize() { }

        /// <summary>
        /// 入場フレームに一度だけ呼ばれる（Updateは次フレームから）
        /// </summary>
        public virtual void OnEnter() { }

        /// <summary>
        /// 稼働中の毎フレーム呼ばれる
        /// </summary>
        public virtual void OnUpdate() { }

        /// <summary>
        /// 離脱フレームに一度だけ呼ばれる
        /// </summary>
        public virtual void OnExit() { }

        /// <summary>
        /// 子イベントを追加
        /// </summary>
        public void AddChild(TKey id, TEvent ev, int priority = 0)
            => _ctx.AddChild(id, ev, priority);

        /// <summary>
        /// 子へ遷移（任意タイミング）。前の子があればExit→新しい子Enter（次フレーム）
        /// </summary>
        public void SwitchChild(TKey id)
            => _ctx.SwitchChild(id);

        /// <summary>
        /// 自分を終了（親の直下に戻す）
        /// </summary>
        public void ExitSelf()
            => _ctx.ExitSelf();

        /// <summary>
        /// ルートへ強制切替（優先度・ルール無視）
        /// </summary>
        /// <remarks>
        /// exclusive=trueなら他の実行中ルートを全て終了
        /// </remarks>
        public void SwitchRootForce(TKey id, bool exclusive = true)
            => _ctx.Machine.SwitchRootForce(id, exclusive);
    }
}
