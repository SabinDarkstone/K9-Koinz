﻿using Humanizer;
using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;
using K9_Koinz.Services;

namespace K9_Koinz.Pages.SavingsGoals {
    public class CreateModel : AbstractCreateModel<SavingsGoal> {
        public CreateModel(KoinzContext context, ILogger<AbstractDbPage> logger,
            IAccountService accountService, ITagService tagService)
                : base(context, logger, accountService, tagService) { }

        protected override void BeforeSaveActions() {
            var account = _context.Accounts.Find(Record.AccountId);
            Record.AccountName = account.Name;

            Record.SavedAmount = 0d;
        }
    }
}
