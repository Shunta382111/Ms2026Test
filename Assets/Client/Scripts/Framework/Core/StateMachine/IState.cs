using Framework.Core.Event;
using System;
using UnityEngine;

namespace Framework.Core.State
{
    /// <summary>
    /// イベント基底。必要に応じて継承して振る舞いを定義。
    /// </summary>
    public abstract class IState<TState> : IEnter, IUpdate, IExit
        where TState : IState<TState>
    {
        private StateContext<TState> _ctx;

        internal void __Bind(StateContext<TState> ctx) => _ctx = ctx;

        /// <summary>
        /// 生成直後に一度だけ呼ばれる
        /// </summary>
        public virtual void OnInitialize() { }

        /// <summary>
        /// 入場フレームに一度だけ呼ばれる
        /// </summary>
        /// <remarks>
        /// Updateは次フレームから
        /// </remarks>
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
        /// ステートの切り替え
        /// </summary>
        public void SwitchRoot(int id)
            => _ctx.Machine.SwitchRoot(id);

        /// <summary>
        /// ステートの切り替え
        /// </summary>
        public void SwitchRoot<TEnum>(TEnum id) where TEnum : Enum
            => _ctx.Machine.SwitchRoot<TEnum>(id);
    }
}
