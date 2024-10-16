﻿using System.Windows.Media.Media3D;
using System.Windows;

namespace HelixToolkit.Wpf;

/// <summary>
/// Provides a base class for text items.
/// </summary>
public abstract class TextItem
{
    /// <summary>
    /// Gets or sets the horizontal alignment.
    /// </summary>
    /// <value>The horizontal alignment.</value>
    public HorizontalAlignment HorizontalAlignment { get; set; }

    /// <summary>
    /// Gets or sets the position.
    /// </summary>
    /// <value>The position.</value>
    public Point3D Position { get; set; }

    /// <summary>
    /// Gets or sets the text.
    /// </summary>
    /// <value>The text.</value>
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the vertical alignment.
    /// </summary>
    /// <value>The vertical alignment.</value>
    public VerticalAlignment VerticalAlignment { get; set; }
}
