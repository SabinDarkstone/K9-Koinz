namespace K9_Koinz.Models {
    public class Merchant {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public ICollection<Transaction> Transactions { get; set; }

		public override int GetHashCode() {
			return Id.GetHashCode();
		}
	}
}
