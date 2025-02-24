using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace VisualFlow
{
    public partial class UIPage : UserControl
    {
        public UIPage()
        {
            InitializeComponent();
        }

        // Drop event handler for Canvas
        private void UIContainer_Drop(object sender, DragEventArgs e)
        {
            string elementType = e.Data.GetData("UIElement") as string;
            if (string.IsNullOrEmpty(elementType)) return;

            UIElement newElement = CreateElement(elementType);
            if (newElement == null) return;

            var visualElement = new VisualFlowElement(newElement, elementType);
            visualElement.AttachEventHandlers(); // Ensure event handlers are added

            Canvas.SetLeft(visualElement, e.GetPosition(UIContainer).X);
            Canvas.SetTop(visualElement, e.GetPosition(UIContainer).Y);
            UIContainer.Children.Add(visualElement);
        }

        private UIElement CreateElement(string elementType)
        {
            return elementType switch
            {
                "Button" => new Button { Content = "New Button" },
                "Label" => new Label { Content = "New Label" },
                "TextBox" => new TextBox { Width = 100 },
                "Box" => new Border { Width = 100, Height = 100, Background = Brushes.LightGray },
                "ProgressBar" => new ProgressBar { Width = 100, Height = 20, Value = 50 },
                _ => null
            };
        }
    }
}
