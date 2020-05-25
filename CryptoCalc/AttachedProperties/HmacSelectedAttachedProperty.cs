using CryptoCalc.Core;
using System.Windows;
using System.Windows.Media;

namespace CryptoCalc
{
    /// <summary>
    /// Collapses the hash algorithims that do not implement the Hmac option
    /// </summary>
    public class HmacSelectedProperty : BaseAttachedProperty<HmacSelectedProperty, bool>
    {
        public override void OnValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            // Get the HashItemListControl
            var listControl = (sender as HashItemListControl);

            //Make sure we have the right control
            if (listControl == null)
                return;

            //cycle through the items int the items control
            foreach (var item in listControl.TheItemsControl.Items)
            {
                //make sure the item is a HashItemViewModel
                if (item is HashItemViewModel itemViewModel)
                {
                    //true if hmac not possible
                    if (itemViewModel.HmacNotPossible)
                    {
                        //get the content presenter from the view model
                        var cont = listControl.TheItemsControl.ItemContainerGenerator.ContainerFromItem(item);
                        
                        //Get the visual control of the content presenter
                        var someControl = VisualTreeHelper.GetChild(cont, 0);

                        //cast to the Hash item control
                        var hashItemControl = (someControl as HashItemControl);

                        // Ignore any non Hash item controls
                        if (!(hashItemControl is HashItemControl))
                            continue;

                        //set the visibility
                        if ((bool)e.NewValue)
                        {
                            hashItemControl.Visibility = Visibility.Collapsed;
                        }
                        else
                        {
                            hashItemControl.Visibility = Visibility.Visible;
                        }
                    }
                }
            }
        }
    }
}