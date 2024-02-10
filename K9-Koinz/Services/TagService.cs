using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Services.Meta;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace K9_Koinz.Services {
    public interface  ITagService : ICustomService {
        public abstract void CreateTagsIfNeeded();
        public abstract SelectList GetTagList();
    }

    public class TagService : AbstractService<TagService>, ITagService {
        public TagService(KoinzContext context, ILogger<TagService> logger) : base(context, logger) { }

        public void CreateTagsIfNeeded() {
            if (!_context.Tags.Any()) {
                var tags = new List<Tag> {
                    new Tag { Id = Guid.NewGuid(), Name = "Sabin Allowance", ShortForm = "S", HexColor = "#ffc107" },
                    new Tag { Id = Guid.NewGuid(), Name = "Liz Allowance", ShortForm = "L", HexColor = "#0d6efd" }
                };

                _context.Tags.AddRange(tags);
                _context.SaveChanges();
            }
        }

        public SelectList GetTagList() {
            return new SelectList(_context.Tags.OrderBy(tag => tag.Name).ToList(), nameof(Tag.Id), nameof(Tag.Name));
        }
    }
}
