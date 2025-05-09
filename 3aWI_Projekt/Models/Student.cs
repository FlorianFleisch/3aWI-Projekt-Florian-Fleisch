public class Student : Person
{
    public string SchoolClass { get; set; }

    public string Vorname { get; set; }
    public string Nachname { get; set; }

    public Student(string geschlecht, DateTime geburtsdatum,
                   string schoolClass, string vorname, string nachname)
        : base(geschlecht, geburtsdatum)
    {
        SchoolClass = schoolClass;
        Vorname = vorname;
        Nachname = nachname;
    }
}
