using Microsoft.EntityFrameworkCore;
using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace K9_Koinz.Pages.Categories {
    public class DeleteModel : AbstractDeleteModel<Category> {
        [Display(Name = "Replace Category With")]
        [BindProperty]
        public Guid? SelectedCategoryId { get; set; }

        public DeleteModel(KoinzContext context, ILogger<AbstractDbPage> logger)
            : base(context, logger) { }

        protected override async Task<Category> QueryRecordAsync(Guid id) {
            return await _context.Categories
                .Include(cat => cat.ChildCategories)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        protected override void AdditioanlDatabaseActions() {
            _context.Categories.RemoveRange(Record.ChildCategories);
        }

        protected override void BeforeDeleteActions() {
            if (SelectedCategoryId.HasValue) {
                var targetCategory = _context.Categories.Find(SelectedCategoryId.Value);

                var transactionsToUpdate = _context.Transactions
                    .Where(trans => trans.CategoryId == Record.Id)
                    .ToList();

                var billsToUpdate = _context.Bills
                    .Where(bill => bill.CategoryId == Record.Id)
                    .ToList();

                var transfersToUpdate = _context.Transfers
                    .Where(fer => fer.CategoryId == Record.Id)
                    .ToList();

                var categoriesToUpdate = _context.Categories
                    .Where(cat => cat.ParentCategoryId == Record.Id)
                    .ToList();

                foreach (var trans in transactionsToUpdate) {
                    trans.CategoryId = targetCategory.Id;
                    trans.CategoryName = targetCategory.Name;
                }

                foreach (var bill in billsToUpdate) {
                    bill.CategoryId = targetCategory.Id;
                    bill.CategoryName = targetCategory.Name;
                }

                foreach (var fer in transfersToUpdate) {
                    fer.CategoryId = targetCategory.Id;
                }

                foreach (var cat in categoriesToUpdate) {
                    cat.ParentCategoryId = targetCategory.Id;
                    cat.ParentCategoryName = targetCategory.Name;
                }

                var dbTransaction = _context.Database.BeginTransaction();
                dbTransaction.CreateSavepoint("BeforeReplacement");
                try {
                    _context.UpdateRange(transactionsToUpdate);
                    _context.UpdateRange(billsToUpdate);
                    _context.UpdateRange(transfersToUpdate);
                    _context.UpdateRange(categoriesToUpdate);

                    _context.SaveChanges();

                    dbTransaction.Commit();

                    var transactionIds = transactionsToUpdate.Select(trans => trans.Id).ToList();
                    var billIds = billsToUpdate.Select(bill => bill.Id).ToList();
                    var transferIds = transfersToUpdate.Select(fer => fer.Id).ToList();
                    var categoryIds = categoriesToUpdate.Select(cat => cat.Id).ToList();

                    var updatedTransactions = _context.Transactions
                        .Where(trans => transactionIds.Contains(trans.Id))
                        .ToList();
                    foreach (var trans in updatedTransactions) {
                        if (trans.CategoryId != targetCategory.Id && trans.CategoryName != targetCategory.Name) {
                            throw new Exception("Replacement failed");
                        }
                    }

                    var updatedBills = _context.Bills
                        .Where(bill => billIds.Contains(bill.Id))
                        .ToList();
                    foreach (var bill in updatedBills) {
                        if (bill.CategoryId != targetCategory.Id && bill.MerchantName != targetCategory.Name) {
                            throw new Exception("Replacement failed");
                        }
                    }

                    var updatedTransfers = _context.Transfers
                        .Where(fer => transferIds.Contains(fer.Id))
                        .ToList();
                    foreach (var fer in updatedTransfers) {
                        if (fer.CategoryId != targetCategory.Id) {
                            throw new Exception("Replacement failed");
                        }
                    }

                    var updatedCategories = _context.Categories
                        .Where(cat => categoryIds.Contains(cat.Id))
                        .ToList();
                    foreach (var cat in updatedCategories) {
                        if (cat.ParentCategoryId != targetCategory.Id) {
                            throw new Exception("Replacement failed");
                        }
                    }
                } catch (Exception) {
                    dbTransaction.RollbackToSavepoint("BeforeReplacement");
                    throw;
                }
            }
        }
    }
}
