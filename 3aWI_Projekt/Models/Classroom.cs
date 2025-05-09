using _3aWI_Projekt.Models;

public class Classroom : School
{
    public string Size { get; set; }
    public int NumberOfSeats { get; set; }
    public bool HasCynap { get; set; }

    public Classroom(string size, int numberOfSeats, bool hasCynap)
    {
        Size = size;
        NumberOfSeats = numberOfSeats;
        HasCynap = hasCynap;
    }

    public void AddStudent(Student student)
    {
        if (Students.Count < NumberOfSeats)
        {
            Students.Add(student);
        }
        else
        {
            // keine Plätze mehr frei
        }
    }
}
