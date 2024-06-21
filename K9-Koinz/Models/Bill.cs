using System.ComponentModel;
using K9_Koinz.Models.Meta;

namespace K9_Koinz.Models {

    public class Bill : BaseEntity, INameable, IAmount {

        [DisplayName("Name")]
        public string Name { get; set; }

        [DisplayName("Account")]
        public Guid AccountId { get; set; }
        public Account Account { get; set; }

        [DisplayName("Merchant")]
        public Guid MerchantId { get; set; }
        public Merchant Merchant { get; set; }

        [DisplayName("Category")]
        public Guid? CategoryId { get; set; }
        public Category Category { get; set; }

        [DisplayName("First Due Date")]
        public DateTime StartDate { get; set; }

        [DisplayName("Next Due Date")]
        public DateTime NextDueDate { get; set; }

        [DisplayName("Final Due Date")]
        public DateTime FinalDueDate { get; set; }

        [DisplayName("PreviousDueDate")]
        public DateTime PreviousDueDate { get; set; }

        //[DisplayName("Repeat Settings")]
        //public Guid? RepeatConfigId { get; set; }
        //public RepeatConfig RepeatConfig { get; set; }

        [DisplayName("Amount")]
        public double Amount { get; set; }

        [DisplayName("Repeating Bill?")]
        public bool IsRepeatBill { get; set; }

        [DisplayName("Mark as Autopay")]
        public bool IsAutopay { get; set; }

        [DisplayName("Currently Active?")]
        public bool IsActive { get; set; }

        public ICollection<Transaction> Transactions { get; set; }
    }
}
