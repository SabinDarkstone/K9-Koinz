using K9_Koinz.Data;
using K9_Koinz.Models.Meta;
using System.ComponentModel.DataAnnotations;

namespace K9_Koinz.Utils {
    public class UniqueAttribute<T> : ValidationAttribute where T : BaseEntity, INameable {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext) {
            //if (value == null) {
            //    return ValidationResult.Success;
            //}
            //var context = (KoinzContext)validationContext.GetService(typeof(KoinzContext));
            //if (!context.Set<T>().Any(a => a.Name == value.ToString())) {
            //    return ValidationResult.Success;
            //}
            //return new ValidationResult("You must use a unique name.");
            return ValidationResult.Success;
        }
    }
}
