using System.Collections.ObjectModel;
using System.Windows.Input;

namespace CryptoCalc.Core
{
    public class TreeDesignModel : TreeViewModel
    {
        #region Singleton

        /// <summary>
        /// A single instance of the design model
        /// </summary>
        public static TreeDesignModel Instance => new TreeDesignModel();

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public TreeDesignModel()
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
            };
        } 

        #endregion
    }
}
