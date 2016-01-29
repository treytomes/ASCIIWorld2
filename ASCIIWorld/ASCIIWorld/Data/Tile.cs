using GameCore.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASCIIWorld.Data
{
	/// <summary>
	/// A Tile is composed of multiple frames, and can be animated.
	/// </summary>
	public class Tile
	{
		#region Fields

		private List<TileFrame> _frames;
		private Dictionary<string, object> _properties;
		private int _frameIndex;
		private double _totalElapsedSeconds;

		#endregion

		#region Constructors

		public Tile(int framesPerSecond, IEnumerable<TileFrame> frames)
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
			_frames = new List<TileFrame>(frames);
			_properties = new Dictionary<string, object>();

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

		public TileFrame this[int index]
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

		public bool HasProperty(string propertyName)
		{
			return _properties.ContainsKey(propertyName);
		}

		public T GetProperty<T>(string propertyName)
		{
			return (T)Convert.ChangeType(_properties[propertyName], typeof(T));
		}

		public void SetProperty<T>(string propertyName, T value)
		{
			_properties[propertyName] = value;
		}

		public void Update(TimeSpan elapsed)
		{
			_totalElapsedSeconds += elapsed.TotalSeconds;
			FrameIndex = (int)(_totalElapsedSeconds * FramesPerSecond);
		}

		public void Render(ITessellator tessellator, int x, int y)
		{
			_frames[_frameIndex].Render(tessellator, x, y);
		}

		#endregion
	}
}
