using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace VisualFlow
{
    public partial class VisualFlowElement : UserControl
    {
        private bool isDragging = false;
        private bool isResizing = false;
        private Point clickPosition;
        private ResizeDirection resizeDirection;
        private DispatcherTimer resizeTimer;

        public string ElementType { get; private set; }

        public VisualFlowElement(UIElement element, string type)
        {
            InitializeComponent();
            ElementType = type;

            if (element is Button button)
            {
                // Ensure buttons allow dragging
                button.PreviewMouseDown += UIElement_MouseLeftButtonDown;
            }
            else
            {
                element.MouseLeftButtonDown += UIElement_MouseLeftButtonDown;
            }

            ElementContainer.Children.Add(element);
            AttachEventHandlers();
        }

        public void AttachEventHandlers()
        {
            this.PreviewMouseDown += UIElement_MouseLeftButtonDown;
            this.MouseMove += UIElement_MouseMove;
            this.MouseLeftButtonUp += UIElement_MouseLeftButtonUp;
            this.MouseEnter += UIElement_MouseEnter;
            this.MouseLeave += UIElement_MouseLeave;
            AddRightClickMenu(this);
        }

        // Handles dragging
        private void UIElement_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            clickPosition = e.GetPosition(this);

            // Check if the mouse is near the edge to resize
            resizeDirection = GetResizeDirection(clickPosition);
            if (resizeDirection != ResizeDirection.None)
            {
                isResizing = true;
            }
            else
            {
                isDragging = true; // Otherwise, allow dragging
            }

            CaptureMouse();
        }


        private void UIElement_MouseMove(object sender, MouseEventArgs e)
        {
            if (isResizing) // If we are resizing
            {
                var position = e.GetPosition(Parent as UIElement);
                ResizeElement(position);
            }
            else if (isDragging) // If we are dragging
            {
                var position = e.GetPosition(Parent as UIElement);
                Canvas.SetLeft(this, position.X - clickPosition.X);
                Canvas.SetTop(this, position.Y - clickPosition.Y);
            }
        }


        private void UIElement_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isResizing = false; // Stop resizing
            isDragging = false; // Stop dragging
            ReleaseMouseCapture();
        }


        // Change cursor when near an edge
        private void UIElement_MouseEnter(object sender, MouseEventArgs e)
        {
            if (IsMouseNearEdge(e.GetPosition(this)))
            {
                this.Cursor = Cursors.SizeAll;
            }
            else
            {
                this.Cursor = Cursors.Arrow;
            }
        }

        private void UIElement_MouseLeave(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.Arrow;
        }

        // Right-click menu
        private void AddRightClickMenu(UIElement element)
        {
            ContextMenu contextMenu = new ContextMenu();

            MenuItem renameItem = new MenuItem { Header = "Rename" };
            renameItem.Click += (s, e) => RenameElement(element);
            contextMenu.Items.Add(renameItem);

            MenuItem deleteItem = new MenuItem { Header = "Delete" };
            deleteItem.Click += (s, e) => (Parent as Canvas)?.Children.Remove(this);
            contextMenu.Items.Add(deleteItem);

            MenuItem duplicateItem = new MenuItem { Header = "Duplicate" };
            duplicateItem.Click += (s, e) => DuplicateElement();
            contextMenu.Items.Add(duplicateItem);

            this.ContextMenu = contextMenu;
        }

        private void RenameElement(UIElement element)
        {
            if (element is Button btn) btn.Content = "Renamed Button";
            if (element is Label lbl) lbl.Content = "Renamed Label";
            if (element is TextBox txt) txt.Text = "Renamed TextBox";
            if (element is Border box) box.Background = Brushes.LightBlue;
        }

        private void DuplicateElement()
        {
            UIElement duplicate = CreateDuplicate();
            if (duplicate == null) return;

            var visualElement = new VisualFlowElement(duplicate, this.ElementType);
            visualElement.AttachEventHandlers();

            (Parent as Canvas)?.Children.Add(visualElement);

            // Offset slightly from original
            Canvas.SetLeft(visualElement, Canvas.GetLeft(this) + 10);
            Canvas.SetTop(visualElement, Canvas.GetTop(this) + 10);
        }

        private UIElement CreateDuplicate()
        {
            return ElementType switch
            {
                "Button" => new Button { Content = "New Button" },
                "Label" => new Label { Content = "New Label" },
                "TextBox" => new TextBox { Width = 100, Text = "New TextBox" },
                "Box" => new Border { Width = 100, Height = 100, Background = Brushes.LightGray },
                "ProgressBar" => new ProgressBar { Width = 100, Height = 20, Value = 50 },
                _ => null
            };
        }

        #region Resize Logic

        private void ResizeElement(Point mousePosition)
        {
            double minSize = 20; // Prevent elements from shrinking too much

            switch (resizeDirection)
            {
                case ResizeDirection.Top:
                    double newHeightTop = this.ActualHeight - (mousePosition.Y - Canvas.GetTop(this));
                    if (newHeightTop > minSize)
                    {
                        this.Height = newHeightTop;
                        Canvas.SetTop(this, mousePosition.Y);
                    }
                    break;

                case ResizeDirection.Bottom:
                    double newHeightBottom = mousePosition.Y - Canvas.GetTop(this);
                    if (newHeightBottom > minSize)
                        this.Height = newHeightBottom;
                    break;

                case ResizeDirection.Left:
                    double newWidthLeft = this.ActualWidth - (mousePosition.X - Canvas.GetLeft(this));
                    if (newWidthLeft > minSize)
                    {
                        this.Width = newWidthLeft;
                        Canvas.SetLeft(this, mousePosition.X);
                    }
                    break;

                case ResizeDirection.Right:
                    double newWidthRight = mousePosition.X - Canvas.GetLeft(this);
                    if (newWidthRight > minSize)
                        this.Width = newWidthRight;
                    break;

                case ResizeDirection.TopLeft:
                    double newWidthTL = this.ActualWidth - (mousePosition.X - Canvas.GetLeft(this));
                    double newHeightTL = this.ActualHeight - (mousePosition.Y - Canvas.GetTop(this));
                    if (newWidthTL > minSize && newHeightTL > minSize)
                    {
                        this.Width = newWidthTL;
                        this.Height = newHeightTL;
                        Canvas.SetLeft(this, mousePosition.X);
                        Canvas.SetTop(this, mousePosition.Y);
                    }
                    break;

                case ResizeDirection.TopRight:
                    double newWidthTR = mousePosition.X - Canvas.GetLeft(this);
                    double newHeightTR = this.ActualHeight - (mousePosition.Y - Canvas.GetTop(this));
                    if (newWidthTR > minSize && newHeightTR > minSize)
                    {
                        this.Width = newWidthTR;
                        this.Height = newHeightTR;
                        Canvas.SetTop(this, mousePosition.Y);
                    }
                    break;

                case ResizeDirection.BottomLeft:
                    double newWidthBL = this.ActualWidth - (mousePosition.X - Canvas.GetLeft(this));
                    double newHeightBL = mousePosition.Y - Canvas.GetTop(this);
                    if (newWidthBL > minSize && newHeightBL > minSize)
                    {
                        this.Width = newWidthBL;
                        this.Height = newHeightBL;
                        Canvas.SetLeft(this, mousePosition.X);
                    }
                    break;

                case ResizeDirection.BottomRight:
                    double newWidthBR = mousePosition.X - Canvas.GetLeft(this);
                    double newHeightBR = mousePosition.Y - Canvas.GetTop(this);
                    if (newWidthBR > minSize && newHeightBR > minSize)
                    {
                        this.Width = newWidthBR;
                        this.Height = newHeightBR;
                    }
                    break;
            }
        }

        private bool IsMouseNearEdge(Point mousePosition)
        {
            double tolerance = 10;
            return mousePosition.X <= tolerance || mousePosition.X >= this.ActualWidth - tolerance ||
                   mousePosition.Y <= tolerance || mousePosition.Y >= this.ActualHeight - tolerance;
        }

        private ResizeDirection GetResizeDirection(Point mousePosition)
        {
            double tolerance = 10;

            bool nearLeft = mousePosition.X <= tolerance;
            bool nearRight = mousePosition.X >= this.ActualWidth - tolerance;
            bool nearTop = mousePosition.Y <= tolerance;
            bool nearBottom = mousePosition.Y >= this.ActualHeight - tolerance;

            if (nearLeft && nearTop) return ResizeDirection.TopLeft;
            if (nearRight && nearTop) return ResizeDirection.TopRight;
            if (nearLeft && nearBottom) return ResizeDirection.BottomLeft;
            if (nearRight && nearBottom) return ResizeDirection.BottomRight;
            if (nearLeft) return ResizeDirection.Left;
            if (nearRight) return ResizeDirection.Right;
            if (nearTop) return ResizeDirection.Top;
            if (nearBottom) return ResizeDirection.Bottom;

            return ResizeDirection.None;
        }

        #endregion
    }
    public enum ResizeDirection
    {
        None,
        Left,
        Right,
        Top,
        Bottom,
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight
    }
}

