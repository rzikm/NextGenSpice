namespace NextGenSpice.Core.Helpers
{
    public class StateHelper<T> where T : struct
    {
        public ref T Value => ref value;

        private T value;
        private T backup;

        public void Rollback()
        {
            Value = backup;
        }

        public void Commit()
        {
            backup = Value;
        }
    }
}