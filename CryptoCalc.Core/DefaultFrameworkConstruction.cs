namespace CryptoCalc.Core
{
    /// <summary>
    /// Creates a default framework construction containing all 
    /// the default configuration and services
    /// </summary>
    /// <example>
    /// 
    /// <para>
    ///     This is an example setup code for building a Dna Framework Construction
    /// </para>
    /// 
    /// <code>
    /// 
    ///     // Build the framework adding any required services
    ///     Framework.Construct&lt;DefaultFrameworkConstruction&gt;()
    ///             .AddFileLogger()
    ///             .Build();
    ///             
    /// </code>
    /// 
    /// </example>
    public class DefaultFrameworkConstruction : FrameworkConstruction
    {
        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public DefaultFrameworkConstruction()
        {
        }

        #endregion
    }
}