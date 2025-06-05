
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
        
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        [HttpGet("students/count")]
        public IActionResult GetNumberOfStudents()
        {
            int count = _context.Students.Count();
            return Ok(count);
        }

        [HttpGet("students/gender-count")]
        public IActionResult GetMaleAndFemaleStudentCount()
        {
            int male = _context.Students.Count(s => s.Gender == Person.Genders.m);
            int female = _context.Students.Count(s => s.Gender == Person.Genders.w);
            return Ok(new { male, female });
        }

        [HttpGet("classrooms/count")]
        public IActionResult GetNumberOfClassrooms()
        {
            int count = _context.Classrooms.Count();
            return Ok(count);
        }

        [HttpGet("students/average-age")]
        public IActionResult GetAverageAge()
        {
            double avg = _context.Students.Any() ? _context.Students.Average(s => s.Age) : 0;
            return Ok(avg);
        }

        [HttpGet("classrooms/with-cynap")]
        public IActionResult GetClassroomsWithCynap()
        {
            var rooms = _context.Classrooms.Where(c => c.Cynap)
                .Select(r => new { r.ID, r.Name });
            return Ok(rooms);
        }

        [HttpGet("classes/count")]
        public IActionResult GetNumberOfClasses()
        {
            int count = _context.Students.Select(s => s.SchoolClass).Distinct().Count();
            return Ok(count);
        }

        [HttpGet("classes/student-counts")]
        public IActionResult GetClassStudentCounts()
        {
            var result = _context.Students
                .GroupBy(s => s.SchoolClass.ToString())
                .ToDictionary(g => g.Key, g => g.Count());
            return Ok(result);
        }

        [HttpGet("classes/{className}/female-percentage")]
        public IActionResult GetFemalePercentageInClass(string className)
        {
            var students = _context.Students
                .Where(s => s.SchoolClass.ToString() == className);
            int total = students.Count();
            if (total == 0) return Ok(0);
            int female = students.Count(s => s.Gender == Person.Genders.w);
            double percentage = (double)female / total * 100;
            return Ok(percentage);
        }

        [HttpGet("classes/{className}/can-fit/{roomId}")]
        public IActionResult CanClassFitInRoom(string className, int roomId)
        {
            var room = _context.Classrooms.Find(roomId);
            if (room == null) return NotFound();
            int size = _context.Students.Count(s => s.SchoolClass.ToString() == className);
            return Ok(room.Seats >= size);
        }

        // ----- New Endpoints -----

        [HttpPost("schools/{schoolId}/students/{studentId}")]
        public IActionResult AddStudentToSchool(int schoolId, int studentId)
        {
            var school = _context.Schools
                .Include(s => s.Students)
                .FirstOrDefault(s => s.ID == schoolId);
            var student = _context.Students.Find(studentId);
            if (school == null || student == null) return NotFound();
            school.AddStudent(student);
            _context.SaveChanges();
            return Ok();
        }

        [HttpPost("schools/{schoolId}/classrooms/{classroomId}")]
        public IActionResult AddClassroomToSchool(int schoolId, int classroomId)
        {
            var school = _context.Schools
                .Include(s => s.Classrooms)
                .FirstOrDefault(s => s.ID == schoolId);
            var room = _context.Classrooms.Find(classroomId);
            if (school == null || room == null) return NotFound();
            school.AddClassroom(room);
            _context.SaveChanges();
            return Ok();
        }

        [HttpPost("classrooms/{classroomId}/students/{studentId}")]
        public IActionResult AddStudentToClassroom(int classroomId, int studentId)
        {
            var classroom = _context.Classrooms
                .Include(c => c.Students)
                .FirstOrDefault(c => c.ID == classroomId);
            var student = _context.Students.Find(studentId);
            if (classroom == null || student == null) return NotFound();
            classroom.AddStudent(student);
            _context.SaveChanges();
            return Ok();
        }

        [HttpGet("schools/{schoolId}/values")]
        public IActionResult GetSchoolValues(int schoolId)
        {
            var school = _context.Schools
                .Include(s => s.Students)
                .Include(s => s.Classrooms)
                .FirstOrDefault(s => s.ID == schoolId);
            if (school == null) return NotFound();

            var (male, female) = school.GetMaleAndFemaleStudentCount();
            var values = new
            {
                numberOfStudents = school.GetNumberOfStudents(),
                numberOfMaleStudents = male,
                numberOfFemaleStudents = female,
                averageAgeOfStudents = school.GetAverageAge(),
                numberOfClassrooms = school.GetNumberOfClassrooms(),
                classroomsWithCynap = school.GetClassroomsWithCynap().Select(r => r.ID),
                classroomsWithNumberOfStudents = school.GetClassStudentCounts()
            };

            return Ok(values);
        }

        [HttpGet("schools/{schoolId}/classes/{className}/female-percentage")]
        public IActionResult GetFemalePercentageInClass(int schoolId, string className)
        {
            var school = _context.Schools
                .Include(s => s.Students)
                .FirstOrDefault(s => s.ID == schoolId);
            if (school == null) return NotFound();
            double percentage = school.GetFemalePercentageInClass(className);
            return Ok(percentage);
        }

        [HttpGet("schools/{schoolId}/classrooms/{roomId}/can-fit/{className}")]
        public IActionResult CanClassFitInRoom(int schoolId, int roomId, string className)
        {
            var school = _context.Schools
                .Include(s => s.Students)
                .FirstOrDefault(s => s.ID == schoolId);
            var room = _context.Classrooms.Find(roomId);
            if (school == null || room == null) return NotFound();
            bool ok = school.CanClassFitInRoom(className, room);
            return Ok(ok);
        }
    }
}

