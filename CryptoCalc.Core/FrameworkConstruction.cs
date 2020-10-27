using Microsoft.Extensions.DependencyInjection;
using System;

namespace CryptoCalc.Core
{
    /// <summary>
    /// The construction information when starting up and configuring Dna.Framework
    /// </summary>
    public class FrameworkConstruction
    {
        #region Protected Members

        /// <summary>
        /// The services that will get used and compiled once the framework is built
        /// </summary>
        protected IServiceCollection mServices;

        #endregion

        #region Public Properties

        /// <summary>
        /// The dependency injection service provider
        /// </summary>
        public IServiceProvider Provider { get; protected set; }

        /// <summary>
        /// The services that will get used and compiled once the framework is built
        /// </summary>
        public IServiceCollection Services
        {
            get => mServices;
            set
            {
                // Set services
                mServices = value;
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor, a new <see cref="ServiceCollection"/> will be created for the Services</param>
        /// </summary>
        public FrameworkConstruction()
        {
            // Create a new list of dependencies
            Services = new ServiceCollection();
        }

        #endregion

        #region Build Methods

        /// <summary>
        /// Builds the service collection into a service provider
        /// </summary>
        public void Build(IServiceProvider provider = null)
        {
            // Use given provider or build it
            Provider = provider ?? Services.BuildServiceProvider();
        }

        #endregion
    }
}
