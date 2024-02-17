using K9_Koinz.Models.Meta;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace K9_Koinz.Models {
    public class SavingsGoal : BaseEntity, INameable {
        public string Name { get; set; }
        public string Description { get; set; }
        public double? StartingAmount { get; set; }
        public double TargetAmount { get; set; }
        public double SavedAmount { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Date only")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? TargetDate { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Date only")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }
        [DisplayName("Account")]
        public Guid AccountId { get; set; }
        public Account Account { get; set; }
        public string AccountName {  get; set; }

        public ICollection<Transaction> Transactions { get; set; }

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
