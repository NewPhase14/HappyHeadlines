using Microsoft.AspNetCore.Mvc;

namespace ArticleDatabase.Controllers;

[ApiController]
[Route("[controller]")]
public class DatabaseController : ControllerBase
{
    private Database database = Database.GetInstance();
    
    [HttpPost]
    public void Recreate()
    {
        database.RecreateDatabase();
    }
    
    [HttpDelete]
    public void Delete()
    {
        database.DeleteDatabase();
    }
}