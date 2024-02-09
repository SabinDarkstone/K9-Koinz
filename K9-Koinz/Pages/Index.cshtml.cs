﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using K9_Koinz.Data;
using K9_Koinz.Utils;
using K9_Koinz.Services;

namespace K9_Koinz.Pages {

    public class IndexModel : PageModel {
        private readonly KoinzContext _context;
        private readonly SpendingGraphService _spendingGraph;
        private readonly BillService _bills;

        public IndexModel(KoinzContext context, SpendingGraphService spendingGraph, BillService bills) {
            _context = context;
            _spendingGraph = spendingGraph;
            _bills = bills;
        }

        public string ThisMonthSpendingJson { get; set; }
        public string LastMonthSpendingJson { get; set; }

        public async Task OnGetAsync() {
            NamingUtils.AssignNames(_context);
            TagUtils.CreateTagsIfNeeded(_context);

            var results = await _spendingGraph.CreateGraphData();
            ThisMonthSpendingJson = results[0];
            LastMonthSpendingJson = results[1];
        }
    }
}
