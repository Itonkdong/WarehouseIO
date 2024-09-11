using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using WarehouseIO.ControlClasses;

namespace WarehouseIO.CustomHtmlHelpers
{
    public static class CustomHtmlHelper
    {
        public static MvcHtmlString ToolTip(this HtmlHelper html, string tooltipText, string tooltipIcon = "🛈")
        {
            // Create the <span> tag
            TagBuilder spanTag = new TagBuilder("span");
            spanTag.AddCssClass("custom-tooltip");
            spanTag.MergeAttribute("data-title", tooltipText);
            spanTag.SetInnerText(tooltipIcon); // Tooltip icon

            return MvcHtmlString.Create(spanTag.ToString());
        }

        public static MvcHtmlString ActionLinkButton(this HtmlHelper html, string text, string action, string controller, string? classList=null,object? routeValues=null)
        {

            return html.ActionLink(text, action, controller, routeValues: routeValues,
                htmlAttributes: new { @class = "btn-default page-main-button " + classList });
        }

        public static MvcHtmlString ErrorMessageSummary(this HtmlHelper html, params string[] errorMessages)
        {
            TagBuilder pTag = new TagBuilder("p");
            pTag.AddCssClass("page-error-message");
            pTag.InnerHtml = ErrorHandler.DoesErrorExist(errorMessages) switch
            {
                true => ErrorHandler.GetFirstExistingError(errorMessages),
                false => "&#8203;"
            };

            return MvcHtmlString.Create(pTag.ToString());
        }


    }
}