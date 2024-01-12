using K9_Koinz.Utils;

namespace K9_Koinz.Models {
    public class Merchant : INameable {
        public Guid Id { get; set; }
        [Unique<Merchant>]
        public string Name { get; set; }

        public ICollection<Transaction> Transactions { get; set; }

		public override int GetHashCode() {
			return Id.GetHashCode();
		}
	}
}
