using System;
using System.Collections.Generic;

namespace Framework.Core.State
{
	/// <summary>
	/// ステート実行のコンテキスト
	/// </summary>
	public sealed class StateContext<TState> : IEnter, IExit
		where TState : IState<TState>
	{
		public int Id { get; }
		public int Priority { get; }
		public TState State { get; }
		public StateMachine<TState> Machine { get; }

		internal StateContext(
			int id, int priority, TState state,
			StateMachine<TState> machine)
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
