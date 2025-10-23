
using System;
namespace Framework.Core.Event
{
    /// <summary>
    /// イベント基底。必要に応じて継承して振る舞いを定義。
    /// </summary>
    public abstract class IEvent
    {
        private EventContext _ctx;

        internal void __Bind(EventContext ctx) => _ctx = ctx;

        /// <summary>生成直後に一度だけ呼ばれる</summary>
        public virtual void OnInitialize() { }

        /// <summary>入場フレームに一度だけ呼ばれる（Updateは次フレームから）</summary>
        public virtual void OnEnter() { }

        /// <summary>稼働中の毎フレーム呼ばれる</summary>
        public virtual void OnUpdate() { }

        /// <summary>離脱フレームに一度だけ呼ばれる</summary>
        public virtual void OnExit() { }

        #region Convenience APIs (文脈付きショートカット)

        /// <summary>子イベントを追加</summary>
        public void AddChild<TEnum>(TEnum id, IEvent ev, int priority = 0) where TEnum : Enum
            => _ctx.AddChild(Convert.ToInt32(id), ev, priority);

        /// <summary>子へ遷移（任意タイミング）。前の子があればExit→新しい子Enter（次フレーム）</summary>
        public void SwitchChild<TEnum>(TEnum id) where TEnum : Enum
            => _ctx.SwitchChild(Convert.ToInt32(id));

        /// <summary>自分を終了（親の直下に戻す）</summary>
        public void ExitSelf()
            => _ctx.ExitSelf();

        /// <summary>ルートへ強制切替（優先度・ルール無視）。exclusive=trueなら他の実行中ルートを全て終了</summary>
        public void SwitchRootForce<TEnum>(TEnum id, bool exclusive = true) where TEnum : Enum
            => _ctx.Machine.SwitchRootForce(Convert.ToInt32(id), exclusive);

        #endregion
    }
}
