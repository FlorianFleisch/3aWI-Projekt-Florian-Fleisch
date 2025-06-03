namespace _3aWI_Projekt.Models;
public class Classroom : Room
{
    private int _Seats;
    public int Seats { get { return _Seats; } }
    private bool _Cynap;
    public bool Cynap { get { return _Cynap; } }
    private List<Student> _Students = new List<Student>();
    public List<Student> Students { get { return _Students; } }

    public Classroom(string name, string size, int seats, bool cynap) : base(name, size)
    {
        _Seats = seats;
        _Cynap = cynap;
    }

    public void AddStudent(Student student)
    {
        if (Students.Count < Seats)
        {
            Students.Add(student);
        }
        else
        {
            Console.WriteLine("Kein Platz für diesen Schüler.");
        }
    }
    public void RemoveStudent(Student student)
    {
        if (Students.Contains(student))
        {
            _Students.Remove(student);
        }
        else
        {
            Console.WriteLine("Dieser Schüler ist nicht in diesem Raum.");
        }
    }

    public void ClearStudents()
    {
        _Students.Clear();
    }

    public void ChangeSeats(int seats)
    {
        if (seats > 0)
        {
            _Seats = seats;
        }
        else
        {
            Console.WriteLine("Die Anzahl der Plätze muss größer als 0 sein.");
        }
    }

    public void ChangeCynap(bool cynap)
    {
        _Cynap = cynap;
    }
}
