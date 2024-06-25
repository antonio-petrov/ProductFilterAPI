using ProductFilterService.Constants;
using ProductFilterService.Interfaces;

namespace ProductFilterService.Services
{
    public class AppConfig : IAppConfig
    {
        // This could for example read azure configuration settings
        public string DatabaseUrl { get; set; }
    }
}
