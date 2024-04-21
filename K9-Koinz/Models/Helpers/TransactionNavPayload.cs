namespace K9_Koinz.Models.Helpers {
    public class TransactionNavPayload {
        public string SortOrder { get; set; }
        public string CatFilter { get; set; }
        public string AccountFilter { get; set; }
        public string MinDate { get; set; }
        public string MaxDate { get; set; }
        public string MerchFilter { get; set; }
        public int PageIndex { get; set; }
        public string SearchString { get; set; }
        public bool HideTransfers { get; set; }

        public TransactionNavPayload() {
            SortOrder = "";
            CatFilter = "";
            AccountFilter = "";
            MinDate = "";
            MaxDate = "";
            MerchFilter = "";
            PageIndex = 1;
            SearchString = "";
            HideTransfers = false;
        }
    }
}
