using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq.Expressions;
using System.Reflection;

namespace K9_Koinz.Utils {
    public static class HtmlHelpers {
        private static string ConvertToHtmlString(this IHtmlContent content) {
            using (var writer = new StringWriter()) {
                content.WriteTo(writer, System.Text.Encodings.Web.HtmlEncoder.Default);
                return writer.ToString();
            }
        }

        private static void GenerateSelectOptions(this TagBuilder selectBuilder, IEnumerable<SelectListItem> selectList) {
            foreach (var item in selectList) {
                var optionBuilder = new TagBuilder("option");

                if (item.Value != null) {
                    optionBuilder.Attributes.Add("value", item.Value.ToString());
                }

                optionBuilder.InnerHtml.Append(item.Text);
                selectBuilder.InnerHtml.AppendHtml(optionBuilder.ConvertToHtmlString());
            }
        }

        private static Type GetPropertyType<TModel, TValue>(Expression<Func<TModel, TValue>> expression) {
            if (expression.Body is MemberExpression memberExpression && memberExpression.Member is PropertyInfo propertyInfo) {
                return propertyInfo.PropertyType;
            }

            throw new ArgumentException("Expression is not a property");
        }

        public static IHtmlContent FloatingFormInput<TModel, TValue>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TValue>> expression, string type = "text", string divId = "", string autocompleteKey = "", string value = "") {            
            var divBuilder = new TagBuilder("div");
            divBuilder.AddCssClass("form-group form-floating");
            if (!string.IsNullOrEmpty(divId)) {
                divBuilder.Attributes.Add("id", divId);
            }

            if (string.IsNullOrEmpty(autocompleteKey)) {
                IHtmlContent inputHelper;
                if (string.IsNullOrEmpty(value)) {
                    inputHelper = htmlHelper.TextBoxFor(expression, new { @class = "form-control", type, placeholder = htmlHelper.DisplayNameFor(expression).ToString() });
                } else {
                    inputHelper = htmlHelper.TextBoxFor(expression, new { @class = "form-control", type, placeholder = htmlHelper.DisplayNameFor(expression).ToString(), value = value });
                }
                divBuilder.InnerHtml.AppendHtml(inputHelper.ConvertToHtmlString());
            } else {
                var inputHelper = new TagBuilder("input");
                inputHelper.Attributes.Add("type", "text");
                inputHelper.Attributes.Add("id", "txt" + autocompleteKey);
                inputHelper.Attributes.Add("placeholder", htmlHelper.DisplayNameFor(expression).ToString());
                inputHelper.Attributes.Add("value", value);
                inputHelper.AddCssClass("form-control");
                divBuilder.InnerHtml.AppendHtml(inputHelper.ConvertToHtmlString());
            }

            var labelBuilder = new TagBuilder("label");
            labelBuilder.MergeAttribute("asp-for", htmlHelper.NameFor(expression).ToString());
            labelBuilder.InnerHtml.Append(htmlHelper.DisplayNameFor(expression).ToString());

            var validationSpanBuilder = new TagBuilder("span");
            validationSpanBuilder.MergeAttribute("asp-validation-for", htmlHelper.NameFor(expression).ToString());
            validationSpanBuilder.AddCssClass("text-danger");

            IHtmlContent hiddenInputHelper = null;
            if (!string.IsNullOrEmpty(autocompleteKey)) {
                hiddenInputHelper = htmlHelper.TextBoxFor(expression, new { type = "hidden", id = "hf" + autocompleteKey });
            }

            divBuilder.InnerHtml.AppendHtml(labelBuilder.ConvertToHtmlString());
            divBuilder.InnerHtml.AppendHtml(validationSpanBuilder.ConvertToHtmlString());
            
            if (hiddenInputHelper != null) {
                divBuilder.InnerHtml.AppendHtml(hiddenInputHelper.ConvertToHtmlString());
            }

            return divBuilder;
        }

        public static IHtmlContent FloatingFormSelect<TModel, TValue>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TValue>> expression, string placeholder) {
            var divBuilder = new TagBuilder("div");
            divBuilder.AddCssClass("form-group form-floating");

            var selectBuilder = new TagBuilder("select");
            selectBuilder.MergeAttribute("class", "form-control");
            selectBuilder.MergeAttribute("asp-for", htmlHelper.NameFor(expression).ToString());

            var enumType = GetPropertyType(expression);
            var selectList = new SelectList(Enum.GetNames(enumType));

            var defaultOptionBuilder = new TagBuilder("option");
            defaultOptionBuilder.Attributes.Add("value", "");
            defaultOptionBuilder.Attributes.Add("selected", "selected");
            defaultOptionBuilder.Attributes.Add("disabled", "disabled");
            defaultOptionBuilder.InnerHtml.Append(placeholder);

            selectBuilder.InnerHtml.Append(defaultOptionBuilder.ConvertToHtmlString());
            selectBuilder.GenerateSelectOptions(selectList);

            var labelBuilder = new TagBuilder("label");
            labelBuilder.MergeAttribute("asp-for", htmlHelper.NameFor(expression).ToString());
            labelBuilder.InnerHtml.Append(htmlHelper.DisplayNameFor(expression).ToString());

            var validationSpanBuilder = new TagBuilder("span");
            validationSpanBuilder.MergeAttribute("asp-validation-for", htmlHelper.NameFor(expression).ToString());
            validationSpanBuilder.AddCssClass("text-danger");

            divBuilder.InnerHtml.AppendHtml(selectBuilder.ConvertToHtmlString());
            divBuilder.InnerHtml.AppendHtml(labelBuilder.ConvertToHtmlString());
            divBuilder.InnerHtml.AppendHtml(validationSpanBuilder.ConvertToHtmlString());

            return divBuilder;
        }

        public static IHtmlContent SubmitButton(this IHtmlHelper htmlHelper, string text = "Create") {
            var divBuilder = new TagBuilder("div");
            divBuilder.AddCssClass("form-group");

            var buttonBuilder = new TagBuilder("button");
            buttonBuilder.Attributes.Add("type", "submit");
            buttonBuilder.AddCssClass("btn btn-primary mb-2");

            var iconBuilder = new TagBuilder("i");
            iconBuilder.AddCssClass("fa-solid fa-floppy-disk");

            buttonBuilder.InnerHtml.AppendHtml(iconBuilder.ConvertToHtmlString() + "&nbsp" + text);
            divBuilder.InnerHtml.AppendHtml(buttonBuilder.ConvertToHtmlString());

            return divBuilder;
        }
    }
}
