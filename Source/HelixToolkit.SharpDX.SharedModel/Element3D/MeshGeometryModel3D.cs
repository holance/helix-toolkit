/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using global::SharpDX;
using global::SharpDX.Direct3D11;
using System.Collections.Generic;

#if NETFX_CORE
using Windows.UI.Xaml;
using Color = Windows.UI.Color;
using Colors = Windows.UI.Colors;
namespace HelixToolkit.UWP
#else
using System.Windows;
using Color = System.Windows.Media.Color;
using Colors = System.Windows.Media.Colors;
namespace HelixToolkit.Wpf.SharpDX
#endif
{
    using Model;
    using Model.Scene;
    
    /// <summary>
    /// 
    /// </summary>
    public class MeshGeometryModel3D : Element3D
    {
        #region Dependency Properties        
        /// <summary>
        /// The cull mode property
        /// </summary>
        public static readonly DependencyProperty CullModeProperty = DependencyProperty.Register("CullMode", typeof(CullMode), typeof(MeshGeometryModel3D),
            new PropertyMetadata(CullMode.None, (d, e) => { ((d as Element3DCore).SceneNode as MeshNode).RasterComp.CullMode = (CullMode)e.NewValue; }));

        /// <summary>
        /// The front counter clockwise property
        /// </summary>
        public static readonly DependencyProperty FrontCounterClockwiseProperty = DependencyProperty.Register("FrontCounterClockwise", typeof(bool), typeof(MeshGeometryModel3D),
            new PropertyMetadata(true, (d, e) => { ((d as Element3DCore).SceneNode as MeshNode).RasterComp.FrontCCW = (bool)e.NewValue; }));
        /// <summary>
        /// The invert normal property
        /// </summary>
        public static readonly DependencyProperty InvertNormalProperty = DependencyProperty.Register("InvertNormal", typeof(bool), typeof(MeshGeometryModel3D),
            new PropertyMetadata(false, (d, e) => { ((d as Element3DCore).SceneNode as MeshNode).InvertNormalComp.InvertNormal = (bool)e.NewValue; }));

        /// <summary>
        /// The render wireframe property
        /// </summary>
        public static readonly DependencyProperty RenderWireframeProperty =
            DependencyProperty.Register("RenderWireframe", typeof(bool), typeof(MeshGeometryModel3D), new PropertyMetadata(false, (d, e) =>
            { ((d as Element3DCore).SceneNode as MeshNode).WireframeComp.RenderWireframe = (bool)e.NewValue; }));

        /// <summary>
        /// The wireframe color property
        /// </summary>
        public static readonly DependencyProperty WireframeColorProperty =
            DependencyProperty.Register("WireframeColor", typeof(Color), typeof(MeshGeometryModel3D), new PropertyMetadata(Colors.SkyBlue, (d, e) =>
            { ((d as Element3DCore).SceneNode as MeshNode).WireframeComp.WireframeColor = ((Color)e.NewValue).ToColor4(); }));
        #region DependencyProperties        
        /// <summary>
        /// The depth bias property
        /// </summary>
        public static readonly DependencyProperty DepthBiasProperty =
            DependencyProperty.Register("DepthBias", typeof(int), typeof(MeshGeometryModel3D), new PropertyMetadata(0, (d, e) =>
            {
                ((d as Element3DCore).SceneNode as MeshNode).RasterComp.DepthBias = (int)e.NewValue;
            }));

        /// <summary>
        /// The enable view frustum check property
        /// </summary>
        public static readonly DependencyProperty EnableViewFrustumCheckProperty =
            DependencyProperty.Register("EnableViewFrustumCheck", typeof(bool), typeof(MeshGeometryModel3D), new PropertyMetadata(true,
                (d, e) =>
                {
                    ((d as Element3DCore).SceneNode as MeshNode).GeometryComp.EnableFrustumCheck = (bool)e.NewValue;
                }));

        /// <summary>
        /// The fill mode property
        /// </summary>
        public static readonly DependencyProperty FillModeProperty = DependencyProperty.Register("FillMode", typeof(FillMode), typeof(MeshGeometryModel3D),
            new PropertyMetadata(FillMode.Solid, (d, e) =>
            {
                ((d as Element3DCore).SceneNode as MeshNode).RasterComp.FillMode = (FillMode)e.NewValue;
            }));

        /// <summary>
        /// The geometry property
        /// </summary>
        public static readonly DependencyProperty GeometryProperty =
            DependencyProperty.Register("Geometry", typeof(Geometry3D), typeof(MeshGeometryModel3D), new PropertyMetadata(null,
                (d, e) =>
                {
                    ((d as Element3DCore).SceneNode as MeshNode).GeometryComp.Geometry = e.NewValue as Geometry3D;
                }));
        /// <summary>
        /// List of instance matrix.
        /// </summary>
        public static readonly DependencyProperty InstancesProperty =
            DependencyProperty.Register("Instances", typeof(IList<Matrix>), typeof(MeshGeometryModel3D), new PropertyMetadata(null, (d, e) =>
            {
                ((d as Element3DCore).SceneNode as MeshNode).GeometryComp.Instances = e.NewValue as IList<Matrix>;
            }));

        /// <summary>
        /// The is depth clip enabled property
        /// </summary>
        public static readonly DependencyProperty IsDepthClipEnabledProperty = DependencyProperty.Register("IsDepthClipEnabled", typeof(bool), typeof(MeshGeometryModel3D),
            new PropertyMetadata(true, (d, e) =>
            {
                ((d as Element3DCore).SceneNode as MeshNode).RasterComp.IsDepthClipEnabled = (bool)e.NewValue;
            }));

        /// <summary>
        /// The is multisample enabled property
        /// </summary>
        public static readonly DependencyProperty IsMultisampleEnabledProperty =
            DependencyProperty.Register("IsMultisampleEnabled", typeof(bool), typeof(MeshGeometryModel3D), new PropertyMetadata(true, (d, e) =>
            {
                ((d as Element3DCore).SceneNode as MeshNode).RasterComp.IsMSAAEnabled = (bool)e.NewValue;
            }));

        /// <summary>
        /// The is scissor enabled property
        /// </summary>
        public static readonly DependencyProperty IsScissorEnabledProperty =
            DependencyProperty.Register("IsScissorEnabled", typeof(bool), typeof(MeshGeometryModel3D), new PropertyMetadata(true, (d, e) =>
            {
                ((d as Element3DCore).SceneNode as MeshNode).RasterComp.IsScissorEnabled = (bool)e.NewValue;
            }));

        /// <summary>
        /// The is selected property
        /// </summary>
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(MeshGeometryModel3D), new PropertyMetadata(false));

        public static readonly DependencyProperty IsThrowingShadowProperty =
                                                        DependencyProperty.Register("IsThrowingShadow", typeof(bool), typeof(MeshGeometryModel3D), new PropertyMetadata(false, (d, e) =>
                {
                    if ((d as Element3D).SceneNode is Core.IThrowingShadow t)
                    {
                        t.IsThrowingShadow = (bool)e.NewValue;
                    }
                }));
        /// <summary>
        /// Specifiy if model material is transparent. 
        /// During rendering, transparent objects are rendered after opaque objects. Transparent objects' order in scene graph are preserved.
        /// </summary>
        public static readonly DependencyProperty IsTransparentProperty =
            DependencyProperty.Register("IsTransparent", typeof(bool), typeof(MeshGeometryModel3D), new PropertyMetadata(false, (d, e) =>
            {
                ((d as Element3DCore).SceneNode as MeshNode).IsTransparentComp.IsTransparent = (bool)e.NewValue;
            }));

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty MaterialProperty =
            DependencyProperty.Register("Material", typeof(Material), typeof(MeshGeometryModel3D), new PropertyMetadata(null, (d, e) =>
            {
                ((d as Element3DCore).SceneNode as MeshNode).MaterialComp.Material = e.NewValue as Material;
            }));

        // Using a DependencyProperty as the backing store for PostEffects.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PostEffectsProperty =
            DependencyProperty.Register("PostEffects", typeof(string), typeof(MeshGeometryModel3D), new PropertyMetadata("", (d, e) =>
            {
                ((d as Element3DCore).SceneNode as MeshNode).PostEffectComp.PostEffects = e.NewValue as string;
            }));

        /// <summary>
        /// The slope scaled depth bias property
        /// </summary>
        public static readonly DependencyProperty SlopeScaledDepthBiasProperty =
            DependencyProperty.Register("SlopeScaledDepthBias", typeof(double), typeof(MeshGeometryModel3D), new PropertyMetadata(0.0, (d, e) =>
            {
                ((d as Element3DCore).SceneNode as MeshNode).RasterComp.SlopeScaledDepthBias = (float)(double)e.NewValue;
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

        /// <summary>
        /// Specifiy if model material is transparent. 
        /// During rendering, transparent objects are rendered after opaque objects. Transparent objects' order in scene graph are preserved.
        /// </summary>
        public bool IsTransparent
        {
            get { return (bool)GetValue(IsTransparentProperty); }
            set { SetValue(IsTransparentProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public Material Material
        {
            get { return (Material)this.GetValue(MaterialProperty); }
            set { this.SetValue(MaterialProperty, value); }
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
        /// Gets or sets the cull mode.
        /// </summary>
        /// <value>
        /// The cull mode.
        /// </value>
        public CullMode CullMode
        {
            set
            {
                SetValue(CullModeProperty, value);
            }
            get
            {
                return (CullMode)GetValue(CullModeProperty);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [front counter clockwise].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [front counter clockwise]; otherwise, <c>false</c>.
        /// </value>
        public bool FrontCounterClockwise
        {
            set
            {
                SetValue(FrontCounterClockwiseProperty, value);
            }
            get
            {
                return (bool)GetValue(FrontCounterClockwiseProperty);
            }
        }

        /// <summary>
        /// Invert the surface normal during rendering
        /// </summary>
        public bool InvertNormal
        {
            set
            {
                SetValue(InvertNormalProperty, value);
            }
            get
            {
                return (bool)GetValue(InvertNormalProperty);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [render overlapping wireframe].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [render wireframe]; otherwise, <c>false</c>.
        /// </value>
        public bool RenderWireframe
        {
            get { return (bool)GetValue(RenderWireframeProperty); }
            set { SetValue(RenderWireframeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the color of the wireframe.
        /// </summary>
        /// <value>
        /// The color of the wireframe.
        /// </value>
        public Color WireframeColor
        {
            get { return (Color)GetValue(WireframeColorProperty); }
            set { SetValue(WireframeColorProperty, value); }
        }
        #endregion        
        /// <summary>
        /// Assigns the default values to core.
        /// </summary>
        /// <param name="node">The node.</param>
        protected override void AssignDefaultValuesToSceneNode(SceneNode node)
        {
            var c = node as MeshNode;
            c.InvertNormalComp.InvertNormal = this.InvertNormal;
            c.WireframeComp.WireframeColor = this.WireframeColor.ToColor4();
            c.WireframeComp.RenderWireframe = this.RenderWireframe;
            c.RasterComp.DepthBias = this.DepthBias;
            c.RasterComp.IsDepthClipEnabled = this.IsDepthClipEnabled;
            c.RasterComp.SlopeScaledDepthBias = (float)this.SlopeScaledDepthBias;
            c.RasterComp.IsMSAAEnabled = this.IsMultisampleEnabled;
            c.RasterComp.FillMode = this.FillMode;
            c.RasterComp.IsScissorEnabled = this.IsScissorEnabled;
            c.GeometryComp.EnableFrustumCheck = this.EnableViewFrustumCheck;
            c.PostEffectComp.PostEffects = this.PostEffects;
            c.MaterialComp.Material = this.Material;
            base.AssignDefaultValuesToSceneNode(node);
        }

        /// <summary>
        /// Called when [create scene node].
        /// </summary>
        /// <returns></returns>
        protected override SceneNode OnCreateSceneNode()
        {
            return new MeshNode();
        }
    }
}
