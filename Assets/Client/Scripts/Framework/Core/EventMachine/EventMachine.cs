using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Framework.Core.Event
{
    /// <summary>
    /// ルート複数同時稼働・優先度選出・階層子遷移・グローバル強制切替をサポートしたイベントマシン
    /// </summary>
    public sealed class EventMachine<TKey, TEvent>
        where TEvent : IEvent<TKey, TEvent>
    {
        private readonly Dictionary<TKey, EventContext<TKey, TEvent>> _roots = new();
        private readonly List<EventContext<TKey, TEvent>> _runningRoots = new(); // Update対象（ルートのみトラッキング）
        internal readonly SwitchQueue<EventContext<TKey, TEvent>> Queue = new();

        /// <summary>
        /// ルートイベントを登録
        /// </summary>
        public EventContext<TKey, TEvent> AddRoot(TKey rootId, TEvent ev, int priority = 0)
        {
            if (_roots.ContainsKey(rootId))
            {
                DebugEx.LogError($"ID が既に登録されています: {rootId}");
                return _roots[rootId];
            }
            var ctx = new EventContext<TKey, TEvent>(rootId, priority, ev, parent: null, machine: this);
            _roots.Add(rootId, ctx);
            return ctx;
        }

        /// <summary>
        /// ルートにルールを追加（自動入場に利用）
        /// </summary>
        public void AddRootRule(TKey rootId, IRule<TKey, TEvent> rule)
        {
            if (!_roots.TryGetValue(rootId, out var ctx))
            {
                DebugEx.LogError($"イベントを切り替えようとしましたが、ID が登録されていません: {rootId}");
                return;
            }
            ctx.AddRule(rule);
        }

        /// <summary>
        /// ルートへ強制切替（優先度・ルール無視）。
        /// exclusive=true なら現在稼働中のルートを全てExitして、指定ルートだけを入場。
        /// exclusive=false なら、既存は残したまま指定ルートも追加で入場。
        /// </summary>
        public void SwitchRootForce(TKey rootId, bool exclusive = true)
        {
            if (!_roots.TryGetValue(rootId, out var target))
            {
                DebugEx.LogError($"イベントを切り替えようとしましたが、ID が登録されていません: {rootId}");
                return;
            }

            if (exclusive)
            {
                // 稼働中ルートを全て終了予約
                foreach (var r in _runningRoots.ToArray())
                    Queue.AddExit(r);
            }

            // 指定ルートを入場予約（次のCallEnterで入場）
            Queue.AddEnter(target);
        }

        /// <summary>
        /// 1フレーム分の処理：
        /// 1) ルートの自動入場選出 → Enter
        /// 2) 既存ランナーをUpdate（子の遷移予約はここで積まれる）
        /// 3) Exit実行 → ランナーから除外
        /// 4) 今フレームEnterしたルートをランナーへ追加（次フレームからUpdate）
        /// </summary>
        public void Update()
        {
            // 1) ルートの自動入場（ルールtrueの中から最大Priority、同率は全部）
            var autoRoots = SelectAutoEnterRoots();
            foreach (var r in autoRoots)
                Queue.AddEnter(r);

            // 1.5) ルートEnter実行（当フレームはUpdate参加させない）
            var enteredThisFrame = Queue.CallEnter();

            // 2) 既存稼働中ルートをUpdate
            for (int i = 0; i < _runningRoots.Count; i++)
                _runningRoots[i].Update();

            // 3) Exit実行（ルート or 子）
            var exited = Queue.CallExit();
            // ルートがExitしたらランニングから外す
            foreach (var e in exited)
            {
                if (e.Parent == null) // Root
                    _runningRoots.Remove(e);
            }

            // 4) Enterしたルートをランナーへ追加（次フレームからUpdate）
            foreach (var e in enteredThisFrame)
            {
                if (e.Parent == null && !_runningRoots.Contains(e))
                    _runningRoots.Add(e);
            }
        }

        #region Helpers

        private IEnumerable<EventContext<TKey, TEvent>> SelectAutoEnterRoots()
        {
            // 既に稼働していない & ルールtrue のルートを抽出
            var eligibles = _roots.Values
                .Where(r => !_runningRoots.Contains(r) && r.CanAutoEnter())
                .ToList();

            if (eligibles.Count == 0) return Array.Empty<EventContext<TKey, TEvent>>();

            // 最大Priorityを算出し、その値を持つものだけ残す（同率は全て採用）
            int maxP = eligibles.Max(r => r.Priority);
            return eligibles.Where(r => r.Priority == maxP);
        }

        #endregion
    }
}
