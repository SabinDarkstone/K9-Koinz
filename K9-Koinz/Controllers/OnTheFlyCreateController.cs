using K9_Koinz.Data;
using K9_Koinz.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace K9_Koinz.Controllers {
    public class OnTheFlyCreateController : Controller {
        private readonly KoinzContext _context;

        public OnTheFlyCreateController(KoinzContext context) {
            _context = context;
        }

        [HttpPost]
        public async Task<JsonResult> AddMerchantAsync(string merchantName) {
            var isExisting = await _context.Merchants.Where(merc => merc.Name == merchantName).AnyAsync();
            if (isExisting) {
                return new JsonResult("DUPLICATE");
            } else {
                var newMerchant = new Merchant { Name = merchantName };
                try {
                    _context.Merchants.Add(newMerchant);
                    await _context.SaveChangesAsync();
                    return new JsonResult(newMerchant.Id.ToString());
                } catch (Exception) {
                    return new JsonResult("ERROR");
                }
            }
        }
    }
}
