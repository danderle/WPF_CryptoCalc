using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoCalc.Core
{
    public class BouncyHashViewModel : HashViewModel
    {
        public BouncyHashViewModel()
        {
            HashList = new HashItemListViewModel(true);
        }
    }
}
