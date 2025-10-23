using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Framework.Core.State
{
    public class StateMachine<TState>
        where TState : IState<TState>
    {
        private readonly Dictionary<int, StateContext<TState>> _roots = new();
        private StateContext<TState> _runningRoot = null; // Update対象（ルートのみトラッキング）
        internal readonly SwitchQueue<StateContext<TState>> Queue = new();
        public TState State => _runningRoot?.State;

        #region Setup

        /// <summary>ルートイベントを登録</summary>
        public StateContext<TState> AddRoot<TEnum>(TEnum id, TState state, int priority = 0) where TEnum : Enum
        {
            int key = Convert.ToInt32(id);
            if (_roots.ContainsKey(key))
            {
                DebugEx.LogError($"ID が既に登録されています: {key}");
                return _roots[key];
            }
            var ctx = new StateContext<TState>(key, priority, state, machine: this);
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

        #region Global switching

        /// <summary>
        /// ステートの切り替え
        /// </summary>
        public void SwitchRoot<TEnum>(TEnum id) where TEnum : Enum
            => SwitchRootForce(Convert.ToInt32(id));

        public void SwitchRootForce(int rootId)
        {
            if (!_roots.TryGetValue(rootId, out var target))
            {
                DebugEx.LogError($"Root not found: {rootId}");
                return;
            }

            if (_runningRoot != null)
            {
                // 稼働中のステートの終了予約
                Queue.AddExit(_runningRoot);
            }

            // 指定ルートを入場予約（次のCallEnterで入場）
            Queue.AddEnter(target);
        }

        #endregion

        public void Clear()
        {
            Queue.Clear();
            _roots.Clear();
            _runningRoot = null;
        }
    }
}
