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
        private readonly AppDbContext _Context;

        public SchoolController(AppDbContext context)
        {
            _Context = context;
        }

        [HttpPost("Createschool")]
        public IActionResult CreateSchool([FromBody] SchoolDto request)
        {
            var school = new School(request.Name);
            _Context.Schools.Add(school);
            _Context.SaveChanges();
            return Created($"/api/school/{school.ID}", new { id = school.ID });
        }

        [HttpPost("students")]
        public IActionResult CreateStudent([FromBody] StudentDto dto)
        {
            var student = new Student(dto.Firstname, dto.Lastname, dto.Gender, dto.Birthdate, dto.SchoolClass, dto.Track);
            _Context.Students.Add(student);
            _Context.SaveChanges();
            return Created($"/api/student/{student.ID}", new { id = student.ID });
        }

        [HttpPost("classrooms")]
        public IActionResult CreateClassroom([FromBody] ClassroomDto dto)
        {
            var room = new Classroom(dto.Name, dto.Size, dto.Seats, dto.Cynap);
            _Context.Classrooms.Add(room);
            _Context.SaveChanges();
            return Created($"/api/classroom/{room.ID}", new { id = room.ID });
        }

        [HttpGet("schools")]
        public IActionResult GetSchools()
        {
            return Ok(_Context.Schools.Select(s => new { s.ID, s.Name }));
        }

        [HttpGet("students/count")]
        public IActionResult GetNumberOfStudents()
        {
            int count = _Context.Students.Count();
            return Ok(count);
        }

        [HttpGet("students/gender-count")]
        public IActionResult GetMaleAndFemaleStudentCount()
        {
            int male = _Context.Students.Count(s => s.Gender == Person.Genders.m);
            int female = _Context.Students.Count(s => s.Gender == Person.Genders.w);
            return Ok(new { male, female });
        }

        [HttpGet("classrooms/count")]
        public IActionResult GetNumberOfClassrooms()
        {
            int count = _Context.Classrooms.Count();
            return Ok(count);
        }

        [HttpGet("students/average-age")]
        public IActionResult GetAverageAge()
        {
            double avg = _Context.Students.Any() ? _Context.Students.Average(s => s.Age) : 0;
            return Ok(avg);
        }

        [HttpGet("classrooms/with-cynap")]
        public IActionResult GetClassroomsWithCynap()
        {
            var rooms = _Context.Classrooms.Where(c => c.Cynap)
                .Select(r => new { r.ID, r.Name });
            return Ok(rooms);
        }

        [HttpGet("classes/count")]
        public IActionResult GetNumberOfClasses()
        {
            int count = _Context.Students.Select(s => s.SchoolClass).Distinct().Count();
            return Ok(count);
        }

        [HttpGet("classes/student-counts")]
        public IActionResult GetClassStudentCounts()
        {
            var result = _Context.Students
                .GroupBy(s => s.SchoolClass.ToString())
                .ToDictionary(g => g.Key, g => g.Count());
            return Ok(result);
        }

        [HttpGet("classes/{className}/female-percentage")]
        public IActionResult GetFemalePercentageInClass(string className)
        {
            var students = _Context.Students
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
            var room = _Context.Classrooms.Find(roomId);
            if (room == null) return NotFound();
            int size = _Context.Students.Count(s => s.SchoolClass.ToString() == className);
            return Ok(room.Seats >= size);
            {
                return Ok(_context.Schools.Select(s => new { s.ID, s.Name }));
            }
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

