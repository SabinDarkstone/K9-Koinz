using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq.Expressions;

namespace K9_Koinz.Utils.HtmlHelpers {
    public static class InputHelpers {
        
        // TODO: Fix for dates
        public static IHtmlContent FloatingFormInput<TModel, TValue>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TValue>> expression, string type = "text", string divId = "") {
            var divBuilder = Utils.MakeFloatingDiv(divId);

            var inputBuilder = new TagBuilder("input");
            inputBuilder.AddCssClass("form-control");
            inputBuilder.Attributes.Add("type", type);
            inputBuilder.Attributes.Add("asp-for", htmlHelper.NameFor(expression).ToString());

            if (type == "date") {
                inputBuilder.Attributes.Add("value", htmlHelper.Value(htmlHelper.NameFor(expression), "{0:yyyy-MM-dd}"));
            } else {
                inputBuilder.Attributes.Add("value", htmlHelper.ValueFor(expression));
            }

            var labelBuilder = new TagBuilder("label");
            labelBuilder.Attributes.Add("asp-for", htmlHelper.NameFor(expression).ToString());
            labelBuilder.InnerHtml.Append(htmlHelper.DisplayNameFor(expression).ToString());

            var validationSpanBuilder = htmlHelper.ValidationMessageFor(expression);

            divBuilder.InnerHtml.AppendHtml(inputBuilder);
            divBuilder.InnerHtml.AppendHtml(labelBuilder);
            divBuilder.InnerHtml.AppendHtml(validationSpanBuilder);

            return divBuilder;
        }

        public static IHtmlContent FloatingFormAutocompleteInput<TModel, TValue>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TValue>> expression, string autocompleteKey, string divId = "", string text = "") {
            var divBuilder = Utils.MakeFloatingDiv(divId);

            var inputBuilder = new TagBuilder("input");
            inputBuilder.Attributes.Add("type", "text");
            inputBuilder.Attributes.Add("id", "txt" + autocompleteKey);
            inputBuilder.Attributes.Add("placeholder", htmlHelper.DisplayNameFor(expression).ToString());
            inputBuilder.Attributes.Add("value", text);
            inputBuilder.AddCssClass("form-control");

            var labelBuilder = new TagBuilder("label");
            labelBuilder.Attributes.Add("asp-for", htmlHelper.NameFor(expression).ToString());
            labelBuilder.InnerHtml.Append(htmlHelper.DisplayNameFor(expression).ToString());

            var validationSpanBuilder = htmlHelper.ValidationMessageFor(expression);

            var hiddenInputBuilder = new TagBuilder("input");
            hiddenInputBuilder.Attributes.Add("type", "hidden");
            hiddenInputBuilder.Attributes.Add("asp-for", htmlHelper.NameFor(expression).ToString());
            hiddenInputBuilder.Attributes.Add("value", htmlHelper.ValueFor(expression).ToString());

            hiddenInputBuilder.Attributes.Add("id", "hf" + autocompleteKey);

            divBuilder.InnerHtml.AppendHtml(inputBuilder);
            divBuilder.InnerHtml.AppendHtml(labelBuilder);
            divBuilder.InnerHtml.AppendHtml(validationSpanBuilder);
            divBuilder.InnerHtml.AppendHtml(hiddenInputBuilder);

            return divBuilder;
        }

        public static IHtmlContent FloatingFormDataSelect<TModel, TValue>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TValue>> expression, List<SelectListItem> items, string divId = "", string placeholder = "None") {
            var divBuilder = Utils.MakeFloatingDiv(divId);

            var selectBuilder = new TagBuilder("select");
            selectBuilder.AddCssClass("form-control");
            selectBuilder.Attributes.Add("asp-for", htmlHelper.NameFor(expression).ToString());

            if (string.IsNullOrEmpty(htmlHelper.ValueFor(expression))) {
                var defaultOptionBuilder = new TagBuilder("option");
                defaultOptionBuilder.Attributes.Add("value", "");
                defaultOptionBuilder.Attributes.Add("selected", "selected");
                defaultOptionBuilder.Attributes.Add("disabled", "disabled");
                defaultOptionBuilder.InnerHtml.Append(placeholder);

                selectBuilder.InnerHtml.AppendHtml(defaultOptionBuilder);
            }

            foreach (var item in items) {
                var optionBuilder = new TagBuilder("option");
                if (string.IsNullOrEmpty(item.Value)) {
                    optionBuilder.Attributes.Add("value", item.Value.ToString());
                }

                if (item.Value == htmlHelper.ValueFor(expression).ToString()) {
                    optionBuilder.Attributes.Add("selected", "selected");
                }

                optionBuilder.InnerHtml.Append(item.Text);

                selectBuilder.InnerHtml.AppendHtml(optionBuilder);
            }

            var labelBuilder = new TagBuilder("label");
            labelBuilder.MergeAttribute("asp-for", htmlHelper.NameFor(expression).ToString());
            labelBuilder.InnerHtml.Append(htmlHelper.DisplayNameFor(expression).ToString());

            var validationSpanBuilder = htmlHelper.ValidationMessageFor(expression);

            divBuilder.InnerHtml.AppendHtml(selectBuilder);
            divBuilder.InnerHtml.AppendHtml(labelBuilder);
            divBuilder.InnerHtml.AppendHtml(validationSpanBuilder);

            return divBuilder;
        }

        public static IHtmlContent FloatingFormEnumSelect<TModel, TValue>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TValue>> expression, Type enumType, string divId = "", string placeholder = "None") {
            var divBuilder = Utils.MakeFloatingDiv(divId);

            var selectItems = Utils.GetOptionsFromEnum(enumType);

            var selectBuilder = new TagBuilder("select");
            selectBuilder.AddCssClass("form-control");
            selectBuilder.Attributes.Add("asp-for", htmlHelper.NameFor(expression).ToString());

            if (string.IsNullOrEmpty(htmlHelper.ValueFor(expression))) {
                var defaultOptionBuilder = new TagBuilder("option");
                defaultOptionBuilder.Attributes.Add("value", "");
                defaultOptionBuilder.Attributes.Add("selected", "selected");
                defaultOptionBuilder.Attributes.Add("disabled", "disabled");
                defaultOptionBuilder.InnerHtml.Append(placeholder);

                selectBuilder.InnerHtml.AppendHtml(defaultOptionBuilder);
            }

            foreach (var item in selectItems) {
                var optionBuilder = new TagBuilder("option");
                if (string.IsNullOrEmpty(item.Value)) {
                    optionBuilder.Attributes.Add("value", item.Value.ToString());
                }

                if (item.Value == htmlHelper.ValueFor(expression).ToString()) {
                    optionBuilder.Attributes.Add("selected", "selected");
                }

                optionBuilder.InnerHtml.Append(item.Text);

                selectBuilder.InnerHtml.AppendHtml(optionBuilder);
            }

            var labelBuilder = new TagBuilder("label");
            labelBuilder.MergeAttribute("asp-for", htmlHelper.NameFor(expression).ToString());
            labelBuilder.InnerHtml.Append(htmlHelper.DisplayNameFor(expression).ToString());

            var validationSpanBuilder = htmlHelper.ValidationMessageFor(expression);

            divBuilder.InnerHtml.AppendHtml(selectBuilder);
            divBuilder.InnerHtml.AppendHtml(labelBuilder);
            divBuilder.InnerHtml.AppendHtml(validationSpanBuilder);

            return divBuilder;
        }
    }
}
