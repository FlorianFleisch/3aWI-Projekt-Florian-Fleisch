using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Ein einziger Controller, der alles bedient, was dein HTML braucht.
/// Die echten Klassen (School, Student, Classroom) bleiben unverändert ―
/// wir halten nur externe IDs in Dictionaries.
/// </summary>
[ApiController]
[Route("api")]
public class SchoolController : ControllerBase
{
    private static readonly Dictionary<int, School> Schools = new();
    private static readonly Dictionary<int, Student> Students = new();
    private static readonly Dictionary<int, Classroom> Classrooms = new();

    private static int _nextSchoolId = 1;
    private static int _nextStudentId = 1;
    private static int _nextClassroomId = 1;

    public record StudentDto(string SchoolClass, string Gender, DateTime Birthdate);
    public record ClassroomDto(string Size, int Seats, bool Cynap);

    // ---------- Create ----------
    [HttpPost("schools")]
    public ActionResult<object> CreateSchool()
    {
        int id = _nextSchoolId++;
        Schools[id] = new School();
        return Created($"/api/schools/{id}", new { id });
    }

    [HttpPost("students")]
    public ActionResult<object> CreateStudent([FromBody] StudentDto dto)
    {
        int id = _nextStudentId++;
        var genderStr = dto.Gender switch { "0" => "Männlich", "1" => "Weiblich", _ => "Non-binary" };
        var student = new Student(genderStr, dto.Birthdate,
                                    MapClass(dto.SchoolClass), $"Student{id}", $"L{id}");
        Students[id] = student;
        return Created($"/api/students/{id}", new { id, student.Vorname, student.Nachname });
    }

    [HttpPost("classrooms")]
    public ActionResult<object> CreateClassroom([FromBody] ClassroomDto dto)
    {
        int id = _nextClassroomId++;
        var room = new Classroom(dto.Size, dto.Seats, dto.Cynap);
        Classrooms[id] = room;
        return Created($"/api/classrooms/{id}", new { id, room.Size, room.NumberOfSeats });
    }

    // ---------- Relationen ----------
    [HttpPost("schools/{schoolId}/students/{studentId}")]
    public IActionResult AddStudentToSchool(int schoolId, int studentId)
    {
        if (!Schools.TryGetValue(schoolId, out var school) ||
            !Students.TryGetValue(studentId, out var student)) return NotFound();
        school.AddStudent(student);
        return NoContent();
    }

    [HttpPost("schools/{schoolId}/classrooms/{roomId}")]
    public IActionResult AddClassroomToSchool(int schoolId, int roomId)
    {
        if (!Schools.TryGetValue(schoolId, out var school) ||
            !Classrooms.TryGetValue(roomId, out var room)) return NotFound();
        school.AddClassroom(room);
        return NoContent();
    }

    [HttpPost("classrooms/{roomId}/students/{studentId}")]
    public IActionResult AddStudentToClassroom(int roomId, int studentId)
    {
        if (!Classrooms.TryGetValue(roomId, out var room) ||
            !Students.TryGetValue(studentId, out var student)) return NotFound();
        room.AddStudent(student);
        return NoContent();
    }

    // ---------- Listen ----------
    [HttpGet("schools")] public IActionResult GetSchools() => Ok(Schools.Keys.Select(id => new { id }));
    [HttpGet("students")] public IActionResult GetStudents() => Ok(Students.Select(k => new { id = k.Key, k.Value.Vorname, k.Value.Nachname }));
    [HttpGet("classrooms")] public IActionResult GetClassrooms() => Ok(Classrooms.Select(k => new { id = k.Key, k.Value.Size, k.Value.NumberOfSeats }));

    // ---------- Kennzahlen ----------
    [HttpGet("schools/{schoolId}/values")]
    public IActionResult GetSchoolValues(int schoolId)
    {
        if (!Schools.TryGetValue(schoolId, out var school)) return NotFound();
        var (male, female) = school.GetMaleAndFemaleStudentCount();
        var values = new
        {
            numberOfStudents = school.GetNumberOfStudents(),
            numberOfMaleStudents = male,
            numberOfFemaleStudents = female,
            averageAgeOfStudents = school.GetAverageAge(),
            numberOfClassrooms = school.GetNumberOfClassrooms(),
            classroomsWithCynap = school.GetClassroomsWithCynap()
                                               .Select(c => Classrooms.First(x => x.Value == c).Key),
            classroomsWithNumberOfStudents = school.GetClassStudentCounts()
        };
        return Ok(values);
    }

    [HttpGet("schools/{schoolId}/classes/{className}/female-percentage")]
    public IActionResult GetFemalePercentage(int schoolId, string className)
    {
        if (!Schools.TryGetValue(schoolId, out var school)) return NotFound();
        return Ok(school.GetFemalePercentageInClass(className));
    }

    [HttpGet("schools/{schoolId}/classrooms/{roomId}/can-fit/{className}")]
    public IActionResult CanClassFit(int schoolId, int roomId, string className)
    {
        if (!Schools.TryGetValue(schoolId, out var school) ||
            !Classrooms.TryGetValue(roomId, out var room)) return NotFound();
        return Ok(school.CanClassFitInRoom(className, room));
    }

    private static string MapClass(string code) => code switch
    {
        "0" => "3aWI",
        "1" => "3bWI",
        _ => code
    };
}
