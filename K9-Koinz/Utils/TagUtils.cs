using K9_Koinz.Data;
using K9_Koinz.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace K9_Koinz.Utils {
    public static class TagUtils {
        public static void CreateTagsIfNeeded(KoinzContext context) {
            if (!context.Tags.Any()) {
                var tags = new List<Tag> {
                    new Tag { Id = Guid.NewGuid(), Name = "Sabin Allowance", ShortForm = "S", HexColor = "#ffc107" },
                    new Tag { Id = Guid.NewGuid(), Name = "Liz Allowance", ShortForm = "L", HexColor = "#0d6efd" }
                };

                context.Tags.AddRange(tags);
                context.SaveChanges();
            }
        }

        public static SelectList GetTagList(KoinzContext context) {
            return new SelectList(context.Tags.OrderBy(tag => tag.Name).ToList(), nameof(Tag.Id), nameof(Tag.Name));
        }
    }
}
