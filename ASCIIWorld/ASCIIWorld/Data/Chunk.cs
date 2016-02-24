using GameCore.Rendering;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;

namespace ASCIIWorld.Data
{
	/// <summary>
	/// A Chunk is composed of a 3D array of Blocks.
	/// A Level is composed of many Chunks, but only certain Chunks will be updated each frame.
	/// A World is composed of many Levels, each of the same size, all stacked on top of eachother.
	/// </summary>
	public class Chunk
	{
		#region Constants

		//private const int CHUNK_ROWS = 256;
		//private const int CHUNK_COLUMNS = 256;
		private const int CHUNK_ROWS = 64;
		private const int CHUNK_COLUMNS = 64;

		private const float BLOCK_SCALE = 24;

		#endregion

		#region Fields

		private BlockRegistry _blockRegistry;
		private int[,,] _blockIndex;
		private ITessellator _tessellator;
		
		#endregion

		#region Constructors

		public Chunk(BlockRegistry blockRegistry)
		{
			_blockRegistry = blockRegistry;
			_blockIndex = new int[Enum.GetValues(typeof(ChunkLayer)).Length, CHUNK_ROWS, CHUNK_COLUMNS];
			_tessellator = new VertexBufferTessellator() { Mode = VertexTessellatorMode.Render };
		}

		#endregion

		#region Properties

		public int Rows
		{
			get
			{
				return CHUNK_ROWS;
			}
		}

		public int Columns
		{
			get
			{
				return CHUNK_COLUMNS;
			}
		}

		public int this[ChunkLayer layer, int row, int column]
		{
			get
			{
				if ((row < 0) || (row >= CHUNK_ROWS) || (column < 0) || (column >= CHUNK_COLUMNS))
				{
					return 0;
				}
				else
				{
					return _blockIndex[(int)layer, row, column];
				}
			}
			set
			{
				// TODO: Verify that the input block id exists in this Chunk's BlockRegistry.
				if ((value != 0) && !_blockRegistry.IsDefined(value))
				{
					throw new ArgumentException("This id is not registered.");
				}

				if ((row >= 0) && (row < CHUNK_ROWS) && (column >= 0) && (column < CHUNK_COLUMNS))
				{
					_blockIndex[(int)layer, row, column] = value;
				}
			}
		}

		#endregion

		#region Methods

		public void Render(OrthographicProjection projection)
		{
			// TODO: blocks at lower levels should be darker, higher levels should be brighter.

			projection.Apply();

			_tessellator.Begin(PrimitiveType.Quads);
			_tessellator.LoadIdentity();
			_tessellator.Scale(BLOCK_SCALE, BLOCK_SCALE);

			var minRow = (float)Math.Floor(projection.Top / BLOCK_SCALE);
			var maxRow = (float)Math.Ceiling(projection.Bottom / BLOCK_SCALE);
			var minColumn = (float)Math.Floor(projection.Left / BLOCK_SCALE);
			var maxColumn = (float)Math.Ceiling(projection.Right / BLOCK_SCALE);

			RenderLayer(projection, ChunkLayer.Background, minRow, maxRow, minColumn, maxColumn);
			RenderLayer(projection, ChunkLayer.Floor, minRow, maxRow, minColumn, maxColumn);

			// TODO: Render entities here.

			RenderLayer(projection, ChunkLayer.Blocking, minRow, maxRow, minColumn, maxColumn);
			RenderLayer(projection, ChunkLayer.Ceiling, minRow, maxRow, minColumn, maxColumn);

			_tessellator.End();
		}

		private void RenderLayer(IProjection projection, ChunkLayer layer, float minRow, float maxRow, float minColumn, float maxColumn)
		{
			for (float row = minRow; row < maxRow; row++)
			{
				for (float column = minColumn; column < maxColumn; column++)
				{
					if (this[layer, (int)row, (int)column] > 0)
					{
						var position = _tessellator.Transform(new Vector3(column, row, 0));
						position.Z = (int)layer;

						_tessellator.Translate(position);
						_blockRegistry.GetById(this[layer, (int)row, (int)column]).Render(_tessellator);
						_tessellator.Translate(-position);
					}
				}
			}
		}

		#endregion
	}
}
