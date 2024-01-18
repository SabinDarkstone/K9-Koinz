﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Utils;

namespace K9_Koinz.Pages.BudgetLines {
    public class EditModel : PageModel {
        private readonly KoinzContext _context;
        private readonly ILogger<EditModel> _logger;

        public EditModel(KoinzContext context, ILogger<EditModel> logger) {
            _context = context;
            _logger = logger;
        }

        [BindProperty]
        public BudgetLine BudgetLine { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(Guid? id) {
            if (id == null) {
                return NotFound();
            }

            var budgetLine = await _context.BudgetLines
                .Include(line => line.BudgetCategory)
                .Include(line => line.Budget)
                .FirstAsync(line => line.Id == id);
            if (budgetLine == null) {
                return NotFound();
            }

            BudgetLine = budgetLine;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync() {
            if (!ModelState.IsValid) {
                return Page();
            }

            var oldBudgetLine = _context.BudgetLines.Find(BudgetLine.Id);

            if (BudgetLine.DoRollover && !oldBudgetLine.DoRollover) {
                //var historialPeriods = CheckAndHandleHistoricalRollover();
                //_context.BudgetLinePeriods.AddRange(historialPeriods);
            }

            _context.Attach(BudgetLine).State = EntityState.Modified;

            try {
                await _context.SaveChangesAsync();
            } catch (DbUpdateConcurrencyException) {
                if (!BudgetLineExists(BudgetLine.Id)) {
                    return NotFound();
                } else {
                    throw;
                }
            }

            return RedirectToPage("/Budgets/Edit", new { id = BudgetLine.BudgetId });
        }

        public IActionResult OnGetCategoryAutoComplete(string text) {
            text = text.Trim();
            var categories = _context.Categories
                .Include(cat => cat.ParentCategory)
                .AsNoTracking()
                .AsEnumerable()
                .Where(cat => cat.FullyQualifiedName.Contains(text, StringComparison.CurrentCultureIgnoreCase))
                .Select(cat => new {
                    label = cat.ParentCategoryId != null ? cat.ParentCategory.Name + ": " + cat.Name : cat.Name,
                    val = cat.Id
                }).ToList();
            return new JsonResult(categories);
        }

        private bool BudgetLineExists(Guid id) {
            return _context.BudgetLines.Any(e => e.Id == id);
        }

        private List<BudgetLinePeriod> CheckAndHandleHistoricalRollover() {
            var timespan = BudgetLine.Budget.Timespan;

            // Get first transaction for budget category
            Transaction firstTransaction = _context.Transactions
                .Where(trans => trans.CategoryId == BudgetLine.BudgetCategoryId)
                .OrderBy(trans => trans.Date)
                .FirstOrDefault();

            if (firstTransaction == null) {
                return new List<BudgetLinePeriod>();
            }

            // Find the budget period for this transaction
            var foundPeriod = false;
            var currentDateToCheck = DateTime.Now;
            var periodsChecked = 0;
            while (!foundPeriod) {
                var (periodStartDate, periodEndDate) = timespan.GetStartAndEndDate(currentDateToCheck);
                if (firstTransaction.Date >= periodStartDate && firstTransaction.Date <= periodEndDate) {
                    foundPeriod = true;
                } else {
                    periodsChecked++;
                    currentDateToCheck = currentDateToCheck.GetPreviousPeriod(timespan);
                }
            }

            List<BudgetLinePeriod> periodsCreated = new();
            BudgetLinePeriod previousPeriod = null;
            for (var i = 0; i <= periodsChecked; i++) {
                var (startDate, endDate) = BudgetLine.Budget.Timespan.GetStartAndEndDate(currentDateToCheck);
                BudgetLinePeriod period = CreateHistoricalBudgetLinePeriod(startDate, endDate, previousPeriod);
                previousPeriod = period;
                periodsCreated.Add(period);
                currentDateToCheck = currentDateToCheck.GetNextPeriod(timespan);
            }

            return periodsCreated;
        }

        private BudgetLinePeriod CreateHistoricalBudgetLinePeriod(DateTime startDate, DateTime endDate, BudgetLinePeriod previous) {
            var moneySpentInPeriod = _context.Transactions
                .Where(trans => trans.Date >= startDate && trans.Date <= endDate)
                .Where(trans => trans.CategoryId == BudgetLine.BudgetCategoryId)
                .Sum(trans => trans.Amount) * -1;

            var budgetLinePeriod = new BudgetLinePeriod {
                BudgetLineId = BudgetLine.Id,
                StartDate = startDate,
                EndDate = endDate,
                SpentAmount = moneySpentInPeriod,
                StartingAmount = previous == null ? 0d : (BudgetLine.BudgetedAmount - previous.SpentAmount) * -1
            };

            return budgetLinePeriod;
        }
    }
}
