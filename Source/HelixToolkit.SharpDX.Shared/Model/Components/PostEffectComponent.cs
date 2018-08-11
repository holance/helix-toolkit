using System.Collections.Generic;
using System;

#if NETFX_CORE
namespace HelixToolkit.UWP.Model.Components
#else
namespace HelixToolkit.Wpf.SharpDX.Model.Components
#endif
{
    using Model.Scene;

    public sealed class PostEffectComponent : EntityComponent
    {
        private string postEffects;
        /// <summary>
        /// Gets or sets the post effects.
        /// </summary>
        /// <value>
        /// The post effects.
        /// </value>
        public string PostEffects
        {
            get { return postEffects; }
            set
            {
                if (Set(ref postEffects, value))
                {
                    ClearPostEffect();
                    if (value is string effects)
                    {
                        if (!string.IsNullOrEmpty(effects))
                        {
                            foreach (var effect in EffectAttributes.Parse(effects))
                            {
                                AddPostEffect(effect);
                            }
                        }
                    }
                }
            }
        }
        #region POST EFFECT        
        /// <summary>
        /// Gets or sets the post effects.
        /// </summary>
        /// <value>
        /// The post effects.
        /// </value>
        private readonly Dictionary<string, IEffectAttributes> postEffectNames = new Dictionary<string, IEffectAttributes>();

        /// <summary>
        /// Gets the post effect names.
        /// </summary>
        /// <value>
        /// The post effect names.
        /// </value>
        public IEnumerable<string> PostEffectNames
        {
            get { return postEffectNames.Keys; }
        }
        /// <summary>
        /// Gets a value indicating whether this instance has any post effect.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has any post effect; otherwise, <c>false</c>.
        /// </value>
        public bool HasAnyPostEffect { get { return postEffectNames.Count > 0; } }
        /// <summary>
        /// Adds the post effect.
        /// </summary>
        /// <param name="effect">The effect.</param>
        public void AddPostEffect(IEffectAttributes effect)
        {
            if (postEffectNames.ContainsKey(effect.EffectName))
            {
                return;
            }
            postEffectNames.Add(effect.EffectName, effect);
            InvalidateRender();
        }
        /// <summary>
        /// Removes the post effect.
        /// </summary>
        /// <param name="effectName">Name of the effect.</param>
        public void RemovePostEffect(string effectName)
        {
            if (postEffectNames.Remove(effectName))
            {
                InvalidateRender();
            }
        }
        /// <summary>
        /// Determines whether [has post effect] [the specified effect name].
        /// </summary>
        /// <param name="effectName">Name of the effect.</param>
        /// <returns>
        ///   <c>true</c> if [has post effect] [the specified effect name]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasPostEffect(string effectName)
        {
            return postEffectNames.ContainsKey(effectName);
        }
        /// <summary>
        /// Tries the get post effect.
        /// </summary>
        /// <param name="effectName">Name of the effect.</param>
        /// <param name="effect">The effect.</param>
        /// <returns></returns>
        public bool TryGetPostEffect(string effectName, out IEffectAttributes effect)
        {
            return postEffectNames.TryGetValue(effectName, out effect);
        }
        /// <summary>
        /// Clears the post effect.
        /// </summary>
        public void ClearPostEffect()
        {
            postEffectNames.Clear();
            InvalidateRender();
        }
        #endregion

        private readonly SceneNode node;
        public PostEffectComponent(SceneNode node)
        {
            this.node = node;
            if (node == null)
            {
                throw new ArgumentNullException("Arguments cannot be null.");
            }
        }

        protected override void OnAttach()
        {
        }

        protected override void OnDetach()
        {
        }

        private void InvalidateRender()
        {
            node.InvalidateRender();
        }
    }
}
