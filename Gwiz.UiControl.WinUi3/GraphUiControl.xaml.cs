using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System.Collections.Generic;
using System.Linq;
using Microsoft.UI.Input;
using System;
using Windows.Foundation;
using Gwiz.Core.Contract;
using SkiaSharp.Views.Windows;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Gwiz.UiControl.WinUi3
{
    public enum InteractionState
    {
        None,
        DraggingNode,
        ResizeAll,
        ResizeHorz,
        ResizeVert,
        DraggingView
    }

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GraphUiControl : Page
    {
        private class Bounds
        {
            public int MaxX;
            public int MaxY;

            public int MinX;
            public int MinY;

            public Bounds()
            {
                Reset();
            }

            public void Reset()
            {
                MaxX = int.MinValue;
                MaxY = int.MinValue;
                MinX = int.MaxValue;
                MinY = int.MaxValue;
            }

            public void Add(int xPos, int yPos, int width, int height)
            {
                MaxX = Math.Max(MaxX, xPos + width);
                MaxY = Math.Max(MaxY, yPos + height);

                MinX = Math.Min(MinX, xPos);
                MinY = Math.Min(MinY, yPos);
            }
        }

        private static readonly int MinNodeSize = 80;

        private Bounds _bounds = new();

        private Draw _draw = new Draw();

        private GraphDrawer _graphDrawer = new();

        private INode? _hoveredNode;

        private InteractionState _currentInteractionState = InteractionState.None;

        private Point _interactionStartPosition = new();

        private Point _mouseToNodeDelta;
        
        private InteractionState _potentialInteractionState = InteractionState.None;
                
        private Point _resizeStartSize = new Point();

        private Point _scrollPosition = new Point(0, 0);
        
        private Point _scrollStartPosition = new Point(0, 0);

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

        private void DrawGraph(object sender, SKPaintSurfaceEventArgs e)
        {
            var canvas = e.Surface.Canvas;
            // Apply translation for scrolling
            canvas.Translate(-(float)_scrollPosition.X, -(float)_scrollPosition.Y);
            _draw.DrawingSession = e.Surface.Canvas;
            _graphDrawer.DrawGraph();
        }

        private void Invalidate()
        {
            _bounds.Reset();
            foreach (var node in Nodes)
            {
                _bounds.Add(node.X, node.Y, node.Width, node.Height);
            }

            if (_scrollPosition.X < _bounds.MinX)
            {
                _scrollPosition.X = _bounds.MinX;
            }
            if (_scrollPosition.Y < _bounds.MinY)
            {
                _scrollPosition.Y = _bounds.MinY;
            }
            if (_scrollPosition.X > _bounds.MaxX - _canvasControl.CanvasSize.Width)
            {
                _scrollPosition.X = _bounds.MaxX - _canvasControl.CanvasSize.Width;
            }
            if (_scrollPosition.Y > _bounds.MaxY - _canvasControl.CanvasSize.Height)
            {
                _scrollPosition.Y = _bounds.MaxY - _canvasControl.CanvasSize.Height;
            }

            _canvasControl.Invalidate();
        }

        private void OnPointerMoved(object sender, PointerRoutedEventArgs e)
        {
            var screenPointerPosition = e.GetCurrentPoint(this).Position;
            var worldPointerPosition = screenPointerPosition;
            worldPointerPosition.X += _scrollPosition.X;
            worldPointerPosition.Y += _scrollPosition.Y;

            double xDelta = 0;
            double yDelta = 0;

            switch (_currentInteractionState)
            {
                case InteractionState.None:
                    // Check if the pointer is over any node
                    _hoveredNode = Nodes?.FirstOrDefault(node =>
                    worldPointerPosition.X >= node.X &&
                    worldPointerPosition.X <= node.X + node.Width &&
                    worldPointerPosition.Y >= node.Y &&
                    worldPointerPosition.Y <= node.Y + node.Height);

                    if (_hoveredNode != null)
                    {
                        // Check if the mouse cursor is over the both resize icon
                        if ((_hoveredNode.Resize == Resize.Both || _hoveredNode.Resize == Resize.HorzVertBoth) &&
                            worldPointerPosition.X >= _hoveredNode.X + _hoveredNode.Width - _graphDrawer.IconSize &&
                            worldPointerPosition.Y >= _hoveredNode.Y + _hoveredNode.Height - _graphDrawer.IconSize)
                        {
                            ProtectedCursor = InputSystemCursor.Create(InputSystemCursorShape.SizeNorthwestSoutheast);
                            _potentialInteractionState = InteractionState.ResizeAll;
                        }
                        else if ((_hoveredNode.Resize == Resize.HorzVert || _hoveredNode.Resize == Resize.HorzVertBoth) &&
                                 worldPointerPosition.X >= _hoveredNode.X + _hoveredNode.Width - (int)(_graphDrawer.IconSize * 0.75) &&
                                 worldPointerPosition.Y >= _hoveredNode.Y + _hoveredNode.Height / 2 - _graphDrawer.IconSize / 2 &&
                                 worldPointerPosition.Y <= _hoveredNode.Y + _hoveredNode.Height / 2 + _graphDrawer.IconSize / 2)
                        {
                            ProtectedCursor = InputSystemCursor.Create(InputSystemCursorShape.SizeWestEast);
                            _potentialInteractionState = InteractionState.ResizeHorz;
                        }
                        else if ((_hoveredNode.Resize == Resize.HorzVert || _hoveredNode.Resize == Resize.HorzVertBoth) &&
                                 worldPointerPosition.X >= _hoveredNode.X + _hoveredNode.Width / 2 - _graphDrawer.IconSize / 2 &&
                                 worldPointerPosition.X <= _hoveredNode.X + _hoveredNode.Width / 2 + _graphDrawer.IconSize / 2 &&
                                 worldPointerPosition.Y >= _hoveredNode.Y + _hoveredNode.Height - (int)(_graphDrawer.IconSize * 0.75))
                        {
                            ProtectedCursor = InputSystemCursor.Create(InputSystemCursorShape.SizeNorthSouth);
                            _potentialInteractionState = InteractionState.ResizeVert;
                        }
                        else
                        {
                            ProtectedCursor = InputSystemCursor.Create(InputSystemCursorShape.Hand);
                            _potentialInteractionState = InteractionState.DraggingNode;
                        }
                    }
                    else
                    {
                        ProtectedCursor = InputSystemCursor.Create(InputSystemCursorShape.Arrow);
                        _potentialInteractionState = InteractionState.DraggingView;
                    }
                    break;

                case InteractionState.DraggingNode:
                    if (_hoveredNode == null)
                    {
                        throw new InvalidOperationException("Dragging without seleceted node");
                    }

                    _hoveredNode.X = (int)(worldPointerPosition.X + _mouseToNodeDelta.X);
                    _hoveredNode.Y = (int)(worldPointerPosition.Y + _mouseToNodeDelta.Y);

                    Invalidate();
                    break;

                case InteractionState.ResizeAll:
                    if (_hoveredNode == null)
                    {
                        throw new InvalidOperationException("Resizing without seleceted node");
                    }

                    xDelta = worldPointerPosition.X - _interactionStartPosition.X;
                    yDelta = worldPointerPosition.Y - _interactionStartPosition.Y;

                    _hoveredNode.Width = Math.Max((int)(_resizeStartSize.X + xDelta), MinNodeSize);
                    _hoveredNode.Height = Math.Max((int)(_resizeStartSize.Y + yDelta), MinNodeSize);

                    Invalidate();
                    break;

                case InteractionState.ResizeHorz:
                    if (_hoveredNode == null)
                    {
                        throw new InvalidOperationException("Resizing without seleceted node");
                    }

                    xDelta = worldPointerPosition.X - _interactionStartPosition.X;

                    _hoveredNode.Width = Math.Max((int)(_resizeStartSize.X + xDelta), MinNodeSize);

                    Invalidate();
                    break;

                case InteractionState.ResizeVert:
                    if (_hoveredNode == null)
                    {
                        throw new InvalidOperationException("Resizing without seleceted node");
                    }

                    yDelta = worldPointerPosition.Y - _interactionStartPosition.Y;

                    _hoveredNode.Height = Math.Max((int)(_resizeStartSize.Y + yDelta), MinNodeSize);

                    _canvasControl.Invalidate();
                    break;
                case InteractionState.DraggingView:
                    _scrollPosition.X = _scrollStartPosition.X  - screenPointerPosition.X;
                    _scrollPosition.Y = _scrollStartPosition.Y - screenPointerPosition.Y;

                    Invalidate();
                    break;
            }
        }

        private void OnPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            _currentInteractionState = _potentialInteractionState;
            _interactionStartPosition = e.GetCurrentPoint(this).Position;

            _interactionStartPosition.X += _scrollPosition.X;
            _interactionStartPosition.Y += _scrollPosition.Y;

            if (_currentInteractionState == InteractionState.DraggingView)
            {
                _scrollStartPosition = _interactionStartPosition    ;
            }
            if (_hoveredNode != null)
            { 
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
            _scrollPosition = new(0, 0);
            _graphDrawer.Edges = Edges;
            _graphDrawer.Nodes = Nodes;
        }
    }
}
