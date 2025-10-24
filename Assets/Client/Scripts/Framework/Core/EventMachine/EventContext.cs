using System.Collections.Generic;

namespace Framework.Core.Event
{
    /// <summary>
    /// イベント実行のコンテキスト（階層・親子関係・子の現在状態などを保持）
    /// </summary>
    public sealed class EventContext<TKey, TEvent> : IEnter, IExit
        where TEvent : IEvent<TKey, TEvent>
    {
        public TKey Id { get; }
        public int Priority { get; }
        public TEvent Event { get; }
        public EventContext<TKey, TEvent> Parent { get; }
        public EventMachine<TKey, TEvent> Machine { get; }

        private readonly Dictionary<TKey, EventContext<TKey, TEvent>> _children = new();
        private readonly List<IRule<TKey, TEvent>> _rules = new(); // 自動入場（主にルート用）
        private EventContext<TKey, TEvent> _activeChild;           // 現在アクティブな子（nullなら自分を更新）
        private EventContext<TKey, TEvent> _pendingEnterChild;     // 次フレームにEnter予定の子

        internal EventContext(
            TKey id, int priority, TEvent ev,
            EventContext<TKey, TEvent> parent, EventMachine<TKey, TEvent> machine)
        {
            Id = id;
            Priority = priority;
            Event = ev;
            Parent = parent;
            Machine = machine;

            ev?.__Bind(this);
            ev?.OnInitialize();
        }

        public void AddRule(IRule<TKey, TEvent> rule)
        {
            if (rule == null) { DebugEx.LogWarning("Rule is null"); return; }
            _rules.Add(rule);
        }

        /// <summary>
        /// 自動入場の可否。ルールが1つでもtrueならtrue（ORポリシー）。
        /// ルール未設定なら false（＝自動入場しない）。
        /// </summary>
        internal bool CanAutoEnter()
        {
            if (_rules.Count == 0) return false;
            foreach (var r in _rules)
            {
                if (r == null) continue;
                if (r.CanExecute(Event, Machine)) return true;
            }
            return false;
        }

        public IEnumerable<EventContext<TKey, TEvent>> Children => _children.Values;

        public void AddChild(TKey id, TEvent ev, int priority = 0)
        {
            if (_children.ContainsKey(id))
            {
                DebugEx.LogError($"Child id already exists: {id}");
                return;
            }
            var ctx = new EventContext<TKey, TEvent>(id, priority, ev, this, Machine);
            _children.Add(id, ctx);
        }

        /// <summary>
        /// 子へ遷移予約（今の子があればExit、次フレームで新しい子Enter）。
        /// 親はアクティブのままで、Updateの委譲先が子に切り替わる。
        /// </summary>
        public void SwitchChild(TKey id)
        {
            if (!_children.TryGetValue(id, out var next))
            {
                DebugEx.LogError($"Child id not found: {id}");
                return;
            }

            // 現在の子があればExit予約
            if (_activeChild != null)
                Machine.Queue.AddExit(_activeChild);

            // 次の子をEnter予約し、親側にも記録
            _pendingEnterChild = next;
            Machine.Queue.AddEnter(next, onAfterEnter: () =>
            {
                // Enter完了後に「親のアクティブ子」を確定
                _activeChild = next;
            });
        }

        /// <summary>
        /// 自分を終了（親に制御を戻す）。親のActive子が自分なら解除。
        /// </summary>
        public void ExitSelf()
        {
            // 自分配下の子が動いていたら先に落とす
            if (_activeChild != null)
                Machine.Queue.AddExit(_activeChild);

            Machine.Queue.AddExit(this, onAfterExit: () =>
            {
                if (Parent != null && Parent._activeChild == this)
                    Parent._activeChild = null;
            });
        }

        public void OnEnter()
        {
            Event?.OnEnter();

            // 子のEnterが予約されている場合、Enterの完了はキュー側で親の_activeChildに反映される
            // ここでは _pendingEnterChild は触らない（CallEnter側で処理する）
        }

        public void Update()
        {
            if (_activeChild == null) Event?.OnUpdate();
            else _activeChild.Update();
        }

        public void OnExit()
        {
            Event?.OnExit();

            // 念のため子が生きていたら落とす
            if (_activeChild != null)
            {
                Machine.Queue.AddExit(_activeChild);
                // 即時ではなく次のCallExitで落ちる
                _activeChild = null;
            }
        }
    }
}
