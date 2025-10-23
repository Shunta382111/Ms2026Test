using System;
using System.Collections.Generic;

namespace Framework.Core
{
    /// <summary>
    /// 入場・離脱を「次にまとめて」実行するキュー。
    /// Dequeueで消費し、必要な副作用（親の_activeChild反映など）もここで完結させる。
    /// </summary>
    public sealed class SwitchQueue<TContext>
        where TContext : class, IEnter, IExit
    {
        private readonly Queue<(TContext ctx, Action after)> _enterQ = new();
        private readonly Queue<(TContext ctx, Action after)> _exitQ = new();

        public void AddEnter(TContext ctx, Action onAfterEnter = null)
        {
            if (ctx == null) return;
            _enterQ.Enqueue((ctx, onAfterEnter));
        }

        public void AddExit(TContext ctx, Action onAfterExit = null)
        {
            if (ctx == null) return;
            _exitQ.Enqueue((ctx, onAfterExit));
        }

        /// <summary>Enterを一括実行（戻り値：今回入場したコンテキスト）</summary>
        public List<TContext> CallEnter()
        {
            var entered = new List<TContext>(_enterQ.Count);
            while (_enterQ.Count > 0)
            {
                var (ctx, after) = _enterQ.Dequeue();
                ctx.OnEnter();
                after?.Invoke();
                entered.Add(ctx);
            }
            return entered;
        }

        /// <summary>Exitを一括実行（戻り値：今回離脱したコンテキスト）</summary>
        public List<TContext> CallExit()
        {
            var exited = new List<TContext>(_exitQ.Count);
            while (_exitQ.Count > 0)
            {
                var (ctx, after) = _exitQ.Dequeue();
                ctx.OnExit();
                after?.Invoke();
                exited.Add(ctx);
            }
            return exited;
        }

        public void Clear()
        {
            _enterQ.Clear();
            _exitQ.Clear();
        }
    }
}
