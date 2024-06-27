using K9_Koinz.Data;
using K9_Koinz.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace K9_Koinz.Pages {
    public class DataImportWizardModel : PageModel {
        private readonly KoinzContext _context;
        private readonly ILogger<DataImportWizardModel> _logger;

        [BindProperty]
        public string Message { get; set; }

        public DataImportWizardModel(KoinzContext context, ILogger<DataImportWizardModel> logger) {
            _context = context;
            _logger = logger;
        }

        [BindProperty, Display(Name = "File")]
        public IFormFile UploadedFile { get; set; }

        public IActionResult OnPostUpload(IList<IFormFile> files) {
            try {
                _logger.LogInformation(UploadedFile.FileName);
                var lines = UploadedFile.ReadAsList();
                var importer = new DataImport(_context, _logger);
                importer.ParseFileData(lines);
                Message = "Success";
            } catch (Exception ex) {
                Message = JsonConvert.SerializeObject(ex, Formatting.Indented, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
            }
            return Page();
        }
    }
}
