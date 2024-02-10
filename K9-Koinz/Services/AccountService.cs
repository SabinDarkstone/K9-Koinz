using K9_Koinz.Data;
using K9_Koinz.Services.Meta;
using K9_Koinz.Utils;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace K9_Koinz.Services {
    public interface IAccountService : ICustomService {
        public abstract List<SelectListItem> GetAccountList(bool doGrouping);
    }

    public class AccountService : AbstractService<AccountService>, IAccountService {
        public AccountService(KoinzContext context, ILogger<AccountService> logger) : base(context, logger) { }

        public List<SelectListItem> GetAccountList(bool doGrouping = false) {
            var result = new List<SelectListItem>();
            if (doGrouping) {
                var accountList = _context.Accounts.GroupBy(acct => acct.Type).AsEnumerable().OrderBy(grp => grp.Key.ToString()).ToList();
                List<SelectListGroup> groups = new List<SelectListGroup>();
                foreach (var grouping in accountList) {
                    var currentGroup = new SelectListGroup { Name = grouping.Key.GetAttribute<DisplayAttribute>().Name };
                    foreach (var account in grouping.OrderBy(acct => acct.Name)) {
                        result.Add(new SelectListItem { Value = account.Id.ToString(), Text = account.Name, Group = currentGroup });
                    }
                }
            } else {
                result = _context.Accounts.OrderBy(acct => acct.Name).Select(acct => new SelectListItem { Value = acct.Id.ToString(), Text = acct.Name }).ToList();
            }

            return result;
        }
    }
}
