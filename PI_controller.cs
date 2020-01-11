using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace HT
{
    /// <summary>
    /// The PI controller class
    /// </summary>
    public class PI_Controller
    {
        private double Kp_gain = 0.1;
        private int difference;

        private int unlimitedControl;
        private double integrator = 0.0;
        private double integrationTime = 0.01;
        private double controlPeriod;

        private DateTime lastUpdate = DateTime.MinValue;
        private DateTime nowTime = DateTime.MinValue;

        /// <value> Gets and sets goal pressure value </value>
        public int goal { get; set; } = 0;
        /// <value> Gets and sets control minumum limit value </value>
        public int minLimit { get; set; } = 0;
        /// <value> Gets and sets control maximum limit value </value>
        public int maxLimit { get; set; } = 0;

        /// <summary>
        /// Constructer of the PI class
        /// </summary>
        /// <param name="goal"> Goal pressure value </param>
        /// <param name="minLimit"> Minimum limit of the control value </param>
        /// <param name="maxLimit"> Maximum limit of the control value </param>
        public PI_Controller(int goal, int minLimit, int maxLimit)
        {
            this.goal = goal;
            this.minLimit = minLimit;
            this.maxLimit = maxLimit;
        }
        /// <summary>
        /// Method calculates the best control value to achieve
        /// goal pressure value from current pressure value
        /// </summary>
        /// <param name="currentPressure"> Current pressure of the container </param>
        /// <returns> The best control value </returns>
        public int getControlChange(int currentPressure)
        {
            // Difference of current and goal pressure value
            difference = currentPressure - goal;

            // Current call time
            nowTime = DateTime.Now;
            if (lastUpdate != DateTime.MinValue)
            {
                // Calculates the diffrence between current call time and the last call time
                controlPeriod = (nowTime - lastUpdate).TotalSeconds;

                // The formula for the PI controllers I component
                integrator = integrator + Kp_gain * integrationTime / controlPeriod * difference;
            }
            lastUpdate = nowTime;
            // The formula for the best control value
            unlimitedControl = (int)(Kp_gain * difference + integrator);

            System.Threading.Thread.Sleep(50);

            // Checks that calculated control value is between limits
            return clamp(unlimitedControl);
        }
        /// <summary>
        /// Method to check if calculated control value is between limits
        /// </summary>
        /// <param name="calculatedControl"> The calculated control value</param>
        /// <returns> Returns either calculated control or one of the limits </returns>
        private int clamp(int calculatedControl)
        {
            // Control is smaller than minimum limit
            if (calculatedControl < minLimit)
            {
                calculatedControl = minLimit;
            }
            // Control is bigger than maximum limit
            else if (calculatedControl > maxLimit)
            {
                calculatedControl = maxLimit;
            }
            return calculatedControl;
        }

    }
}
