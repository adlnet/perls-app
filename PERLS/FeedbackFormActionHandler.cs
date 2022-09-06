using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using Float.HttpServer;
using PERLS.Data.Definition.Services;
using TinCan;
using TinCan.Json;
using Xamarin.Forms;

namespace PERLS
{
    /// <summary>
    /// Handles JS Feedback Form Submissions.
    /// </summary>
    public class FeedbackFormActionHandler : IHttpResponder
    {
        private const string JsonResponse = @"[{""command"": ""insert"",""method"": ""html"",""selector"": ""#webform-submission-content-specific-webform-node-:nodeId-form-ajax"",""data"": ""<form class=\""webform-submission-form webform-submission-add-form webform-submission-content-specific-webform-form webform-submission-content-specific-webform-add-form webform-submission-content-specific-webform-node-:nodeId-form webform-submission-content-specific-webform-node-:nodeId-add-form c-form c-form--webform-submission-content-specific-webform-node-:nodeId-add-form--OiKldT-dqUU u-spacing\"" data-drupal-selector=\""webform-submission-content-specific-webform-node-:nodeId-add-form-oikldt-dquu\"" action=\""/node/:nodeId\"" method=\""post\"" id=\""webform-submission-content-specific-webform-node-:nodeId-add-form--OiKldT-dqUU\"" accept-charset=\""UTF-8\"">\n<div class=\""webform-confirmation\"">\n<div class=\""webform-confirmation__message\"">Thank you for your feedback!</div>\n</div>\n</form>\n"",""settings"": null}]";

        /// <summary>
        /// Initializes a new instance of the <see cref="FeedbackFormActionHandler"/> class.
        /// </summary>
        public FeedbackFormActionHandler()
        {
        }

        /// <inheritdoc/>
        public void GenerateResponse(in HttpListenerRequest httpRequest, ref HttpListenerResponse httpResponse, IDictionary<string, string> parameters)
        {
            if (httpRequest == null)
            {
                throw new ArgumentNullException(nameof(httpRequest));
            }

            if (httpResponse == null)
            {
                throw new ArgumentNullException(nameof(httpResponse));
            }

            if (httpRequest.HttpMethod.ToUpperInvariant() != "POST")
            {
                throw new Exception("Method not supported");
            }

            string body;
            using (var reader = new StreamReader(httpRequest.InputStream, httpRequest.ContentEncoding))
            {
                body = reader.ReadToEnd();
            }

            var query = HttpUtility.ParseQueryString(httpRequest.UrlReferrer.Query);
            var activity = new Activity
            {
                id = new Uri(query["activity_id"]),
            };

            var splitBody = HttpUtility.ParseQueryString(body);
            var webformId = string.Empty;
            var score = 0;
            var feedback = string.Empty;
            foreach (var key in splitBody.AllKeys)
            {
                var value = splitBody[key];
                switch (key)
                {
                    case "form_id":
                        var prefix = "webform_submission_";
                        var suffix = "_node";
                        var formId = value;
                        if (!formId.Contains(prefix) || !formId.Contains(suffix))
                        {
                            break;
                        }

                        var formIdSplit = formId.Split(new[] { prefix }, StringSplitOptions.None);
                        formIdSplit = formIdSplit[1].Split(new[] { suffix }, StringSplitOptions.None);
                        webformId = formIdSplit[0];
                        break;
                    case "content_relevant":
                        score = int.Parse(value);
                        break;
                    case "feedback":
                        feedback = value;
                        break;
                    default:
                        continue;
                }
            }

            var reporting = DependencyService.Get<IReportingService>();
            reporting.ReportArticleFeedbackByActivity(activity, webformId, score, feedback);
            var nodeId = parameters["nodeId"];
            var response = JsonResponse.Replace(":nodeId", nodeId);
            var outputStream = httpResponse.OutputStream;
            var genericResponse = Encoding.UTF8.GetBytes(response);
            httpResponse.ContentLength64 = genericResponse.Length;
            httpResponse.ContentType = "application/json";
            httpResponse.AddHeader("X-Drupal-Ajax-Token", "1");
            outputStream.Write(genericResponse, 0, genericResponse.Length);
        }
    }
}
