using NextGenSpice.LargeSignal.NumIntegration;
using Xunit;

namespace NextGenSpiceTest
{
    public class MiscTests
    {
        [Fact]
        public void GeneratesAdamsMoultonCoefficients()
        {
            var coeffs = AdamsMoultonIntegrationMethod.GetCoefficients(4);
            Assert.Equal(new []{9/24.0, 19/24.0, -5/24.0, 1/24.0}, coeffs, new DoubleComparer(1e-13));
        }

        [Fact]
        public void GeneratesGearCoefficients()
        {
            var coeffs = GearIntegrationMethod.GetCoefficients(4);
            Assert.Equal(new[] { 12/25.0, 48/25.0, -36/25.0, 16/25.0,-3/25.0 }, coeffs, new DoubleComparer(1e-13));
        }
    }
}