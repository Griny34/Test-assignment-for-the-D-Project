namespace SolutionDIcotaner
{
    public class RocketPack : Item
    {
        private int _charges;

        public RocketPack(string name, int charges) : base(name)
        {
            _charges = charges;
        }
    }
}

