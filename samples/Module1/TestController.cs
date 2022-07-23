using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Module1.Services;

[Route("[module]/[controller]")]
public class TestController : Controller
{
    private readonly ILogger<TestController> _logger;
    private readonly ITestService _testService;

    public TestController(ILogger<TestController> logger, ITestService testService)
    {
        this._logger = logger;
        this._testService = testService;
    }

    [HttpGet("ExecuteConnectTest")]
    public ActionResult<bool> ExecuteConnectTest()
    {
        _logger.LogInformation("ExecuteConnectTest");
        return _testService.ExecuteConnectTest();
    }

    [HttpGet]
    public ActionResult<string> Index()
    {
        _logger.LogInformation("Index");
        return "Hello World from TestController in Module 1 Index";
    }

    /// <summary>
    /// InterModule
    /// </summary>
    /// <returns></returns>
    [HttpGet("InterModule")]
    public ActionResult<string> InterModule()
    {
        return $"{0} in TestController in Module 1 InterModule";
    }
}