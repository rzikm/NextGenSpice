namespace NextGenSpice.Core.Helpers
{
    /// <summary>
    /// Helper class for managing revertable state changes.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class StateHelper<T> where T : struct
    {
        /// <summary>
        /// Stored state value.
        /// </summary>
        public ref T Value => ref value;

        private T value;
        private T backup;

        /// <summary>
        /// Rollback to last commited value.
        /// </summary>
        public void Rollback()
        {
            Value = backup;
        }

        /// <summary>
        /// Backs up the current value so it can be reverted to later.
        /// </summary>
        public void Commit()
        {
            backup = Value;
        }
    }
}