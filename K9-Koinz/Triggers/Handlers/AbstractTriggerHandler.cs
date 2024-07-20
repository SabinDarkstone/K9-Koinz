using K9_Koinz.Data;
using K9_Koinz.Models.Meta;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace K9_Koinz.Triggers.Handlers {
    public abstract class AbstractTriggerHandler<TEntity> where TEntity : BaseEntity {
        protected readonly KoinzContext context;
        protected readonly ILogger logger;

        protected ModelStateDictionary modelState;

        protected AbstractTriggerHandler(KoinzContext context, ILogger logger) {
            this.context = context;
            this.logger = logger;
        }

        public void SetModelState(ModelStateDictionary modelState) {
            this.modelState = modelState;
        }
    }
}
