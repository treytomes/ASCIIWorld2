using ASCIIWorld.Data;
using ASCIIWorld.Rendering;
using GameCore;
using GameCore.Rendering;
using OpenTK;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace ASCIIWorld
{
	/// <summary>
	/// Manage data and rendering for the game world.
	/// </summary>
	public class WorldManager
	{
		#region Fields

		private ChunkRenderer _chunkRenderer;

		#endregion

		#region Constructors

		public WorldManager(Viewport viewport, Level level)
		{
			Level = level;

			GeneratePlayer();

			Camera = GameCore.Camera.CreateOrthographicCamera(viewport);
			_chunkRenderer = new ChunkRenderer(viewport);
		}

		#endregion

		#region Properties

		public Camera<OrthographicProjection> Camera { get; private set; }

		public Level Level { get; private set; }

		public PlayerEntity Player { get; private set; }

		#endregion

		#region Methods

		public void Load(string filename)
		{
			Player = PlayerEntity.Load("0edb7a22-611b-479b-b4a9-e2c5e7ee4263.player");
			//Player = PlayerEntity.Load(Player);

			var fileStream = new FileStream(filename, FileMode.Open);
			var formatter = new BinaryFormatter();
			try
			{
				// The player is not loaded from the level file.
				Level = (Level)formatter.Deserialize(fileStream);
				Level.AddEntity(Player);
				Console.WriteLine($"Level loaded from '{filename}'.");
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			finally
			{
				fileStream.Close();
			}
		}

		public void Save(string filename)
		{
			var fileStream = new FileStream(filename, FileMode.Create);
			var formatter = new BinaryFormatter();
			try
			{
				// Don't save the player with the level!
				var playerChunk = Level.GetChunk(Player);
				playerChunk.RemoveEntity(Player);
				formatter.Serialize(fileStream, Level);
				playerChunk.AddEntity(Player);
				Console.WriteLine($"Level saved to '{filename}'.");
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			finally
			{
				fileStream.Close();
			}

			PlayerEntity.Save(Player);
		}

		public void Resize(Viewport viewport)
		{
			Camera.Resize(viewport);
		}

		public void Update(TimeSpan elapsed)
		{
			BlockRegistry.Instance.UpdateRenderer(elapsed);

			Camera.MoveTo(Player.Position);
			if (!Player.IsAlive)
			{
				throw new NotImplementedException("The player is dead.");
			}

			Level.Update(elapsed, Player);
		}

		public void Render()
		{
			Camera.Apply();
			_chunkRenderer.Render(Camera, Level);
		}

		private void GeneratePlayer()
		{
			Player = new PlayerEntity();

			Player.Toolbelt.SetFirstCompatibleSlot(ItemStack.FromName("Pickaxe"));
			Player.Toolbelt.SetFirstCompatibleSlot(ItemStack.FromName("Hoe"));
			Player.Toolbelt.SetFirstCompatibleSlot(ItemStack.FromName("Grass"));

			var spawnPoint = Level.GetChunk(Player).FindSpawnPoint();
			if (!spawnPoint.HasValue)
			{
				throw new Exception("Unable to spawn the player.");
			}
			else
			{
				Player.MoveTo(Level, new Vector2(spawnPoint.Value.X, spawnPoint.Value.Y));
			}

			Level.GetChunk(Player).AddEntity(Player);
		}

		#endregion
	}
}
