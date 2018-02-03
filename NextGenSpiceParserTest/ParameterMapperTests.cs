using NextGenSpice;
using NextGenSpice.Core.Elements;
using NextGenSpice.Utils;
using Xunit;

namespace NextGenSpiceParserTest
{
    public class ParameterMapperTests
    {
        [Fact]
        public void MapsProperties()
        {
            ParameterMapper<DiodeModelParams> mapper = new ParameterMapper<DiodeModelParams>();
            mapper.Target = DiodeModelParams.Default;
            mapper.Map(p => p.SaturationCurrent, "IS");
            mapper.Map(p => p.SaturationCurrent, 0);

            mapper.Set("IS", 1);
            Assert.Equal(1, mapper.Target.SaturationCurrent);

            mapper.Set(0, 45);
            Assert.Equal(45, mapper.Target.SaturationCurrent);
        }
    }
}