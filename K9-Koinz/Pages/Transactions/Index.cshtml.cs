﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using K9_Koinz.Data;
using K9_Koinz.Models;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using K9_Koinz.Utils;
using K9_Koinz.Services;
using Microsoft.Extensions.Caching.Memory;
using K9_Koinz.Models.Helpers;
using NuGet.Protocol;

namespace K9_Koinz.Pages.Transactions
{
    public class IndexModel : PageModel {
        private readonly KoinzContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<IndexModel> _logger;
        private readonly IDropdownPopulatorService _dropdownService;
        private readonly IMemoryCache _cache;

        public IndexModel(KoinzContext context, IConfiguration configuration, ILogger<IndexModel> logger, IDropdownPopulatorService dropdownService, IMemoryCache cache) {
            _context = context;
            _configuration = configuration;
            _logger = logger;
            _dropdownService = dropdownService;
            _cache = cache;
        }

        private bool? _hideTransfers;

        public List<Guid> CategoryFilters { get; set; }
        public Guid MerchantFilter { get; set; }
        public Guid AccountFilter { get; set; }
        public Guid TagFilter { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? MinDateFilter { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? MaxDateFilter { get; set; }

        public string MerchantSort { get; set; }
        public string DateSort { get; set; }
        public string AmountSort { get; set; }
        public string CurrentSort { get; set; }
        public string SelectedCategory { get; set; }
        public string SelectedMerchant { get; set; }
        public string SelectedAccount { get; set; }
        public string SelectedTag { get; set; }
        public string SearchText { get; set; }

        public bool HideTransfers {
            get {
                if (_hideTransfers.HasValue) {
                    return _hideTransfers.Value;
                } else {
                    return false;
                }
            }
        }

        public string MinDateString {
            get {
                if (MinDateFilter.HasValue) {
                    return MinDateFilter.Value.FormatForUrl();
                } else {
                    return null;
                }
            }
        }
        public string MaxDateString {
            get {
                if (MaxDateFilter.HasValue) {
                    return MaxDateFilter.Value.FormatForUrl();
                } else {
                    return null;
                }
            }
        }

        public PaginatedList<Transaction> Transactions { get; set; }
        public SelectList CategoryOptions;
        public List<SelectListItem> AccountOptions;

        public async Task OnGetAsync(string sortOrder, string catFilter, string merchFilter, string accountFilter, string tagId, DateTime? minDate, DateTime? maxDate, int? pageIndex, string searchString, bool? hideTransfers) {
            CategoryOptions = new SelectList(_context.Categories.OrderBy(cat => cat.Name).ToList(), nameof(Category.Id), nameof(Category.Name));
            AccountOptions = await _dropdownService.GetAccountListAsync();

            DateSort = string.IsNullOrEmpty(sortOrder) || sortOrder == "Date" ? "date_desc" : "Date";
            MerchantSort = sortOrder == "Merchant" ? "merchant_desc" : "Merchant";
            AmountSort = sortOrder == "Amount" ? "amount_desc" : "Amount";
            CurrentSort = sortOrder;
            SelectedCategory = catFilter;
            SelectedMerchant = merchFilter;
            SelectedAccount = accountFilter;
            SearchText = searchString;
            SelectedTag = tagId;
            MinDateFilter = minDate;
            MaxDateFilter = maxDate;

            IQueryable<Transaction> transactionsIQ = from trans in _context.Transactions
                                                     select trans;

            transactionsIQ = transactionsIQ.Include(trans => trans.Category)
                .ThenInclude(cat => cat.ParentCategory);

            if (!string.IsNullOrWhiteSpace(catFilter)) {
                CategoryFilters = [Guid.Parse(SelectedCategory)];
                var childCategories = _context.Categories.Where(cat => cat.ParentCategoryId == Guid.Parse(SelectedCategory)).Include(cat => cat.ChildCategories).Select(cat => cat.Id).ToList();
                CategoryFilters.AddRange(childCategories);
                transactionsIQ = transactionsIQ.Where(trans => trans.CategoryId.HasValue && CategoryFilters.Contains(trans.CategoryId.Value));
                transactionsIQ = transactionsIQ.Where(trans => !trans.IsSplit); 
            } else {
                transactionsIQ = transactionsIQ.Where(trans => !trans.ParentTransactionId.HasValue);
            }

            if (!string.IsNullOrWhiteSpace(merchFilter)) {
                MerchantFilter = Guid.Parse(SelectedMerchant);
                transactionsIQ = transactionsIQ.Where(trans => trans.MerchantId == MerchantFilter);
            }

            if (!string.IsNullOrWhiteSpace(accountFilter)) {
                AccountFilter = Guid.Parse(SelectedAccount);
                transactionsIQ = transactionsIQ.Where(trans => trans.AccountId == AccountFilter);
            }

            if (!string.IsNullOrWhiteSpace(tagId)) {
                TagFilter = Guid.Parse(SelectedTag);
                transactionsIQ = transactionsIQ.Where(trans => trans.TagId == TagFilter);
            }

            if (MinDateFilter.HasValue) {
                transactionsIQ = transactionsIQ.Where(trans => trans.Date.Date >= MinDateFilter.Value.Date);
            }

            if (MaxDateFilter.HasValue) {
                transactionsIQ = transactionsIQ.Where(trans => trans.Date.Date <= MaxDateFilter.Value.Date);
            }

            if (!string.IsNullOrWhiteSpace(searchString)) {
                if (double.TryParse(searchString, out double value)) {
                    transactionsIQ = transactionsIQ.Where(trans => trans.Amount == value || trans.Amount == -1 * value);
                } else if (searchString.ToLower() == "hidden") {
                    transactionsIQ = transactionsIQ.Where(trans => trans.IsSavingsSpending);
                } else {
                    var lcSearchString = searchString.ToLower();
                    transactionsIQ = transactionsIQ.Where(trans => trans.Notes.ToLower().Contains(lcSearchString) ||
                        trans.AccountName.ToLower().Contains(lcSearchString) || trans.CategoryName.ToLower().Contains(lcSearchString) ||
                        trans.MerchantName.ToLower().Contains(lcSearchString) || trans.SavingsGoalName.ToLower().Contains(lcSearchString));
                }
            }

            _hideTransfers = hideTransfers;
            if (hideTransfers.HasValue && hideTransfers.Value) {
                transactionsIQ = transactionsIQ.Include(trans => trans.Category)
                    .Where(trans => trans.Category.CategoryType != CategoryType.TRANSFER);
            }

            switch (sortOrder) {

                case "Merchant":
                    transactionsIQ = transactionsIQ.OrderBy(trans => trans.MerchantName);
                    break;
                case "merchant_desc":
                    transactionsIQ = transactionsIQ.OrderByDescending(trans => trans.MerchantName);
                    break;
                case "Amount":
                    transactionsIQ = transactionsIQ.OrderBy(trans => Math.Abs(trans.Amount));
                    break;
                case "amount_desc":
                    transactionsIQ = transactionsIQ.OrderByDescending(trans => Math.Abs(trans.Amount));
                    break;
                case "Date":
                    transactionsIQ = transactionsIQ.OrderBy(trans => trans.Date);
                    break;
                case "date_desc":
                default:
                    transactionsIQ = transactionsIQ.OrderByDescending(trans => trans.Date);
                    break;
            }
            
            transactionsIQ = transactionsIQ.Include(trans => trans.Tag);

            Response.Cookies.Append("backToTransactions", new TransactionNavPayload {
                AccountFilter = SelectedAccount,
                CatFilter = SelectedCategory,
                HideTransfers = HideTransfers,
                MaxDate = MaxDateString,
                MinDate = MinDateString,
                MerchFilter = SelectedMerchant,
                PageIndex = pageIndex ?? 1,
                SearchString = SearchText,
                SortOrder = CurrentSort
            }.ToJson());

            Transactions = await PaginatedList<Transaction>.CreateAsync(transactionsIQ.AsSplitQuery().AsNoTracking(), pageIndex ?? 1, 50);
        }
    }
}
