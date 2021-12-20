using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Module1;

[Route("[module]/[controller]")]
public class TestController : Controller
{
    ILogger<TestController> logger;
    private readonly ITestService testService;

    public TestController(ILogger<TestController> logger, ITestService testService)
    {
        this.logger = logger;
        this.testService = testService;
    }

    [HttpGet("ExecuteConnectTest")]
    public ActionResult<bool> ExecuteConnectTest()
    {
        logger.LogInformation("ojbbjbj");
        return testService.ExecuteConnectTest();
    }

    [HttpGet]
    public ActionResult<string> Index()
    {
        logger.LogInformation("ojbbjbj");
        return "Hello World from TestController in Module 1";
    }

    /// <summary>
    /// InterModule
    /// </summary>
    /// <returns></returns>
    [HttpGet("InterModule")]
    public ActionResult<string> InterModule()
    {
        return $"{0} in TestController in Module 1";
    }
}