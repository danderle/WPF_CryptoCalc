using CryptoCalc.Core;
using System.Collections.ObjectModel;

namespace CryptoCalc
{
    public class FolderBrowserDialogDesignModel : FolderBrowserDialogViewModel
    {
        #region Singleton

        /// <summary>
        /// A single instance of the design model
        /// </summary>
        public static FolderBrowserDialogDesignModel Instance => new FolderBrowserDialogDesignModel();

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public FolderBrowserDialogDesignModel()
        {
            Title = "Desgin Time";
            FolderDialogTree = new TreeViewModel(null)
            {
                LogicalDrives = new ObservableCollection<TreeItemViewModel>
                {
                    new TreeItemViewModel("Item1", null, TreeItemType.LogicalDrive)
                    {
                        IsExpanded = true,
                        Children = new ObservableCollection<TreeItemViewModel>
                        {
                            new TreeItemViewModel("Item1.1", null, TreeItemType.Directory),
                            new TreeItemViewModel("Item1.2", null, TreeItemType.Directory),
                            new TreeItemViewModel("Item1.3", null, TreeItemType.Directory),
                        }
                    },
                    new TreeItemViewModel("Item2", null, TreeItemType.File),
                    new TreeItemViewModel("Item3", null, TreeItemType.File),
                    new TreeItemViewModel("Item4", null, TreeItemType.File),
                }
            };
        } 

        #endregion
    }
}
