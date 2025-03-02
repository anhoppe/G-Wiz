using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
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
using System.Numerics;
using Microsoft.Graphics.Canvas.Text;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Gwiz.UiControl.WinUi3
{
    public enum InteractionState
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
        private static readonly int MinNodeSize = 80;

        private Draw _draw = new Draw();

        private GraphDrawer _graphDrawer = new();

        private INode? _hoveredNode;

        private InteractionState _currentInteractionState = InteractionState.None;

        private Point _interactionStartPosition = new();

        private Point _mouseToNodeDelta;
        
        private InteractionState _potentialInteractionState = InteractionState.None;
                
        private Point _resizeStartSize = new Point();

        public GraphUiControl()
        {
            _graphDrawer.Draw = _draw;

            this.InitializeComponent();
        }

        // Edges Dependency Property
        public static readonly DependencyProperty EdgesProperty =
        DependencyProperty.Register(
            nameof(Edges),
            typeof(IList<IEdge>),
            typeof(GraphUiControl),
            new PropertyMetadata(new List<IEdge>(), OnGraphDataChanged)
        );

        public List<IEdge> Edges
        {
            get => (List<IEdge>)GetValue(EdgesProperty);
            set => SetValue(EdgesProperty, value);
        }

        // Nodes Dependency Property
        public static readonly DependencyProperty NodesProperty =
        DependencyProperty.Register(
            nameof(Nodes),
            typeof(IList<INode>),
            typeof(GraphUiControl),
            new PropertyMetadata(new List<INode>(), OnGraphDataChanged)
        );

        public List<INode> Nodes
        {
            get => (List<INode>)GetValue(NodesProperty);
            set => SetValue(NodesProperty, value); 
        }

        private static void OnGraphDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var uiControl = d as GraphUiControl;

            if (uiControl != null)
            {
                uiControl.UpdateGraphDrawer();

                if (uiControl._canvasControl != null)
                {
                    uiControl._canvasControl.Invalidate();
                }
            }
        }

        private void DrawGraph(CanvasControl sender, CanvasDrawEventArgs args)
        {
            _draw.DrawingSession = args.DrawingSession;
            _graphDrawer.DrawGraph();
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
                        if ((_hoveredNode.Resize == Resize.Both || _hoveredNode.Resize == Resize.HorzVertBoth) &&
                            pointerPosition.X >= _hoveredNode.X + _hoveredNode.Width - _graphDrawer.IconSize &&
                            pointerPosition.Y >= _hoveredNode.Y + _hoveredNode.Height - _graphDrawer.IconSize)
                        {
                            ProtectedCursor = InputSystemCursor.Create(InputSystemCursorShape.SizeNorthwestSoutheast);
                            _potentialInteractionState = InteractionState.ResizeAll;
                        }
                        else if ((_hoveredNode.Resize == Resize.HorzVert || _hoveredNode.Resize == Resize.HorzVertBoth) &&
                                 pointerPosition.X >= _hoveredNode.X + _hoveredNode.Width - (int)(_graphDrawer.IconSize * 0.75) &&
                                 pointerPosition.Y >= _hoveredNode.Y + _hoveredNode.Height / 2 - _graphDrawer.IconSize / 2 &&
                                 pointerPosition.Y <= _hoveredNode.Y + _hoveredNode.Height / 2 + _graphDrawer.IconSize / 2)
                        {
                            ProtectedCursor = InputSystemCursor.Create(InputSystemCursorShape.SizeWestEast);
                            _potentialInteractionState = InteractionState.ResizeHorz;

                        }
                        else if ((_hoveredNode.Resize == Resize.HorzVert || _hoveredNode.Resize == Resize.HorzVertBoth) &&
                                 pointerPosition.X >= _hoveredNode.X + _hoveredNode.Width / 2 - _graphDrawer.IconSize / 2 &&
                                 pointerPosition.X <= _hoveredNode.X + _hoveredNode.Width / 2 + _graphDrawer.IconSize / 2 &&
                                 pointerPosition.Y >= _hoveredNode.Y + _hoveredNode.Height - (int)(_graphDrawer.IconSize * 0.75))
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

        private void UpdateGraphDrawer()
        {
            _graphDrawer.Edges = Edges;
            _graphDrawer.Nodes = Nodes;
        }
    }
}
