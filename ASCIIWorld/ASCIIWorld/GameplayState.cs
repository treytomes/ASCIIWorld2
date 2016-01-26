using GameCore.StateManagement;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASCIIWorld
{
	public class GameplayState : GameState
	{
		#region Constructors

		public GameplayState(GameStateManager manager)
			: base(manager)
		{
		}

		#endregion

		#region Methods

		public override void Update(TimeSpan elapsed)
		{
			base.Update(elapsed);
			
			if (Keyboard.GetState().IsKeyDown(Key.Escape))
			{
				LeaveState();
			}
		}

		public override void Render()
		{
			base.Render();

			GL.MatrixMode(MatrixMode.Projection);
			GL.LoadIdentity();
			GL.Ortho(-1.0, 1.0, -1.0, 1.0, 0.0, 4.0);

			GL.Begin(PrimitiveType.Triangles);

			GL.Color3(Color.MidnightBlue);
			GL.Vertex2(-1.0f, 1.0f);
			GL.Color3(Color.SpringGreen);
			GL.Vertex2(0.0f, -1.0f);
			GL.Color3(Color.Ivory);
			GL.Vertex2(1.0f, 1.0f);

			GL.End();
		}

		#endregion
	}
}
