using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;

namespace K9_Koinz.Utils.HtmlHelpers {
    public static class Utils {
        public static string ConvertToHtmlString(this IHtmlContent content) {
            using (var writer = new StringWriter()) {
                content.WriteTo(writer, System.Text.Encodings.Web.HtmlEncoder.Default);
                return writer.ToString();
            }
        }

        public static TagBuilder MakeFloatingDiv(string divId = null) {
            var divBuilder = new TagBuilder("div");
            divBuilder.AddCssClass("form-group form-floating");
            if (divId != null) {
                divBuilder.Attributes.Add("id", divId);
            }
            return divBuilder;
        }

        public static List<SelectListItem> GetOptionsFromEnum(Type enumType) {
            var selectList = new List<SelectListItem>();

            foreach (var value in Enum.GetValues(enumType)) {
                var fieldInfo = value.GetType().GetField(value.ToString());
                var displayAttribute = fieldInfo.GetCustomAttribute<DisplayAttribute>();

                var text = displayAttribute != null ? displayAttribute.GetName() : value.ToString();
                var item = new SelectListItem {
                    Text = text,
                    Value = ((int)value).ToString()
                };

                selectList.Add(item);
            }

            return selectList;
        }
    }
}
