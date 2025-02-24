using System;
using System.Windows.Media;

namespace VisualFlow.Models
{
    public class UIElementModel
    {
        public string Type { get; set; }  // Button, Label, TextBox, etc.
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public string Color { get; set; }  // Could be hex value or predefined color name

        public UIElementModel(string type, double x, double y, double width, double height, string color)
        {
            Type = type;
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Color = color;
        }
    }
}
