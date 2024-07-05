using System.ComponentModel.DataAnnotations;

namespace K9_Koinz.Models.Enums {

    public enum RecurrenceType {
        [Display(Name = "Weekly")]
        WEEKLY,
        [Display(Name = "Monthy")]
        MONTHLY,
        [Display(Name = "Yearly")]
        YEARLY
    }
}