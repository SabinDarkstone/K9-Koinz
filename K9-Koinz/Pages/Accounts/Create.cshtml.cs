﻿using K9_Koinz.Data;
using K9_Koinz.Models;
using Humanizer;
using K9_Koinz.Pages.Meta;
using K9_Koinz.Services;

namespace K9_Koinz.Pages.Accounts {
    public class CreateModel : AbstractCreateModel<Account> {
        public CreateModel(KoinzContext context, ILogger<AbstractDbPage> logger,
            IAccountService accountService, IAutocompleteService autocompleteService,
            ITagService tagService)
                : base(context, logger, accountService, autocompleteService, tagService) { }

        protected override async Task AfterSaveActions() {
            return;
        }

        protected override async Task BeforeSaveActions() {
            Record.InitialBalanceDate = Record.InitialBalanceDate.AtMidnight();
        }
    }
}
