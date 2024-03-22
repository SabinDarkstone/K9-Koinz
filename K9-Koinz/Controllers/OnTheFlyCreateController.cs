using K9_Koinz.Data;
using K9_Koinz.Models;
using Microsoft.AspNetCore.Mvc;

namespace K9_Koinz.Controllers {
    public class OnTheFlyCreateController : GenericController {
        public OnTheFlyCreateController(IRepositoryWrapper data)
            : base(data) { }

        [HttpPost]
        public async Task<JsonResult> OnPostAddMerchant(string merchantName) {
            var isExisting = await _data.MerchantRepository.DoesExistsByName(merchantName);
            if (isExisting) {
                return new JsonResult("DUPLICATE");
            } else {
                var newMerchant = new Merchant { Name = merchantName };
                try {
                    _data.MerchantRepository.Add(newMerchant);
                    await _data.SaveAsync();
                    return new JsonResult(newMerchant.Id.ToString());
                } catch (Exception) {
                    return new JsonResult("ERROR");
                }
            }
        }
    }
}
