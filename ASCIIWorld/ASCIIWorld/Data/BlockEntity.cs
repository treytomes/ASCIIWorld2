using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameCore.Rendering;
using OpenTK;
using System.Drawing;

namespace ASCIIWorld.Data
{
	public class BlockEntity : Entity
	{
		#region Fields

		private int _blockId;
		private float _rotation;

		private Color _color;

		#endregion

		#region Constructors

		public BlockEntity(int blockId)
			: base(BlockRegistry.Instance.GetById(blockId).Renderer)
		{
			_blockId = blockId;
			_rotation = 0.0f;
			_color = Color.White;
		}

		#endregion

		#region Methods

		public override void Render(ITessellator tessellator)
		{
			tessellator.BindColor(Color.FromArgb(196, _color));
			tessellator.PushTransform();

			var position = tessellator.Transform(Vector3.Zero);
			_rotation += 0.4f;

			tessellator.LoadIdentity();
			tessellator.Scale(0.5f, 0.5f);
			tessellator.Translate(-0.25f, -0.25f); // center the rotation
			tessellator.Rotate(_rotation, 0, 0, 1);
			tessellator.Translate(position);
			tessellator.Translate(0.5f, 0.5f); // center on the current tile position

			base.Render(tessellator);

			tessellator.PopTransform();
		}

		public override void Touch(Entity touchedBy)
		{
			base.Touch(touchedBy);

			if (_color == Color.White)
			{
				_color = Color.Red;
			}
			else
			{
				_color = Color.White;
			}
		}

		public override string ToString()
		{
			return $"BlockEntity(blockId:={_blockId})";
		}

		#endregion
	}
}
