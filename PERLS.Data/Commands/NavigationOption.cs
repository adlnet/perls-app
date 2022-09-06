using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace PERLS.Data.Commands
{
    /// <summary>
    /// A navigation option the user can select from.
    /// </summary>
    /// <typeparam name="T">The destination type (probably an enum).</typeparam>
    public class NavigationOption<T> : IEquatable<NavigationOption<T>>, INavigationOption<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationOption{T}"/> class.
        /// </summary>
        public NavigationOption()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationOption{T}"/> class.
        /// </summary>
        /// <param name="destination">The destination.</param>
        /// <param name="text">An optional label for the option.</param>
        /// <param name="showArrow">Whether or not to show a navigation arrow.</param>
        /// <param name="useAltColor">Whether or not to use an alternate color for this option.</param>
        public NavigationOption(T destination, string text = null, bool showArrow = true, bool useAltColor = false)
        {
            Destination = destination;
            Text = text;
            ShowArrow = showArrow;
            TextColor = (Color)Application.Current.Resources[useAltColor ? "SecondaryColor" : "PrimaryTextColor"];
        }

        /// <inheritdoc />
        public string Text { get; set; }

        /// <inheritdoc />
        public T Destination { get; set; }

        /// <inheritdoc />
        public Color TextColor { get; set; }

        /// <inheritdoc />
        public bool ShowArrow { get; set; }

        /// <summary>
        /// Determine if two <see cref="NavigationOption{T}"/> are equal.
        /// </summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns><c>true</c> if both instances are equivalent, <c>false</c> otherwise.</returns>
        public static bool operator ==(NavigationOption<T> left, NavigationOption<T> right)
        {
            if (ReferenceEquals(left, null))
            {
                return ReferenceEquals(right, null);
            }

            return left.Equals(right);
        }

        /// <summary>
        /// Determine if two <see cref="NavigationOption{T}"/> are inequal.
        /// </summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns><c>true</c> if both instances are not equivalent, <c>false</c> otherwise.</returns>
        public static bool operator !=(NavigationOption<T> left, NavigationOption<T> right)
        {
            return !(left == right);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        /// <inheritdoc />
        public bool Equals(NavigationOption<T> other)
        {
            if (ReferenceEquals(other, null))
            {
                return ReferenceEquals(this, null);
            }

            return EqualityComparer<T>.Default.Equals(Destination, other.Destination);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return 1821182775 + EqualityComparer<T>.Default.GetHashCode(Destination);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            if (Text != null)
            {
                return Text;
            }

            return base.ToString();
        }
    }
}
