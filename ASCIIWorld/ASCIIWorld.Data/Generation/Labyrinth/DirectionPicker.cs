using CommonCore.Math;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ASCIIWorld.Data.Generation.Labyrinth
{
	public class DirectionPicker
	{
		#region Fields

		private Random _random;
		private List<Direction> _directionsPicked;
		private Direction _previousDirection;
		private double _changeDirectionModifer;

		#endregion

		#region Constructors

		/// <param name="changeDirectionModifier">Value from 0.0 to 1.0.  Percentage chance of changing direction.</param>
		public DirectionPicker(Random random, Direction previousDirection, double changeDirectionModifier)
		{
			_random = random;
			_directionsPicked = new List<Direction>();
			_previousDirection = previousDirection;
			_changeDirectionModifer = changeDirectionModifier;
		}

		#endregion

		#region Properties

		public bool HasNextDirection
		{
			get
			{
				return _directionsPicked.Count < Direction.All.Count();
			}
		}

		#endregion

		#region Methods

		public Direction GetNextDirection()
		{
			if (!HasNextDirection)
			{
				throw new InvalidOperationException("No directions available.");
			}

			Direction directionPicked;
			do
			{
				directionPicked = MustChangeDirection(_changeDirectionModifer) ? PickDifferentDirection() : _previousDirection;
			}
			while (_directionsPicked.Contains(directionPicked));

			_directionsPicked.Add(directionPicked);
			return directionPicked;
		}

		private Direction PickDifferentDirection()
		{
			Direction directionPicked;
			do
			{
				directionPicked = Direction.Random;
			} while ((directionPicked == _previousDirection) && (_directionsPicked.Count < 3));
			return directionPicked;
		}

		/// <param name="changeDirectionModifier">Value from 0.0 to 1.0.  Percentage chance of changing direction.</param>
		public bool MustChangeDirection(double changeDirectionModifier)
		{
			return (_directionsPicked.Count > 0) || (changeDirectionModifier > _random.NextDouble());
		}

		#endregion
	}
}