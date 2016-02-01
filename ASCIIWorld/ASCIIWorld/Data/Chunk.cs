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

		private const int CHUNK_ROWS = 64;
		private const int CHUNK_COLUMNS = 64;

		/// <summary>
		/// Chunk layers:
		///		0 = Background.
		///		1 = Floor.
		///		2 = Blocking objects and entities.
		///		3 = Ceiling.
		/// </summary>
		// TODO: Make this configurable.
		private const int CHUNK_HEIGHT = 4;
		private const int ENTITY_LAYER = 2;

		private const float BLOCK_SCALE = 48;

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
			_blockIndex = new int[CHUNK_HEIGHT, CHUNK_ROWS, CHUNK_COLUMNS];
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

		public int this[int layer, int row, int column]
		{
			get
			{
				if ((layer < 0) || (layer >= CHUNK_HEIGHT) || (row < 0) || (row >= CHUNK_ROWS) || (column < 0) || (column >= CHUNK_COLUMNS))
				{
					return 0;
				}
				else
				{
					return _blockIndex[layer, row, column];
				}
			}
			set
			{
				// TODO: Verify that the input block id exists in this Chunk's BlockRegistry.
				if (!_blockRegistry.IsDefined(value))
				{
					throw new ArgumentException("This id is not registered.");
				}

				if ((layer >= 0) && (layer < CHUNK_HEIGHT) && (row >= 0) && (row < CHUNK_ROWS) && (column >= 0) && (column < CHUNK_COLUMNS))
				{
					_blockIndex[layer, row, column] = value;
				}
			}
		}

		#endregion

		#region Methods

		private int _renderCount;
		public void Render(OrthographicProjection projection)
		{
			projection.Apply();

			_renderCount = 0;

			_tessellator.Begin(PrimitiveType.Quads);
			_tessellator.LoadIdentity();
			_tessellator.Scale(BLOCK_SCALE, BLOCK_SCALE);

			// There is some kind of floating-point error going on here.
			//var minRow = 0; // (int)Math.Floor(projection.Top / BLOCK_SCALE);
			//var maxRow = CHUNK_ROWS; // (int)Math.Floor(projection.Bottom / BLOCK_SCALE);
			//var minColumn = 0; // (int)Math.Floor(projection.Left / BLOCK_SCALE);
			//var maxColumn = CHUNK_COLUMNS; // (int)Math.Floor(projection.Right / BLOCK_SCALE);
			var minRow = ((float)projection.Top / BLOCK_SCALE);
			var maxRow = ((float)projection.Bottom / BLOCK_SCALE);
			var minColumn = ((float)projection.Left / BLOCK_SCALE);
			var maxColumn = ((float)projection.Right / BLOCK_SCALE);

			Console.WriteLine("{0}, {1}", minRow, minColumn);

			for (var layer = 0; layer < ENTITY_LAYER; layer++)
			{
				RenderLayer(projection, layer, minRow, maxRow, minColumn, maxColumn);
			}

			// TODO: Render entities here.

			for (var layer = ENTITY_LAYER; layer < CHUNK_HEIGHT; layer++)
			{
				RenderLayer(projection, layer, minRow, maxRow, minColumn, maxColumn);
			}

			_tessellator.End();

			Console.WriteLine("Rendered blocks: {0}", _renderCount);
		}

		private void RenderLayer(IProjection projection, int layer, float minRow, float maxRow, float minColumn, float maxColumn)
		{
			//Console.WriteLine(_tessellator.Transform(new Vector3(minColumn, minRow, 0)));
			//var position = BLOCK_SCALE * new Vector2(minColumn, minRow);
			for (float row = minRow; row < maxRow; row++)
			{
				for (float column = minColumn; column < maxColumn; column++)
				{
					if (this[layer, (int)row, (int)column] > 0)
					{
						var position = _tessellator.Transform(new Vector3(column, row, 0));
						position.Z = layer;
						if (projection.Contains(position.X, position.Y) ||
							projection.Contains(position.X + BLOCK_SCALE, position.Y) ||
							projection.Contains(position.X, position.Y + BLOCK_SCALE) ||
							projection.Contains(position.X + BLOCK_SCALE, position.Y + BLOCK_SCALE))
						{
							_tessellator.Translate(position);
							_blockRegistry.GetById(this[layer, (int)row, (int)column]).Render(_tessellator);
							_tessellator.Translate(-position);
							_renderCount++;
						}
					}

					//position.X += BLOCK_SCALE;
					//_tessellator.Translate(BLOCK_SCALE, 0);
				}

				//position.Y += BLOCK_SCALE;
				
				//_tessellator.Translate(-BLOCK_SCALE * (maxColumn - minColumn) - 12, BLOCK_SCALE); // carriage return & line feed
				//position.X -= BLOCK_SCALE * (maxColumn - minColumn);
			}

			//_tessellator.Translate(0, -BLOCK_SCALE * (maxRow - minRow));
		}

		#endregion
	}
}
