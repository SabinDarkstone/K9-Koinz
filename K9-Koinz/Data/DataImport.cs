﻿using K9_Koinz.Models;
using K9_Koinz.Pages;
using K9_Koinz.Utils;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;

namespace K9_Koinz.Data {
    public class DataImport {
        private readonly KoinzContext _context;
        private readonly ILogger<DataImportWizardModel> _logger;

        private Dictionary<string, Guid> AccountMap = new();
        private Dictionary<string, Guid> MerchantMap = new();
        private Dictionary<string, Guid> CategoryMap = new();

        private Category dummyCategory;

        public DataImport(KoinzContext context, ILogger<DataImportWizardModel> logger) {
            _context = context;
            _logger = logger;
        }

        public void ParseFileData(List<string> rowsOfCsv) {
            var accounts = new HashSet<Account>();
            var merchants = new HashSet<Merchant>();
            var transactions = new List<Transaction>();

            CreateCategoryMap();

            foreach (var line in rowsOfCsv.Skip(1)) {
                var splitRow = line.Split(',');
                var account = ParseAccount(splitRow);
                var merchant = ParseMerchant(splitRow);
                var transaction = ParseTransaction(splitRow, account, merchant);

                if (!accounts.Any(acct => acct.Id == account.Id)) {
                    accounts.Add(account);
                }

				if (!merchants.Any(merch => merch.Id == merchant.Id)) {
					merchants.Add(merchant);
				}

				transactions.Add(transaction);
            }

            _context.Accounts.AddRange(accounts);
            _context.Merchants.AddRange(merchants);
            _context.SaveChanges();

            _context.Transactions.AddRange(transactions);
            _context.SaveChanges();

            foreach (var x in _context.Accounts) {
                _logger.LogInformation(x.Id + " >>> " + x.Name);
            }

            foreach (var x in _context.Merchants) {
                _logger.LogInformation(x.Id + " >>> " + x.Name);
            }

            foreach (var x in _context.Categories) {
                _logger.LogInformation(x.Id + " >>> " + x.Name);
            }

            foreach (var x in _context.Transactions) {
                _logger.LogInformation(x.Id + " >>> " + x.Amount);
            }
        }

        private void GetCategoryMap() {
            CategoryMap.AddRange(
                _context.Categories.Select(cat => new KeyValuePair<string, Guid>(cat.Name, cat.Id))
            );
        }

        private void CreateCategoryMap() {
            var categories = CategoriesCreator.MakeTopLevelCategories();
            _context.Categories.AddRange(categories);
            _context.SaveChanges();

            CategoryMap.AddRange(
                categories.Select(cat => new KeyValuePair<string, Guid>(cat.Name, cat.Id))
            );

            categories = CategoriesCreator.MakeChildCategories(CategoryMap);
            _context.Categories.AddRange(categories);
            _context.SaveChanges();

            CategoryMap.AddRange(
                categories.Select(cat => new KeyValuePair<string, Guid>(cat.Name, cat.Id))
            );
        }

        private Account ParseAccount(string[] row) {
            var parsedAccount = new Account { Name = row[5] };
            if (AccountMap.ContainsKey(parsedAccount.Name)) {
                parsedAccount.Id = AccountMap[parsedAccount.Name];
            } else {
                parsedAccount.Id = Guid.NewGuid();
                switch (row[6]) {
                    case "Property":
                        parsedAccount.Type = AccountType.PROPERTY;
                        break;
                    case "Investments":
                        parsedAccount.Type = AccountType.INVESTMENT;
                        break;
                    case "Credit Card":
                        parsedAccount.Type = AccountType.CREDIT_CARD;
                        break;
                    case "Checking":
                        parsedAccount.Type = AccountType.CHECKING;
                        break;
                    case "Loan":
                        parsedAccount.Type = AccountType.LOAN;
                        break;
                    default:
                    case "Savings":
                        parsedAccount.Type = AccountType.SAVINGS;
                        break;
                }
                AccountMap.Add(parsedAccount.Name, parsedAccount.Id);
            }

            return parsedAccount;
        }

        private Merchant ParseMerchant(string[] row) {
            var parsedMerchant = new Merchant { Name = row[1] };
            if (MerchantMap.ContainsKey(parsedMerchant.Name)) {
                parsedMerchant.Id = MerchantMap[parsedMerchant.Name];
            } else {
                parsedMerchant.Id = Guid.NewGuid();
                MerchantMap.Add(parsedMerchant.Name, parsedMerchant.Id);
            }

            return parsedMerchant;
        }

        private Transaction ParseTransaction(string[] row, Account account, Merchant merchant) {
            var parsedTransaction = new Transaction {
                Id = Guid.NewGuid(),
                AccountId = account.Id,
                MerchantId = merchant.Id,
                Amount = decimal.Parse(row[2]),
                Date = DateTime.Parse(row[0]),
                CategoryId = CategoryMap[row[4]]
            };

            var transactionType = row[3];
            if (transactionType == "debit") {
                parsedTransaction.Amount *= -1;
            }

            return parsedTransaction;
        }
    }

    public class DataImportResults {
        public bool IsSuccess { get; set; }
        public string ErrorMessage {  get; set; }
        public List<Account> Accounts { get; set; }
        public List<Transaction> Transactions { get; set; }
        public List<Merchant> Merchants { get; set; }
    }
}
