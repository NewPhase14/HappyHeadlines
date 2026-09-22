using Microsoft.AspNetCore.Mvc;

namespace DraftDatabase.Controllers;

[ApiController]
[Route("[controller]")]
public class DraftDatabaseController : ControllerBase
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