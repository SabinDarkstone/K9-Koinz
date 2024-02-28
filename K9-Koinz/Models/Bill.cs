using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using K9_Koinz.Models.Meta;
using K9_Koinz.Utils;

namespace K9_Koinz.Models {

    public enum Frequency {
        [Display(Name = "Monthly")]
        MONTHLY,
        [Display(Name = "Annually")]
        YEARLY,
        [Display(Name = "Weekly")]
        WEEKLY
    }

    public class Bill : BaseEntity, INameable {
        public string Name { get; set; }
        [DisplayName("Account")]
        public Guid AccountId { get; set; }
        public Account Account { get; set; }
        public string AccountName { get; set; }
        [DisplayName("Merchant")]
        public Guid MerchantId { get; set; }
        public Merchant Merchant { get; set; }
        public string MerchantName { get; set; }
        [DisplayName("Category")]
        public Guid? CategoryId { get; set; }
        public Category Category { get; set; }
        public string CategoryName { get; set; }
        [DisplayName("First Due Date")]
        [DataType(DataType.Date, ErrorMessage = "Date only")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FirstDueDate { get; set; }
        [DisplayName("Repeat Frequency")]
        public Frequency RepeatFrequency { get; set; }
        [DisplayName("Repeat Gap")]
        public int RepeatFrequencyCount { get; set; }
        [DisplayName("Bill End Date")]
        [DataType(DataType.Date, ErrorMessage = "Date only")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? EndDate { get; set; }
        [DisplayName("Amount")]
        public double BillAmount { get; set; }

        [DisplayName("Last Due Date")]
        [DataType(DataType.Date, ErrorMessage = "Date only")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? LastDueDate { get; set; }

        public ICollection<Transaction> Transactions { get; set; }

        [NotMapped]
        public string RepeatString {
            get {
                string value = "Every";
                if (RepeatFrequencyCount > 1) {
                    value += " " + RepeatFrequencyCount.ToString();
                }

                if (RepeatFrequency == Frequency.MONTHLY) {
                    value += " Month" + (RepeatFrequencyCount > 1 ? "s" : string.Empty);
                } else if (RepeatFrequency == Frequency.YEARLY) {
                    value += " Year" + (RepeatFrequencyCount > 1 ? "s" : string.Empty);
                } else if (RepeatFrequency == Frequency.WEEKLY) {
                    value += " Week" + (RepeatFrequencyCount > 1 ? "s" : string.Empty);
                }

                return value;
            }
        }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date, ErrorMessage = "Date only")]
        public DateTime? NextDueDate {
            get {
                DateTime? nextDate;
                if (LastDueDate.HasValue) {
                    if (RepeatFrequency == Frequency.MONTHLY) {
                        nextDate = LastDueDate.Value.AddMonths(RepeatFrequencyCount);
                    } else if (RepeatFrequency == Frequency.YEARLY) {
                        nextDate = LastDueDate.Value.AddYears(RepeatFrequencyCount);
                    } else if (RepeatFrequency == Frequency.WEEKLY) {
                        nextDate = LastDueDate.Value.AddDays(RepeatFrequencyCount * 7);
                    } else {
                        throw new Exception("Unknown repeat type for bill");
                    }

                    return nextDate;
                } else {
                    nextDate = FirstDueDate;
                }

                if (EndDate.HasValue && EndDate.Value < nextDate) {
                    nextDate = null;
                }

                return nextDate;
            }
        }

        [NotMapped]
        public bool IsExpired {
            get {
                return EndDate.HasValue && EndDate.Value < NextDueDate.Value;
            }
        }
    }
}
