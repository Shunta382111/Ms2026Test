
using Framework.Core.Event;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Framework.Core.State
{
    public class StateMachine
    {
        private readonly Dictionary<int, StateContext> _roots = new();
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

        }

        #endregion

    }
}
