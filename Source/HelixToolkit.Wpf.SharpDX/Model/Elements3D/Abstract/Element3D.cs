﻿using HelixToolkit.SharpDX.Model.Scene;
using HelixToolkit.Wpf.SharpDX.Model;
using SharpDX;
using System.Windows;
using System.Windows.Input;

namespace HelixToolkit.Wpf.SharpDX;

/// <summary>
/// Base class for renderable elements.
/// </summary>
public abstract class Element3D : Element3DCore, IVisible
{
    #region Dependency Properties
    /// <summary>
    /// Indicates, if this element should be rendered,
    /// default is true
    /// </summary>
    public static readonly DependencyProperty IsRenderingProperty =
        DependencyProperty.Register("IsRendering", typeof(bool), typeof(Element3D), new PropertyMetadata(true,
            (d, e) =>
            {
                if (d is Element3D element)
                {
                    element.SceneNode.Visible = (bool)e.NewValue && element.Visibility == Visibility.Visible;
                }
            }));

    /// <summary>
    /// Indicates, if this element should be rendered.
    /// Use this also to make the model visible/unvisible
    /// default is true
    /// </summary>
    public bool IsRendering
    {
        get
        {
            return (bool)GetValue(IsRenderingProperty);
        }
        set
        {
            SetValue(IsRenderingProperty, value);
        }
    }


    /// <summary>
    /// 
    /// </summary>
    public static readonly DependencyProperty VisibilityProperty =
        DependencyProperty.Register("Visibility", typeof(Visibility), typeof(Element3D), new PropertyMetadata(Visibility.Visible, (d, e) =>
        {
            if (d is Element3D element)
            {
                element.SceneNode.Visible = (Visibility)e.NewValue == Visibility.Visible && element.IsRendering;
            }
        }));

    /// <summary>
    /// 
    /// </summary>
    public Visibility Visibility
    {
        set
        {
            SetValue(VisibilityProperty, value);
        }
        get
        {
            return (Visibility)GetValue(VisibilityProperty);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public static readonly DependencyProperty TransformProperty =
        DependencyProperty.Register("Transform", typeof(Transform3D), typeof(Element3D), new PropertyMetadata(Transform3D.Identity,
            (d, e) =>
            {
                if (d is Element3D element)
                {
                    element.SceneNode.ModelMatrix = (e.NewValue as Transform3D)?.Value.ToMatrix() ?? Matrix.Identity;
                }
            }));

    /// <summary>
    /// 
    /// </summary>
    public Transform3D Transform
    {
        get
        {
            return (Transform3D)this.GetValue(TransformProperty);
        }
        set
        {
            this.SetValue(TransformProperty, value);
        }
    }

    /// <summary>
    /// The is hit test visible property
    /// </summary>
    public static readonly DependencyProperty IsHitTestVisibleProperty = DependencyProperty.Register("IsHitTestVisible", typeof(bool), typeof(Element3D),
        new PropertyMetadata(true, (d, e) =>
        {
            if (d is Element3D element)
            {
                element.SceneNode.IsHitTestVisible = (bool)e.NewValue;
            }
        }));

    /// <summary>
    /// Indicates, if this element should be hit-tested.
    /// default is true
    /// </summary>
    public bool IsHitTestVisible
    {
        set
        {
            SetValue(IsHitTestVisibleProperty, value);
        }
        get
        {
            return (bool)GetValue(IsHitTestVisibleProperty);
        }
    }


    /// <summary>
    /// Gets or sets the manual render order.
    /// </summary>
    /// <value>
    /// The render order.
    /// </value>
    public int RenderOrder
    {
        get
        {
            return (int)GetValue(RenderOrderProperty);
        }
        set
        {
            SetValue(RenderOrderProperty, value);
        }
    }

    /// <summary>
    /// The render order property
    /// </summary>
    public static readonly DependencyProperty RenderOrderProperty =
        DependencyProperty.Register("RenderOrder", typeof(int), typeof(Element3D), new PropertyMetadata(0, (d, e) =>
        {
            if (d is Element3D element)
            {
                element.SceneNode.RenderOrder = (ushort)Math.Max(0, Math.Min(ushort.MaxValue, (int)e.NewValue));
            }
        }));
    #endregion

    #region Events
    public static readonly RoutedEvent MouseDown3DEvent =
        EventManager.RegisterRoutedEvent("MouseDown3D", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Element3D));

    public static readonly RoutedEvent MouseUp3DEvent =
        EventManager.RegisterRoutedEvent("MouseUp3D", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Element3D));

    public static readonly RoutedEvent MouseMove3DEvent =
        EventManager.RegisterRoutedEvent("MouseMove3D", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Element3D));

    /// <summary>
    /// Provide CLR accessors for the event 
    /// </summary>
    public event RoutedEventHandler MouseDown3D
    {
        add
        {
            AddHandler(MouseDown3DEvent, value);
        }
        remove
        {
            RemoveHandler(MouseDown3DEvent, value);
        }
    }

    /// <summary>
    /// Provide CLR accessors for the event 
    /// </summary>
    public event RoutedEventHandler MouseUp3D
    {
        add
        {
            AddHandler(MouseUp3DEvent, value);
        }
        remove
        {
            RemoveHandler(MouseUp3DEvent, value);
        }
    }

    /// <summary>
    /// Provide CLR accessors for the event 
    /// </summary>
    public event RoutedEventHandler MouseMove3D
    {
        add
        {
            AddHandler(MouseMove3DEvent, value);
        }
        remove
        {
            RemoveHandler(MouseMove3DEvent, value);
        }
    }

    protected virtual void OnMouse3DDown(object? sender, RoutedEventArgs e)
    {
        Mouse3DDown?.Invoke(this, e as MouseDown3DEventArgs);
    }

    protected virtual void OnMouse3DUp(object? sender, RoutedEventArgs e)
    {
        Mouse3DUp?.Invoke(this, e as MouseUp3DEventArgs);
    }

    protected virtual void OnMouse3DMove(object? sender, RoutedEventArgs e)
    {
        Mouse3DMove?.Invoke(this, e as MouseMove3DEventArgs);
    }

    public event EventHandler<MouseDown3DEventArgs?>? Mouse3DDown;
    public event EventHandler<MouseUp3DEventArgs?>? Mouse3DUp;
    public event EventHandler<MouseMove3DEventArgs?>? Mouse3DMove;
    #endregion

    public Element3D()
    {
        this.MouseDown3D += OnMouse3DDown;
        this.MouseUp3D += OnMouse3DUp;
        this.MouseMove3D += OnMouse3DMove;
        OnSceneNodeCreated += Element3D_OnSceneNodeCreated;
    }

    private void Element3D_OnSceneNodeCreated(object? sender, SceneNodeCreatedEventArgs e)
    {
        e.Node.MouseDown += Node_MouseDown;
        e.Node.MouseMove += Node_MouseMove;
        e.Node.MouseUp += Node_MouseUp;
    }

    private void Node_MouseUp(object? sender, SceneNodeMouseUpArgs e)
    {
        RaiseEvent(new MouseUp3DEventArgs(this, e.HitResult, new Point(e.Position.X, e.Position.Y), e.Viewport as Viewport3DX, e.OriginalInputEventArgs as InputEventArgs));
    }

    private void Node_MouseMove(object? sender, SceneNodeMouseMoveArgs e)
    {
        RaiseEvent(new MouseMove3DEventArgs(this, e.HitResult, new Point(e.Position.X, e.Position.Y), e.Viewport as Viewport3DX, e.OriginalInputEventArgs as InputEventArgs));
    }

    private void Node_MouseDown(object? sender, SceneNodeMouseDownArgs e)
    {
        RaiseEvent(new MouseDown3DEventArgs(this, e.HitResult, new Point(e.Position.X, e.Position.Y), e.Viewport as Viewport3DX, e.OriginalInputEventArgs as InputEventArgs));
    }

    /// <summary>
    /// Looks for the first visual ancestor of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of visual ancestor.</typeparam>
    /// <param name="obj">The respective <see cref="DependencyObject"/>.</param>
    /// <returns>
    /// The first visual ancestor of type <typeparamref name="T"/> if exists, else <c>null</c>.
    /// </returns>
    public static T? FindVisualAncestor<T>(DependencyObject obj) where T : DependencyObject
    {
        if (obj != null)
        {
            var parent = System.Windows.Media.VisualTreeHelper.GetParent(obj);
            while (parent != null)
            {
                if (parent is T typed)
                {
                    return typed;
                }

                parent = System.Windows.Media.VisualTreeHelper.GetParent(parent);
            }
        }

        return null;
    }

    //protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
    //{
    //    var pm = e.Property.GetMetadata(this);
    //    if (pm is FrameworkPropertyMetadata fm)
    //    {
    //        if (fm.AffectsRender)
    //        {
    //            InvalidateRender();
    //        }
    //    }
    //    base.OnPropertyChanged(e);
    //}

    public static implicit operator Element3D?(SceneNode node)
    {
        return node.WrapperSource as Element3D;
    }
}
