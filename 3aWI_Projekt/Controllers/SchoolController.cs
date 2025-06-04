using Microsoft.AspNetCore.Mvc;
using _3aWI_Projekt.Database;
using _3aWI_Projekt.DTO;
using _3aWI_Projekt.Models;

namespace _3aWI_Projekt.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SchoolController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SchoolController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("schools")]
        public IActionResult CreateSchool([FromBody] SchoolDto request)
        {
            var school = new School(request.Name);
            _context.Schools.Add(school);
            _context.SaveChanges();
            return Created($"/api/school/{school.ID}", new { id = school.ID });
        }

        [HttpPost("students")]
        public IActionResult CreateStudent([FromBody] StudentDto dto)
        {
            var student = new Student(dto.Firstname, dto.Lastname, dto.Gender, dto.Birthdate, dto.SchoolClass, dto.Track);
            _context.Students.Add(student);
            _context.SaveChanges();
            return Created($"/api/student/{student.ID}", new { id = student.ID });
        }

        [HttpPost("classrooms")]
        public IActionResult CreateClassroom([FromBody] ClassroomDto dto)
        {
            var room = new Classroom(dto.Name, dto.Size, dto.Seats, dto.Cynap);
            _context.Classrooms.Add(room);
            _context.SaveChanges();
            return Created($"/api/classroom/{room.ID}", new { id = room.ID });
        }

        [HttpGet("schools")]
        public IActionResult GetSchools()
        {
            return Ok(_context.Schools.Select(s => new { s.ID, s.Name }));
        }
    }
}

