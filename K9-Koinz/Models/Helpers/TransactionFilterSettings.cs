namespace K9_Koinz.Models.Helpers {
    public struct TransactionFilterSettings {
        public Guid? MerchantFilter { get; set; }
        public Guid? AccountFilter { get; set; }
        public Guid? TagFilter
    }
}
