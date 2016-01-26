using GameCore.Rendering.Text;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using System.Drawing;

namespace GameCore.UI
{
	public class ListItem : INotifyPropertyChanged, ITextDescription
	{
		#region Constants

		private const string DEFAULT_FONT_FAMILY = "Consolas";
		private const float DEFAULT_FONT_SIZE = 12.0f;
		private const FontStyle DEFAULT_FONT_SYLTE = FontStyle.Regular;

		#endregion

		#region Events

		public event PropertyChangedEventHandler PropertyChanged;
		public event EventHandler Activated;

		#endregion

		#region Fields

		private static GLTextWriter _writer;
		private static TextPrinter _printer;

		private object _value;
		private bool _isSelected;

		private Font _font;
		private Vector2 _position;
		private TextPrinterOptions _options;
		private TextAlignment _alignment;
		private TextDirection _direction;

		#endregion

		#region Constructors

		static ListItem()
		{
			_writer = new GLTextWriter();
			_printer = new TextPrinter();
		}

		public ListItem(object value, bool isSelected = false)
		{
			Value = value;
			IsSelected = isSelected;

			Font = new Font(DEFAULT_FONT_FAMILY, DEFAULT_FONT_SIZE, DEFAULT_FONT_SYLTE);
			Position = Vector2.Zero;
			Options = TextPrinterOptions.Default;
			Alignment = TextAlignment.Near;
			Direction = TextDirection.LeftToRight;
		}

		public ListItem()
			: this(null)
		{
		}

		#endregion

		#region Properties

		public object Value
		{
			get
			{
				return _value;
			}
			set
			{
				if (_value != value)
				{
					_value = value;
					NotifyPropertyChanged("Value");
				}
			}
		}

		public bool IsSelected
		{
			get
			{
				return _isSelected;
			}
			set
			{
				if (_isSelected != value)
				{
					_isSelected = value;
					NotifyPropertyChanged("IsSelected");
				}
			}
		}

		public Font Font
		{
			get
			{
				return _font;
			}
			set
			{
				if (_font != value)
				{
					_font = value;
					NotifyPropertyChanged("Font");
				}
			}
		}

		public Color Color
		{
			get
			{
				return IsSelected ? Color.Yellow : Color.White;
			}
		}

		public Vector2 Position
		{
			get
			{
				return _position;
			}
			set
			{
				if (_position != value)
				{
					_position = value;
					NotifyPropertyChanged("Position");
				}
			}
		}

		public TextPrinterOptions Options
		{
			get
			{
				return _options;
			}
			set
			{
				if (_options != value)
				{
					_options = value;
					NotifyPropertyChanged("Options");
				}
			}
		}

		public TextAlignment Alignment
		{
			get
			{
				return _alignment;
			}
			set
			{
				if (_alignment != value)
				{
					_alignment = value;
					NotifyPropertyChanged("Alignment");
				}
			}
		}

		public TextDirection Direction
		{
			get
			{
				return _direction;
			}
			set
			{
				if (_direction != value)
				{
					_direction = value;
					NotifyPropertyChanged("Direction");
				}
			}
		}

		public RectangleF Bounds
		{
			get
			{
				var extents = _printer.Measure((Value ?? string.Empty).ToString(), Font);
				var bounds = extents.BoundingBox;
				bounds.X += Position.X;
				bounds.Y += Position.Y;
				return bounds;
			}
		}

		#endregion

		#region Methods

		public void Activate()
		{
			if (Activated != null)
			{
				Activated(this, EventArgs.Empty);
			}
		}

		public virtual void Update(TimeSpan elapsed)
		{
		}

		public virtual void Render()
		{
			_writer.Apply(this);
			_writer.Write((Value ?? string.Empty).ToString());
		}

		protected void NotifyPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		#endregion
	}
}