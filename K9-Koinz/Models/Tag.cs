namespace K9_Koinz.Models {
    public class Tag : BaseEntity, INameable {
        public string Name { get; set; }
    }

    public class TransactionTag : BaseEntity {

        public Guid TransactionId { get; set; }
        public Transaction Transaction { get; set; }

        public Guid TagId { get; set; }
        public Tag Tag { get; set; }
    }
}
