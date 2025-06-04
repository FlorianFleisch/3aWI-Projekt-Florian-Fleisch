using _3aWI_Projekt.DTO;
using _3aWI_Projekt.Models;
using _3aWI_Projekt.Database;
using Microsoft.AspNetCore.Mvc;



[ApiController]
[Route("api")]
public class SchoolController : ControllerBase
{
    [HttpPost("Createschool")]
    public IActionResult Createschool([FromBody] SchoolDto request)
    {
        var school = new School
        {
            Name = request.Name
        };
        AppContext.School.Add(school);
        AppContext.SaveChanges(); 
        return Created($"/api/schools/{school.ID}", new { id = school.ID });
    }
