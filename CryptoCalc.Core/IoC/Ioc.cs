using Ninject;

namespace CryptoCalc.Core
{
    /// <summary>
    /// The Ioc container for the application
    /// </summary>
    public static class Ioc
    {
        #region Public Properties
        
        /// <summary>
        /// The kernek for the Ioc container
        /// </summary>
        public static IKernel Kernal { get; private set; } = new StandardKernel();

        /// <summary>
        /// A shortcut to acces the <see cref="IUIManager"/>
        /// </summary>
        public static IUIManager UI => Get<IUIManager>();

        /// <summary>
        /// Gets the Application view model
        /// </summary>
        public static ApplicationViewModel Application => Ioc.Get<ApplicationViewModel>();

        /// <summary>
        /// Gets the Data format view model
        /// </summary>
        public static DataFormatViewModel DataFormat => Ioc.Get<DataFormatViewModel>();

        #endregion

        #region Conbstruction

        /// <summary>
        /// Sets up the Ioc container, binds all information required and is ready for use
        /// NOTE: Must be called as soon as the application starts up to ensure all services can be found
        /// </summary>
        public static void Setup()
        {
            // Bind all requird viewmodels
            BindViewModels();
        }

        /// <summary>
        /// Binds the all the singleton view models
        /// </summary>
        private static void BindViewModels()
        {
            //Bind to a single instance of the application view model
            Kernal.Bind<ApplicationViewModel>().ToConstant(new ApplicationViewModel());

            //Bind to a single instance of the data format view model
            Kernal.Bind<DataFormatViewModel>().ToConstant(new DataFormatViewModel());
        }

        #endregion

        /// <summary>
        /// Gets aservice of the Ioc, of the specified type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Get<T>()
        {
            return Kernal.Get<T>();
        }

        
    }
}
