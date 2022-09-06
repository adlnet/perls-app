using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Float.TinCan.ActivityLibrary.Definition;
using Newtonsoft.Json;
using PERLS.Data.Definition;
using PERLS.Data.ExperienceAPI;

namespace PERLS.DataImplementation.Models
{
    /// <summary>
    /// Common logic for nodes that are also xAPI types.
    /// </summary>
    /// <remarks>
    /// A casual observer may note a number of unused properties that are hidden from IntelliSense.
    /// This class implements <see cref="IActivity"/> from ActivityLibrary, which was pulled straight out of another project.
    /// As a result, that interface has a number of fields that are not actually required, but are required to be implemented.
    /// </remarks>
    [Serializable]
    public abstract class TinCanNode : Node, IActivity
    {
        /// <summary>
        /// Gets the file for downloading this package's archived representation.
        /// </summary>
        /// <value>The archived package.</value>
        [JsonProperty("file")]
        public File PackageFile { get; internal set; }

        /// <inheritdoc />
        public string UUID => Id.ToString();

        /// <inheritdoc />
        public Float.TinCan.ActivityLibrary.Definition.IFile Thumbnail => Image == null ? null : new TinCanFile(Image);

        /// <inheritdoc />
        public string Section => Topic.Name;

        /// <inheritdoc />
        public string ActivityType => Type;

        /// <inheritdoc />
        public Uri TinCanActivityId => ActivityBuilder.FromResource(this).id;

        /// <inheritdoc />
        public Uri TinCanActivityType => Data.ExperienceAPI.Profiles.Perls.ActivityTypes.Article;

        /// <inheritdoc />
        public ContentStatus ContentCompletableStatus => IsComplete ? ContentStatus.Complete : ContentStatus.Other;

        /// <inheritdoc />
        public Uri ContentUri => TinCanActivityId;

        /// <inheritdoc />
        public IActivityMetaData MetaData => new TinCanMetaData(ContentUri.ToString(), UUID, Name, LastFileModificationTime ?? DateTimeOffset.Now);

        /// <inheritdoc />
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string Keywords => string.Join(", ", Tags.Select(tag => tag.Name));

        /// <inheritdoc />
        [EditorBrowsable(EditorBrowsableState.Never)]
        public IEnumerable<IAudience> Audiences => Enumerable.Empty<IAudience>();

        /// <inheritdoc />
        [EditorBrowsable(EditorBrowsableState.Never)]
        public IEnumerable<Float.TinCan.ActivityLibrary.Definition.IFile> Files => PackageFile == null ? Enumerable.Empty<Float.TinCan.ActivityLibrary.Definition.IFile>() : new List<Float.TinCan.ActivityLibrary.Definition.IFile> { new TinCanFile(PackageFile) };

        /// <inheritdoc />
        [EditorBrowsable(EditorBrowsableState.Never)]
        public IActivityGroup ActivityGroup => new TinCanGroup(Topic);

        /// <inheritdoc />
        [EditorBrowsable(EditorBrowsableState.Never)]
        public DateTimeOffset? LastFileModificationTime => PackageFile?.IsDownloaded == true ? new DateTimeOffset(new System.IO.FileInfo(PackageFile.LocalPath).LastWriteTimeUtc) : default;

        /// <inheritdoc />
        [EditorBrowsable(EditorBrowsableState.Never)]
        public DateTimeOffset? CompletionDate { get; set; }

        /// <inheritdoc />
        [EditorBrowsable(EditorBrowsableState.Never)]
        public DateTimeOffset? NewUntilDate { get; }

        /// <inheritdoc />
        [EditorBrowsable(EditorBrowsableState.Never)]
        public DateTimeOffset? LastUpdatedDate { get; }

        /// <inheritdoc />
        [EditorBrowsable(EditorBrowsableState.Never)]
        public IEnumerable<IPointOfInterest> PointsOfInterest => Enumerable.Empty<IPointOfInterest>();

        /// <inheritdoc />
        [EditorBrowsable(EditorBrowsableState.Never)]
        public double PercentComplete { get; set; }

        /// <inheritdoc />
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsComplete => CompletionDate != null;

        /// <summary>
        /// A class to implement <see cref="Float.TinCan.ActivityLibrary.Definition.IFile"/>.
        /// </summary>
        protected class TinCanFile : Float.TinCan.ActivityLibrary.Definition.IFile
        {
            internal TinCanFile(Data.Definition.IFile file)
            {
                if (file == null)
                {
                    throw new ArgumentNullException(nameof(file));
                }

                this.UUID = Guid.NewGuid().ToString();
                this.Name = file.Name;
                this.FileSize = 1;
                this.LastModificationTime = DateTimeOffset.Now;
                this.MimeType = "application/text";
                this.Url = file.Url;
            }

            internal TinCanFile(string id, string name, double size, DateTimeOffset off, string mime, Uri url)
            {
                this.UUID = id;
                this.Name = name;
                this.FileSize = size;
                this.LastModificationTime = off;
                this.MimeType = mime;
                this.Url = url;
            }

            /// <inheritdoc />
            public string UUID { get; }

            /// <inheritdoc />
            public string Name { get; }

            /// <inheritdoc />
            public double FileSize { get; }

            /// <inheritdoc />
            public DateTimeOffset LastModificationTime { get; }

            /// <inheritdoc />
            public string MimeType { get; }

            /// <inheritdoc />
            public Uri Url { get; }
        }

        /// <summary>
        /// A class to implement <see cref="IActivityGroup"/>.
        /// </summary>
        protected class TinCanGroup : IActivityGroup
        {
            readonly ITopic topic;

            internal TinCanGroup(ITopic topic)
            {
                this.topic = topic;
            }

            /// <inheritdoc />
            public event PropertyChangedEventHandler PropertyChanged
            {
                add { }
                remove { }
            }

            /// <inheritdoc />
            [EditorBrowsable(EditorBrowsableState.Never)]
            public string UUID => topic?.Id.ToString();

            /// <inheritdoc />
            [EditorBrowsable(EditorBrowsableState.Never)]
            public string Name => topic?.Name;

            /// <inheritdoc />
            [EditorBrowsable(EditorBrowsableState.Never)]
            public Uri TinCanActivityId => topic?.Url;

            /// <inheritdoc />
            [EditorBrowsable(EditorBrowsableState.Never)]
            public Uri TinCanActivityType => Data.ExperienceAPI.Profiles.Perls.ActivityTypes.Category;

            /// <inheritdoc />
            [EditorBrowsable(EditorBrowsableState.Never)]
            public ContentStatus ContentCompletableStatus => IsComplete ? ContentStatus.Complete : ContentStatus.Other;

            /// <inheritdoc />
            [EditorBrowsable(EditorBrowsableState.Never)]
            public Uri ContentUri => topic?.Url;

            /// <inheritdoc />
            [EditorBrowsable(EditorBrowsableState.Never)]
            public string Keywords => null;

            /// <inheritdoc />
            [EditorBrowsable(EditorBrowsableState.Never)]
            public int Weight => 0;

            /// <inheritdoc />
            [EditorBrowsable(EditorBrowsableState.Never)]
            public Float.TinCan.ActivityLibrary.Definition.IFile Thumbnail => null;

            /// <inheritdoc />
            [EditorBrowsable(EditorBrowsableState.Never)]
            public IEnumerable<IAudience> Audiences => Enumerable.Empty<IAudience>();

            /// <inheritdoc />
            [EditorBrowsable(EditorBrowsableState.Never)]
            public IEnumerable<IActivity> Activities => Enumerable.Empty<IActivity>();

            /// <inheritdoc />
            [EditorBrowsable(EditorBrowsableState.Never)]
            public Float.TinCan.ActivityLibrary.Definition.IPackage Package => null;

            /// <inheritdoc />
            [EditorBrowsable(EditorBrowsableState.Never)]
            public IActivityGroup ParentActivityGroup => null;

            /// <inheritdoc />
            [EditorBrowsable(EditorBrowsableState.Never)]
            public IEnumerable<IActivityGroup> ChildActivityGroups => Enumerable.Empty<IActivityGroup>();

            /// <inheritdoc />
            [EditorBrowsable(EditorBrowsableState.Never)]
            public IEnumerable<Float.TinCan.ActivityLibrary.Definition.ILink> Links => Enumerable.Empty<Float.TinCan.ActivityLibrary.Definition.ILink>();

            /// <inheritdoc />
            [EditorBrowsable(EditorBrowsableState.Never)]
            public int NumNewActivities => 0;

            /// <inheritdoc />
            [EditorBrowsable(EditorBrowsableState.Never)]
            public bool IsCertificateAvailable => false;

            /// <inheritdoc />
            [EditorBrowsable(EditorBrowsableState.Never)]
            public double PercentComplete { get => IsComplete ? 1 : 0; set => _ = value; }

            /// <inheritdoc />
            [EditorBrowsable(EditorBrowsableState.Never)]
            public bool IsComplete => false;
        }
    }
}
