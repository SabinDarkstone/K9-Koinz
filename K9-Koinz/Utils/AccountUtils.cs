using K9_Koinz.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace K9_Koinz.Utils {
    public static class AccountUtils {
        public static List<SelectListItem> GetAccountList(KoinzContext context, bool doGrouping = false) {
            var result = new List<SelectListItem>();
            if (doGrouping) {
                var accountList = context.Accounts.GroupBy(acct => acct.Type).AsEnumerable().OrderBy(grp => grp.Key.ToString()).ToList();
                List<SelectListGroup> groups = new List<SelectListGroup>();
                foreach (var grouping in accountList) {
                    var currentGroup = new SelectListGroup { Name = grouping.Key.GetAttribute<DisplayAttribute>().Name };
                    foreach (var account in grouping.OrderBy(acct => acct.Name)) {
                        result.Add(new SelectListItem { Value = account.Id.ToString(), Text = account.Name, Group = currentGroup });
                    }
                }
            } else {
                result = context.Accounts.OrderBy(acct => acct.Name).Select(acct => new SelectListItem { Value = acct.Id.ToString(), Text = acct.Name }).ToList();
            }

            return result;
        }
    }
}
