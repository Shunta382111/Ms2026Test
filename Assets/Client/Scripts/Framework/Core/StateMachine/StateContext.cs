using System;
using System.Collections.Generic;

namespace Framework.Core.State
{
	/// <summary>
	/// ステート実行のコンテキスト
	/// </summary>
	public sealed class StateContext<TKey, TState> : IEnter, IExit
		where TState : IState<TKey, TState>
	{
		public TKey Id { get; }
		public int Priority { get; }
		public TState State { get; }
		public StateMachine<TKey, TState> Machine { get; }

		internal StateContext(
			TKey id, int priority, TState state,
			StateMachine<TKey, TState> machine)
		{
			Id = id;
			Priority = priority;
			State = state;
			Machine = machine;

			state?.__Bind(this);
			state?.OnInitialize();
		}

        public void OnEnter()
		{
			State?.OnEnter();
		}

        public void Update()
		{
			State?.OnUpdate();
		}

        public void OnExit()
		{
			State?.OnExit();
		}
	}
}
