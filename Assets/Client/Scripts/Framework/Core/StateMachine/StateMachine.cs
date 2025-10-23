using System;
using System.Collections.Generic;
using System.Linq;

namespace Framework.Core.State
{
    public class StateMachine
    {
        private readonly Dictionary<int, StateContext> _roots = new();
        private StateContext _runningRoot = null; // Update対象（ルートのみトラッキング）
        internal readonly SwitchQueue<StateContext> Queue = new();

        #region Setup

        /// <summary>ルートイベントを登録</summary>
        public StateContext AddRoot<TEnum>(TEnum id, IState state, int priority = 0) where TEnum : Enum
        {
            int key = Convert.ToInt32(id);
            if (_roots.ContainsKey(key))
            {
                DebugEx.LogError($"ID が既に登録されています: {key}");
                return _roots[key];
            }
            var ctx = new StateContext(key, priority, state, machine: this);
            _roots.Add(key, ctx);
            return ctx;
        }

        #endregion

        #region Frame


        public void Update()
        {
            // 1) ルートEnter実行（当フレームはUpdate参加させない）
            var enteredThisFrame = Queue.CallEnter();

            // 2) 既存稼働中ルートをUpdate
            _runningRoot?.Update();

            // 3) Exit実行（ルート or 子）
            var exited = Queue.CallExit();

            // 4) Enterしたルートをランナーへ追加（次フレームからUpdate）
            var enterdLastState = enteredThisFrame.LastOrDefault();
            if (enterdLastState != null)
            {
                _runningRoot = enterdLastState;
            }
        }

        #endregion

    }
}
