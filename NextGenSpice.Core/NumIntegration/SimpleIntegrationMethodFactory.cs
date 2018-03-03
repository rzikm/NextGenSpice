using System;

namespace NextGenSpice.Core.NumIntegration
{
    /// <summary>
    ///     Simple wrapper around a functor returning new instance of IIntegrationMethod implementation.
    /// </summary>
    public class SimpleIntegrationMethodFactory : IIntegrationMethodFactory
    {
        private readonly Func<IIntegrationMethod> factoryFunc;

        /// <summary>Initializes a new instance of the <see cref="T:System.Object"></see> class.</summary>
        public SimpleIntegrationMethodFactory(Func<IIntegrationMethod> factoryFunc)
        {
            this.factoryFunc = factoryFunc;
        }
        
        /// <summary>
        ///     Creates new instance of the integration method implementation.
        /// </summary>
        /// <returns></returns>
        public IIntegrationMethod CreateInstance()
        {
            return factoryFunc();
        }
    }
}