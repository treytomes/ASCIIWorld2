using GameCore.Rendering;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ASCIIWorld.Rendering
{
	/// <summary>
	/// A Animation is composed of multiple renderable frames.
	/// </summary>
	public class Animation : IBlockRenderer
	{
		#region Fields

		private List<IRenderable> _frames;
		private int _frameIndex;
		private double _totalElapsedSeconds;

		#endregion

		#region Constructors

		public Animation(int framesPerSecond, IEnumerable<IRenderable> frames)
		{
			if (framesPerSecond <= 0)
			{
				throw new ArgumentException("framesPerSeconds");
			}
			if (frames.Count() == 0)
			{
				throw new ArgumentException("frames");
			}

			FramesPerSecond = framesPerSecond;
			_frames = new List<IRenderable>(frames);
			_totalElapsedSeconds = 0;
		}

		#endregion

		#region Properties

		public int FramesPerSecond { get; private set; }

		public int Count
		{
			get
			{
				return _frames.Count;
			}
		}

		public IRenderable this[int index]
		{
			get
			{
				return _frames[index];
			}
		}

		public int FrameIndex
		{
			get
			{
				return _frameIndex;
			}
			set
			{
				_frameIndex = value % _frames.Count;
			}
		}

		#endregion

		#region Methods

		public void Update(TimeSpan elapsed)
		{
			_totalElapsedSeconds += elapsed.TotalSeconds;
			FrameIndex = (int)(_totalElapsedSeconds * FramesPerSecond);
		}

		public void Render(ITessellator tessellator)
		{
			_frames[_frameIndex].Render(tessellator);
		}

		public void Render(ITessellator tessellator, float x, float y)
		{
			var position = tessellator.WorldToScreenPoint(new Vector2(x, y));
			tessellator.Translate(position);
			Render(tessellator);
			tessellator.Translate(-position);
		}

		#endregion
	}
}
