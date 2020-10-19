using System;
using System.ServiceProcess;
using System.Windows;
using CryptoCalc.Core;
using Dna;
using Microsoft.Extensions.DependencyInjection;

namespace CryptoCalc
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Custom setup which loads the Ioc immeadialty after OnStartup
        /// </summary>
        /// <param name="e"></param>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            //Setup the main application
            ApplicationSetup();


            //Show the main window
            Current.MainWindow = new MainWindow();
            Current.MainWindow.Show();
        }

        /// <summary>
        /// Configure the application
        /// </summary>
        private void ApplicationSetup()
        {
            // Setup the Dna Framework
            Framework.Construct<DefaultFrameworkConstruction>()
                .AddCryptoCalcViewModels()
                .AddCryptoCalcClientServices()
                .Build();
        }
    }
}
