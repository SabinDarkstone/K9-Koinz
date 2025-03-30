using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Services.Meta;
using K9_Koinz.Utils;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace K9_Koinz.Services {
    public interface IDropdownPopulatorService : ICustomService {
        public abstract Task<SelectList> GetTagListAsync();
        public abstract Task<List<SelectListItem>> GetAccountListAsync();
        public abstract Task<SelectList> GetSavingsGoalsAsync(Guid? accountId);
    }

    public class DropdownPopulatorService : AbstractService<DropdownPopulatorService>, IDropdownPopulatorService {
        
        public DropdownPopulatorService(KoinzContext context, ILogger<DropdownPopulatorService> logger)
            : base(context, logger) { }

        public async Task<List<SelectListItem>> GetAccountListAsync() {
            var result = new List<SelectListItem>();

            var accountList = (await _context.Accounts
                .Where(acct => !acct.IsRetired)
                .GroupBy(acct => acct.Type).ToListAsync())
                .OrderBy(grp => grp.Key.ToString())
                .ToList();
            List<SelectListGroup> groups = new List<SelectListGroup>();
            foreach (var grouping in accountList) {
                var currentGroup = new SelectListGroup {
                    Name = grouping.Key.GetAttribute<DisplayAttribute>().Name
                };
                foreach (var account in grouping.OrderBy(acct => acct.Name)) {
                    result.Add(new SelectListItem {
                        Value = account.Id.ToString(),
                        Text = account.Name,
                        Group = currentGroup
                    });
                }
            }

            return result;
        }

        public async Task<SelectList> GetTagListAsync() {
            return new SelectList(await _context.Tags
                .Where(tag => !tag.IsRetired)
                .OrderBy(tag => tag.Name)
                .ToListAsync(), nameof(Tag.Id), nameof(Tag.Name));
        }

        public async Task<SelectList> GetSavingsGoalsAsync(Guid? accountId) {
            var savingsIQ = _context.SavingsGoals
                .AsNoTracking()
                .Where(goal => goal.IsActive);

            if (accountId.HasValue) {
                savingsIQ = savingsIQ.Where(goal => goal.AccountId == accountId.Value);
            }

            return new SelectList(
                await savingsIQ.OrderBy(goal => goal.Name).ToListAsync(),
                nameof(SavingsGoal.Id),
                nameof(SavingsGoal.Name)
            );
        }
    }
}
