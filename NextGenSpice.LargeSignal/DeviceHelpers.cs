using System;

namespace NextGenSpice.LargeSignal.Devices
{
    /// <summary>
    /// Helper class for calculations for semiconductor devices
    /// </summary>
    public static class DeviceHelpers
    {
        /// <summary>
        /// Limits voltage change to prevent numerical overflows.
        /// </summary>
        /// <param name="voltage">Voltage over the diode.</param>
        /// <param name="oldVoltage">Previous voltage over the didode.</param>
        /// <param name="thermalVoltage">Thermal voltage of the PN junction.</param>
        /// <param name="criticalVoltage">Critical voltage of the PN junction.</param>
        /// <returns></returns>
        public static double PnLimitVoltage(double voltage, double oldVoltage, double thermalVoltage, double criticalVoltage)
        {
            double arg;
            if (voltage > criticalVoltage && Math.Abs(voltage - oldVoltage) > 2 * thermalVoltage)
            {
                if (oldVoltage > 0)
                {
                    arg = (voltage - oldVoltage) / thermalVoltage;
                    if (arg > 0)
                        voltage = oldVoltage + thermalVoltage * (2 + Math.Log10(arg - 2));
                    else
                        voltage = oldVoltage - thermalVoltage * (2 + Math.Log10(2 - arg));
                }
                else voltage = oldVoltage < 0 ? thermalVoltage * Math.Log10(voltage / thermalVoltage) : criticalVoltage;
            }
            else
            {
                if (voltage < 0)
                {
                    arg = oldVoltage > 0 ? -1 - oldVoltage : 2 * oldVoltage - 1;
                    if (voltage < arg) voltage = arg;
                }
            }
            return voltage;
        }

        /// <summary>
        /// Calculates the current across the PN junction and its derivative.
        /// </summary>
        /// <param name="saturationCurrent">Saturation current PN junction.</param>
        /// <param name="voltage">Voltage across the PN junction.</param>
        /// <param name="thermalVoltage">Thermal voltage of the PN junction.</param>
        /// <param name="current">Current of the diode.</param>
        /// <param name="conductance">Equivalent conductance of the diode (derivative of the current).</param>
        public static void PnJunction(double saturationCurrent, double voltage, double thermalVoltage, out double current,
            out double conductance)
        {
            if (voltage < -3 * thermalVoltage)
            {
                double a = 3 * thermalVoltage / (voltage * Math.E);
                a = a * a * a;
                current = -saturationCurrent * (1 + a);
                conductance = +saturationCurrent * 3 * a / voltage;
                return;
            }

            var e = Math.Exp(voltage / thermalVoltage);
            current = saturationCurrent * (e - 1);
            conductance = saturationCurrent * e / thermalVoltage;
        }

        /// <summary>
        /// Calculates the ciritcal voltage of the PN junction.
        /// </summary>
        /// <param name="saturationCurrent">Saturation current of the PN junction.</param>
        /// <param name="thermalVoltage">Thermal voltage of the PN junction.</param>
        /// <returns></returns>
        public static double PnCriticalVoltage(double saturationCurrent, double thermalVoltage)
        {
            return thermalVoltage * Math.Log(thermalVoltage / Math.Sqrt(2) / saturationCurrent);
        }
    }
}