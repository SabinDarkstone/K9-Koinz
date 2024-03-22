namespace K9_Koinz.Models.Helpers {
    public struct TransactionFilterSetting {
        public string SortOrder { get; set; }
        public List<Guid> CategoryFilters { get; set; }
        public Guid? MerchantFilter { get; set; }
        public Guid? AccountFilter { get; set; }
        public Guid? TagFilter { get; set;  }
        public DateTime? DateRangeStart { get; set; }
        public DateTime? DateRangeEnd { get; set; }
        public int? PageIndex { get; set; }
        public string SearchString { get; set; }
        public bool? HideTransfers { get; set; }

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
