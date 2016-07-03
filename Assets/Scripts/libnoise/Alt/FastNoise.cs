// 
// Copyright (c) 2013 Jason Bell
// 
// Permission is hereby granted, free of charge, to any person obtaining a 
// copy of this software and associated documentation files (the "Software"), 
// to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, 
// and/or sell copies of the Software, and to permit persons to whom the 
// Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included 
// in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS 
// OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.
// 

using System;

namespace LibNoise.Generator
{
    public class FastNoise:ModuleBase
    {
        public double Frequency { get; set; }
        public double Persistence { get; set; }
		public QualityMode NoiseQuality { get; set; }
        private int mOctaveCount;
        public double Lacunarity { get; set; }

        private const int MaxOctaves = 30;

        public FastNoise()
            : this(0)
        {

        }

		public FastNoise(int seed):base(0)
            
        {
			Seed = seed;
            Frequency = 1.0;
            Lacunarity = 2.0;
            OctaveCount = 6;
            Persistence = 0.5;
			NoiseQuality = QualityMode.High;
        }

        public override double GetValue(double x, double y, double z)
        {
            double value = 0.0;
            double signal = 0.0;
            double curPersistence = 1.0;
            long seed;

            x *= Frequency;
            y *= Frequency;
            z *= Frequency;

            for (int currentOctave = 0; currentOctave < OctaveCount; currentOctave++)
            {
                seed = (Seed + currentOctave) & 0xffffffff;
				//signal = Utils.GradientCoherentNoise3D (x, y, z, seed, NoiseQuality);//GradientCoherentNoise(x, y, z, (int)seed, NoiseQuality);
				signal = GradientCoherentNoise(x, y, z, (int)seed, NoiseQuality);
                value += signal * curPersistence;

                x *= Lacunarity;
                y *= Lacunarity;
                z *= Lacunarity;
                curPersistence *= Persistence;
            }

            return value;
        }

        public int OctaveCount
        {
            get { return mOctaveCount; }
            set
            {
                if (value < 1 || value > MaxOctaves)
                    throw new ArgumentException("Octave count must be greater than zero and less than " + MaxOctaves);

                mOctaveCount = value;
            }
        }


		private int[] RandomPermutations = new int[512];
		private int[] SelectedPermutations = new int[512];
		private float[] GradientTable = new float[512];

		private int mSeed;


	
		public double GradientCoherentNoise(double x, double y, double z, int seed, QualityMode noiseQuality)
		{
			int x0 = (x > 0.0 ? (int)x : (int)x - 1);
			int y0 = (y > 0.0 ? (int)y : (int)y - 1);
			int z0 = (z > 0.0 ? (int)z : (int)z - 1);

			int X = x0 & 255;
			int Y = y0 & 255;
			int Z = z0 & 255;

			double u = 0, v = 0, w = 0;
			switch (noiseQuality)
			{
			case QualityMode.Low:
				u = (x - x0);
				v = (y - y0);
				w = (z - z0);
				break;
			case QualityMode.Medium:
				u = SCurve3(x - x0);
				v = SCurve3(y - y0);
				w = SCurve3(z - z0);
				break;
			case QualityMode.High:
				u = SCurve5(x - x0);
				v = SCurve5(y - y0);
				w = SCurve5(z - z0);
				break;
			}

			int A = SelectedPermutations[X] + Y, AA = SelectedPermutations[A] + Z, AB = SelectedPermutations[A + 1] + Z,
			B = SelectedPermutations[X + 1] + Y, BA = SelectedPermutations[B] + Z, BB = SelectedPermutations[B + 1] + Z;

			double a = LinearInterpolate(GradientTable[AA], GradientTable[BA], u);
			double b = LinearInterpolate(GradientTable[AB], GradientTable[BB], u);
			double c = LinearInterpolate(a, b, v);
			double d = LinearInterpolate(GradientTable[AA + 1], GradientTable[BA + 1], u);
			double e = LinearInterpolate(GradientTable[AB + 1], GradientTable[BB + 1], u);
			double f = LinearInterpolate(d, e, v);
			return LinearInterpolate(c, f, w);
		}

		public int Seed
		{
			get { return mSeed; }
			set
			{
				mSeed = value;

				// Generate new random permutations with this seed.
				Random random = new Random(mSeed);
				for (int i = 0; i < 512; i++)
					RandomPermutations[i] = random.Next(255);
				for (int i = 0; i < 256; i++)
					SelectedPermutations[256 + i] = SelectedPermutations[i] = RandomPermutations[i];

				// Generate a new gradient table
				float[] kkf = new float[256];
				for (int i = 0; i < 256; i++)
					kkf[i] = -1.0f + 2.0f * ((float)i / 255.0f);

				for (int i = 0; i < 256; i++)
					GradientTable[i] = kkf[SelectedPermutations[i]];
				for (int i = 256; i < 512; i++)
					GradientTable[i] = GradientTable[i & 255];
			}
		}


		/// <summary>
		/// Returns the given value clamped between the given lower and upper bounds.
		/// </summary>
		public static int ClampValue(int value, int lowerBound, int upperBound)
		{
			if (value < lowerBound)
			{
				return lowerBound;
			}
			else if (value > upperBound)
			{
				return upperBound;
			}
			else
			{
				return value;
			}
		}

		/// <summary>
		/// Returns the cubic interpolation of two values bound between two other values.
		/// </summary>
		/// <param name="n0">The value before the first value.</param>
		/// <param name="n1">The first value.</param>
		/// <param name="n2">The second value.</param>
		/// <param name="n3">The value after the second value.</param>
		/// <param name="a">The alpha value.</param>
		/// <returns></returns>
		protected double CubicInterpolate(double n0, double n1, double n2, double n3, double a)
		{
			double p = (n3 - n2) - (n0 - n1);
			double q = (n0 - n1) - p;
			double r = n2 - n0;
			double s = n1;
			return p * a * a * a + q * a * a + r * a + s;
		}

		/// <summary>
		/// Returns the smaller of the two given numbers.
		/// </summary>
		public static double GetSmaller(double a, double b)
		{
			return (a < b ? a : b);
		}

		/// <summary>
		/// Returns the larger of the two given numbers.
		/// </summary>
		public static double GetLarger(double a, double b)
		{
			return (a > b ? a : b);
		}

		/// <summary>
		/// Swaps the values contained by the two given variables.
		/// </summary>
		public static void SwapValues(ref double a, ref double b)
		{
			double c = a;
			a = b;
			b = c;
		}

		/// <summary>
		/// Returns the linear interpolation of two values with the given alpha.
		/// </summary>
		protected double LinearInterpolate(double n0, double n1, double a)
		{
			return ((1.0 - a) * n0) + (a * n1);
		}

		/// <summary>
		/// Returns the given value, modified to be able to fit into a 32-bit integer.
		/// </summary>
		/*public double MakeInt32Range(double n)
        {
            if (n >= 1073741824.0)
            {
                return ((2.0 * System.Math.IEEERemainder(n, 1073741824.0)) - 1073741824.0);
            }
            else if (n <= -1073741824.0)
            {
                return ((2.0 * System.Math.IEEERemainder(n, 1073741824.0)) + 1073741824.0);
            }
            else
            {
                return n;
            }
        }*/

		/// <summary>
		/// Returns the given value mapped onto a cubic S-curve.
		/// </summary>
		protected double SCurve3(double a)
		{
			return (a * a * (3.0 - 2.0 * a));
		}

		/// <summary>
		/// Returns the given value mapped onto a quintic S-curve.
		/// </summary>
		protected double SCurve5(double a)
		{
			double a3 = a * a * a;
			double a4 = a3 * a;
			double a5 = a4 * a;
			return (6.0 * a5) - (15.0 * a4) + (10.0 * a3);
		}

		/// <summary>
		/// Returns the value of the mathematical constant PI.
		/// </summary>
		public static readonly double PI = 3.1415926535897932385;

		/// <summary>
		/// Returns the square root of 2.
		/// </summary>
		public static readonly double Sqrt2 = 1.4142135623730950488;

		/// <summary>
		/// Returns the square root of 3.
		/// </summary>
		public static readonly double Sqrt3 = 1.7320508075688772935;

		/// <summary>
		/// Returns PI/180.0, used for converting degrees to radians.
		/// </summary>
		public static readonly double DEG_TO_RAD = PI / 180.0;

		/// <summary>
		/// Provides the X, Y, and Z coordinates on the surface of a sphere 
		/// cooresponding to the given latitude and longitude.
		/// </summary>
		protected void LatLonToXYZ(double lat, double lon, ref double x, ref double y, ref double z)
		{
			double r = System.Math.Cos (DEG_TO_RAD * lat);
			x = r * System.Math.Cos(DEG_TO_RAD * lon);
			y = System.Math.Sin(DEG_TO_RAD * lat);
			z = r * System.Math.Sin(DEG_TO_RAD * lon);
		}
    }


}
