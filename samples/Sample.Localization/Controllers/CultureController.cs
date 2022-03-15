﻿using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Sample.Localization.Controllers
{
    [Route("[controller]")]
    public class CultureController : Controller
    {
        private readonly ILogger<CultureController> _logger;
        private readonly IFreeSql freeSql;
        private readonly IStringLocalizer stringLocalizer;

        public CultureController(ILogger<CultureController> logger, IFreeSql freeSql, IStringLocalizerFactory localizerFactory)
        {
            _logger = logger;
            this.freeSql = freeSql;
            this.stringLocalizer = localizerFactory.Create(null);
        }

        [HttpGet("{id}")]
        public string Get(int id)
        {
            return stringLocalizer["Request Localization"] + id;
        }

        [HttpGet("GetProvider")]
        public async Task<ProviderCultureResult> GetProvider()
        {
            var requestCultureFeature = HttpContext.Features.Get<IRequestCultureFeature>();
            var providerResult = await requestCultureFeature.Provider.DetermineProviderCultureResult(HttpContext);

            return providerResult;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var requestCultureFeature = HttpContext.Features.Get<IRequestCultureFeature>();
            var requestCulture = requestCultureFeature.RequestCulture;

            ViewBag.SR = stringLocalizer;
            ViewBag.requestCultureFeature = requestCultureFeature;
            ViewBag.requestCulture = requestCulture;

            return View();
        }
    }
}
