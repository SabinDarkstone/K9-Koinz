using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Services.Meta;
using K9_Koinz.Utils;
using Microsoft.EntityFrameworkCore;

namespace K9_Koinz.Services {
    public interface IDbCleanupService : ICustomService {
        public abstract Task DateMigrateBillSchedules();
        public abstract Task InstantiateRecurringTransfers();
        public abstract Task FixTransferDates();
    }

    class TransactionPair {
        public Transaction FromTransaction { get; set; }
        public Transaction ToTransaction { get; set; }
    }

    public class DbCleanupService : AbstractService<DbCleanupService>, IDbCleanupService {
        public DbCleanupService(KoinzContext context, ILogger<DbCleanupService> logger) : base(context, logger) { }

        public async Task DateMigrateBillSchedules() {
            _logger.LogInformation("Checking for bills to migration schedules");
            var bills = await _context.Bills
                .Where(bill => bill.RepeatConfigId == null)
                .ToListAsync();

            if (bills.Count() == 0) {
                _logger.LogInformation("No bills with out data model found");
                return;
            }

            var newRepeatConfigs = new List<RepeatConfig>();
            foreach (var bill in bills) {
                var config = new RepeatConfig {
                    FirstFiring = bill.FirstDueDate,
                    Frequency = bill.RepeatFrequency,
                    IntervalGap = bill.RepeatFrequencyCount,
                    PreviousFiring = bill.LastDueDate,
                    Mode = RepeatMode.INTERVAL,
                    TerminationDate = bill.EndDate
                };

                newRepeatConfigs.Add(config);
            }

            await _context.RepeatConfigs.AddRangeAsync(newRepeatConfigs);

            for (var i = 0; i < bills.Count; i++) {
                bills[i].RepeatConfigId = newRepeatConfigs[i].Id;
            }

            _context.UpdateRange(bills);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Bill schedules migrated successfully: " + bills.Count());
        }

        public async Task InstantiateRecurringTransfers() {
            var recurringTransfers = await _context.Transfers
                .Include(fer => fer.Transactions)
                .Include(fer => fer.RepeatConfig)
                .Include(fer => fer.InstantiatedFromRecurring)
                .Where(fer => fer.RepeatConfigId != null)
                .Where(fer => fer.Transactions.Count() > 0)
                .ToListAsync();

            // Bail out early, if possible
            if (recurringTransfers.Count == 0) {
                return;
            }

            List<Transfer> instantiatedTransfers = new();
            List<Transaction> updatedTransactions = new();

            foreach (var transfer in recurringTransfers) {
                // Pair up transactions, important if there is more than one instance of this transfer
                Dictionary<string, TransactionPair> groupedTransactions = new();
                foreach (var transaction in transfer.Transactions) {
                    // Construct a key from the amount and the date
                    var absAmount = Math.Abs(transaction.Amount);
                    var date = transaction.Date.Date.ToShortDateString();
                    var key = absAmount + ':' + date;

                    // Create key if it does not exist
                    if (!groupedTransactions.ContainsKey(key)) {
                        groupedTransactions.Add(key, new TransactionPair());
                    }

                    // Add transaction to either the "from" or "to" transaction entry
                    if (transaction.AccountId == transfer.FromAccountId) {
                        groupedTransactions[key].FromTransaction = transaction;
                    } else {
                        groupedTransactions[key].ToTransaction = transaction;
                    }
                }

                // Go through each pairing and create a new instance transfer that is related to the
                // recurring transfer.
                foreach (var (key, transPair) in groupedTransactions) {
                    var instanceTransfer = _context.GetInstanceOfRecurring(transfer);

                    // Add the transactions to the instance transfer
                    instanceTransfer.Transactions = [
                        transPair.FromTransaction,
                        transPair.ToTransaction,
                    ];

                    _context.Transfers.Add(instanceTransfer);
                }

                // Remove transactions from recurring transfer
                transfer.Transactions.Clear();
            }

            _context.SaveChanges();
        }

        public async Task FixTransferDates() {
            var transfers = await _context.Transfers
                .Include(fer => fer.Transactions)
                .Where(fer => fer.Transactions.Any())
                .ToListAsync();

            foreach (var transfer in transfers) {
                if (transfer.Date.Date != transfer.FromTransaction.Date.Date) {
                    var transDate = transfer.FromTransaction.Date;
                    transfer.Date = transDate;

                    _context.Transfers.Update(transfer);
                }
            }

            _context.SaveChanges();
        }
    }
}
