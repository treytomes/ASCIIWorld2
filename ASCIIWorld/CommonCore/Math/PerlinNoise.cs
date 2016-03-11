namespace CommonCore.Math
{
	public class PerlinNoise
	{
		#region Fields

		private double _persistence;
		private double _frequency;
		private double _amplitude;
		private int _octaves;
		private int _randomSeed;

		#endregion

		#region Constructors

		public PerlinNoise(double persistence = 0, double frequency = 0, double amplitude = 0, int octaves = 0, int randomSeed = 0)
		{
			_persistence = persistence;
			_frequency = frequency;
			_amplitude = amplitude;
			_octaves = octaves;
			_randomSeed = 2 + randomSeed * randomSeed;
		}

		#endregion

		#region Properties

		public double Persistence
		{
			get
			{
				return _persistence;
			}
			set
			{
				_persistence = value;
			}
		}

		public double Frequency
		{
			get
			{
				return _frequency;
			}
			set
			{
				_frequency = value;
			}
		}

		public double Amplitude
		{
			get
			{
				return _amplitude;
			}
			set
			{
				_amplitude = value;
			}
		}

		public int Octaves
		{
			get
			{
				return _octaves;
			}
			set
			{
				_octaves = value;
			}
		}

		public int RandomSeed
		{
			get
			{
				return _randomSeed;
			}
			set
			{
				_randomSeed = value;
			}
		}

		#endregion

		#region Methods

		public double GetHeight(double x, double y)
		{
			return _amplitude * Total(x, y);
		}

		private double Total(double i, double j)
		{
			// Properties of 1 octave (changing each loop).
			var t = 0.0;
			var amplitude = 1.0;
			var freq = _frequency;

			for (var k = 0; k < _octaves; k++)
			{
				t += GetValue(j * freq + _randomSeed, i * freq + _randomSeed) * amplitude;
				amplitude *= _persistence;
				freq *= 2;
			}

			return t;
		}

		private double GetValue(double x, double y)
		{
			var Xint = (int)x;
			var Yint = (int)y;
			var Xfrac = x - Xint;
			var Yfrac = y - Yint;

			// Noise values.
			var n01 = Noise(Xint - 1, Yint - 1);
			var n02 = Noise(Xint + 1, Yint - 1);
			var n03 = Noise(Xint - 1, Yint + 1);
			var n04 = Noise(Xint + 1, Yint + 1);
			var n05 = Noise(Xint - 1, Yint);
			var n06 = Noise(Xint + 1, Yint);
			var n07 = Noise(Xint, Yint - 1);
			var n08 = Noise(Xint, Yint + 1);
			var n09 = Noise(Xint, Yint);

			var n12 = Noise(Xint + 2, Yint - 1);
			var n14 = Noise(Xint + 2, Yint + 1);
			var n16 = Noise(Xint + 2, Yint);

			var n23 = Noise(Xint - 1, Yint + 2);
			var n24 = Noise(Xint + 1, Yint + 2);
			var n28 = Noise(Xint, Yint + 2);

			var n34 = Noise(Xint + 2, Yint + 2);

			// Find the noise values of the four corners.
			var x0y0 = 0.0625 * (n01 + n02 + n03 + n04) + 0.125 * (n05 + n06 + n07 + n08) + 0.25 * (n09);
			var x1y0 = 0.0625 * (n07 + n12 + n08 + n14) + 0.125 * (n09 + n16 + n02 + n04) + 0.25 * (n06);
			var x0y1 = 0.0625 * (n05 + n06 + n23 + n24) + 0.125 * (n03 + n04 + n09 + n28) + 0.25 * (n08);
			var x1y1 = 0.0625 * (n09 + n16 + n28 + n34) + 0.125 * (n08 + n14 + n06 + n24) + 0.25 * (n04);

			// Interpolate between those values according to the x and y fractions.
			var v1 = Interpolate(x0y0, x1y0, Xfrac); // interpolate in x direction (y)
			var v2 = Interpolate(x0y1, x1y1, Xfrac); // interpolate in x direction (y+1)
			var fin = Interpolate(v1, v2, Yfrac);    // interpolate in y direction

			return fin;
		}

		private double Interpolate(double x, double y, double a)
		{
			var negA = 1.0 - a;
			var negASqr = negA * negA;
			var fac1 = 3.0 * (negASqr) - 2.0 * (negASqr * negA);
			var aSqr = a * a;
			var fac2 = 3.0 * aSqr - 2.0 * (aSqr * a);

			return x * fac1 + y * fac2; // add the weighted factors
		}

		private double Noise(int x, int y)
		{
			var n = x + y * 57;
			n = (n << 13) ^ n;
			var t = (n * (n * n * 15731 + 789221) + 1376312589) & 0x7fffffff;
			return 1.0 - (double)t * 0.931322574615478515625e-9; /// 1073741824.0);
		}

		#endregion
	}
}
