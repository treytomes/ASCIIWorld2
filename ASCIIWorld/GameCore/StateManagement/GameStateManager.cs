﻿using GameCore.IO;
using GameCore.Rendering;
using System;
using System.Collections.Generic;

namespace GameCore.StateManagement
{
	public class GameStateManager
	{
		#region Fields

		private ContentManager _content;
		private List<GameState> _states;

		#endregion

		#region Constructors

		public GameStateManager(IGameWindow gameWindow, ContentManager content)
		{
			GameWindow = gameWindow;

			_content = content;
			_states = new List<GameState>();
		}

		#endregion

		#region Properties

		public IGameWindow GameWindow { get; private set; }

		public int Count
		{
			get
			{
				return _states.Count;
			}
		}

		public GameState CurrentState
		{
			get
			{
				return (Count > 0) ? _states[0] : null;
			}
		}

		#endregion

		#region Methods

		public void EnterState(GameState state)
		{
			state.LoadContent(_content);
			_states.Insert(0, state);
		}

		public void LeaveState()
		{
			_states[0].UnloadContent();
			_states[0].Dispose();
			_states.RemoveAt(0);
		}

		public void SwitchStates(GameState state)
		{
			LeaveState();
			EnterState(state);
		}

		public void Update(TimeSpan elapsed)
		{
			for (var index = 0; index < _states.Count; index++)
			{
				var state = _states[index];

				if (state.IsUpdateable)
				{
					state.Update(elapsed);
				}
			}
		}

		public void Render()
		{
			for (var index = _states.Count - 1; index >= 0; index--)
			{
				var state = _states[index];

				if (state.IsRenderable)
				{
					state.Render();
				}
			}
		}

		public void Resize(Viewport viewport)
		{
			foreach (var state in _states)
			{
				state.Resize(viewport);
			}
		}

		#endregion
	}
}
