using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;

namespace CryptoCalc.Core
{
    /// <summary>
    /// The view model for the Tree view control
    /// </summary>
    public class TreeViewModel
    {
        #region Public Properties

        /// <summary>
        /// The name of the tree view item
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The logical drives are the start off point for this tree view
        /// </summary>
        public ObservableCollection<TreeItemViewModel> LogicalDrives { get; set; } = new ObservableCollection<TreeItemViewModel>();

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public TreeViewModel()
        {
        }

        /// <summary>
        /// Overloaded constructor
        /// </summary>
        /// <param name="FileSelectedCommand">The command to execute when selecting an item</param>
        public TreeViewModel(ICommand fileSelectedCommand)
        {
            GetAllLogicalDrives(fileSelectedCommand);
        }

        /// <summary>
        /// Gets all the logical drives to start off the tree view
        /// </summary>
        /// <param name="fileSelectedCommand">The command to execute when selecting an item</param>
        private void GetAllLogicalDrives(ICommand fileSelectedCommand)
        {
            foreach (var item in Directory.GetLogicalDrives())
            {
                LogicalDrives.Add(new TreeItemViewModel(item, fileSelectedCommand, TreeItemType.LogicalDrive)
                {
                    Name = item
                });
            }
        }

        #endregion
    }
}
