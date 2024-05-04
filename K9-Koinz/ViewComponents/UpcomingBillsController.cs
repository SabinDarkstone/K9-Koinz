using K9_Koinz.Data;
using K9_Koinz.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace K9_Koinz.ViewComponents {
    [ViewComponent(Name = "UpcomingBills")]
    public class UpcomingBillsController : ViewComponent {
        private readonly KoinzContext _context;

        public List<Bill> Bills { get; set; }

        public UpcomingBillsController(KoinzContext context) {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync() {
            var startDate = DateTime.Today;
            var endDate = DateTime.Today.AddDays(7);

            Bills = (await _context.Bills
                .AsNoTracking()
                .Include(bill => bill.RepeatConfig)
                .ToListAsync())
                .Where(bill => bill.RepeatConfig.CalculatedNextFiring >= startDate && bill.RepeatConfig.CalculatedNextFiring <= endDate)
                .ToList();

            return View(this);
        }
    }
}
