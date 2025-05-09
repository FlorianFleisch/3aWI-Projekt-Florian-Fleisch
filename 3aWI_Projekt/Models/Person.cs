namespace _3aWI_Projekt.Models;   
public class Person
{
    public string Geschlecht { get; set; }
    public DateTime Geburtsdatum { get; set; }

    public Person(string geschlecht, DateTime geburtsdatum)
    {
        Geschlecht = geschlecht;
        Geburtsdatum = geburtsdatum;
    }
}
