using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace K9_Koinz.Utils.HtmlHelpers {
    public static class ButtonHelpers {

        public static IHtmlContent SubmitButton(this IHtmlHelper htmlHelper, string text, string color = "primary", string icon = null) {
            var divBuilder = new TagBuilder("div");
            divBuilder.AddCssClass("form-group");

            var buttonBuilder = new TagBuilder("button");
            buttonBuilder.Attributes.Add("type", "submit");
            buttonBuilder.AddCssClass("btn btn-" + color + " mb-2");

            if (icon != null) {
                var iconBuilder = new TagBuilder("i");
                iconBuilder.AddCssClass("fa-solid " + icon);

                buttonBuilder.InnerHtml.AppendHtml(iconBuilder.ConvertToHtmlString() + "&nbsp;" + text);
            } else {
                buttonBuilder.InnerHtml.Append(text);
            }

            divBuilder.InnerHtml.AppendHtml(buttonBuilder.ConvertToHtmlString());

            return divBuilder;
        }
    }
}
