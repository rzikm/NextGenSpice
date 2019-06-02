using NextGenSpice.Core.Devices.Parameters;
using NextGenSpice.Parser.Utils;
using Xunit;

namespace NextGenSpice.Parser.Test
{
    public class ParameterMapperTests
    {
        [Fact]
        public void MapsProperties()
        {
            var mapper = new ParameterMapper<DiodeParams>();
            mapper.Target = DiodeParams.Default;
            mapper.Map(p => p.SaturationCurrent, "IS");
            mapper.Map(p => p.SaturationCurrent, 0);

            mapper.Set("IS", 1);
            Assert.Equal(1, mapper.Target.SaturationCurrent);

            mapper.Set(0, 45);
            Assert.Equal(45, mapper.Target.SaturationCurrent);
        }
    }
}