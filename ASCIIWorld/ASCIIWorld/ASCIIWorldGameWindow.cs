using ASCIIWorld.IO;
using GameCore;
using System;

namespace ASCIIWorld
{
	public class ASCIIWorldGameWindow : BasicGameWindow
	{
		#region Fields

		//private ScriptManager _scriptManager;

		#endregion

		#region Constructors

		public ASCIIWorldGameWindow(AppSettings settings)
			: base(1280, 720, "ASCII World", settings.ContentRootPath)
		{
			Load += ASCIIWorldGameWindow_Load;
			Content.RegisterContentProvider(new BlockContentProvider());
			
			//_scriptManager = new ScriptManager();
		}

		#endregion

		#region Event Handlers

		/// <summary>
		/// Setup settings, load textures, sounds.
		/// </summary>
		private void ASCIIWorldGameWindow_Load(object sender, EventArgs e)
		{
			States.EnterState(new GameplayState(States));
			//_scriptManager.Show();
		}

		#endregion
	}
}
