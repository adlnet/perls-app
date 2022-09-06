using System.Linq;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(Shell), typeof(PERLS.iOS.Renderers.DefaultShellRenderer))]

namespace PERLS.iOS.Renderers
{
    /// <summary>
    /// Customizes the top tabs in the section renderer.
    /// </summary>
    internal class PERLSShellSectionRenderer : ShellSectionRenderer
    {
        readonly IShellContext context;
        bool topTabsReplaced;

        /// <summary>
        /// Initializes a new instance of the <see cref="PERLSShellSectionRenderer"/> class.
        /// </summary>
        /// <param name="context">The shell context.</param>
        public PERLSShellSectionRenderer(IShellContext context) : base(context)
        {
            this.context = context;
        }

        /// <inheritdoc />
        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();
            ReplaceTopTabs();
        }

        /// <summary>
        /// Visually "replaces" the tabs at the top of the shell section by
        /// adding in new tabs on top of the old tabs.
        /// </summary>
        /// <remarks>
        /// Xamarin.Forms 4.3.0 does not provide a way to customize the appearance
        /// of the cells at the top of the section. However, we can add another set of
        /// tabs on top of the built-in ones.
        /// </remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "The new header cannot be disposed--it is kept for the lifetime of the view.")]
        void ReplaceTopTabs()
        {
            if (topTabsReplaced)
            {
                return;
            }

            topTabsReplaced = true;

            var oldHeader = TopViewController.ChildViewControllers.FirstOrDefault(vc => vc is ShellSectionRootHeader) as ShellSectionRootHeader;
            if (oldHeader == null)
            {
                return;
            }

            var newHeader = new PERLSShellSectionRootHeader(context, new LocalizableCollectionViewFlowLayout())
            {
                ShellSection = ShellSection,
            };

            // The trick in getting the custom tabs to work is that it needs
            // to be added as a child to the original tabs.
            // The original tabs will receive all the layout instructions
            // and as a child, we can inherit those ensuring that our tabs
            // always display on top of the old ones.
            oldHeader.AddChildViewController(newHeader);
            oldHeader.View.AddSubview(newHeader.View);

            newHeader.View.Frame = oldHeader.View.Frame;
            newHeader.View.AutoresizingMask = UIViewAutoresizing.FlexibleHeight | UIViewAutoresizing.FlexibleWidth;

            // Find and remove the shadow.
            // "9002" is the zposition set by Xamarin.Forms and can be used to identify the view
            // https://github.com/xamarin/Xamarin.Forms/blob/f9114b1306f2896cce07d358725f63ce6ab8cac5/Xamarin.Forms.Platform.iOS/Renderers/ShellSectionRootHeader.cs#L158
            oldHeader.CollectionView.Subviews.First(v => v.Layer.ZPosition == 9002).Hidden = true;
        }
    }
}
