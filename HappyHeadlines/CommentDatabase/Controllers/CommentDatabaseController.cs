using Microsoft.AspNetCore.Mvc;

namespace CommentDatabase.Controllers;

[ApiController]
[Route("[controller]")]
public class CommentDatabaseController : ControllerBase
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