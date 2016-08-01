using CommonCore.Math;
using System;
using System.Collections.Generic;

namespace ASCIIWorld.Data
{
	/// <summary>
	/// A Chunk is composed of a 3D array of Blocks.
	/// A Level is composed of a 2D array of Chunks, but only certain Chunks will be updated each frame.
	/// A World is composed of many Levels, each of the same size, all stacked on top of eachother.
	/// </summary>
	[Serializable]
	public class Chunk : IChunkAccess
	{
		#region Constants

		private const int NULL_BLOCK_ID = 0;

		#endregion

		#region Fields

		private int _width;
		private int _height;
		private int[,,] _blockIndex;
		private int[,,] _blockMetadata;
		private List<Entity> _entities;
		private float _ambientLightLevel;

		#endregion

		#region Constructors

		public Chunk(int width, int height)
		{
			_width = width;
			_height = height;
			_blockIndex = new int[Enum.GetValues(typeof(ChunkLayer)).Length, _height, _width];
			_blockMetadata = new int[Enum.GetValues(typeof(ChunkLayer)).Length, _height, _width];
			_entities = new List<Entity>();
		}

		#endregion

		#region Properties

		public int Width
		{
			get
			{
				return _width;
			}
		}

		public int Height
		{
			get
			{
				return _height;
			}
		}

		public int this[ChunkLayer layer, int x, int y]
		{
			get
			{
				if (MathHelper.IsInRange(x, 0, Width) && MathHelper.IsInRange(y, 0, Height))
				{
					return _blockIndex[(int)layer, y, x];
				}
				else
				{
					return 0;
				}
			}
			set
			{
				if (MathHelper.IsInRange(x, 0, Width) && MathHelper.IsInRange(y, 0, Height))
				{
					_blockIndex[(int)layer, y, x] = value;

					if (value == 0)
					{
						SetMetadata(layer, x, y, 0);
					}
				}
			}
		}
		
		public IEnumerable<Entity> Entities
		{
			get
			{
				foreach (var entity in _entities.ToArray())
				{
					yield return entity;
				}
				yield break;
			}
		}

		public float AmbientLightLevel
		{
			get
			{
				return _ambientLightLevel;
			}
		}

		#endregion

		#region Methods

		public void Update(TimeSpan elapsed, Level level)
		{
			_ambientLightLevel = level.AmbientLightLevel;

			UpdateBlocks(elapsed, level, ChunkLayer.Background);
			UpdateBlocks(elapsed, level, ChunkLayer.Floor);

			UpdateEntities(elapsed, level);

			UpdateBlocks(elapsed, level, ChunkLayer.Blocking);
			UpdateBlocks(elapsed, level, ChunkLayer.Ceiling);
		}

		/// <summary>
		/// This allows us to either update all of the entities in the level, or only the entities in the given chunk, i.e. the player's chunk.
		/// This may not be necessary, or it may need to be expanded to the nearest 3 chunks, etc.
		/// Looking up the player's chunk every frame may slow things down more than just updating every entity.  I'm not sure yet.
		/// </summary>
		private void UpdateEntities(TimeSpan elapsed, Level level)
		{
			var deadEntities = new List<Entity>();
			foreach (var entity in Entities)
			{
				entity.Update(level, elapsed);
				if (!entity.IsAlive)
				{
					deadEntities.Add(entity);
				}
			}

			foreach (var entity in deadEntities)
			{
				level.GetChunk(entity).RemoveEntity(entity);
			}
		}

		private void UpdateBlocks(TimeSpan elapsed, Level level, ChunkLayer layer)
		{
			for (var blockX = 0; blockX < Width; blockX++)
			{
				for (var blockY = 0; blockY < Height; blockY++)
				{
					var blockId = _blockIndex[(int)layer, blockY, blockX];
					if (blockId != NULL_BLOCK_ID)
					{
						BlockRegistry.Instance.GetById(blockId).Update(elapsed, level, layer, blockX, blockY);
					}
				}
			}
		}

		public void SetMetadata(ChunkLayer layer, int x, int y, int metadata)
		{
			_blockMetadata[(int)layer, x, y] = metadata;
		}

		public int GetMetadata(ChunkLayer layer, int x, int y)
		{
			return _blockMetadata[(int)layer, x, y];
		}

		public void AddEntity(Entity entity)
		{
			if (!_entities.Contains(entity))
			{
				_entities.Add(entity);
				Console.WriteLine($"Adding entity: {entity.GetType()}");
			}
		}

		public void RemoveEntity(Entity entity)
		{
			if (_entities.Contains(entity))
			{
				_entities.Remove(entity);
				Console.WriteLine($"Removing entity: {entity.GetType()}");
			}
		}

		/// <summary>
		/// Search the chunk randomly for a spot that isn't blocked.
		/// </summary>
		public Vector2I? FindSpawnPoint()
		{
			const int MAX_SPAWN_ATTEMPTS = 32;
			var random = new Random();
			for (var n = 0; n < MAX_SPAWN_ATTEMPTS; n++)
			{
				var x = random.Next(0, _width);
				var y = random.Next(0, _height);
				if (!IsBlockedAt(x, y))
				{
					return new Vector2I(x, y);
				}
			}
			return null;
		}

		public bool IsBlockedAt(int blockX, int blockY)
		{
			return this[ChunkLayer.Blocking, blockX, blockY] != NULL_BLOCK_ID;
		}

		public ChunkLayer GetHighestVisibleLayer(int blockX, int blockY)
		{
			if (this[ChunkLayer.Ceiling, blockX, blockY] != 0)
			{
				return ChunkLayer.Ceiling;
			}
			else if (this[ChunkLayer.Blocking, blockX, blockY] != 0)
			{
				return ChunkLayer.Blocking;
			}
			else if (this[ChunkLayer.Floor, blockX, blockY] != 0)
			{
				return ChunkLayer.Floor;
			}
			else if (this[ChunkLayer.Background, blockX, blockY] != 0)
			{
				return ChunkLayer.Background;
			}
			return ChunkLayer.Background;
		}

		public bool CanSeeSky(ChunkLayer layer, int blockX, int blockY)
		{
			if (!layer.HasLayerAbove())
			{
				return true;
			}
			else
			{
				var layerAbove = layer.GetLayerAbove();
				return CanSeeSky(layerAbove, blockX, blockY) && ((this[layerAbove, blockX, blockY] == NULL_BLOCK_ID) || !BlockRegistry.Instance.GetById(this[layerAbove, blockX, blockY]).IsOpaque);
			}
		}

		#endregion
	}
}
