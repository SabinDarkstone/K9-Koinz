using K9_Koinz.Models.Meta;
using System.ComponentModel;

namespace K9_Koinz.Models {
    public class ScheduledSplit : BaseEntity {

        [DisplayName("Parent Transfer")]
        public Guid ParentTransferId { get; set; }
        public Transfer ParentTransfer { get; set; }

        public double Amount { get; set; }

        [DisplayName("Savings Goal")]
        public Guid? SavingsGoalId { get; set; }
        public SavingsGoal SavingsGoal { get; set; }

        [DisplayName("Category")]
        public Guid? CategoryId { get; set; }
        public Category Category { get; set; }

        [DisplayName("Tag")]
        public Guid? TagId { get; set; }
        public Tag Tag { get; set; }
    }
}
