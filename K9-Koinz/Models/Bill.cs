using System.ComponentModel.DataAnnotations;

namespace K9_Koinz.Models {

    public enum Frequency {
        [Display(Name = "Monthly")]
        MONTHLY,
        [Display(Name = "Annually")]
        YEARLY
    }

    public class Bill : BaseEntity, INameable {
        public string Name { get; set; }
        public Guid AccountId { get; set; }
        public Account Account { get; set; }
        public string AccountName { get; set; }
        public Guid MerchantId {  get; set; }
        public Merchant Merchant { get; set; }
        public string MerchantName { get; set; }
        public DateTime StartDate { get; set; }
        public Frequency RepeatFrequency { get; set; }
        public int RepeatFrequencyCount { get; set; }
        public double BillAmount { get; set; }
        
        public ICollection<Transaction> Transactions { get; set; }
    }
}
