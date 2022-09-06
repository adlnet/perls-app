using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using PERLS.Data;
using PERLS.Data.Definition;
using PERLS.Data.ViewModels;
using Xamarin.Forms;

namespace PERLS.Pages.Settings
{
    /// <summary>
    /// The acknowledgements page.
    /// </summary>
    public partial class AcknowledgementsPage : BasePage
    {
        private const string MitLicenseText = "Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the \"Software\"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and / or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:\n\nThe above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.\n\nTHE SOFTWARE IS PROVIDED \"AS IS\", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.";
        private const string ApacheLicenseText = "Licensed under the Apache License, Version 2.0 (the \"License\"); you may not use this file except in compliance with the License. You may obtain a copy of the License at\n\nhttp://www.apache.org/licenses/LICENSE-2.0\n\nUnless required by applicable law or agreed to in writing, software distributed under the License is distributed on an \"AS IS\" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License.";
        private const string MicrosoftPublicLicenseText = "This license governs use of the accompanying software. If you use the software, you accept this license. If you do not accept the license, do not use the software.\n\n1. Definitions\nThe terms \"reproduce,\" \"reproduction,\" \"derivative works,\" and \"distribution\" have the same meaning here as under U.S. copyright law.\n\nA \"contribution\" is the original software, or any additions or changes to the software.\n\nA \"contributor\" is any person that distributes its contribution under this license.\n\n\"Licensed patents\" are a contributor's patent claims that read directly on its contribution.\n\n2. Grant of Rights\n\n(A) Copyright Grant- Subject to the terms of this license, including the license conditions and limitations in section 3, each contributor grants you a non-exclusive, worldwide, royalty-free copyright license to reproduce its contribution, prepare derivative works of its contribution, and distribute its contribution or any derivative works that you create.\n\n(B) Patent Grant- Subject to the terms of this license, including the license conditions and limitations in section 3, each contributor grants you a non-exclusive, worldwide, royalty-free license under its licensed patents to make, have made, use, sell, offer for sale, import, and/or otherwise dispose of its contribution in the software or derivative works of the contribution in the software.\n\n3. Conditions and Limitations\n\n(A) No Trademark License- This license does not grant you rights to use any contributors' name, logo, or trademarks.\n\n(B) If you bring a patent claim against any contributor over patents that you claim are infringed by the software, your patent license from such contributor to the software ends automatically.\n\n(C) If you distribute any portion of the software, you must retain all copyright, patent, trademark, and attribution notices that are present in the software.\n\n(D) If you distribute any portion of the software in source code form, you may do so only under this license by including a complete copy of this license with your distribution. If you distribute any portion of the software in compiled or object code form, you may only do so under a license that complies with this license.\n\n(E) The software is licensed \"as-is.\" You bear the risk of using it. The contributors give no express warranties, guarantees or conditions. You may have additional consumer rights under your local laws which this license cannot change. To the extent permitted under your local laws, the contributors exclude the implied warranties of merchantability, fitness for a particular purpose and non-infringement.";

        /// <summary>
        /// Initializes a new instance of the <see cref="AcknowledgementsPage"/> class.
        /// </summary>
        public AcknowledgementsPage()
        {
            InitializeComponent();
            LoadAcknowledgements();
            Title = Strings.ViewAcknowledgementsLabel.ToUpper(CultureInfo.CurrentCulture);
        }

        /// <inheritdoc />
        protected override void OnAppearing()
        {
            base.OnAppearing();

            listView.SelectedItem = null;
        }

        void AcknowledgementSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
            {
                return;
            }

            listView.SelectedItem = null;
        }

        /// <summary>
        /// Loads the acknowledgements.
        /// </summary>
        void LoadAcknowledgements()
        {
            // these are just sorted by how the projects appear in the solution navigator
            // as you add dependencies, please post the acknowledgement under the applicable section (as marked by commments)
            var acknowledgements = new List<Acknowledgement>
            {
                new Acknowledgement
                {
                    Product = ".NET",
                    Copyright = "The MIT License (MIT), Copyright (c) Microsoft, All rights reserved.",
                    LicenseText = MitLicenseText,
                },
                new Acknowledgement
                {
                    Product = "Acr.UserDialogs",
                    Copyright = "The MIT License, Copyright (c) 2016 Allan Ritchie",
                    LicenseText = MitLicenseText,
                },
                new Acknowledgement
                {
                    Product = "Float.Corcav.Behaviors",
                    Copyright = "The MIT License, Copyright (c) 2013-2016 Maksim Valkau, (c) 2022 Float",
                    LicenseText = MitLicenseText,
                },
                new Acknowledgement
                {
                    Product = "TinCan Library",
                    Copyright = "Copyright 2014 Rustici Software",
                    LicenseText = ApacheLicenseText,
                },
                new Acknowledgement
                {
                    Product = "JsonNet.PrivateSettersContractResolvers",
                    Copyright = "The MIT License, Copyright (c) 2016, 2017, 2018 Daniel Wertheim",
                    LicenseText = MitLicenseText,
                },
                new Acknowledgement
                {
                    Product = "JsonSubTypes",
                    Copyright = "The MIT License, Copyright (c) 2017 Emmanuel Counasse",
                    LicenseText = MitLicenseText,
                },
                new Acknowledgement
                {
                    Product = "Microsoft App Center",
                    Copyright = "The MIT License (MIT), Copyright (c) Microsoft, All rights reserved.",
                    LicenseText = MitLicenseText,
                },
                new Acknowledgement
                {
                    Product = "Microsoft.CodeAnalysis.FxCopAnalyzers",
                    Copyright = "Copyright (c) .NET Foundation Contributors",
                    LicenseText = ApacheLicenseText,
                },
                new Acknowledgement
                {
                    Product = "NewtonSoft.JSON",
                    Copyright = "The MIT License (MIT), Copyright (c) 2007 James Newton-King",
                    LicenseText = MitLicenseText,
                },
                new Acknowledgement
                {
                    Product = "NuGet.Build.Packaging",
                    Copyright = "Copyright (c) .NET Foundation Contributors",
                    LicenseText = ApacheLicenseText,
                },
                new Acknowledgement
                {
                    Product = "Rg.Plugins.Popup",
                    Copyright = "The MIT License (MIT)",
                    LicenseText = MitLicenseText,
                },
                new Acknowledgement
                {
                    Product = "SkiaSharp.Views.Forms",
                    Copyright = "The MIT License (MIT), Copyright (c) 2015-2016 Xamarin, Inc., Copyright (c) 2017-2018 Microsoft Corporation",
                    LicenseText = MitLicenseText,
                },
                new Acknowledgement
                {
                    Product = "StyleCop.Analyzers",
                    Copyright = "Copyright (c) Tunnel Vision Laboratories, LLC",
                    LicenseText = ApacheLicenseText,
                },
                new Acknowledgement
                {
                    Product = "Xamarin.Android SDK",
                    Copyright = "The MIT License (MIT), Copyright (c) .NET Foundation Contributors",
                    LicenseText = MitLicenseText,
                },
                new Acknowledgement
                {
                    Product = "Xamarin Essentials",
                    Copyright = "The MIT License (MIT), Copyright (c) Microsoft Corporation",
                    LicenseText = MitLicenseText,
                },
                new Acknowledgement
                {
                    Product = "FFImageLoading - Fast & Furious Image Loading",
                    Copyright = "The MIT License (MIT), Copyright (c) 2015 Daniel Luberda & Fabien Molinet",
                    LicenseText = MitLicenseText,
                },
                new Acknowledgement
                {
                    Product = "Xamarin.Forms",
                    Copyright = "The MIT License (MIT), Copyright (c) Microsoft Corporation",
                    LicenseText = MitLicenseText,
                },
                new Acknowledgement
                {
                    Product = "Xamarin.Forms.PageControl",
                    Copyright = "Microsoft Public License (MS-PL)",
                    LicenseText = MicrosoftPublicLicenseText,
                },
                new Acknowledgement
                {
                    Product = "Xamarin SDK",
                    Copyright = "The MIT License (MIT), Copyright (c) .NET Foundation Contributors",
                    LicenseText = MitLicenseText,
                },
                new Acknowledgement
                {
                    Product = "Float.XFormsTouch",
                    Copyright = "(c) 2018 Perpetual Mobile GmbH, (c) 2022 Float, LLC",
                    LicenseText = ApacheLicenseText,
                },
            };

            if (Device.RuntimePlatform == Device.Android)
            {
                acknowledgements.Add(new Acknowledgement
                {
                    Product = "Android Open Source Project",
                    Copyright = "Copyright (C) 2011 The Android Open Source Project",
                    LicenseText = ApacheLicenseText,
                });
            }

            listView.ItemsSource = new AcknowledgementListViewModel(acknowledgements.OrderBy((arg) => arg.Product));
        }
    }
}
