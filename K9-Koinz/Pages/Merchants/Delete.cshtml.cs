using Microsoft.EntityFrameworkCore;
using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using K9_Koinz.Utils;

namespace K9_Koinz.Pages.Merchants {
    public class DeleteModel : AbstractDeleteModel<Merchant> {
        [Display(Name = "Replace Merchant With")]
        [BindProperty]
        public Guid? SelectedMerchantId { get; set; }

        public DeleteModel(KoinzContext context, ILogger<AbstractDbPage> logger)
            : base(context, logger) { }

        protected override async Task<Merchant> QueryRecordAsync(Guid id) {
            return await _context.Merchants
                .Include(merch => merch.Transactions)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        protected override void BeforeDeleteActions() {
            if (SelectedMerchantId.HasValue) {
                var targetMerchant = _context.Merchants.Find(SelectedMerchantId.Value);

                var transactionsToUpdate = _context.Transactions
                    .Where(trans => trans.MerchantId == Record.Id)
                    .ToList();

                var billsToUpdate = _context.Bills
                    .Where(bill => bill.MerchantId == Record.Id)
                    .ToList();

                var transfersToUpdate = _context.Transfers
                    .Where(fer => fer.MerchantId == Record.Id)
                    .ToList();

                foreach (var trans in transactionsToUpdate) {
                    trans.MerchantId = targetMerchant.Id;
                    trans.MerchantName = targetMerchant.Name;
                }

                foreach (var bill in billsToUpdate) {
                    bill.MerchantId = targetMerchant.Id;
                    bill.MerchantName = targetMerchant.Name;
                }

                foreach (var fer in transfersToUpdate) {
                    fer.MerchantId = targetMerchant.Id;
                }

                var dbTransaction = _context.Database.BeginTransaction();
                try {
                    _context.UpdateRange(transactionsToUpdate);
                    _context.UpdateRange(billsToUpdate);
                    _context.UpdateRange(transfersToUpdate);

                    _context.SaveChanges();

                    dbTransaction.Commit();

                    var transactionIds = transactionsToUpdate.Select(trans => trans.Id).ToList();
                    var billIds = billsToUpdate.Select(bill => bill.Id).ToList();
                    var transferIds = transfersToUpdate.Select(fer => fer.Id).ToList();

                    var updatedTransactions = _context.Transactions
                        .AsNoTracking()
                        .Where(trans => transactionIds.Contains(trans.Id))
                        .ToList();
                    foreach (var trans in updatedTransactions) {
                        if (trans.MerchantId != targetMerchant.Id && trans.MerchantName != targetMerchant.Name) {
                            throw new Exception("Replacement failed");
                        }
                    }

                    var updatedBills = _context.Bills
                        .AsNoTracking()
                        .Where(bill => billIds.Contains(bill.Id))
                        .ToList();
                    foreach (var bill in updatedBills) {
                        if (bill.MerchantId != targetMerchant.Id && bill.MerchantName != targetMerchant.Name) {
                            throw new Exception("Replacement failed");
                        }
                    }

                    var updatedTransfers = _context.Transfers
                        .AsNoTracking()
                        .Where(fer => transferIds.Contains(fer.Id))
                        .ToList();
                    foreach (var fer in updatedTransfers) {
                        if (fer.MerchantId != targetMerchant.Id) {
                            throw new Exception("Replacement failed");
                        }
                    }
                } catch (Exception) {
                    dbTransaction.Rollback();
                    RedirectToPage(PagePaths.MerchantIndex);
                }
            }
        }
    }
}
