﻿using OpenTK.Graphics.OpenGL;
using System;
using System.IO;

namespace GameCore.Rendering
{
	public class ShaderProgram : IDisposable
	{
		#region Fields

		private bool _disposed;

		private int _programID;
		private int _vertexShaderID;
		private int _fragmentShaderID;

		//private int _uniformTexture;

		#endregion

		#region Constructors

		static ShaderProgram()
		{
			var versionText = GL.GetString(StringName.Version);
			var major = Convert.ToInt32(versionText[0].ToString());
			//var minor = Convert.ToInt32(versionText[2].ToString());
			if (major < 2)
			{
				throw new Exception($"Shaders require OpenGL version >= 2.0.  Your version is {versionText}.");
			}
		}

		public ShaderProgram(string vertexShaderPath, string fragmentShaderPath)
		{
			_disposed = false;

			_programID = GL.CreateProgram();
			_vertexShaderID = LoadShader(vertexShaderPath, ShaderType.VertexShader, _programID);
			_fragmentShaderID = LoadShader(fragmentShaderPath, ShaderType.FragmentShader, _programID);
			GL.LinkProgram(_programID);
			Console.WriteLine(GL.GetProgramInfoLog(_programID));
		}

		~ShaderProgram()
		{
			Dispose(false);
		}

		#endregion

		#region Methods

		public virtual void Begin()
		{
			GL.UseProgram(_programID);
		}

		public void End()
		{
			GL.UseProgram(0);
		}

		public ShaderVariable GetVariable(string name)
		{
			var id = GL.GetUniformLocation(_programID, name);
			if (id == -1)
			{
				throw new Exception("Variable not defined.");
			}
			return new ShaderVariable(id);
		}

		private int LoadShader(string filename, ShaderType type, int programID)
		{
			var address = GL.CreateShader(type);
			GL.ShaderSource(address, File.ReadAllText(filename));
			GL.CompileShader(address);

			Console.WriteLine(GL.GetShaderInfoLog(address));

			int statusCode;
			GL.GetShader(address, ShaderParameter.CompileStatus, out statusCode);
			if (statusCode != 1)
			{
				throw new ApplicationException();
			}

			GL.AttachShader(programID, address);
			return address;
		}

		public void Dispose()
		{
			Dispose(true);
		}

		protected void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				if (disposing)
				{
					GL.DeleteProgram(_programID);
					GL.DeleteShader(_fragmentShaderID);
					GL.DeleteShader(_vertexShaderID);
				}
				_disposed = true;
			}
		}

		#endregion
	}
}