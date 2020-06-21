using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoCalc.Core
{
    public class MsdnHashViewModel : HashViewModel
    {
        public MsdnHashViewModel()
        {
            HashList = new HashItemListViewModel(false);
        }
    }
}
