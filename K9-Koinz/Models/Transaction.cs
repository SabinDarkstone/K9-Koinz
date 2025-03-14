﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using K9_Koinz.Models.Meta;
using K9_Koinz.Utils;
using K9_Koinz.Utils.Attributes;

namespace K9_Koinz.Models {
    public enum TransactionType {
        [Display(Name = "Debit")]
        MINUS,
        [Display(Name = "Credit")]
        PLUS
    }
    public class Transaction : BaseEntity, IAmount {
        private string _merchantName;
        private string _categoryName;

        [Required]
        [DisplayName("Account")]
        public Guid AccountId { get; set; } = Guid.Empty;
        public Account Account { get; set; }
        [RecycleBinProp("Account")]
        public string AccountName { get; set; }

        [DataType(DataType.Date, ErrorMessage = "Date only")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        [RecycleBinProp("Date")]
        public DateTime Date { get; set; }

        [Required]
        [DisplayName("Merchant")]
        public Guid MerchantId { get; set; }
        public Merchant Merchant { get; set; }
        [RecycleBinProp("Merchant")]
        public string MerchantNameTruncated {
            get {
                return _merchantName.Truncate();
            }
        }
        public string MerchantName {
            get {
                return _merchantName;
            }
            set {
                _merchantName = value;
            }
        }

        [DisplayName("Category")]
        public Guid? CategoryId { get; set; }
        public Category Category { get; set; }
        [RecycleBinProp("Category")]
        public string CategoryName {
            get {
                return _categoryName.Truncate();
            }
            set {
                _categoryName = value;
            }
        }

        [Column(TypeName = "decimal(10, 2)")]
        [RecycleBinProp("Amount")]
        public double Amount { get; set; }

        public string Notes { get; set; }

        [DisplayName("Does this transaction occur AFTER the initial balance was set?")]
        public bool DoNotSkip { get; set; }

        [DisplayName("Tag")]
        public Guid? TagId { get; set; }
        public Tag Tag { get; set; }

        [DisplayName("Bill")]
        public Guid? BillId { get; set; }
        public Bill Bill { get; set; }

        [DisplayName("Savings Goal")]
        public Guid? SavingsGoalId { get; set; }
        public SavingsGoal SavingsGoal { get; set; }
        public string SavingsGoalName { get; set; }

        [DisplayName("Hide from Budgets and Trends")]
        public bool IsSavingsSpending { get; set; }

        [DisplayName("Include in Budget")]
        public bool CountAgainstBudget { get; set; }

        public Guid? TransferId { get; set; }
        public Transfer Transfer { get; set; }
        public bool IsSplit { get; set; }
        public Guid? ParentTransactionId { get; set; }
        public Transaction ParentTransaction { get; set; }

        public List<Transaction> SplitTransactions { get; set; }

        [NotMapped]
        public bool IsUnCategorized {
            get {
                return CategoryId == Guid.Empty;
            }
        }

        [NotMapped]
        public TransactionType TransactionType {
            get {
                if (Amount > 0) {
                    return TransactionType.PLUS;
                } else {
                    return TransactionType.MINUS;
                }
            }
        }

        [NotMapped]
        public string CategoryIcon {
            get {
                if (Category == null) {
                    return "";
                }

                if (!string.IsNullOrEmpty(Category.FontAwesomeIcon)) {
                    return Category.FontAwesomeIcon;
                }

                if (Category.ParentCategoryId.HasValue && Category.ParentCategory != null && !string.IsNullOrEmpty(Category.ParentCategory.FontAwesomeIcon)) {
                    return Category.ParentCategory.FontAwesomeIcon;
                }

                return "";
            }
        }

        [NotMapped]
        public string FormattedAmount {
            get {
                return Amount.FormatCurrency(2);
            }
        }

        [NotMapped]
        public string FormattedDate {
            get {
                return Date.ToShortDateString();
            }
        }
    }

    public struct TinyTransaction {
        public Guid Id { get; init; }
        public double Amount { get; init; }

        public TinyTransaction(Transaction transaction) {
            this.Id = transaction.Id;
            this.Amount = transaction.Amount;
        }
    }
}
