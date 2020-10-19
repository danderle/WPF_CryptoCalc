using CryptoCalc.Core;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace CryptoCalc
{
    /// <summary>
    /// The view model for a tree view item
    /// </summary>
    public class TreeItemViewModel
    {
        #region Private fields

        /// <summary>
        /// Flag to let us know if the tree view item is expanded
        /// </summary>
        private bool isExpanded = false;

        /// <summary>
        /// Flag when whe tree item is selected
        /// </summary>
        private bool isSelected = false;

        /// <summary>
        /// The full path of the item
        /// </summary>
        private string fullPath = string.Empty;

        #endregion

        #region Public properties

        /// <summary>
        /// The name of the tree view item
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The type of tree item
        /// </summary>
        public TreeItemType Type { get; set; }

        /// <summary>
        /// Flag to let us know if the tree view item is expanded
        /// </summary>
        public bool IsExpanded
        {
            get => isExpanded;
            set
            {
                isExpanded = value;
                if (isExpanded)
                {
                    LoadChildren();
                }
                else
                {
                    if (Children.Count > 0)
                    {
                        Children.Clear();
                        Children.Add(null);
                    }
                }
            }
        }

        /// <summary>
        /// Flag when whe tree item is selected
        /// </summary>
        public bool IsSelected 
        { 
            get => isSelected;
            set
            {
                isSelected = value;
                if(isSelected && Type.Equals(TreeItemType.File))
                {
                    FileSelectedCommand.Execute(fullPath);
                }
            }
        }

        /// <summary>
        /// The tree view item children or subitems
        /// </summary>
        public ObservableCollection<TreeItemViewModel> Children { get; set; } = new ObservableCollection<TreeItemViewModel>();

        #endregion

        #region Commands

        /// <summary>
        /// The command to execute when a file is selected
        /// </summary>
        public ICommand FileSelectedCommand { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public TreeItemViewModel()
        {
        }

        /// <summary>
        /// Overloaded constructor
        /// </summary>
        /// <param name="path">the path of ths tree view item</param>
        /// <param name="isDirectory">Flag to determine if the item is a directory</param>
        public TreeItemViewModel(string path, ICommand fileSelectedCommand, TreeItemType type)
        {
            //Initialize properties and fields
            FileSelectedCommand = fileSelectedCommand;
            fullPath = path;
            var trimmedPath = path.TrimEnd(Path.DirectorySeparatorChar);
            Name = trimmedPath.Split(Path.DirectorySeparatorChar).Last();
            Type = type;

            //Add dummy child if not a file
            if (!type.Equals(TreeItemType.File))
            {
                Children.Add(null);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Methods to load the children of a directory tree view item
        /// </summary>
        private void LoadChildren()
        {
            try
            {
                if (Directory.Exists(fullPath))
                {
                    Children.Clear();
                    var directoryInfo = new DirectoryInfo(fullPath);

                    //Check if the current path has the hidden or read only flag set, true if not
                    if (!directoryInfo.Attributes.HasFlag(FileAttributes.Hidden | FileAttributes.ReadOnly))
                    {
                        //Check if the current path is a directory
                        if (directoryInfo.Attributes.HasFlag(FileAttributes.Directory))
                        {
                            //Get directories inside this directory
                            GetAllSubdirectories();

                            //Get all files inside this directory
                            GetAllFiles();
                        }
                    }
                }
            }
            catch(Exception e) 
            {
                Dialog.OpenErrorMessageBoxAsync(e, "Directory/File Load Error", WindowDialogType.Error);
            }
        }

        /// <summary>
        /// Gets all directories inside the directory path
        /// </summary>
        private void GetAllSubdirectories()
        {
            var items = Directory.GetDirectories(fullPath);
            if (items.Length > 0)
            {
                foreach (var item in items)
                {
                    FileAttributes itemAttribute = File.GetAttributes(item);
                    if (!itemAttribute.HasFlag(FileAttributes.Hidden))
                    {
                        Children.Add(new TreeItemViewModel(item, FileSelectedCommand, TreeItemType.Directory)
                        {
                        });
                    }
                }
            }
        }

        /// <summary>
        /// Gets all files inside the directory path
        /// </summary>
        private void GetAllFiles()
        {
            var items = Directory.GetFiles(fullPath);
            if (items.Length > 0)
            {
                foreach (var item in items)
                {
                    FileAttributes itemAttribute = File.GetAttributes(item);
                    if (!itemAttribute.HasFlag(FileAttributes.Hidden))
                    {
                        Children.Add(new TreeItemViewModel(item, FileSelectedCommand, TreeItemType.File)
                        {
                        });
                    }
                }
            }
        }
        #endregion
    }
}
