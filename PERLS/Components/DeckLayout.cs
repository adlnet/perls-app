using System;
using System.Collections.Generic;
using System.Linq;
using PERLS.Effects;
using Xamarin.Forms;

namespace PERLS.Components
{
    /// <summary>
    /// A layout which arranges it's children to look like a deck of cards.
    /// The deck can be browsed by moving forward or back through the deck.
    /// </summary>
    public class DeckLayout : AbsoluteLayout
    {
        /// <summary>
        /// Identifies the <see cref="SelectedIndex"/> property.
        /// </summary>
        public static readonly BindableProperty SelectedIndexProperty = BindableProperty.Create(nameof(SelectedIndex), typeof(int), typeof(DeckLayout), 0, BindingMode.TwoWay, null, new BindableProperty.BindingPropertyChangedDelegate(SelectedIndexPropertyChanged));

        readonly List<View> viewsToRearrange = new List<View>();

        /// <summary>
        /// Gets or sets the index which should be the top of the deck.
        /// </summary>
        /// <value>The index of the child at the top of the deck.</value>
        public int SelectedIndex
        {
            get => (int)GetValue(SelectedIndexProperty);
            set
            {
                var index = Math.Max(0, Math.Min(value, Children.Count - 1));

                if (index == SelectedIndex)
                {
                    return;
                }

                SetValue(SelectedIndexProperty, index);
                UpdateLayout();
            }
        }

        /// <summary>
        /// Gets or sets the amount to translate each child.
        /// </summary>
        /// <value>The translation offset applied to each child.</value>
        public int TranslateOffset { get; set; } = 20;

        /// <summary>
        /// Gets or sets the amount to decrease the scale of each child.
        /// </summary>
        /// <value>The scale offset applied to each child.</value>
        public double ScaleOffset { get; set; } = 0.05;

        /// <summary>
        /// Gets or sets the amount to rotate each child.
        /// </summary>
        /// <value>The rotation offset applied to each child.</value>
        public double RotateOffset { get; set; } = 0;

        /// <summary>
        /// Gets or sets the amount to decrease the opacity of each child.
        /// </summary>
        /// <value>The opacity offset.</value>
        public double OpacityOffset { get; set; } = 0;

        /// <summary>
        /// Gets or sets the amount in which the backviews will be covered per index.
        /// </summary>
        /// <value>The opacity to cover the backviews.</value>
        public double BackViewCoverOpacity { get; set; } = 0.2;

        /// <summary>
        /// Gets or sets the number of children that are visible.
        /// </summary>
        /// <value>The number of visible children.</value>
        public uint VisibleDepth { get; set; } = 3;

        /// <summary>
        /// Gets or sets the top item's occupied width.
        /// </summary>
        /// <value>
        /// How much width the top item will occupy, as a percentage.
        /// </value>
        public double TopItemOccupiedWidth { get; set; } = 0.95;

        /// <summary>
        /// Gets or sets the number of milliseconds the animation should last for when animating the deck to a new position.
        /// </summary>
        /// <value>The animation duration.</value>
        public uint AnimationDuration { get; set; } = 250;

        /// <summary>
        /// Gets or sets the easing function to use when animating children to their new position.
        /// </summary>
        /// <value>The animation easing function.</value>
        public Easing AnimationEasing { get; set; } = Easing.SinOut;

        /// <summary>
        /// Advances to the deck to the next child.
        /// </summary>
        public void Next() => SelectedIndex++;

        /// <summary>
        /// Retreats to the previous child.
        /// </summary>
        public void Previous() => SelectedIndex--;

        /// <summary>
        /// Called when the Selectedindex property changes.
        /// </summary>
        /// <param name="bindable">The bindable object.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        protected static void SelectedIndexPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is DeckLayout layout)
            {
                layout.UpdateLayout();
            }
        }

        /// <inheritdoc />
        protected override void OnAdded(View view)
        {
            PrepareView(view);
            base.OnAdded(view);

            // Child views must be ordered in reverse so that the first child
            // appears as the topmost card on the deck.
            // The child cannot be moved while it is being added, so instead we
            // keep track of which children need to be rearranged and take care
            // of that before the next layout pass.
            viewsToRearrange.Add(view);
        }

        /// <inheritdoc />
        protected override void OnRemoved(View view)
        {
            base.OnRemoved(view);
            viewsToRearrange.Remove(view);
        }

        /// <inheritdoc />
        protected override void LayoutChildren(double x, double y, double width, double height)
        {
            // Check if new children have been added since the last layout pass.
            if (viewsToRearrange.Any())
            {
                RearrangeChildren();
                return;
            }

            base.LayoutChildren(x, y, width, height);
        }

        /// <summary>
        /// Animated the update the layout of each child.
        /// </summary>
        protected void UpdateLayout()
        {
            // If there are any views to rearrange, we defer updating the layout.
            // Once the views have all been rearranged, `UpdateLayout` will be invoked.
            // See `RearrangeChildren`.
            if (viewsToRearrange.Any())
            {
                return;
            }

            for (var i = 0; i < Children.Count; ++i)
            {
                var child = Children[i];

                SetLayoutFlags(child, AbsoluteLayoutFlags.All);
                SetLayoutBounds(child, new Rectangle(0, 0, TopItemOccupiedWidth, 1));

                var deckPosition = Children.Count - i - SelectedIndex - 1;
                TransformView(child, deckPosition);
            }
        }

        /// <summary>
        /// Prepares the view to be part of the deck.
        /// This initializes the translation, scale, rotation, and opacity of the view.
        /// </summary>
        /// <param name="view">The view to prepare.</param>
        protected virtual void PrepareView(View view)
        {
            var preparedDepth = (int)Math.Min(Children.Count, VisibleDepth);

            var translation = GetTranslation(preparedDepth);
            view.TranslationX = translation.X;
            view.TranslationY = translation.Y;
            view.Scale = GetScale(preparedDepth);
            view.Rotation = GetRotation(preparedDepth);
            view.Opacity = GetOpacity(preparedDepth);
            view.IsVisible = false;
        }

        /// <summary>
        /// Animates the transformation of the view to the specifed position in the deck.
        /// </summary>
        /// <param name="view">The view to animate.</param>
        /// <param name="toPosition">The position index to animate to where 0 is the front-most child.</param>
        /// <remarks>
        /// "Position" refers to the distance of the view from the currently visible child in the deck.
        /// A position of 0 means the child is currently the visible child at the top of the deck.
        /// A positive position means it is under the visible child at the top of the deck.
        /// A negative position means it is off the top of the deck.
        /// </remarks>
        protected virtual void TransformView(View view, int toPosition)
        {
            view.IsEnabled = toPosition == 0;
            view.InputTransparent = toPosition != 0;
            view.IsVisible = IsPositionVisible(toPosition);

            ViewExtensions.CancelAnimations(view);

            var translation = GetTranslation(toPosition);
            if (view.TranslationX != translation.X || view.TranslationY != translation.Y)
            {
                view.TranslateTo(translation.X, translation.Y, AnimationDuration, AnimationEasing);
            }

            var scale = GetScale(toPosition);
            if (view.Scale != scale)
            {
                view.ScaleTo(GetScale(toPosition), AnimationDuration, AnimationEasing);
            }

            var rotation = GetRotation(toPosition);
            if (view.Rotation != rotation)
            {
                view.RotateTo(GetRotation(toPosition), AnimationDuration, AnimationEasing);
            }

            var opacity = GetOpacity(toPosition);
            if (view.Opacity != opacity)
            {
                view.FadeTo(GetOpacity(toPosition), AnimationDuration, AnimationEasing);
            }

            if (toPosition == 0)
            {
                if (view.Effects.Any((arg) => arg is DarkenEffect))
                {
                    var darkenEffect = view.Effects.Where((arg) => arg is DarkenEffect).FirstOrDefault();
                    view.Effects.Remove(darkenEffect);
                }
            }
            else
            {
                if (!view.Effects.Any((arg) => arg is DarkenEffect) && view.IsVisible)
                {
                    view.Effects.Add(new DarkenEffect()
                    {
                        DarkenMultiplier = .1 * toPosition,
                        CornerRadius = 10.0,
                    });
                }
                else if (view.Effects.FirstOrDefault((arg) => arg is DarkenEffect) is DarkenEffect darkenEffect)
                {
                    darkenEffect.DarkenMultiplier = .1 * toPosition;
                }
            }
        }

        /// <summary>
        /// Gets the X and Y translation for the specified position in the deck.
        /// </summary>
        /// <param name="position">The deck position.</param>
        /// <returns>The X and Y translation for the specified deck position.</returns>
        protected virtual Point GetTranslation(int position)
        {
            if (position < 0)
            {
                return new Point(-Width, 0);
            }

            return new Point(position * TranslateOffset, 0);
        }

        /// <summary>
        /// Gets the scale for the specified position in the deck.
        /// </summary>
        /// <param name="position">The deck position.</param>
        /// <returns>The scale for the specified deck position.</returns>
        protected virtual double GetScale(int position)
        {
            if (position < 0)
            {
                return 1;
            }

            return 1 - (position * ScaleOffset);
        }

        /// <summary>
        /// Gets the rotation for the specified position in the deck.
        /// </summary>
        /// <param name="position">The deck position.</param>
        /// <returns>The rotation for the specified deck position.</returns>
        protected virtual double GetRotation(int position)
        {
            if (position < 0)
            {
                return 0;
            }

            return position * RotateOffset;
        }

        /// <summary>
        /// Gets the opacity for the specified position in the deck.
        /// </summary>
        /// <param name="position">The deck position.</param>
        /// <returns>The opacity for the specified deck position.</returns>
        protected virtual double GetOpacity(int position)
        {
            if (position > VisibleDepth - 1)
            {
                return 0;
            }

            if (position < 0)
            {
                return 0;
            }

            return 1 - (position * OpacityOffset);
        }

        /// <summary>
        /// Gets whether the view should be visible when it is at the specified position in the deck.
        /// </summary>
        /// <param name="position">The deck position.</param>
        /// <returns>A value indicating whether the view should be visible.</returns>
        protected virtual bool IsPositionVisible(int position)
        {
            return position >= -1 && position < VisibleDepth;
        }

        /// <summary>
        /// Ensures that children are ordered in reverse so that the first added
        /// child appears as the topmost card on the deck.
        /// (Making the first added child the last view in the layout.)
        /// </summary>
        void RearrangeChildren()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                bool shouldUpdateLayout = viewsToRearrange.Any();
                foreach (var v in viewsToRearrange.ToList())
                {
                    LowerChild(v);
                    viewsToRearrange.Remove(v);
                }

                if (shouldUpdateLayout)
                {
                    UpdateLayout();
                }
            });
        }
    }
}
