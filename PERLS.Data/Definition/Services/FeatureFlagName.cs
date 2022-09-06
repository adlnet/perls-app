using System;
using Float.Core.Exceptions;

namespace PERLS.Data.Definition.Services
{
    /// <summary>
    /// The feature flag name.
    /// </summary>
    [Serializable]
    public struct FeatureFlagName : IEquatable<FeatureFlagName>
    {
        /// <summary>
        /// A constant value for the enhanced dashboard feature flag name.
        /// </summary>
        public static readonly FeatureFlagName EnhancedDashboard = new FeatureFlagName("new_dashboard");

        readonly string value;

        /// <summary>
        /// Initializes a new instance of the <see cref="FeatureFlagName"/> struct.
        /// </summary>
        /// <param name="name">The name.</param>
        public FeatureFlagName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new InvalidStringArgumentException(nameof(name));
            }

            value = name;
        }

        public static bool operator ==(FeatureFlagName left, FeatureFlagName right) => left.value == right.value;

        public static bool operator !=(FeatureFlagName left, FeatureFlagName right) => !(left == right);

        /// <inheritdoc/>
        public bool Equals(FeatureFlagName other) => ToString() == other.ToString();

        /// <inheritdoc/>
        public override string ToString() => value;

        /// <inheritdoc/>
        public override bool Equals(object obj) => obj is FeatureFlagName name && Equals(name);

        /// <inheritdoc/>
        public override int GetHashCode() => value.GetHashCode();
    }

    /// <summary>
    /// The feature flag.
    /// </summary>
    [Serializable]
    public class FeatureFlag : IEquatable<FeatureFlag>
    {
        /// <summary>
        /// Gets or sets the flag name.
        /// </summary>
        /// <value>
        /// The flag name.
        /// </value>
        public FeatureFlagName Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not this flag is enabled.
        /// </summary>
        /// <value>
        /// A value indicating whether or not this flag is enabled.
        /// </value>
        public bool IsEnabled { get; set; }

        /// <inheritdoc/>
        public override bool Equals(object obj) => obj is FeatureFlag flag && Equals(flag);

        /// <inheritdoc/>
        public bool Equals(FeatureFlag other) => Name.Equals(other?.Name);

        /// <inheritdoc/>
        public override int GetHashCode() => Name.GetHashCode();
    }
}
