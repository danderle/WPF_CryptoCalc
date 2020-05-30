using System.Collections.ObjectModel;

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
                new TreeItemViewModel("Item1")
                {
                    SubItems = new ObservableCollection<TreeItemViewModel>
                    {
                        new TreeItemViewModel("Item1.1"),
                        new TreeItemViewModel("Item1.2"),
                        new TreeItemViewModel("Item1.3"),
                    }
                },
                new TreeItemViewModel("Item2"),
                new TreeItemViewModel("Item3"),
                new TreeItemViewModel("Item4"),
            };
        } 

        #endregion
    }
}
