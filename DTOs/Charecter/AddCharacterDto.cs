using rpg.Models;

namespace rpg.DTOs.Charecter
{
    public class AddCharacterDto
    {
        public string Name { get; set; }
        public int HitPoint { get; set; }
        public int Strength { get; set; }
        public int Defense { get; set; }
        public int Intelligence { get; set; }
        public RpgClass Class { get; set; } = RpgClass.Knight;
    }
}