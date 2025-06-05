using Microsoft.AspNetCore.Mvc;
using _3aWI_Projekt.Database;
using _3aWI_Projekt.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Projekt.Models;

[ApiController]
[Route("api")]
public class SchoolController : ControllerBase
{
    private readonly AppDbContext _dbContext;

    public SchoolController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    // ------------------- DTOs -------------------
    public record SchoolDto(string Name);

    public record StudentDto(string Firstname,
                             string Lastname,
                             Person.Genders Gender,
                             DateTime Birthdate,
                             Student.SchoolClasses SchoolClass,
                             Student.Tracks Track);



    // ------------------- Create -------------------
    [HttpPost("CreateSchool")]
    public IActionResult CreateSchool([FromBody] SchoolDto request)
    {
        var school = new School
        {
            Name = request.Name
        };
        _dbContext.Schools.Add(school);
        _dbContext.SaveChangesAsync();
        return Created($"/api/schools/{school.ID}", new { id = school.ID });
    }

    [HttpPost("students")]
    public async Task<ActionResult<object>> CreateStudent([FromBody] StudentDto dto)
    {
        var student = new Student(dto.Firstname, dto.Lastname, dto.Gender, dto.Birthdate, dto.SchoolClass, dto.Track);
        _dbContext.Students.Add(student);
        await _dbContext.SaveChangesAsync();
        return Created($"/api/students/{student.ID}", new { id = student.ID });
    }

    [HttpPost("classrooms")]
    public async Task<ActionResult<object>> CreateClassroom([FromBody] ClassroomDto dto)
    {
        var room = new Classroom(dto.Name, dto.Size, dto.Seats, dto.Cynap);
        _dbContext.Classrooms.Add(room);
        await _dbContext.SaveChangesAsync();
        return Created($"/api/classrooms/{room.ID}", new { id = room.ID });
    }

    // ------------------- Relations -------------------
    [HttpPost("schools/{schoolId}/students/{studentId}")]
    public async Task<IActionResult> AddStudentToSchool(int schoolId, int studentId)
    {
        var school = await _dbContext.Schools.Include(s => s.Students).FirstOrDefaultAsync(s => s.ID == schoolId);
        var student = await _dbContext.Students.FindAsync(studentId);

        if (school is null || student is null)
            return NotFound();

        if (!school.Students.Any(s => s.ID == studentId))
        {
            school.Students.Add(student);
            await _dbContext.SaveChangesAsync();
        }

        return NoContent();
    }

    [HttpPost("schools/{schoolId}/classrooms/{roomId}")]
    public async Task<IActionResult> AddClassroomToSchool(int schoolId, int roomId)
    {
        var school = await _dbContext.Schools.Include(s => s.Classrooms).FirstOrDefaultAsync(s => s.ID == schoolId);
        var room = await _dbContext.Classrooms.FindAsync(roomId);

        if (school is null || room is null)
            return NotFound();

        if (!school.Classrooms.Any(r => r.ID == roomId))
        {
            school.Classrooms.Add(room);
            await _dbContext.SaveChangesAsync();
        }

        return NoContent();
    }

    // ------------------- KPIs -------------------
    [HttpGet("schools/{schoolId}/values")]
    public async Task<IActionResult> GetSchoolValues(int schoolId)
    {
        var school = await _dbContext.Schools
            .Include(s => s.Students)
            .Include(s => s.Classrooms)
            .FirstOrDefaultAsync(s => s.ID == schoolId);

        if (school is null)
            return NotFound();

        var maleStudents = school.Students.Count(s => s.Gender == Person.Genders.m);
        var femaleStudents = school.Students.Count(s => s.Gender == Person.Genders.w);
        var averageAge = school.Students.Any() ? school.Students.Average(s => s.Age) : 0;

        var classroomsWithCynap = school.Classrooms.Where(c => c.Cynap).Select(c => c.ID).ToList();

        var classroomsWithNumberOfStudents = school.Classrooms.Select(c => new
        {
            classroomId = c.ID,
            studentCount = c.Students.Count
        }).ToList();

        var values = new
        {
            numberOfStudents = school.Students.Count,
            numberOfMaleStudents = maleStudents,
            numberOfFemaleStudents = femaleStudents,
            averageAgeOfStudents = Math.Round(averageAge, 2),
            numberOfClassrooms = school.Classrooms.Count,
            classroomsWithCynap,
            classroomsWithNumberOfStudents
        };

        return Ok(values);
    }
}
