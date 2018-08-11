/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using global::SharpDX;
using SharpDX.Direct3D11;
using System.Collections.Generic;

#if NETFX_CORE
using Windows.UI.Xaml;
using Media = Windows.UI;
namespace HelixToolkit.UWP
#else
using System.Windows;
using Media = System.Windows.Media;
namespace HelixToolkit.Wpf.SharpDX
#endif
{
    
    using Model;
    using Model.Scene;

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Element3D" />
    public class LineGeometryModel3D : Element3D
    {
        #region Dependency Properties        
        /// <summary>
        /// The color property
        /// </summary>
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Media.Color), typeof(LineGeometryModel3D), new PropertyMetadata(Media.Colors.Black, (d, e) =>
            {
                ((d as Element3DCore).SceneNode as LineNode).Color = ((Media.Color)e.NewValue).ToColor4();
            }));
        /// <summary>
        /// The hit test thickness property
        /// </summary>
        public static readonly DependencyProperty HitTestThicknessProperty =
            DependencyProperty.Register("HitTestThickness", typeof(double), typeof(LineGeometryModel3D), new PropertyMetadata(1.0, (d, e) =>
            { ((d as Element3DCore).SceneNode as LineNode).HitTestThickness = (double)e.NewValue; }));

        /// <summary>
        /// The smoothness property
        /// </summary>
        public static readonly DependencyProperty SmoothnessProperty =
            DependencyProperty.Register("Smoothness", typeof(double), typeof(LineGeometryModel3D), new PropertyMetadata(0.0,
            (d, e) =>
            {
                ((d as Element3DCore).SceneNode as LineNode).Smoothness = (float)(double)e.NewValue;
            }));

        /// <summary>
        /// The thickness property
        /// </summary>
        public static readonly DependencyProperty ThicknessProperty =
            DependencyProperty.Register("Thickness", typeof(double), typeof(LineGeometryModel3D), new PropertyMetadata(1.0, (d, e) =>
            {
                ((d as Element3DCore).SceneNode as LineNode).Thickness = (float)(double)e.NewValue;
            }));
        #region DependencyProperties        
        /// <summary>
        /// The depth bias property
        /// </summary>
        public static readonly DependencyProperty DepthBiasProperty =
            DependencyProperty.Register("DepthBias", typeof(int), typeof(LineGeometryModel3D), new PropertyMetadata(0, (d, e) =>
            {
                ((d as Element3DCore).SceneNode as LineNode).RasterComp.DepthBias = (int)e.NewValue;
            }));

        /// <summary>
        /// The enable view frustum check property
        /// </summary>
        public static readonly DependencyProperty EnableViewFrustumCheckProperty =
            DependencyProperty.Register("EnableViewFrustumCheck", typeof(bool), typeof(LineGeometryModel3D), new PropertyMetadata(true,
                (d, e) =>
                {
                    ((d as Element3DCore).SceneNode as LineNode).GeometryComp.EnableFrustumCheck = (bool)e.NewValue;
                }));

        /// <summary>
        /// The fill mode property
        /// </summary>
        public static readonly DependencyProperty FillModeProperty = DependencyProperty.Register("FillMode", typeof(FillMode), typeof(LineGeometryModel3D),
            new PropertyMetadata(FillMode.Solid, (d, e) =>
            {
                ((d as Element3DCore).SceneNode as LineNode).RasterComp.FillMode = (FillMode)e.NewValue;
            }));

        /// <summary>
        /// The geometry property
        /// </summary>
        public static readonly DependencyProperty GeometryProperty =
            DependencyProperty.Register("Geometry", typeof(Geometry3D), typeof(LineGeometryModel3D), new PropertyMetadata(null,
                (d, e) =>
                {
                    ((d as Element3DCore).SceneNode as LineNode).GeometryComp.Geometry = e.NewValue as Geometry3D;
                }));
        /// <summary>
        /// List of instance matrix.
        /// </summary>
        public static readonly DependencyProperty InstancesProperty =
            DependencyProperty.Register("Instances", typeof(IList<Matrix>), typeof(LineGeometryModel3D), new PropertyMetadata(null, (d, e) =>
            {
                ((d as Element3DCore).SceneNode as LineNode).GeometryComp.Instances = e.NewValue as IList<Matrix>;
            }));

        /// <summary>
        /// The is depth clip enabled property
        /// </summary>
        public static readonly DependencyProperty IsDepthClipEnabledProperty = DependencyProperty.Register("IsDepthClipEnabled", typeof(bool), typeof(LineGeometryModel3D),
            new PropertyMetadata(true, (d, e) =>
            {
                ((d as Element3DCore).SceneNode as LineNode).RasterComp.IsDepthClipEnabled = (bool)e.NewValue;
            }));

        /// <summary>
        /// The is multisample enabled property
        /// </summary>
        public static readonly DependencyProperty IsMultisampleEnabledProperty =
            DependencyProperty.Register("IsMultisampleEnabled", typeof(bool), typeof(LineGeometryModel3D), new PropertyMetadata(true, (d, e) =>
            {
                ((d as Element3DCore).SceneNode as LineNode).RasterComp.IsMSAAEnabled = (bool)e.NewValue;
            }));

        /// <summary>
        /// The is scissor enabled property
        /// </summary>
        public static readonly DependencyProperty IsScissorEnabledProperty =
            DependencyProperty.Register("IsScissorEnabled", typeof(bool), typeof(LineGeometryModel3D), new PropertyMetadata(true, (d, e) =>
            {
                ((d as Element3DCore).SceneNode as LineNode).RasterComp.IsScissorEnabled = (bool)e.NewValue;
            }));

        /// <summary>
        /// The is selected property
        /// </summary>
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(LineGeometryModel3D), new PropertyMetadata(false));

        public static readonly DependencyProperty IsThrowingShadowProperty =
                                                        DependencyProperty.Register("IsThrowingShadow", typeof(bool), typeof(LineGeometryModel3D), new PropertyMetadata(false, (d, e) =>
                {
                    if ((d as Element3D).SceneNode is Core.IThrowingShadow t)
                    {
                        t.IsThrowingShadow = (bool)e.NewValue;
                    }
                }));
        // Using a DependencyProperty as the backing store for PostEffects.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PostEffectsProperty =
            DependencyProperty.Register("PostEffects", typeof(string), typeof(LineGeometryModel3D), new PropertyMetadata("", (d, e) =>
            {
                ((d as Element3DCore).SceneNode as LineNode).PostEffectComp.PostEffects = e.NewValue as string;
            }));

        /// <summary>
        /// The slope scaled depth bias property
        /// </summary>
        public static readonly DependencyProperty SlopeScaledDepthBiasProperty =
            DependencyProperty.Register("SlopeScaledDepthBias", typeof(double), typeof(LineGeometryModel3D), new PropertyMetadata(0.0, (d, e) =>
            {
                ((d as Element3DCore).SceneNode as LineNode).RasterComp.SlopeScaledDepthBias = (float)(double)e.NewValue;
            }));
        /// <summary>
        /// Gets or sets the depth bias.
        /// </summary>
        /// <value>
        /// The depth bias.
        /// </value>
        public int DepthBias
        {
            get
            {
                return (int)this.GetValue(DepthBiasProperty);
            }
            set
            {
                this.SetValue(DepthBiasProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [enable view frustum check].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [enable view frustum check]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableViewFrustumCheck
        {
            set
            {
                SetValue(EnableViewFrustumCheckProperty, value);
            }
            get
            {
                return (bool)GetValue(EnableViewFrustumCheckProperty);
            }
        }

        /// <summary>
        /// Gets or sets the fill mode.
        /// </summary>
        /// <value>
        /// The fill mode.
        /// </value>
        public FillMode FillMode
        {
            set
            {
                SetValue(FillModeProperty, value);
            }
            get
            {
                return (FillMode)GetValue(FillModeProperty);
            }
        }

        /// <summary>
        /// Gets or sets the geometry.
        /// </summary>
        /// <value>
        /// The geometry.
        /// </value>
        public Geometry3D Geometry
        {
            get
            {
                return (Geometry3D)this.GetValue(GeometryProperty);
            }
            set
            {
                this.SetValue(GeometryProperty, value);
            }
        }

        /// <summary>
        /// List of instance matrix. 
        /// </summary>
        public IList<Matrix> Instances
        {
            get { return (IList<Matrix>)this.GetValue(InstancesProperty); }
            set { this.SetValue(InstancesProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is depth clip enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is depth clip enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsDepthClipEnabled
        {
            set
            {
                SetValue(IsDepthClipEnabledProperty, value);
            }
            get
            {
                return (bool)GetValue(IsDepthClipEnabledProperty);
            }
        }

        /// <summary>
        /// Only works under FillMode = Wireframe. MSAA is determined by viewport MSAA settings for FillMode = Solid
        /// </summary>
        public bool IsMultisampleEnabled
        {
            set
            {
                SetValue(IsMultisampleEnabledProperty, value);
            }
            get
            {
                return (bool)GetValue(IsMultisampleEnabledProperty);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is scissor enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is scissor enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsScissorEnabled
        {
            set
            {
                SetValue(IsScissorEnabledProperty, value);
            }
            get
            {
                return (bool)GetValue(IsScissorEnabledProperty);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is selected.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is selected; otherwise, <c>false</c>.
        /// </value>
        public bool IsSelected
        {
            get
            {
                return (bool)this.GetValue(IsSelectedProperty);
            }
            set
            {
                this.SetValue(IsSelectedProperty, value);
            }
        }

        /// <summary>
        /// <see cref="Core.IThrowingShadow.IsThrowingShadow"/>
        /// </summary>
        public bool IsThrowingShadow
        {
            set
            {
                SetValue(IsThrowingShadowProperty, value);
            }
            get
            {
                return (bool)GetValue(IsThrowingShadowProperty);
            }
        }

        public string PostEffects
        {
            get { return (string)GetValue(PostEffectsProperty); }
            set { SetValue(PostEffectsProperty, value); }
        }
        /// <summary>
        /// Gets or sets the slope scaled depth bias.
        /// </summary>
        /// <value>
        /// The slope scaled depth bias.
        /// </value>
        public double SlopeScaledDepthBias
        {
            get
            {
                return (double)this.GetValue(SlopeScaledDepthBiasProperty);
            }
            set
            {
                this.SetValue(SlopeScaledDepthBiasProperty, value);
            }
        }
        #endregion     
        /// <summary>
        /// Gets or sets the color.
        /// </summary>
        /// <value>
        /// The color.
        /// </value>
        public Media.Color Color
        {
            get { return (Media.Color)this.GetValue(ColorProperty); }
            set { this.SetValue(ColorProperty, value); }
        }
        /// <summary>
        /// Used only for point/line hit test
        /// </summary>
        public double HitTestThickness
        {
            get { return (double)this.GetValue(HitTestThicknessProperty); }
            set { this.SetValue(HitTestThicknessProperty, value); }
        }

        /// <summary>
        /// Gets or sets the smoothness.
        /// </summary>
        /// <value>
        /// The smoothness.
        /// </value>
        public double Smoothness
        {
            get { return (double)this.GetValue(SmoothnessProperty); }
            set { this.SetValue(SmoothnessProperty, value); }
        }

        /// <summary>
        /// Gets or sets the thickness.
        /// </summary>
        /// <value>
        /// The thickness.
        /// </value>
        public double Thickness
        {
            get { return (double)this.GetValue(ThicknessProperty); }
            set { this.SetValue(ThicknessProperty, value); }
        }
        #endregion        
        /// <summary>
        /// Assigns the default values to core.
        /// </summary>
        /// <param name="core">The core.</param>
        protected override void AssignDefaultValuesToSceneNode(SceneNode core)
        {
            if (core is LineNode n)
            {
                n.Color = Color.ToColor4();
                n.Thickness = (float)Thickness;
                n.Smoothness = (float)Smoothness;
                n.GeometryComp.Geometry = Geometry;
                n.GeometryComp.Instances = Instances;
                n.RasterComp.DepthBias = this.DepthBias;
                n.RasterComp.IsDepthClipEnabled = this.IsDepthClipEnabled;
                n.RasterComp.SlopeScaledDepthBias = (float)this.SlopeScaledDepthBias;
                n.RasterComp.IsMSAAEnabled = this.IsMultisampleEnabled;
                n.RasterComp.FillMode = this.FillMode;
                n.RasterComp.IsScissorEnabled = this.IsScissorEnabled;
                n.GeometryComp.EnableFrustumCheck = this.EnableViewFrustumCheck;
                n.PostEffectComp.PostEffects = this.PostEffects;
            }
            base.AssignDefaultValuesToSceneNode(core);
        }

        /// <summary>
        /// Called when [create scene node].
        /// </summary>
        /// <returns></returns>
        protected override SceneNode OnCreateSceneNode()
        {
            return new LineNode();
        }
    }
}
