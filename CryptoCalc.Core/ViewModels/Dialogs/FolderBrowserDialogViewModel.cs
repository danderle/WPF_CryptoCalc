using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CryptoCalc.Core
{
    /// <summary>
    /// The view model for the <see cref="FolderBrowserDialogViewModel.xaml"/>
    /// </summary>
    public class FolderBrowserDialogViewModel : BaseDialogViewModel
    {
        #region Public Properties

        /// <summary>
        /// The message to display
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// The text to use for the ok button
        /// </summary>
        public string OkText { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<TreeItemViewModel> LogicalDrives { get; set; } = new List<TreeItemViewModel>();



        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public FolderBrowserDialogViewModel()
        {
            foreach(var item in Directory.GetLogicalDrives())
            {
                LogicalDrives.Add(new TreeItemViewModel(item)
                {
                    Name = item
                });
            }
        }

        #endregion

        #region MyRegion

        #endregion
    }
}
