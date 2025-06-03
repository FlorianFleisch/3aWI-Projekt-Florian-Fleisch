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
}
