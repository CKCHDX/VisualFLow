using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace VisualFlow
{
    public partial class ToolBox : UserControl
    {
        public ToolBox()
        {
            InitializeComponent();
        }

        private void ToolBoxItem_DragStarted(object sender, MouseButtonEventArgs e)
        {
            if (sender is Button button)
            {
                DataObject dragData = new("UIElement", button.Tag.ToString());
                DragDrop.DoDragDrop(button, dragData, DragDropEffects.Copy);
            }
        }
    }
}
