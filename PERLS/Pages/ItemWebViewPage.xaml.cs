using System.Collections.Generic;
using System.Net;
using PERLS.Data;
using PERLS.Data.ViewModels;
using PERLS.Services;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;

namespace PERLS.Pages
{
    /// <summary>
    /// Html page.
    /// </summary>
    public partial class ItemWebViewPage : BasePage
    {
        LocalHttpServer server = DependencyService.Get<LocalHttpServer>();
        Cookie localServerCookie;

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemWebViewPage"/> class.
        /// </summary>
        public ItemWebViewPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemWebViewPage"/> class.
        /// </summary>
        /// <param name="viewModel">The View model.</param>
        public ItemWebViewPage(AuthenticatingWebViewViewModel viewModel)
        {
            BindingContext = viewModel;
            InitializeComponent();

            var handlers = new List<IJavaScriptActionHandler>();

            if (BindingContext is ItemWebViewViewModel)
            {
                handlers.Add(new AjaxLinkActionHandler());
                handlers.Add(new VidyoTargetChangeHandler());
                handlers.Add(new TagLinkChangeHandler());
                handlers.Add(new WindowCloseHandler(Navigation));
                handlers.Add(new BlobHandler());
                handlers.Add(new NextArticleActionHandler());
            }

            if (BindingContext is LocalItemWebViewViewModel)
            {
                handlers.Add(new AjaxLinkActionHandler());
                handlers.Add(new WindowCloseHandler(Navigation));
                handlers.Add(new TagLinkChangeHandler());
                handlers.Add(new BlobHandler());
                handlers.Add(new NextArticleActionHandler());
            }

            JavaScriptEffect.Handlers = handlers;
        }

        /// <inheritdoc />
        public override bool AllowLandscape => true;

        /// <inheritdoc />
        public override bool HidesBarsInLandscape => Device.Idiom == TargetIdiom.Phone;

        /// <inheritdoc />
        protected override bool UsesCustomNavigationBar => BindingContext is WebViewViewModel;

        /// <inheritdoc />
        public override void OnUnload()
        {
            base.OnUnload();

            DependencyService.Get<HttpServerSecurity>().RemoveCookie(localServerCookie);
            server.PropertyChanged -= ServerPropertyChanged;
        }

        /// <inheritdoc />
        protected override void OnParentSet()
        {
            base.OnParentSet();

            if (CurrentShell != null)
            {
                var label = new Label
                {
                    Style = (Style)Xamarin.Forms.Application.Current.Resources["TextStyle"],
                    Text = Title,
                    HorizontalTextAlignment = TextAlignment.Center,
                    VerticalTextAlignment = TextAlignment.Center,
                    MaxLines = 2,
                    LineBreakMode = LineBreakMode.TailTruncation,
                };
                label.FontSize = Device.GetNamedSize(NamedSize.Small, label);

                Shell.SetTitleView(this, label);
            }
        }

        /// <inheritdoc />
        protected override void OnAppearing()
        {
            base.OnAppearing();

            Xamarin.Forms.Application.Current.On<Xamarin.Forms.PlatformConfiguration.Android>().UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Resize);

            if (BindingContext is AuthenticatingWebViewViewModel authenticatingWebViewViewModel && authenticatingWebViewViewModel.RefreshOnAppear)
            {
                authenticatingWebViewViewModel.Refresh();
            }

            if (BindingContext is LocalItemWebViewViewModel localItemViewModel)
            {
                if (webView.Cookies == null)
                {
                    webView.Cookies = new CookieContainer();
                }

                var cookie = DependencyService.Get<HttpServerSecurity>().GenerateCookie(server.Uri);
                localServerCookie = cookie;
                webView.Cookies.Add(localServerCookie);
                webView.Source = localItemViewModel.Url;
                server.PropertyChanged += ServerPropertyChanged;
            }
        }

        /// <inheritdoc />
        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            if (BindingContext is EditProfileViewModel editProfileViewModel)
            {
                editProfileViewModel.RefreshProfile();
            }

            Xamarin.Forms.Application.Current.On<Xamarin.Forms.PlatformConfiguration.Android>().UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Pan);
        }

        void ServerPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            webView.Reload();
        }
    }
}
