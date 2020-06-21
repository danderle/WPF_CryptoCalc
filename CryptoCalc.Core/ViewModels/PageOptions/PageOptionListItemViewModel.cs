using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Input;

namespace CryptoCalc.Core
{
    /// <summary>
    /// The view model for the page option items
    /// </summary>
    public class PageOptionListItemViewModel : BaseViewModel
    {
        #region Public Properties

        public List<PageOptionItemViewModel> Items { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public PageOptionListItemViewModel()
        {
            //Initialize the commands
            Items = new List<PageOptionItemViewModel>
            {
                new PageOptionItemViewModel
                {
                    PageName = "MSDN Hash",
                },
                new PageOptionItemViewModel
                {
                    PageName = "MSDN Symmetric Encryption",
                },
                new PageOptionItemViewModel
                {
                    PageName = "MSDN Public Key Encryption",
                },
                new PageOptionItemViewModel
                {
                    PageName = "MSDN Digital Signature",
                },
                new PageOptionItemViewModel
                {
                    PageName = "MSDN Key Exchange",
                },
                new PageOptionItemViewModel
                {
                    PageName = "Bouncy Castle Hash",
                },
                new PageOptionItemViewModel
                {
                    PageName = "Bouncy Castle Symmetric Encryption",
                },
                new PageOptionItemViewModel
                {
                    PageName = "Bouncy Castle Public Key Encryption",
                },
                new PageOptionItemViewModel
                {
                    PageName = "Bouncy Castle Digital Signature",
                },
                new PageOptionItemViewModel
                {
                    PageName = "Bouncy Castle Key Exchange",
                },
            };
        }


        #endregion

    }
}
