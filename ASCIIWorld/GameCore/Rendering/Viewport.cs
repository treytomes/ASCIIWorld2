using CommonCore;
using OpenTK.Graphics.OpenGL;
using System;
using System.Drawing;

namespace GameCore.Rendering
{
	public class Viewport : ICloneable<Viewport>
	{
		#region Constructors

		public Viewport(int x, int y, int width, int height)
		{
			X = x;
			Y = y;
			Width = width;
			Height = height;
		}

		public Viewport(Rectangle viewport)
			: this(viewport.X, viewport.Y, viewport.Width, viewport.Height)
		{
		}

		public Viewport(Viewport viewport)
			: this(viewport.X, viewport.Y, viewport.Width, viewport.Height)
		{
		}

		#endregion

		#region Properties

		public int X { get; set; }

		public int Y { get; set; }

		public int Width { get; set; }

		public int Height { get; set; }

		public Size Size
		{
			get
			{
				return new Size(Width, Height);
			}
		}

		public int Left
		{
			get
			{
				return X;
			}
			set
			{
				X = value;
			}
		}

		public int Right
		{
			get
			{
				return X + Width - 1;
			}
			set
			{
				Width = value + 1 - X;
			}
		}

		public int Top
		{
			get
			{
				return Y;
			}
			set
			{
				Y = value;
			}
		}

		public int Bottom
		{
			get
			{
				return Y + Height - 1;
			}
			set
			{
				Height = value + 1 - Y;
			}
		}

		public float AspectRatio
		{
			get
			{
				return Width / (float)Height;
			}
		}

		#endregion

		#region Methods

		public void Apply()
		{
			GL.Viewport(X, Y, Width, Height);
		}

		public Rectangle ToRectangle()
		{
			return new Rectangle(X, Y, Width, Height);
		}

		public bool Contains(Point position)
		{
			return Contains(position.X, position.Y);
		}

		public bool Contains(int x, int y)
		{
			return (Left <= x) && (x <= Right) && (Top <= y) && (y <= Bottom);
		}

		public Viewport Clone()
		{
			return new Viewport(this);
		}

		object ICloneable.Clone()
		{
			return Clone();
		}

		#endregion
	}
}
