using NextGenSpice.Core.Helpers;
using Xunit;

namespace NextGenSpiceTest
{
    public class StateHelperTests
    {
        private struct MyState
        {
            public int Int;
        }

        private readonly StateHelper<int> intHelper;
        private readonly StateHelper<MyState> stateHelper;

        public StateHelperTests()
        {
            intHelper = new StateHelper<int>();
            stateHelper = new StateHelper<MyState>();
        }

        [Fact]
        public void PersistsValue()
        {
            intHelper.Value = 5;
            intHelper.Commit();

            Assert.Equal(5, intHelper.Value);

            intHelper.Value = 100;
            Assert.Equal(100, intHelper.Value);

            intHelper.Rollback();
            Assert.Equal(5, intHelper.Value);
        }

        [Fact]
        public void PersistsValueTypeInStructType()
        {
            stateHelper.Value.Int = 5;
            stateHelper.Commit();

            Assert.Equal(5, stateHelper.Value.Int);

            stateHelper.Value.Int = 100;
            Assert.Equal(100, stateHelper.Value.Int);

            stateHelper.Rollback();
            Assert.Equal(5, stateHelper.Value.Int);
        }
    }
}