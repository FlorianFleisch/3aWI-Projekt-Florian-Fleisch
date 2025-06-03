public class Student : Person
{
    public int ID { get; set; } // Added property for entity key

    public enum SchoolClasses
    {
        Class1a = 0,
        Class1b = 1,
        Class2a = 2,
        Class2b = 3,
        Class3a = 4,
        Class3b = 5,
        Class4a = 6,
        Class4b = 7,
        Class5a = 8,
        Class5b = 9
    }

    public enum Tracks
    {
        WI = 0,
        WL = 1,
        WM = 2,
        WP = 3,
        CI = 4,
        CB = 5,
        CT = 6,
        MD = 7,
        MP = 8
    }

    public SchoolClasses SchoolClass { get; set; }
    public Tracks Track { get; set; }

    public Student(string firstname, string lastname, Genders gender, DateTime birthdate, SchoolClasses schoolClass, Tracks track)
        : base(firstname, lastname, gender, birthdate)
    {
        SchoolClass = schoolClass;
        Track = track;
    }
}
