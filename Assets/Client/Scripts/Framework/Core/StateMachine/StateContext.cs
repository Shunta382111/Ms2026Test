using System.Collections.Generic;

namespace Framework.Core.State
{
	/// <summary>
	/// ステート実行のコンテキスト
	/// </summary>
	public sealed class StateContext : IEnter, IExit
	{
		public int Id { get; }
		public int Priority { get; }
		public IState State { get; }
		public StateMachine Machine { get; }

		internal StateContext(
			int id, int priority, IState state,
			StateMachine machine)
		{
			Id = id;
			Priority = priority;
			State = state;
			Machine = machine;

			state?.__Bind(this);
			state?.OnInitialize();
		}

		#region Update dispatch

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

		#endregion
	}
}
