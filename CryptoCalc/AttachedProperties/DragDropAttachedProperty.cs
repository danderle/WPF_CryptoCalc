using System.Windows;
using System.Windows.Input;

namespace CryptoCalc
{
    /// <summary>
    /// Enables dragging and dropping files onto any framework element
    /// </summary>
    public class DragDropProperty : BaseAttachedProperty<DragDropProperty, ICommand>
    {
        /// <summary>
        /// Fires when a new command is set
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public override void OnValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)sender;
            element.PreviewDragOver += element_PreviewDragOver;
            element.Drop += Element_DropFile;
        }

        /// <summary>
        /// handles the preview drag over for textboxes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void element_PreviewDragOver(object sender, DragEventArgs e)
        {
            e.Handled = true;
        }

        /// <summary>
        /// Calls the command bound to the attached property (see viewmodel)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Element_DropFile(object sender, DragEventArgs e)
        {
            //set the element as a framework element
            FrameworkElement element = (FrameworkElement)sender;

            //Get the bound value from the element
            ICommand command = GetValue(element);

            // Get the paths from the file/s dropped
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            //Pass the object to the bound command method
            command.Execute(files);
        }

    }

}