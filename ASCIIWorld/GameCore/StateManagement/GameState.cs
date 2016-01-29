using GameCore.IO;
using GameCore.Rendering;
using System;

namespace GameCore.StateManagement
{
	public abstract class GameState
	{
		#region Fields

		#endregion

		#region Constructors

		public GameState(GameStateManager manager)
		{
			Manager = manager;

			IsUpdateable = true;
			IsRenderable = true;
		}

		#endregion

		#region Properties

		public bool IsUpdateable { get; protected set; }

		public bool IsRenderable { get; protected set; }

		public bool HasFocus
		{
			get
			{
				return Manager.GameWindow.Focused && (Manager.CurrentState == this);
			}
		}

		protected GameStateManager Manager { get; private set; }

		#endregion

		#region Methods

		public virtual void LoadContent(ContentManager content)
		{
		}

		public virtual void UnloadContent()
		{
		}

		public virtual void Update(TimeSpan elapsed)
		{
		}

		public virtual void Render()
		{
		}

		public virtual void Resize(Viewport viewport)
		{
		}

		protected void EnterState(GameState state)
		{
			Manager.EnterState(state);
		}

		protected void LeaveState()
		{
			Manager.LeaveState();
		}

		protected void SwitchStates(GameState state)
		{
			Manager.SwitchStates(state);
		}

		#endregion
	}
}
