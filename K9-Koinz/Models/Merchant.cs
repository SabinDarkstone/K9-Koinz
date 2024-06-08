using K9_Koinz.Models.Meta;
using System.ComponentModel;

namespace K9_Koinz.Models {
    public class Merchant : BaseEntity, INameable {
        public string Name { get; set; }
        [DisplayName("Retire Merchant")]
        public bool IsRetired { get; set; }

        public ICollection<Transaction> Transactions { get; set; }
        public ICollection<Bill> Bills { get; set; }
    }
}
