namespace K9_Koinz.Models.Helpers {
    public struct TransactionFilterSetting {
        public string SortOrder { get; }
        public List<Guid> CategoryFilters { get; }
        public Guid? MerchantFilter { get; }
        public Guid? AccountFilter { get; }
        public Guid? TagFilter { get; }
        public DateTime? DateRangeStart { get; }
        public DateTime? DateRangeEnd { get; }
        public int? PageIndex { get; }
        public string SearchString { get; }
        public bool? HideTransfers { get; }

        public List<Guid> CategoryIds { get; set; }

        public TransactionFilterSetting(string sortOrder, List<Guid> catFilters, Guid? merchFilter, Guid? accountFilter, Guid? tagId, DateTime? minDate, DateTime? maxDate, int? pageIndex, string searchString, bool? hideTransfers) {
            SortOrder = sortOrder;
            CategoryFilters = catFilters;
            MerchantFilter = merchFilter;
            AccountFilter = accountFilter;
            TagFilter = tagId;
            DateRangeStart = minDate;
            DateRangeEnd = maxDate;
            PageIndex = pageIndex;
            SearchString = searchString;
            HideTransfers = hideTransfers;
        }
    }
}
