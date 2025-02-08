using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using System.Collections.Generic;
using System.Linq;
using Windows.UI;
using Microsoft.UI.Input;
using System;
using Windows.Foundation;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.UI;
using Gwiz.Core.Contract;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace awiz.Graph.UiControl
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GraphUiControl : Page
    {
        private Node? _hoveredNode;
        private bool _nodeIsDragged;

        Point _mouseToNodeDelta;

        public GraphUiControl()
        {
            //PointerMoved += OnPointerMoved;
            //PointerPressed += OnPointerPressed;
            //PointerReleased += OnPointerReleased;
            this.InitializeComponent();
        }


        // Nodes Dependency Property
        public static readonly DependencyProperty NodesProperty =
        DependencyProperty.Register(
            nameof(Nodes),
            typeof(IList<Node>),
            typeof(GraphUiControl),
            new PropertyMetadata(new List<Node>(), OnGraphDataChanged)
        );

        public List<Node> Nodes
        {
            get => (List<Node>)GetValue(NodesProperty);
            set => SetValue(NodesProperty, value);
        }

        // Callback when Nodes or Edges change
        private static void OnGraphDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var uiControl = d as GraphUiControl;

            if (uiControl != null && uiControl._canvasControl != null)
            {
                uiControl._canvasControl.Invalidate();
            }
        }

        private void OnPointerMoved(object sender, PointerRoutedEventArgs e)
        {
            var pointerPosition = e.GetCurrentPoint(this).Position;
            
            if (_nodeIsDragged)
            { 
                if (_hoveredNode == null)
                {
                    throw new NullReferenceException("Dragging without seleceted node");
                }

                _hoveredNode.X = (int)(pointerPosition.X - _mouseToNodeDelta.X);
                _hoveredNode.Y = (int)(pointerPosition.Y - _mouseToNodeDelta.Y);

                _canvasControl.Invalidate();
            }
            else
            {            
                // Check if the pointer is over any node
                _hoveredNode = Nodes?.FirstOrDefault(node =>
                pointerPosition.X >= node.X &&
                pointerPosition.X <= node.X + node.Width &&
                pointerPosition.Y >= node.Y &&
                pointerPosition.Y <= node.Y + node.Height);

                if (_hoveredNode != null)
                {
                    ProtectedCursor = InputSystemCursor.Create(InputSystemCursorShape.SizeAll);
                }
                else
                {
                    ProtectedCursor = InputSystemCursor.Create(InputSystemCursorShape.Arrow);
                }
            }        
        }

        private void OnPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (_hoveredNode != null)
            {
                _nodeIsDragged = true;

                var pointerPosition = e.GetCurrentPoint(this).Position;
                _mouseToNodeDelta.X = _hoveredNode.X - pointerPosition.X;
                _mouseToNodeDelta.Y = _hoveredNode.Y - pointerPosition.Y;
            }
        }

        private void OnPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            _nodeIsDragged = false;
        }

        private void DrawGraph(CanvasControl sender, CanvasDrawEventArgs args)
        {
            var drawingSession = args.DrawingSession;

            // Clear the canvas
            drawingSession.Clear(Colors.CornflowerBlue);

            if (Nodes != null)
            {
                foreach (Node node in Nodes)
                {
                    var rect = new Rectangle
                    {
                        Fill = new SolidColorBrush(Color.FromArgb(100, 0, 255, 0)),
                        Width = node.Width,
                        Height = node.Height,
                        Stroke = new SolidColorBrush(Color.FromArgb(100, 255, 0, 0)),
                        StrokeThickness = 5,
                        RadiusX = 5,
                        RadiusY = 5,
                    };

                    drawingSession.FillRectangle(new Rect(node.X, node.Y, node.Width, node.Height), Color.FromArgb(100, 255, 0, 0));
                }
            }
        }
    }
}
