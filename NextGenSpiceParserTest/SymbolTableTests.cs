using NextGenSpice.Core.Devices.Parameters;
using NextGenSpice.Core.Parser;
using Xunit;

namespace NextGenSpiceParserTest
{
    public class SymbolTableTests
    {
        public SymbolTableTests()
        {
            table = new SymbolTable();
            table.AddModel(new DiodeModelParams(), "default");
            table.FreezeDefaults();
        }

        private readonly SymbolTable table;

        [Fact]
        public void DoesNotShowUpperScopeModels()
        {
            table.AddModel(new DiodeModelParams(), "D");

            Assert.True(table.TryGetModel<DiodeModelParams>("D", out var m));
            Assert.NotNull(m);

            table.EnterSubcircuit();
            Assert.False(table.TryGetModel<DiodeModelParams>("D", out m));
            Assert.Null(m);
            table.ExitSubcircuit();

            Assert.True(table.TryGetModel<DiodeModelParams>("D", out m));
            Assert.NotNull(m);
        }

        [Fact]
        public void HidesInnerModelsFromOuterScope()
        {
            table.EnterSubcircuit();
            table.AddModel(new DiodeModelParams(), "D");

            Assert.True(table.TryGetModel<DiodeModelParams>("D", out var m));
            Assert.NotNull(m);

            table.ExitSubcircuit();
            Assert.False(table.TryGetModel<DiodeModelParams>("D", out m));
            Assert.Null(m);
        }

        [Fact]
        public void IncrementsScope()
        {
            table.EnterSubcircuit();

            Assert.Equal(1, table.SubcircuitDepth);

            table.ExitSubcircuit();

            Assert.Equal(0, table.SubcircuitDepth);
        }

        [Fact]
        public void ShowsDefaultModels()
        {
            table.EnterSubcircuit();

            Assert.True(table.TryGetModel<DiodeModelParams>("default", out var m));
            Assert.NotNull(m);
        }
    }
}