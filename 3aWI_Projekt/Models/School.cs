public class School
{
    public List<Student> Students { get; set; }
    public List<Classroom> Classrooms { get; set; }

    public School()
    {
        Students = new List<Student>();
        Classrooms = new List<Classroom>();
    }

    public void AddStudent(Student student) => Students.Add(student);
    public void AddClassroom(Classroom classroom) => Classrooms.Add(classroom);

    public int GetNumberOfStudents() => Students.Count;

    public (int male, int female) GetMaleAndFemaleStudentCount()
    {
        int maleCount = Students.Count(s => s.Geschlecht.ToLower() == "männlich");
        int femaleCount = Students.Count(s => s.Geschlecht.ToLower() == "weiblich");
        return (maleCount, femaleCount);
    }

    public int GetNumberOfClassrooms() => Classrooms.Count;

    public double GetAverageAge()
        => Students.Average(s => DateTime.Now.Year - s.Geburtsdatum.Year);

    public List<Classroom> GetClassroomsWithCynap()
        => Classrooms.Where(c => c.HasCynap).ToList();

    public int GetNumberOfClasses()
        => Students.Select(s => s.SchoolClass).Distinct().Count();

    public Dictionary<string, int> GetClassStudentCounts()
        => Students.GroupBy(s => s.SchoolClass)
                   .ToDictionary(g => g.Key, g => g.Count());

    public double GetFemalePercentageInClass(string className)
    {
        var classStudents = Students.Where(s => s.SchoolClass == className).ToList();
        int femaleCount = classStudents.Count(s => s.Geschlecht.ToLower() == "weiblich");
        return classStudents.Count > 0
               ? (double)femaleCount / classStudents.Count * 100
               : 0;
    }

    public bool CanClassFitInRoom(string className, Classroom room)
    {
        int classSize = Students.Count(s => s.SchoolClass == className);
        return room.NumberOfSeats >= classSize;
    }
}
