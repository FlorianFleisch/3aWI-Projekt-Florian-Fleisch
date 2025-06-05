using System.ComponentModel.DataAnnotations;
namespace _3aWI_Projekt.DTO
{
    public class CreateCLassroomRequest
    {
        [Required]
        public string Name { get; set; }
        public string Size { get; set; }
        public int Seats { get; set; }
        public bool Cynap { get; set; }
    }
}
