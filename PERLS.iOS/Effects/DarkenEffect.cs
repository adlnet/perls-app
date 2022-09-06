using System;
using System.ComponentModel;
using System.Linq;
using CoreGraphics;
using PERLS.Effects;
using PERLS.iOS.Effects;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportEffect(typeof(AppleDarkenEffect), nameof(DarkenEffect))]

namespace PERLS.iOS.Effects
{
    /// <summary>
    /// The Darken Effect implementation.
    /// </summary>
    public sealed class AppleDarkenEffect : PlatformEffect, IDisposable
    {
        readonly UIView darkenView = new UIView();

        DarkenEffect DarkenEffect => Element?.Effects.FirstOrDefault(e => e is DarkenEffect) as DarkenEffect;

        /// <inheritdoc />
        public void Dispose()
        {
            darkenView?.RemoveFromSuperview();
            darkenView?.Dispose();
        }

        /// <inheritdoc />
        protected override void OnAttached()
        {
            if (DarkenEffect == null || DarkenEffect.DarkenMultiplier < 0)
            {
                return;
            }

            darkenView.Frame = Container.Bounds;

            darkenView.BackgroundColor = UIColor.Black;
            darkenView.Alpha = (nfloat)DarkenEffect.DarkenMultiplier;
            darkenView.Layer.CornerRadius = (nfloat)DarkenEffect.CornerRadius;
            darkenView.Layer.MasksToBounds = true;

            var container = Container.Subviews.FirstOrDefault((arg) => arg.BackgroundColor != null);
            container.AddSubview(darkenView);
            container.BringSubviewToFront(darkenView);

            Element.PropertyChanged += ElementPropertyChanged;
        }

        /// <inheritdoc/>
        protected override void OnDetached()
        {
            Element.PropertyChanged -= ElementPropertyChanged;
            Dispose();
        }

        /// <inheritdoc/>
        protected override void OnElementPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnElementPropertyChanged(args);

            if (darkenView == null || DarkenEffect == null)
            {
                return;
            }

            darkenView.Alpha = (nfloat)DarkenEffect.DarkenMultiplier;
        }

        private void ElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (Container == null)
            {
                return;
            }

            darkenView.Frame = Container.Bounds;
        }
    }
}
