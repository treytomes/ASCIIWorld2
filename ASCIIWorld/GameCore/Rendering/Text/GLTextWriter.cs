using GameCore.UI;
using OpenTK;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace GameCore.Rendering.Text
{
	public class GLTextWriter : TextWriter, ITextDescription
	{
		#region Constants

		private const string DEFAULT_FONT_FAMILY = "Consolas";
		private const float DEFAULT_FONT_SIZE = 12.0f;
		private const FontStyle DEFAULT_FONT_SYLTE = FontStyle.Regular;

		#endregion

		#region Fields

		private TextPrinter _printer;

		#endregion

		#region Constructors

		public GLTextWriter(Font font)
		{
			Font = font;
			_printer = new TextPrinter();
			
			Color = Color.White;
			Position = Vector2.Zero;
			Options = TextPrinterOptions.Default;
			Alignment = TextAlignment.Near;
			Direction = TextDirection.LeftToRight;
		}

		public GLTextWriter(string fontFamily, float fontSize, FontStyle fontStyle)
			: this(new Font(fontFamily, fontSize, fontStyle))
		{
		}

		public GLTextWriter()
			: this(new Font(DEFAULT_FONT_FAMILY, DEFAULT_FONT_SIZE, DEFAULT_FONT_SYLTE))
		{
		}

		#endregion

		#region Properties

		public override Encoding Encoding
		{
			get
			{
				return Encoding.ASCII;
			}
		}

		public Font Font { get; set; }

		public Color Color { get; set; }

		public Vector2 Position { get; set; }

		public TextPrinterOptions Options { get; set; }

		public TextAlignment Alignment { get; set; }

		public TextDirection Direction { get; set; }

		#endregion

		#region Methods

		public SizeF Measure(string text)
		{
			return _printer.Measure(text, Font).BoundingBox.Size;
		}

		public void Apply(ITextDescription description)
		{
			Font = description.Font;
			Color = description.Color;
			Position = description.Position;
			Options = description.Options;
			Alignment = description.Alignment;
			Direction = description.Direction;
		}

		public override void Write(char value)
		{
			SimpleWrite(value.ToString());
		}

		public override void Write(char[] buffer)
		{
			SimpleWrite(string.Concat(buffer));
		}

		public override void Write(char[] buffer, int index, int count)
		{
			SimpleWrite(string.Concat(buffer.Skip(index).Take(count)));
		}

		public override void Write(bool value)
		{
			SimpleWrite(value.ToString());
		}

		public override void Write(decimal value)
		{
			SimpleWrite(value.ToString());
		}

		public override void Write(double value)
		{
			SimpleWrite(value.ToString());
		}

		public override void Write(float value)
		{
			SimpleWrite(value.ToString());
		}

		public override void Write(int value)
		{
			SimpleWrite(value.ToString());
		}

		public override void Write(long value)
		{
			SimpleWrite(value.ToString());
		}

		public override void Write(uint value)
		{
			SimpleWrite(value.ToString());
		}

		public override void Write(ulong value)
		{
			SimpleWrite(value.ToString());
		}

		public override void Write(object value)
		{
			SimpleWrite(value.ToString());
		}

		public override void Write(string value)
		{
			SimpleWrite(value);
		}

		public override void Write(string format, object arg0)
		{
			SimpleWrite(string.Format(format, arg0));
		}

		public override void Write(string format, object arg0, object arg1)
		{
			SimpleWrite(string.Format(format, arg0, arg1));
		}

		public override void Write(string format, object arg0, object arg1, object arg2)
		{
			SimpleWrite(string.Format(format, arg0, arg1, arg2));
		}

		public override void Write(string format, params object[] arg)
		{
			SimpleWrite(string.Format(format, arg));
		}

		private void SimpleWrite(string text)
		{
			_printer.Render(text, Font, Color, Position, Options, Alignment, Direction);
		}

		#endregion
	}
}