namespace _3aWI_Projekt.Models;
public class School
{
    public int ID { get; set; }
    public string Name { get; set; }
    private List<Student> _Students = new List<Student>();
    public List<Student> Students { get; set; } = new();
    private List<Classroom> _Classrooms = new List<Classroom>();
    public List<Classroom> Classrooms { get; set; } = new();

    public School(string name)
    {
        Name = name;
    }

    protected School() { }

    public void AddStudent(Student student)
    {
        Students.Add(student);
    }

    public void AddClassroom(Classroom classroom)
    {
        Classrooms.Add(classroom);
    }

    public int GetNumberOfStudents()
    {
        return Students.Count;
    }

    public (int male, int female) GetMaleAndFemaleStudentCount()
    {
        int maleCount = Students.Count(s => s.Gender == Person.Genders.m);
        int femaleCount = Students.Count(s => s.Gender == Person.Genders.w);
        return (maleCount, femaleCount);
    }

    public int GetNumberOfClassrooms()
    {
        return Classrooms.Count;
    }

    public double GetAverageAge()
    {
        return Students.Count > 0 ? Students.Average(s => s.Age) : 0;
    }

    public List<Classroom> GetClassroomsWithCynap()
    {
        return Classrooms.Where(c => c.Cynap).ToList();
    }

    public int GetNumberOfClasses()
    {
        return Students.Select(s => s.SchoolClass).Distinct().Count();
    }

    public Dictionary<string, int> GetClassStudentCounts()
    {
        return Students.GroupBy(s => s.SchoolClass.ToString())
                       .ToDictionary(g => g.Key, g => g.Count());
    }

    public double GetFemalePercentageInClass(string className)
    {
        var classStudents = Students.Where(s => s.SchoolClass.ToString() == className).ToList();
        int femaleCount = classStudents.Count(s => s.Gender == Person.Genders.w);
        return classStudents.Count > 0 ? (double)femaleCount / classStudents.Count * 100 : 0;
    }

    public bool CanClassFitInRoom(string className, Classroom room)
    {
        int classSize = Students.Count(s => s.SchoolClass.ToString() == className);
        return room.Seats >= classSize;
    }
}
