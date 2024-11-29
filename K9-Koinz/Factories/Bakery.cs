using K9_Koinz.Models.Helpers;
using NuGet.Protocol;

namespace K9_Koinz.Factories {
    public static class Bakery {
        public static object MakeRouteFromCookie(string cookieString) {
            if (cookieString != null) {
                var transactionFilterCookie = cookieString.FromJson<TransactionNavPayload>();
                return new {
                    sortOrder = transactionFilterCookie.SortOrder,
                    catFilter = transactionFilterCookie.CatFilter,
                    pageIndex = transactionFilterCookie.PageIndex,
                    accountFilter = transactionFilterCookie.AccountFilter,
                    minDate = transactionFilterCookie.MinDate,
                    maxDate = transactionFilterCookie.MaxDate,
                    merchFilter = transactionFilterCookie.MerchFilter
                };
            } else {
                return null;
            }
        }
    }
}
