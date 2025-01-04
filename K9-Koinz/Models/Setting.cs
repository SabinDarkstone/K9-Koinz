using K9_Koinz.Models.Meta;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace K9_Koinz.Models {

    public enum SettingType {
        [Display(Name = "String")]
        STRING = 0,
        [Display(Name = "Integer")]
        INTEGER = 1,
        [Display(Name = "Double")]
        DOUBLE = 2,
        [Display(Name = "Checkbox")]
        BOOLEAN = 3
    }

    public class Setting : BaseEntity {
        public string Name { get; set; }
        public string Description { get; set; }
        public SettingType Type { get; set; }
        [DisplayName("Default Value")]
        public string DefaultValue { get; set; }
        public string Value { get; set; }

        public object GetValue() {
            if (Type == SettingType.STRING) {
                return Value;
            } else if (Type == SettingType.INTEGER) {
                return int.Parse(Value);
            } else if (Type == SettingType.DOUBLE) {
                return double.Parse(Value);
            } else if (Type == SettingType.BOOLEAN) {
                return bool.Parse(Value.ToLower());
            } else {
                return null;
            }
        }
    }
}
