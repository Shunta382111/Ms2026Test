using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Framework.Core.State
{
    public class StateMachine<TKey, TState>
        where TState : IState<TKey, TState>
    {
        /*--- フィールド ---*/

        private readonly Dictionary<TKey, StateContext<TKey, TState>> _roots = new();
        private StateContext<TKey, TState> _runningRoot = null; // Update対象（ルートのみトラッキング）
        internal readonly SwitchQueue<StateContext<TKey, TState>> Queue = new();
        public TState State => _runningRoot?.State;
        public Action OnSwitchedCallback;



        /*--- メソッド ---*/

        /// <summary>
        /// ルートステートを登録
        /// </summary>
        public StateContext<TKey, TState> AddRoot(TKey id, TState state, int priority = 0)
        {
            if (_roots.ContainsKey(id))
            {
                DebugEx.LogError($"ID が既に登録されています: {id}");
                return _roots[id];
            }
            var ctx = new StateContext<TKey, TState>(id, priority, state, machine: this);
            _roots.Add(id, ctx);
            return ctx;
        }

        /// <summary>
        /// ステートの切り替え
        /// </summary>
        /// <param name="rootId"></param>
        public void SwitchRoot(TKey rootId)
        {
            if (!_roots.TryGetValue(rootId, out var target))
            {
                DebugEx.LogError($"ステートを切り替えようとしましたが、ID が登録されていません: {rootId}");
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

        /// <summary>
        /// ステートを全て削除
        /// </summary>
        public void Clear()
        {
            Queue.Clear();
            _roots.Clear();
            _runningRoot = null;
        }

        /// <summary>
        /// 更新
        /// </summary>
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
    }
}
