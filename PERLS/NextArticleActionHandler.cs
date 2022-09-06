using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using PERLS.Data;
using PERLS.Data.Definition;
using Xamarin.Forms;

namespace PERLS
{
    /// <summary>
    /// A javascript handler for navigating to the next article.
    /// </summary>
    public class NextArticleActionHandler : IJavaScriptActionHandler
    {
        /// <inheritdoc/>
        public string AttachedJavaScript => @"
        (function() {
            function waitForElement(selector) {
                return new Promise(resolve => {
                    if (document.querySelector(selector)) {
                        return resolve(document.querySelector(selector));
                    }

                    const observer = new MutationObserver(mutations => {
                        if (document.querySelector(selector)) {
                            resolve(document.querySelector(selector));
                            observer.disconnect();
                        }
                    });

                    observer.observe(document.body, {
                        childList: true,
                        subtree: true
                    });
                });
            }

            waitForElement('.related-course-content__courses').then((elm) => {
                // Even though we could use elm here there are likely a number of items that were just added and this is only called once
                var nextCourseCards = document.querySelectorAll('.related-course-content__courses .c-card__link, .related-course-content__courses .c-card__title-link');
                for (var i = 0; i < nextCourseCards.length; i++) {
                    var link = nextCourseCards[i];
                    link.onclick = (e) => {
                        var data = {
                            action: ""next_article"",
                            data: { target: e.srcElement.attributes.href.textContent },
                        };
                        invokeCSharpWebFormSubmit(JSON.stringify(data));
                        e.stopImmediatePropagation();
                        e.preventDefault();
                    }
                }
            });
        })();";

        /// <inheritdoc/>
        public string ActionName => "next_article";

        /// <inheritdoc/>
        public void PerformAction(Effect element, object data)
        {
            if (data is not JObject dataObject)
            {
                return;
            }

            var url = dataObject["target"]?.ToString();
            DependencyService.Get<INavigationCommandProvider>().NextArticleSelected.Execute(new Uri(url));
        }
    }
}
