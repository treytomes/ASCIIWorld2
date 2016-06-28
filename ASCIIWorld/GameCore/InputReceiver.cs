using OpenTK.Input;
using System;

namespace GameCore
{
	public class InputReceiver : IDisposable
	{
		#region Fields

		#endregion

		#region Constructors

		public InputReceiver(IGameWindow window)
		{
			Window = window;

			InputManager.Instance.Keyboard.KeyDown += Keyboard_KeyDown;
			InputManager.Instance.Keyboard.KeyUp += Keyboard_KeyUp;
			InputManager.Instance.Mouse.ButtonDown += Mouse_ButtonDown;
			InputManager.Instance.Mouse.ButtonUp += Mouse_ButtonUp;
			InputManager.Instance.Mouse.Move += Mouse_Move;
			InputManager.Instance.Mouse.WheelChanged += Mouse_WheelChanged;
		}

		#endregion

		#region Properties

		protected IGameWindow Window { get; private set; }

		#endregion

		#region Methods

		public virtual void Dispose()
		{
			Window = null;

			InputManager.Instance.Keyboard.KeyDown -= Keyboard_KeyDown;
			InputManager.Instance.Keyboard.KeyUp -= Keyboard_KeyUp;
			InputManager.Instance.Mouse.ButtonDown -= Mouse_ButtonDown;
			InputManager.Instance.Mouse.ButtonUp -= Mouse_ButtonUp;
			InputManager.Instance.Mouse.Move -= Mouse_Move;
			InputManager.Instance.Mouse.WheelChanged -= Mouse_WheelChanged;
		}

		protected virtual void OnMouseWheelChanged(MouseWheelEventArgs e)
		{
		}

		protected virtual void OnMouseMove(MouseMoveEventArgs e)
		{
		}

		protected virtual void OnMouseButtonUp(MouseButtonEventArgs e)
		{
		}

		protected virtual void OnMouseButtonDown(MouseButtonEventArgs e)
		{
		}

		protected virtual void OnKeyboardKeyUp(KeyboardKeyEventArgs e)
		{
		}

		protected virtual void OnKeyboardKeyDown(KeyboardKeyEventArgs e)
		{
		}

		//private TMouseEvent ProcessMouseEvent<TMouseEvent>(TMouseEvent e)
		//	where TMouseEvent : MouseEventArgs
		//{
		//	if (!e.IsProcessed())
		//	{
		//		var position = e.Position;
		//		position.Y = Window.Height - position.Y;
		//		e.Position = position;

		//		e.IsProcessed(true);
		//	}
		//	return e;
		//}

		#endregion

		#region Event Handlers

		private void Mouse_WheelChanged(object sender, MouseWheelEventArgs e)
		{
			//OnMouseWheelChanged(ProcessMouseEvent(e));
			OnMouseWheelChanged(e);
		}

		private void Mouse_Move(object sender, MouseMoveEventArgs e)
		{
			//OnMouseMove(ProcessMouseEvent(e));
			OnMouseMove(e);
		}

		private void Mouse_ButtonUp(object sender, MouseButtonEventArgs e)
		{
			//OnMouseButtonUp(ProcessMouseEvent(e));
			OnMouseButtonUp(e);
		}

		private void Mouse_ButtonDown(object sender, MouseButtonEventArgs e)
		{
			//OnMouseButtonDown(ProcessMouseEvent(e));
			OnMouseButtonDown(e);
		}

		private void Keyboard_KeyUp(object sender, KeyboardKeyEventArgs e)
		{
			OnKeyboardKeyUp(e);
		}

		private void Keyboard_KeyDown(object sender, KeyboardKeyEventArgs e)
		{
			OnKeyboardKeyDown(e);
		}

		#endregion
	}
}