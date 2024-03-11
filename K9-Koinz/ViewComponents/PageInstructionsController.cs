using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace K9_Koinz.ViewComponents {

    public class PageInstructionDTO {
        public string Title { get; set; }
        public List<PageInstructionItemDTO> Body { get; set; }
    }

    public class PageInstructionItemDTO {
        public string Heading { get; set; }
        public string Body { get; set; }
    }

    [ViewComponent(Name = "PageInstructions")]
    public class PageInstructionsController : ViewComponent {

        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<PageInstructionsController> _logger;

        public PageInstructionsController(IWebHostEnvironment environment, ILogger<PageInstructionsController> logger) {
            _environment = environment;
            _logger = logger;
        }

        public async Task<IViewComponentResult> InvokeAsync(string topicName) {
            var filePath = Path.Combine(_environment.WebRootPath, "pageInstructions", topicName + ".json");
            _logger.LogInformation(filePath);

            var jsonString = await File.ReadAllTextAsync(filePath);
            _logger.LogInformation(jsonString);

            var dto = JsonSerializer.Deserialize<PageInstructionDTO>(jsonString);

            return View(dto);
        }
    }
}