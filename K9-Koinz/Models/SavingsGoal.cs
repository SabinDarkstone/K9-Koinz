using K9_Koinz.Models.Meta;
using K9_Koinz.Utils.Attributes;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace K9_Koinz.Models {

    public enum SavingsType {
        [Display(Name = "Goal")]
        GOAL = 1,
        [Display(Name = "Bucket")]
        BUCKET = 2
    }

    public class SavingsGoal : BaseEntity, INameable {
        [RecycleBinProp("Name")]
        public string Name { get; set; }

        public string Description { get; set; }

        [DisplayName("Savings Type")]
        [RecycleBinProp("Type")]
        public SavingsType SavingsType { get; set; }

        [DisplayName("Starting Amount")]
        [DataType(DataType.Currency)]
        public double? StartingAmount { get; set; }

        [DisplayName("Target Amount")]
        [DataType(DataType.Currency)]
        [RecycleBinProp("Target")]
        public double? TargetAmount { get; set; }

        [DisplayName("Saved Amount")]
        [DataType(DataType.Currency)]
        [RecycleBinProp("Saved Amount")]
        public double SavedAmount { get; set; }

        [DisplayName("Target Date")]
        [DataType(DataType.Date, ErrorMessage = "Date only")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? TargetDate { get; set; }

        [DisplayName("Start Date")]
        [DataType(DataType.Date, ErrorMessage = "Date only")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }

        [DisplayName("Account")]
        public Guid AccountId { get; set; }

        public Account Account { get; set; }

        public string AccountName {  get; set; }

        [DisplayName("Currently Active?")]
        public bool IsActive { get; set; }

        public ICollection<Transaction> Transactions { get; set; }
        public ICollection<Bill> Bills { get; set; }

        [DisplayName("Total Saved")]
        [NotMapped]
        public double TotalContributed {
            get {
                return (StartingAmount ?? 0) + SavedAmount;
            }
        }

        [NotMapped]
        public double? TimePercent {
            get {
                if (SavingsType == SavingsType.BUCKET) {
                    return null;
                }

                if (!TargetDate.HasValue) {
                    return -1d;
                }

                return Math.Clamp((DateTime.Now.Date - StartDate) / (TargetDate.Value - StartDate), 0, 1) * 100;
            }
        }

        [NotMapped]
        public double? SavingsPercent {
            get {
                if (SavingsType == SavingsType.BUCKET) {
                    return null;
                }

                if (!TargetAmount.HasValue) {
                    return null;
                }

                return Math.Clamp(TotalContributed / TargetAmount.Value, 0, 1) * 100;
            }
        }
    }
}
