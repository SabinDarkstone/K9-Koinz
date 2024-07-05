using System.ComponentModel.DataAnnotations;

namespace K9_Koinz.Models.Enums {
    public enum SavingsType {
        [Display(Name = "Goal")]
        GOAL = 1,
        [Display(Name = "Bucket")]
        BUCKET = 2
    }

}