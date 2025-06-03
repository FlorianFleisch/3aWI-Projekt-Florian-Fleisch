public class Person
{
    private string _Firstname;
    public string Firstname { get { return _Firstname; } }
    private string _Lastname;
    public string Lastname { get { return _Lastname; } }
    private DateTime _Birthdate { get; set; }
    public int Age
    {
        get
        {
            return DateTime.Now.Year - _Birthdate.Year;
        }
    }

    public enum Genders 
    {
        m = 0, 
        w = 1,
        d = 2
    }
    private Genders _Gender;
    public Genders Gender { get { return _Gender; } }
    
    public Person(string firstname, string lastname, Genders gender, DateTime birthdate)
    {
        _Firstname = firstname;
        _Lastname = lastname;
        _Gender = gender;
        _Birthdate = birthdate;

    }

    public void changeBirthday(DateTime newBirthdate)
    {
        _Birthdate = newBirthdate;
    }

    public void changeFirstname(string newFirstname)
    {
        _Firstname = newFirstname;
    }

    public void changeLastname(string newLastname)
    {
        _Lastname = newLastname;
    }
}
