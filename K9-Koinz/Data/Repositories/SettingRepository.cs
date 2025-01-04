using K9_Koinz.Data.Repositories.Meta;
using K9_Koinz.Models;

namespace K9_Koinz.Data.Repositories {
    public class SettingRepository : Repository<Setting> {
        public SettingRepository(KoinzContext context) : base(context) { }


    }
}
