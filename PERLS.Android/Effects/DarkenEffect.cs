using System;
using System.ComponentModel;
using System.Linq;
using Android.Views;
using PERLS.Droid.Effects;
using PERLS.Effects;
using Xamarin.Forms.Platform.Android;

[assembly: Xamarin.Forms.ExportEffect(typeof(AndroidDarkenEffect), nameof(DarkenEffect))]

namespace PERLS.Droid.Effects
{
    /// <summary>
    /// The Android implementation of the Darken Effect.
    /// </summary>
    public sealed class AndroidDarkenEffect : PlatformEffect, IDisposable
    {
        View darkenView;

        DarkenEffect DarkenEffect => Element?.Effects.FirstOrDefault(e => e is DarkenEffect) as DarkenEffect;

        /// <inheritdoc />
        public void Dispose()
        {
            darkenView?.RemoveFromParent();
            darkenView?.Dispose();
            darkenView = null;
        }

        /// <inheritdoc />
        protected override void OnAttached()
        {
            if (Container == null || DarkenEffect == null || DarkenEffect.DarkenMultiplier < 0 || DarkenEffect.DarkenMultiplier > 1)
            {
                return;
            }

            darkenView = new View(Container.Context);
            darkenView.SetBackgroundColor(new Android.Graphics.Color(0, 0, 0, (int)(255 * DarkenEffect.DarkenMultiplier)));

            // Find the first parent that has a background and darken that.
            // This pretty aggressively assumes that the first container with a background
            // is the view that is "most visible" and defining the shape of the view.
            var parent = Container;
            while (parent.Background == null && parent.GetChildAt(0) is ViewGroup viewGroup)
            {
                parent = viewGroup;
            }

            parent.AddView(darkenView);
            parent.LayoutChange += OnContainerLayout;
            darkenView.BringToFront();
        }

        /// <inheritdoc/>
        protected override void OnElementPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnElementPropertyChanged(args);

            if (Container == null || DarkenEffect == null || DarkenEffect.DarkenMultiplier < 0 || DarkenEffect.DarkenMultiplier > 1 || darkenView == null)
            {
                return;
            }

            darkenView.SetBackgroundColor(new Android.Graphics.Color(0, 0, 0, (int)(255 * DarkenEffect.DarkenMultiplier)));
        }

        /// <inheritdoc/>
        protected override void OnDetached()
        {
            if (darkenView != null && darkenView.Parent is View view)
            {
                view.LayoutChange -= OnContainerLayout;
            }

            Dispose();
        }

        void OnContainerLayout(object sender, View.LayoutChangeEventArgs e)
        {
            // Resize the darkening view to fill the entire "real container"
            darkenView.Layout(0, 0, e.Right, e.Bottom);
        }
    }
}
