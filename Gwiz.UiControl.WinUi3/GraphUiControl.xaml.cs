using Gwiz.Core.Contract;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using SkiaSharp.Views.Windows;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Core;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Gwiz.UiControl.WinUi3
{
    public enum InteractionState
    {
        None,
        ClickButton,
        CreateEdgeBegin,
        CreateEdgeFinish,
        DraggingNode,
        DraggingView,
        EditText,
        ResizeAll,
        ResizeHorz,
        ResizeVert,
        SelectEdge,
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


        private MenuFlyout _contextMenu;

        private Point _currentScreenPointerPosition = new Point();

        private IButton? _customButtonBelowMouse;

        private Draw _draw = new Draw();

        private IEdgeTemplate? _edgeCreationSourceTemplate;
        
        private INode? _edgeCreationTargetNode;

        private IGridCell? _editGridCell;

        private GraphDrawer _graphDrawer = new((text) => TextSizeCalculator.GetTextSize(text));

        private IEdge? _hoveredEdge;

        private INode? _hoveredNode;

        private InteractionState _currentInteractionState = InteractionState.None;

        private Point _interactionStartPosition = new();

        private Point _mouseToNodeDelta;

        private InteractionState _potentialInteractionState = InteractionState.None;

        private Point _resizeStartSize = new Point();

        private IEdge? _selectedEdge;
        
        private INode? _selectedNode;

        private Point _scrollPosition = new Point(0, 0);

        private Point _scrollStartPosition = new Point(0, 0);        

        public GraphUiControl()
        {
            _graphDrawer.SetDraw(_draw);

            this.InitializeComponent();
            this.IsTabStop = true; 
            
            this.KeyDown += OnKeyDown;

            _deleteKeyAccelerator.IsEnabled = false;

            _contextMenu = new MenuFlyout();
        }

        // Graph Dependency Property
        public static readonly DependencyProperty GraphProperty =
        DependencyProperty.Register(
            nameof(Graph),
            typeof(IGraph),
            typeof(GraphUiControl),
            new PropertyMetadata(null, OnGraphDataChanged)
        );

        public IGraph Graph
        {
            get => (IGraph)GetValue(GraphProperty);
            set => SetValue(GraphProperty, value);
        }

        private void OnDeleteInvoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            if (_selectedNode != null)
            {
                Graph.Remove(_selectedNode);
                _selectedNode = null;
            }
            else if (_selectedEdge != null)
            {
                Graph.Remove(_selectedEdge);
                _selectedEdge = null;
            }
            Invalidate();
        }

        private void DrawGraph(object sender, SKPaintSurfaceEventArgs e)
        {
            var canvas = e.Surface.Canvas;
            // Apply translation for scrolling
            canvas.Translate(-(float)_scrollPosition.X, -(float)_scrollPosition.Y);
            _draw.DrawingSession = e.Surface.Canvas;
            _graphDrawer.DrawGraph();
        }

        [DllImport("user32.dll")]
        private static extern bool GetKeyboardState(byte[] lpKeyState);

        private static int GetStartOfPreviousWord(string text, int cursorPosition, bool goForward)
        {
            if (string.IsNullOrEmpty(text) || 
                (!goForward && cursorPosition <= 0) || 
                (goForward && cursorPosition >= text.Length))
            {
                return cursorPosition;
            }

            int modifier = goForward ? 1 : -1;

            int i = cursorPosition + modifier;

            // Skip trailing spaces before the current word
            while (i > 0 && i < text.Length && char.IsWhiteSpace(text[i]))
            {
                i += modifier;
            }

            // Skip the characters of the current/previous word
            int offset = goForward ? 0 : -1;
            while (i > 0 && i < text.Length && !char.IsWhiteSpace(text[i + offset]))
            {
                i += modifier;
            }

            return i;
        }

        private void Invalidate()
        {
            if (Graph == null)
            {
                return;
            }

            _bounds.Reset();
            foreach (var node in Graph.Nodes)
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

        [DllImport("user32.dll")]
        private static extern uint MapVirtualKey(uint uCode, uint uMapType);

        private void OnKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (_currentInteractionState == InteractionState.EditText &&
                _editGridCell != null)
            {
                var currentGridText = _editGridCell.Text;
                var nextGridText = currentGridText;

                var shift = InputKeyboardSource.GetKeyStateForCurrentThread(VirtualKey.Shift);
                var ctrl = InputKeyboardSource.GetKeyStateForCurrentThread(VirtualKey.Control);

                switch (e.Key)
                {
                    case VirtualKey.Enter:
                    case VirtualKey.Escape:
                        StopEditing();
                        break;
                    case VirtualKey.Back:
                        if (_editGridCell.EditTextPosition > 0)
                        {
                            nextGridText = nextGridText.Remove(_editGridCell.EditTextPosition - 1, 1);
                            _editGridCell.EditTextPosition--;
                        }
                        break;
                    case VirtualKey.Delete:
                        if (_editGridCell.EditTextPosition < nextGridText.Length)
                        {
                            nextGridText = nextGridText.Remove(_editGridCell.EditTextPosition, 1);
                        }
                        break;
                    case VirtualKey.Left:
                        if (ctrl != CoreVirtualKeyStates.Down)
                        {
                            _editGridCell.EditTextPosition = Math.Max(0, _editGridCell.EditTextPosition - 1);
                        }
                        else
                        {
                            _editGridCell.EditTextPosition = GetStartOfPreviousWord(nextGridText, _editGridCell.EditTextPosition, false);
                        }
                        _canvasControl.Invalidate();
                        break;
                    case VirtualKey.Right:
                        if (ctrl != CoreVirtualKeyStates.Down)
                        {
                            _editGridCell.EditTextPosition = Math.Min(currentGridText.Length, _editGridCell.EditTextPosition + 1);
                        }
                        else
                        {
                            _editGridCell.EditTextPosition = GetStartOfPreviousWord(nextGridText, _editGridCell.EditTextPosition, true);
                        }
                        _canvasControl.Invalidate();
                        break;
                    default:
                        StringBuilder buffer = new(2);
                        byte[] keyboardState = new byte[256];
                        GetKeyboardState(keyboardState);

                        uint virtualKey = (uint)e.Key;
                        uint scanCode = MapVirtualKey(virtualKey, 0);

                        int result = ToUnicode(virtualKey, scanCode, keyboardState, buffer, buffer.Capacity, 0);
                        if (result > 0)
                        {
                            string typedChar = buffer.ToString();
                            nextGridText = nextGridText.Insert(_editGridCell.EditTextPosition, typedChar);
                            _editGridCell.EditTextPosition++;
                        }
                        break;

                }

                if (nextGridText != currentGridText)
                {
                    _editGridCell.Text = nextGridText;
                    _canvasControl.Invalidate();
                }
            }
        }

        /// <summary>
        /// Called when the Graph dependency property is set
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnGraphDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var uiControl = d as GraphUiControl;

            if (uiControl != null)
            {
                uiControl.UpdateGraphDrawer();

                uiControl.RegisterContextMenu();

                if (uiControl._canvasControl != null)
                {
                    uiControl._canvasControl.Invalidate();
                }
            }
        }

        private void OnLostFocus(object sender, RoutedEventArgs e)
        {
            this.Focus(FocusState.Programmatic);
        }

        private void OnPointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (Graph == null)
            {
                return;
            }

            _currentScreenPointerPosition = e.GetCurrentPoint(this).Position;
            System.Drawing.Point worldPointerPosition = new System.Drawing.Point((int)_currentScreenPointerPosition.X, (int)_currentScreenPointerPosition.Y);
            worldPointerPosition.X += (int)_scrollPosition.X;
            worldPointerPosition.Y += (int)_scrollPosition.Y;

            double xDelta = 0;
            double yDelta = 0;

            switch (_currentInteractionState)
            {
                case InteractionState.None:

                    // Check if the pointer is over any node
                    // Iterate backwards to honor the drawing order defined by the input yaml / order of adding
                    _hoveredNode = null;
                    _editGridCell = null;
                    for (int i = Graph.Nodes.Count - 1; i >= 0; i--)
                    {
                        var node = Graph.Nodes[i];

                        // Check if mouse is over source edge template icon
                        _edgeCreationSourceTemplate = _graphDrawer.GetSourceEdgeTemplateAtPosition(node, (int)worldPointerPosition.X, (int)worldPointerPosition.Y);
                        if (_edgeCreationSourceTemplate != null)
                        {
                            _hoveredNode = node;
                            ProtectedCursor = InputSystemCursor.Create(InputSystemCursorShape.Hand);
                            _potentialInteractionState = InteractionState.CreateEdgeBegin;
                            break;
                        }

                        // Check if the mouse if over a custom button
                        _customButtonBelowMouse = null;
                        foreach (var button in node.Buttons.Where(p => p.Visible))
                        {
                            var buttonPosition = button.Alignment.ToPosition(new System.Drawing.Rectangle(node.X, node.Y, node.Width, node.Height), Design.IconSize);
                            if (worldPointerPosition.X >= buttonPosition.X &&
                                worldPointerPosition.X <= buttonPosition.X + Design.IconLength &&
                                worldPointerPosition.Y >= buttonPosition.Y &&
                                worldPointerPosition.Y <= buttonPosition.Y + Design.IconLength)
                            {
                                ProtectedCursor = InputSystemCursor.Create(InputSystemCursorShape.Arrow);
                                _potentialInteractionState = InteractionState.ClickButton;
                                _customButtonBelowMouse = button;
                                break;
                            }
                        }

                        if (_customButtonBelowMouse != null)
                        {
                            break;
                        }


                        bool isOverEditButton = false;

                        if (node.IsOver(worldPointerPosition))
                        {
                            _hoveredNode = node;

                            // Check if the mouse is over an edit text button
                            for (int x = 0; x < _hoveredNode.Grid.Cols.Count; x++)
                            {
                                for (int y = 0; y < _hoveredNode.Grid.Rows.Count; y++)
                                {
                                    var cell = _hoveredNode.Grid.Cells[x, y];

                                    if (!cell.Editable)
                                    {
                                        continue;
                                    }

                                    var rect = cell.Rectangle;

                                    if (worldPointerPosition.X >= rect.X && 
                                        worldPointerPosition.X <= rect.X + Design.IconLength &&
                                        worldPointerPosition.Y <= rect.Y + rect.Height / 2 + Design.IconLength / 2 && 
                                        worldPointerPosition.Y >= rect.Y + rect.Height / 2 - Design.IconLength / 2)
                                    {
                                        ProtectedCursor = InputSystemCursor.Create(InputSystemCursorShape.Arrow);
                                        _potentialInteractionState = InteractionState.EditText;

                                        _editGridCell = _hoveredNode.Grid.Cells[x, y];

                                        isOverEditButton = true;
                                        break;
                                    }
                                }

                                if (isOverEditButton)
                                {
                                    break;
                                }
                            }

                            if (isOverEditButton)
                            {
                                break;
                            }

                            // Check if the mouse cursor is over the both resize icon
                            if ((_hoveredNode.Resize == Resize.Both || _hoveredNode.Resize == Resize.HorzVertBoth) &&
                                worldPointerPosition.X >= _hoveredNode.X + _hoveredNode.Width - Design.IconLength &&
                                worldPointerPosition.Y >= _hoveredNode.Y + _hoveredNode.Height - Design.IconLength)
                            {
                                ProtectedCursor = InputSystemCursor.Create(InputSystemCursorShape.SizeNorthwestSoutheast);
                                _potentialInteractionState = InteractionState.ResizeAll;
                            }
                            else if ((_hoveredNode.Resize == Resize.HorzVert || _hoveredNode.Resize == Resize.HorzVertBoth) &&
                                     worldPointerPosition.X >= _hoveredNode.X + _hoveredNode.Width - (int)(Design.IconLength * 0.75) &&
                                     worldPointerPosition.Y >= _hoveredNode.Y + _hoveredNode.Height / 2 - Design.IconLength / 2 &&
                                     worldPointerPosition.Y <= _hoveredNode.Y + _hoveredNode.Height / 2 + Design.IconLength / 2)
                            {
                                ProtectedCursor = InputSystemCursor.Create(InputSystemCursorShape.SizeWestEast);
                                _potentialInteractionState = InteractionState.ResizeHorz;
                            }
                            else if ((_hoveredNode.Resize == Resize.HorzVert || _hoveredNode.Resize == Resize.HorzVertBoth) &&
                                     worldPointerPosition.X >= _hoveredNode.X + _hoveredNode.Width / 2 - Design.IconLength / 2 &&
                                     worldPointerPosition.X <= _hoveredNode.X + _hoveredNode.Width / 2 + Design.IconLength / 2 &&
                                     worldPointerPosition.Y >= _hoveredNode.Y + _hoveredNode.Height - (int)(Design.IconLength * 0.75))
                            {
                                ProtectedCursor = InputSystemCursor.Create(InputSystemCursorShape.SizeNorthSouth);
                                _potentialInteractionState = InteractionState.ResizeVert;
                            }
                            else
                            {
                                ProtectedCursor = InputSystemCursor.Create(InputSystemCursorShape.Hand);
                                _potentialInteractionState = InteractionState.DraggingNode;
                            }

                            break;
                        }

                        if (_hoveredNode == null)
                        {
                            ProtectedCursor = InputSystemCursor.Create(InputSystemCursorShape.Arrow);
                            _potentialInteractionState = InteractionState.DraggingView;
                        }
                    }

                    // Check if the mouse is over an edge
                    if (_hoveredNode == null)
                    {
                        IEdge? previousHoveredEdge = _hoveredEdge;

                        if (_hoveredEdge != null)
                        {
                            _hoveredEdge.Highlight = false;
                        }

                        _hoveredEdge = null;
                        
                        foreach (var edge in Graph.Edges)
                        {
                            if (edge.IsOver(worldPointerPosition))
                            {
                                _hoveredEdge = edge;
                                _hoveredEdge.Highlight = true;
                                ProtectedCursor = InputSystemCursor.Create(InputSystemCursorShape.Arrow);
                                _potentialInteractionState = InteractionState.SelectEdge;
                                break;
                            }
                        }

                        if (_hoveredEdge != previousHoveredEdge)
                        {
                            Invalidate();
                        }
                    }

                    break;

                case InteractionState.CreateEdgeBegin:
                    if (_hoveredNode == null)
                    {
                        throw new InvalidOperationException("Cannot begin edge creation when no node is active");
                    }

                    _graphDrawer.PreparePreviewLine(_hoveredNode, (int)worldPointerPosition.X, (int)worldPointerPosition.Y);
                    
                    // Check if mouse cursor is over a target edge template
                    foreach (var node in Graph.Nodes)
                    {
                        var edgeTemplate = _graphDrawer.GetSourceEdgeTemplateAtPosition(node, (int)worldPointerPosition.X, (int) worldPointerPosition.Y);

                        if (edgeTemplate != null)
                        {
                            _potentialInteractionState = InteractionState.CreateEdgeFinish;
                            _edgeCreationTargetNode = node;

                            break;
                        }
                    }
                    
                    Invalidate();
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
                    _scrollPosition.X = _scrollStartPosition.X - _currentScreenPointerPosition.X;
                    _scrollPosition.Y = _scrollStartPosition.Y - _currentScreenPointerPosition.Y;

                    Invalidate();
                    break;
            }
        }

        private void OnPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (_currentInteractionState == InteractionState.CreateEdgeBegin)
            {
                _graphDrawer.EdgeCreationActiveSourceTemplate = null;
            }
            if (_currentInteractionState == InteractionState.EditText)
            {
                StopEditing();
                return;
            }

            _currentInteractionState = _potentialInteractionState;
            _interactionStartPosition = e.GetCurrentPoint(this).Position;

            _interactionStartPosition.X += _scrollPosition.X;
            _interactionStartPosition.Y += _scrollPosition.Y;

            if (_currentInteractionState == InteractionState.CreateEdgeFinish)
            {
                if (_hoveredNode == null)
                {
                    throw new InvalidOperationException("Cannot create edge because source node is not set");
                }
                if (_edgeCreationSourceTemplate == null)
                {
                    throw new InvalidOperationException("Cannot create edge because source template is not set");
                }
                if (_edgeCreationTargetNode == null)
                {
                    throw new InvalidOperationException("Cannot create edge because target node is not set");
                }

                var edgeBuilder = Graph.AddEdge(_hoveredNode, _edgeCreationTargetNode);
                edgeBuilder.WithTemplate(_edgeCreationSourceTemplate).Build();

                _currentInteractionState = InteractionState.None;
                Invalidate();
            }
            if (_currentInteractionState == InteractionState.ClickButton)
            {
                if (_customButtonBelowMouse == null)
                {
                    throw new InvalidOperationException("Cannot click button when not over custom ");
                }
                _customButtonBelowMouse.Click();
            }
            if (_currentInteractionState == InteractionState.CreateEdgeBegin)
            {
                _graphDrawer.EdgeCreationActiveSourceTemplate = _edgeCreationSourceTemplate;
            }
            if (_currentInteractionState == InteractionState.DraggingView)
            {
                _scrollStartPosition = _interactionStartPosition;
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
            if (_currentInteractionState != InteractionState.EditText &&
                _currentInteractionState != InteractionState.CreateEdgeBegin)
            {
                _deleteKeyAccelerator.IsEnabled = false;

                if (_selectedNode != null)
                {
                    _selectedNode.Select = false;

                }
                _selectedNode = null;


                if (_selectedEdge != null)
                {
                    _selectedEdge.Select = false;
                }
                _selectedEdge = null;

                if (_currentInteractionState == InteractionState.DraggingNode &&
                    _hoveredNode != null)
                {
                    var currentPosition = e.GetCurrentPoint(this).Position;

                    currentPosition.X += _scrollPosition.X;
                    currentPosition.Y += _scrollPosition.Y;

                    // Mouse was not moved, hence the node or edge below the cursor is selected
                    if (_interactionStartPosition == currentPosition)
                    {
                        _selectedNode = _hoveredNode;
                        _selectedNode.Select = true;
                    }
                }

                if (_currentInteractionState == InteractionState.SelectEdge &&
                    _hoveredEdge != null)
                {
                    _hoveredEdge.Highlight = false;
                    _selectedEdge = _hoveredEdge;
                    _selectedEdge.Select = true;
                }

                if (_selectedNode != null || _selectedEdge != null)
                {
                    _deleteKeyAccelerator.IsEnabled = true;
                }

                _currentInteractionState = InteractionState.None;
                Invalidate();
            }
            else if (_currentInteractionState == InteractionState.EditText)
            {
                StartEditing();
            }
        }

        private void RegisterContextMenu()
        {
            Graph.ContextMenuShown += (sender, contextMenuItems) =>
            {
                _contextMenu = new MenuFlyout();

                foreach (var menuItem in contextMenuItems)
                {
                    if (menuItem.SubMenuItems.Any())
                    {
                        var item = new MenuFlyoutSubItem { Text = menuItem.Name };

                        foreach (var subItem in menuItem.SubMenuItems)
                        {
                            var subMenuItem = new MenuFlyoutItem { Text = subItem.Name };
                            subMenuItem.Click += (s, e) => subItem.Callback.Invoke();
                            item.Items.Add(subMenuItem);
                        }
                        _contextMenu.Items.Add(item);
                    }
                    else
                    {
                        var item = new MenuFlyoutItem { Text = menuItem.Name };
                        item.Click += (s, e) => menuItem.Callback.Invoke();
                        _contextMenu.Items.Add(item);
                    }
                }

                _contextMenu.ShowAt(_canvasControl, _currentScreenPointerPosition);
            };
        }

        private void StartEditing()
        {
            if (_editGridCell == null)
            {
                throw new InvalidOperationException("Editing without seleceted node");
            }

            _editGridCell.EditModeEnabled = true;

            _deleteKeyAccelerator.IsEnabled = false;
            this.Focus(FocusState.Programmatic);
            this.LostFocus += OnLostFocus;

            Invalidate();
        }

        private void StopEditing()
        {
            if (_editGridCell == null)
            {
                throw new InvalidOperationException("Editing without seleceted node");
            }

            _editGridCell.EditModeEnabled = false;
            _currentInteractionState = InteractionState.None;
            this.LostFocus -= OnLostFocus;

            _editGridCell = null;

            Invalidate();
        }

        [DllImport("user32.dll")]
        private static extern int ToUnicode(
            uint virtualKeyCode,
            uint scanCode,
            byte[] keyboardState,
            [Out, MarshalAs(UnmanagedType.LPWStr)]
                    StringBuilder receivingBuffer,
            int bufferSize,
            uint flags);

        private void UpdateGraphDrawer()
        {
            _scrollPosition = new(0, 0);
            _graphDrawer.Edges = Graph.Edges;
            _graphDrawer.Nodes = Graph.Nodes;
        }
    }
}
