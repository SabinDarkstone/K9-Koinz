using K9_Koinz.Models.Meta;
using K9_Koinz.Utils;

namespace K9_Koinz.Models {
    public class Merchant : DateTrackedEntity, INameable {
        [Unique<Merchant>]
        public string Name { get; set; }

        public ICollection<Transaction> Transactions { get; set; }
        public ICollection<Bill> Bills { get; set; }
    }
}
