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
using Microsoft.Graphics.Canvas;
using System.IO;
using Microsoft.Graphics.Canvas.Svg;
using System.Numerics;
using System.Reflection;
using System.Xml.Linq;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Gwiz.UiControl.WinUi3
{
    internal enum InteractionState
    {
        None,
        Dragging,
        ResizeAll,
        ResizeHorz,
        ResizeVert,
    }

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GraphUiControl : Page
    {
        private static readonly int IconSize = 30;

        private static readonly int MinNodeSize = 80;

        private Node? _hoveredNode;

        private InteractionState _currentInteractionState = InteractionState.None;

        private Point _interactionStartPosition = new();

        private Point _mouseToNodeDelta;
        
        private InteractionState _potentialInteractionState = InteractionState.None;

        private CanvasSvgDocument? _resizeBottomRight;

        private CanvasSvgDocument? _resizeHorz;
        
        private Point _resizeStartSize = new Point();
        
        private CanvasSvgDocument? _resizeVert;


        public GraphUiControl()
        {
            this.InitializeComponent();

            var assembly = Assembly.GetExecutingAssembly();
            foreach (string resource in assembly.GetManifestResourceNames())
            {
                System.Diagnostics.Debug.WriteLine(resource); // Print all embedded resources
            }

            LoadSvgAsync();
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

        private async void LoadSvgAsync()
        {
            CanvasDevice device = CanvasDevice.GetSharedDevice();
            using (var stream = GetEmbeddedSvgStream("resize-bottom-right.svg"))
            {
                _resizeBottomRight = await CanvasSvgDocument.LoadAsync(device, stream.AsRandomAccessStream());
            }
            using (var stream = GetEmbeddedSvgStream("drag-horizontal-variant.svg"))
            {
                _resizeVert = await CanvasSvgDocument.LoadAsync(device, stream.AsRandomAccessStream());
            }
            using (var stream = GetEmbeddedSvgStream("drag-vertical-variant.svg"))
            {
                _resizeHorz = await CanvasSvgDocument.LoadAsync(device, stream.AsRandomAccessStream());
            }



            _canvasControl.Invalidate();
        }
        private static Stream GetEmbeddedSvgStream(string iconName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            string resourceName = "Gwiz.UiControl.WinUi3.icons." + iconName; // Adjust the namespace

            Stream? stream = assembly.GetManifestResourceStream(resourceName);

            if (stream == null)
            {
                throw new FileNotFoundException($"Resource {resourceName} not found in assembly {assembly.FullName}");
            }
            return stream;
        }

        public static Color ConvertColor(System.Drawing.Color color)
        {
            return Color.FromArgb(color.A, color.R, color.G, color.B);
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
                    // Draw the node shape
                    drawingSession.FillRectangle(new Rect(node.X, node.Y, node.Width, node.Height), ConvertColor(node.Template.BackgroundColor));
                    drawingSession.DrawRectangle(new Rect(node.X, node.Y, node.Width, node.Height), ConvertColor(node.Template.LineColor), 1);

                    // Draw the resize all icon
                    if (node.Template.Resize == Resize.Both || node.Template.Resize == Resize.HorzVertBoth)
                    {
                        args.DrawingSession.DrawSvg(_resizeBottomRight, new Size(IconSize, IconSize), new Vector2(node.X + node.Width - IconSize, node.Y + node.Height - IconSize));
                    }

                    if (node.Template.Resize == Resize.HorzVert || node.Template.Resize == Resize.HorzVertBoth)
                    {
                        // Draw the resize horz icon
                        args.DrawingSession.DrawSvg(_resizeHorz, new Size(IconSize, IconSize), new Vector2(node.X + node.Width - (int)(IconSize * 0.75), node.Y + node.Height / 2 - IconSize / 2));

                        // Draw the resize vert icon
                        args.DrawingSession.DrawSvg(_resizeVert, new Size(IconSize, IconSize), new Vector2(node.X + node.Width / 2 - IconSize / 2, node.Y + node.Height - (int)(IconSize * 0.75)));
                    }                
                }
            }
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

            double xDelta = 0;
            double yDelta = 0;

            switch (_currentInteractionState)
            {
                case InteractionState.None:
                    // Check if the pointer is over any node
                    _hoveredNode = Nodes?.FirstOrDefault(node =>
                    pointerPosition.X >= node.X &&
                    pointerPosition.X <= node.X + node.Width &&
                    pointerPosition.Y >= node.Y &&
                    pointerPosition.Y <= node.Y + node.Height);

                    if (_hoveredNode != null)
                    {
                        // Check if the mouse cursor is over the both resize icon
                        if ((_hoveredNode.Template.Resize == Resize.Both || _hoveredNode.Template.Resize == Resize.HorzVertBoth) &&
                            pointerPosition.X >= _hoveredNode.X + _hoveredNode.Width - IconSize &&
                            pointerPosition.Y >= _hoveredNode.Y + _hoveredNode.Height - IconSize)
                        {
                            ProtectedCursor = InputSystemCursor.Create(InputSystemCursorShape.SizeNorthwestSoutheast);
                            _potentialInteractionState = InteractionState.ResizeAll;
                        }
                        else if ((_hoveredNode.Template.Resize == Resize.HorzVert || _hoveredNode.Template.Resize == Resize.HorzVertBoth) &&
                                 pointerPosition.X >= _hoveredNode.X + _hoveredNode.Width - (int)(IconSize * 0.75) &&
                                 pointerPosition.Y >= _hoveredNode.Y + _hoveredNode.Height / 2 - IconSize / 2 &&
                                 pointerPosition.Y <= _hoveredNode.Y + _hoveredNode.Height / 2 + IconSize / 2)
                        {
                            ProtectedCursor = InputSystemCursor.Create(InputSystemCursorShape.SizeWestEast);
                            _potentialInteractionState = InteractionState.ResizeHorz;

                        }
                        else if ((_hoveredNode.Template.Resize == Resize.HorzVert || _hoveredNode.Template.Resize == Resize.HorzVertBoth) &&
                                 pointerPosition.X >= _hoveredNode.X + _hoveredNode.Width / 2 - IconSize / 2 &&
                                 pointerPosition.X <= _hoveredNode.X + _hoveredNode.Width / 2 + IconSize / 2 &&
                                 pointerPosition.Y >= _hoveredNode.Y + _hoveredNode.Height - (int)(IconSize * 0.75))
                        {
                            ProtectedCursor = InputSystemCursor.Create(InputSystemCursorShape.SizeNorthSouth);
                            _potentialInteractionState = InteractionState.ResizeVert;

                        }
                        else
                        {
                            ProtectedCursor = InputSystemCursor.Create(InputSystemCursorShape.Hand);
                            _potentialInteractionState = InteractionState.Dragging;
                        }
                    }
                    else
                    {
                        ProtectedCursor = InputSystemCursor.Create(InputSystemCursorShape.Arrow);
                        _potentialInteractionState = InteractionState.None;
                    }
                    break;

                case InteractionState.Dragging:
                    if (_hoveredNode == null)
                    {
                        throw new InvalidOperationException("Dragging without seleceted node");
                    }

                    _hoveredNode.X = (int)(pointerPosition.X - _mouseToNodeDelta.X);
                    _hoveredNode.Y = (int)(pointerPosition.Y - _mouseToNodeDelta.Y);

                    _canvasControl.Invalidate();
                    break;

                case InteractionState.ResizeAll:
                    if (_hoveredNode == null)
                    {
                        throw new InvalidOperationException("Resizing without seleceted node");
                    }

                    xDelta = pointerPosition.X - _interactionStartPosition.X;
                    yDelta = pointerPosition.Y - _interactionStartPosition.Y;

                    _hoveredNode.Width = Math.Max((int)(_resizeStartSize.X + xDelta), MinNodeSize);
                    _hoveredNode.Height = Math.Max((int)(_resizeStartSize.Y + yDelta), MinNodeSize);

                    _canvasControl.Invalidate();
                    break;

                case InteractionState.ResizeHorz:
                    if (_hoveredNode == null)
                    {
                        throw new InvalidOperationException("Resizing without seleceted node");
                    }

                    xDelta = pointerPosition.X - _interactionStartPosition.X;

                    _hoveredNode.Width = Math.Max((int)(_resizeStartSize.X + xDelta), MinNodeSize);

                    _canvasControl.Invalidate();
                    break;

                case InteractionState.ResizeVert:
                    if (_hoveredNode == null)
                    {
                        throw new InvalidOperationException("Resizing without seleceted node");
                    }

                    yDelta = pointerPosition.Y - _interactionStartPosition.Y;

                    _hoveredNode.Height = Math.Max((int)(_resizeStartSize.Y + yDelta), MinNodeSize);

                    _canvasControl.Invalidate();
                    break;

            }
        }

        private void OnPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (_hoveredNode != null)
            {
                _currentInteractionState = _potentialInteractionState;

                _interactionStartPosition = e.GetCurrentPoint(this).Position;
                _mouseToNodeDelta.X = _hoveredNode.X - _interactionStartPosition.X;
                _mouseToNodeDelta.Y = _hoveredNode.Y - _interactionStartPosition.Y;

                _resizeStartSize.X = _hoveredNode.Width;
                _resizeStartSize.Y = _hoveredNode.Height;
            }
        }

        private void OnPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            _currentInteractionState = InteractionState.None;
        }

    }
}
