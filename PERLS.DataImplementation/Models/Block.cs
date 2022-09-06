using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Float.Core.Exceptions;
using Newtonsoft.Json;
using PERLS.Data.Converters;
using PERLS.Data.Definition;
using Xamarin.Forms;

namespace PERLS.DataImplementation.Models
{
    /// <summary>
    /// A block of content to display within a section.
    /// </summary>
    [Serializable]
    public class Block : IBlock, IEquatable<IBlock>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Block"/> class.
        /// </summary>
        /// <param name="template">The layout to use for displaying the contents. Required.</param>
        /// <param name="entity">The base entity type to expect for the block contents. Required.</param>
        /// <param name="contentsUri">The API endpoint to get the contents of the block. Required only if the contents dictionary is null.</param>
        /// <param name="blockContents">The block contents. Optional.</param>
        /// <param name="more">The URL to view more. Required.</param>
        /// <param name="name">The name of this block. Required.</param>
        public Block(BlockTemplate template, EntityType entity, Uri contentsUri, BlockContents blockContents, Uri more, string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new InvalidStringArgumentException(nameof(name));
            }

            this.ContentsUri = contentsUri;

            if (template != BlockTemplate.Banner && contentsUri?.IsAbsoluteUri != false)
            {
                throw new ArgumentException("Contents URI must be relative");
            }

            if (blockContents != null)
            {
                this.BlockContents = blockContents;
            }

            this.More = more;

            if (more?.IsAbsoluteUri == false)
            {
                throw new ArgumentException("More URI must be relative, if provided");
            }

            this.Template = template;
            this.Entity = entity;
            this.Name = name;
        }

        /// <inheritdoc />
        public event PropertyChangedEventHandler PropertyChanged;

        /// <inheritdoc />
        public BlockTemplate Template { get; internal set; }

        /// <inheritdoc />
        public EntityType Entity { get; internal set; }

        /// <inheritdoc />
        [JsonProperty("contents_url", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(RelativeUriConverter))]
        public Uri ContentsUri { get; internal set; }

        /// <inheritdoc />
        [JsonProperty("more_url")]
        public Uri More { get; internal set; }

        /// <inheritdoc />
        public string Name { get; internal set; }

        /// <inheritdoc />
        public IBlockContents ContentsDictionary => BlockContents;

        [JsonProperty("contents", NullValueHandling = NullValueHandling.Ignore)]
        BlockContents BlockContents { get; set; }

        /// <summary>
        /// Determines if two blocks are not equivalent using value equality.
        /// </summary>
        /// <param name="left">The first block to compare.</param>
        /// <param name="right">The second block to compare.</param>
        /// <returns><c>true</c> if the blocks are not equivalent, <c>false</c> otherwise.</returns>
        public static bool operator !=(Block left, Block right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Determines if two blocks are equivalent using value equality.
        /// </summary>
        /// <param name="left">The first block to compare.</param>
        /// <param name="right">The second block to compare.</param>
        /// <returns><c>true</c> if the blocks are equivalent, <c>false</c> otherwise.</returns>
        public static bool operator ==(Block left, Block right)
        {
            if (left is null)
            {
                return right is null;
            }

            return left.Equals(right);
        }

        /// <inheritdoc />
        public Task<IEnumerable<IRemoteResource>> GetContentsTask()
        {
            var provider = DependencyService.Get<ICorpusProvider>();
            return provider.GetResources(ContentsUri, Entity);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is IBlock block && Equals(block);
        }

        /// <inheritdoc />
        public bool Equals(IBlock other)
        {
            if (other == null)
            {
                return false;
            }

            return Template == other.Template
                && Entity == other.Entity
                && ContentsUri == other.ContentsUri
                && More == other.More
                && Name == other.Name
                && BlockContents?.Equals(other.ContentsDictionary) != false;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return (Template, Entity, ContentsUri, ContentsDictionary, More, Name).GetHashCode();
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"<Block: Template={Template}, Entity={Entity}, ContentsUri={ContentsUri}, ContentsDictionary={ContentsDictionary}, More={More}, Name={Name}>";
        }
    }
}
