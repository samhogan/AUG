using UnityEngine;
namespace LibNoise.Generator
{
    /// <summary>
    /// A noise module by sam! that outputs a temperature gradient
	/// if you want to know what this means, then ask sam 
    /// </summary>
    public class TempGrad : ModuleBase
    {
      
        private double startValue;//the value at 0,0,0
		private double increment;//how much to increase the temp by each y unit

  
		public TempGrad()
            : base(0)
        {
        }

        
		//sv=startValue, pv=poleValue, r=planet radius
		public TempGrad(double sv, double pv, double r)
            : base(0)
        {
            startValue = sv;

			increment = (pv - sv) / r;
        }

        /// <summary>
        /// Returns the output value for the given input coordinates.
        /// </summary>
        /// <param name="x">The input coordinate on the x-axis.</param>
        /// <param name="y">The input coordinate on the y-axis.</param>
        /// <param name="z">The input coordinate on the z-axis.</param>
        /// <returns>The resulting output value.</returns>
        public override double GetValue(double x, double y, double z)
        {
			return Mathf.Abs((float)y) * increment + startValue;
        }


    }
}