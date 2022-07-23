using Microsoft.AspNetCore.Mvc;

[Route("[module]/[controller]")]
public class TestController : Controller
{
    private readonly IFreeSql _fsql;

    public TestController(IFreeSql fsql)
    {
        this._fsql = fsql;
    }

    [HttpGet]
    public ActionResult<string> Index()
    {
        return "Hello World from TestController in Module 2";
    }

    /// <summary>
    /// InterModule
    /// </summary>
    /// <returns></returns>
    [HttpGet("InterModule")]
    public ActionResult<string> InterModule()
    {
        return $"{0} in TestController in Module 2";
    }
}