using ASCIIWorld.Data;
using ASCIIWorld.Rendering;
using GameCore;
using GameCore.Rendering;
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

			Camera = GameCore.Camera.CreateOrthographicCamera(viewport);
			_chunkRenderer = new ChunkRenderer(viewport);
		}

		#endregion

		#region Properties

		public Camera<OrthographicProjection> Camera { get; private set; }

		public Level Level { get; private set; }

		#endregion

		#region Methods

		public void Load(string filename)
		{
			var fileStream = new FileStream(filename, FileMode.Open);
			var formatter = new BinaryFormatter();
			try
			{
				Level = (Level)formatter.Deserialize(fileStream);
				Console.WriteLine($"Loaded from '{filename}'.");
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
				formatter.Serialize(fileStream, Level);
				Console.WriteLine($"Saved to '{filename}'.");
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

		public void Resize(Viewport viewport)
		{
			Camera.Resize(viewport);
		}

		public void Update(TimeSpan elapsed)
		{
			BlockRegistry.Instance.Update(elapsed);
		}

		public void Render()
		{
			Camera.Apply();
			_chunkRenderer.Render(Camera, Level);
		}

		#endregion
	}
}
