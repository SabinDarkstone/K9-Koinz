using K9_Koinz.Data;
using K9_Koinz.Factories;
using K9_Koinz.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace K9_Tests {
    public class DataFactory {
        private readonly KoinzContext _context;
        private readonly IRepoFactory _repoFactory;

        public DataFactory(KoinzContext context, IRepoFactory repoFactory) {
            _context = context;
            _repoFactory = repoFactory;
        }

        public void ResetDatabase() {
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
        }

        public async Task<Account> MakeAccount(string name, AccountType type, DateTime? startingDate, double startingBalance = 0) {
            var repo = _repoFactory.CreateRepository<Account>();
            var account = new Account {
                Name = name,
                Type = type,
                InitialBalance = startingBalance,
                InitialBalanceDate = startingDate ?? DateTime.Today.AddDays(-20)
            };

            await repo.AddAsync(account);
            return account;
        }

        public async Task<Transaction> MakeTransaction(Guid accountId, Guid merchantId, Guid categoryId, double amount, DateTime date, string notes = "", Guid? savingsGoalId = null, Guid? tagId = null) {
            var repo = _repoFactory.CreateSpecializedRepository<TransactionRepository>();
            var transaction = new Transaction {
                AccountId = accountId,
                MerchantId = merchantId,
                CategoryId = categoryId,
                Amount = amount,
                Date = date,
                Notes = notes,
                SavingsGoalId = savingsGoalId,
                IsSavingsSpending = savingsGoalId.HasValue,
                TagId = tagId
            };

            await repo.AddAsync(transaction);
            return transaction;
        }
    }
}
