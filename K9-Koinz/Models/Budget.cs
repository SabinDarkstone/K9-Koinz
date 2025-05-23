﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using K9_Koinz.Models.Meta;
using K9_Koinz.Utils;
using K9_Koinz.Utils.Attributes;

namespace K9_Koinz.Models {
    public enum BudgetTimeSpan {
        [Display(Name = "Weekly")]
        WEEKLY,
        [Display(Name = "Monthly")]
        MONTHLY,
        [Display(Name = "Yearly")]
        YEARLY
    }

    public class Budget : BaseEntity, INameable {
        [RecycleBinProp("Name")]
        public string Name { get; set; } = "New Budget";
        public string Description { get; set; }
        public int SortOrder { get; set; }
        [RecycleBinProp("Time Period")]
        public BudgetTimeSpan Timespan { get; set; }
        [DisplayName("Tag")]
        public Guid? BudgetTagId { get; set; }
        public Tag BudgetTag { get; set; }
        public string BudgetTagName { get; set; }
        [DisplayName("Ignore Categories")]
        public bool DoNotUseCategories { get; set; }
        [NotMapped]
        [DisplayName("Budgeted Amount")]
        public double? BudgetedAmount { get; set; }
        [NotMapped]
        [DisplayName("Rollover Unspent Money")]
        public bool DoNoCategoryRollover { get; set; }

        public ICollection<BudgetLine> BudgetLines { get; set; } = new List<BudgetLine>();

        [NotMapped]
        public ICollection<BudgetLine> UnallocatedLines { get; set; } = new List<BudgetLine>();

        [NotMapped]
        public ICollection<BudgetLine> IncomeLines {
            get {
                return BudgetLines
                    .Where(line => line.BudgetCategory.CategoryType == CategoryType.INCOME)
                    .OrderBy(line => line.BudgetCategory.ParentCategory?.Name ?? "")
                        .ThenBy(line => line.BudgetCategory.Name)
                    .ToList();
            }
        }

        [NotMapped]
        public ICollection<BudgetLine> ExpenseLines {
            get {
                return BudgetLines
                    .Where(line => line.BudgetCategory.CategoryType == CategoryType.EXPENSE)
                    .OrderBy(line => line.BudgetCategory.ParentCategory?.Name ?? "")
                        .ThenBy(line => line.BudgetCategory.Name)
                    .ToList();
            }
        }

        [NotMapped]
        public ICollection<BudgetLine> UnallocatedIncomes {
            get {
                return UnallocatedLines
                    .Where(line => line.BudgetCategory.CategoryType == CategoryType.INCOME)
                    .OrderBy(line => line.BudgetCategory.ParentCategory?.Name ?? "")
                        .ThenBy(line => line.BudgetCategory.Name)
                    .ToList();
            }
        }

        [NotMapped]
        public ICollection<BudgetLine> UnallocatedExpenses {
            get {
                return UnallocatedLines
                    .Where(line => line.BudgetCategory.CategoryType == CategoryType.EXPENSE)
                    .OrderBy(line => line.BudgetCategory.ParentCategory?.Name ?? line.BudgetCategory.Name)
                        .ThenBy(line => line.BudgetCategory.Name)
                    .ToList();
            }
        }

        [NotMapped]
        public ICollection<BudgetLine> RolloverExpenses {
            get {
                return BudgetLines.Where(line => line.DoRollover).ToList();
            }
        }

        [NotMapped]
        public string TimespanString {
            get {
                return Timespan.GetAttribute<DisplayAttribute>().Name;
            }
        }
    }
}
