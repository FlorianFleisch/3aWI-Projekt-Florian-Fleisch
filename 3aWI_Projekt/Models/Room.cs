namespace _3aWI_Projekt.Models
{
    public class Room
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Size { get; set; }
        public Room(string name, string size)
        {
            Name = name;
            Size = size;
        }
    }
}
