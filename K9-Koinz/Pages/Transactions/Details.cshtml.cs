﻿using Microsoft.EntityFrameworkCore;
using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;

namespace K9_Koinz.Pages.Transactions {
    public class DetailsModel : AbstractDetailsModel<Transaction> {
        public DetailsModel(KoinzContext context, ILogger<AbstractDbPage> logger)
            : base(context, logger) { }

        protected override async Task<Transaction> QueryRecordAsync(Guid id) {
            return await _context.Transactions
                .Include(trans => trans.Tag)
                .Include(trans => trans.Bill)
                .Include(trans => trans.SplitTransactions)
                    .ThenInclude(splt => splt.Tag)
                .Include(trans => trans.Category)
                .Include(trans => trans.Transfer)
                    .ThenInclude(fer => fer.RecurringTransfer)
                .Include(trans => trans.SavingsGoal) 
                .SingleOrDefaultAsync(trans => trans.Id == id);
        }
    }
}
