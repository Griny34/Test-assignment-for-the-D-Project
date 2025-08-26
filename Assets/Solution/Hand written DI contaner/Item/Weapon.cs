namespace SolutionDIcotaner
{
    public class Weapon : Item
    {
        private int _ammo;

        public Weapon(string name, int ammo) : base(name)
        {
            _ammo = ammo;
        }
    }
}