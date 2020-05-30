using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;

namespace CryptoCalc.Core
{
    public class TreeItemViewModel
    {
        public string Name { get; set; }

        public ObservableCollection<TreeItemViewModel> SubItems { get; set; } = new ObservableCollection<TreeItemViewModel>();

        public TreeItemViewModel()
        {

        }

        public TreeItemViewModel(string name)
        {
            Name = name;
            SubItems.Add(null);
            //try
            //{
            //    if (Directory.Exists(Name))
            //    {
            //        var security = new DirectoryInfo(Name);
            //        switch (security.Attributes)
            //        {
            //            //case FileAttributes.Normal:
            //            //case FileAttributes.ReadOnly:
            //            //    var list = Directory.GetDirectories(Name);
            //            //    if (list.Length > 0)
            //            //    {
            //            //        foreach (var item in list)
            //            //        {
            //            //            SubItems.Add(new TreeViewItemViewModel(item)
            //            //            {
            //            //            });
            //            //        }
            //            //    }
            //            //    break;
            //            case FileAttributes.Directory | FileAttributes.Hidden | FileAttributes.System:
            //                var items = Directory.GetDirectories(Name);
            //                if (items.Length > 0)
            //                {
            //                    foreach (var item in items)
            //                    {
            //                        SubItems.Add(new TreeItemViewModel(item)
            //                        {
            //                        });
            //                    }
            //                }
            //                break;
            //        }
            //    }
            //}
            //catch { }
        }
    }
}
