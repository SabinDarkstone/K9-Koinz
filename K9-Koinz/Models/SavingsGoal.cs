using K9_Koinz.Models.Meta;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace K9_Koinz.Models {
    public class SavingsGoal : BaseEntity, INameable {
        public string Name { get; set; }

        public string Description { get; set; }

        [DisplayName("Starting Amount")]
        [DataType(DataType.Currency)]
        public double? StartingAmount { get; set; }

        [DisplayName("Target Amount")]
        [DataType(DataType.Currency)]
        public double TargetAmount { get; set; }

        [DisplayName("Saved Amount")]
        [DataType(DataType.Currency)]
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

        public ICollection<Transaction> Transactions { get; set; }

        [DisplayName("Progress Towards Goal")]
        [NotMapped]
        public double TotalContributed {
            get {
                return (StartingAmount ?? 0) + SavedAmount;
            }
        }

        [NotMapped]
        public double TimePercent {
            get {
                if (!TargetDate.HasValue) {
                    return -1d;
                }
                return Math.Clamp((DateTime.Now - StartDate) / (TargetDate.Value - StartDate), 0, 1) * 100;
            }
        }

        [NotMapped]
        public double SavingsPercent {
            get {
                return Math.Clamp(TotalContributed / TargetAmount, 0, 1) * 100;
            }
        }
    }
}
