using _3aWI_Projekt.Models;

namespace _3aWI_Projekt.DTO
{
    public record StudentDto(
        string Firstname,
        string Lastname,
        Person.Genders Gender,
        DateTime Birthdate,
        Student.SchoolClasses SchoolClass,
        Student.Tracks Track);
}
