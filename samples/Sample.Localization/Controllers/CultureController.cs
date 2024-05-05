using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using FreeSql;

namespace Sample.Localization.Controllers
{
    [Route("[controller]/[action]")]
    public class CultureController : Controller
    {
        private readonly ILogger<CultureController> _logger;
        private readonly IStringLocalizer<CultureController> _stringLocalizer;

        public CultureController(ILogger<CultureController> logger, IStringLocalizer<CultureController> stringLocalizer)
        {
            _logger = logger;
            _stringLocalizer = stringLocalizer;
        }
        
        public string FreeSql()
        {
            return CoreStrings.AsTable_PropertyName_FormatError("table name") ;
        }
        public string Hello()
        {
            return _stringLocalizer["Hello"] ;
        }

        public async Task<ProviderCultureResult> GetProvider()
        {
            var requestCultureFeature = HttpContext.Features.Get<IRequestCultureFeature>();
            var providerResult = await requestCultureFeature.Provider.DetermineProviderCultureResult(HttpContext);

            return providerResult;
        }
        
        public IActionResult Index()
        {
            var requestCultureFeature = HttpContext.Features.Get<IRequestCultureFeature>();
            var requestCulture = requestCultureFeature.RequestCulture;

            ViewBag.SR = _stringLocalizer;
            ViewBag.requestCultureFeature = requestCultureFeature;
            ViewBag.requestCulture = requestCulture;

            return View();
        }
    }
}
