namespace NextGenSpice.Core.NumIntegration
{
    /// <summary>
    ///     Defines Method for creating new instance of IIntegrationMethod object.
    /// </summary>
    public interface IIntegrationMethodFactory
    {
        /// <summary>
        ///     Creates new instance of the integration method implementation.
        /// </summary>
        /// <returns></returns>
        IIntegrationMethod CreateInstance();
    }
}