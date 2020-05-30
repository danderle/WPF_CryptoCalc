using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Design;
using System.IO;

namespace CryptoCalc.Core
{
    public class TreeViewModel
    {
        public string Name { get; set; }

        public ObservableCollection<TreeItemViewModel> LogicalDrives { get; set; } = new ObservableCollection<TreeItemViewModel>();

        public TreeViewModel()
        {
            foreach (var item in Directory.GetLogicalDrives())
            {
                LogicalDrives.Add(new TreeItemViewModel(item)
                {
                    Name = item
                });
            }
        }
    }
}
