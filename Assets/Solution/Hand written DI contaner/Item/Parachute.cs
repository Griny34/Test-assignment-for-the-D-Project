namespace SolutionDIcotaner
{
    public class Parachute : Item
    {
        private bool _isUsed;

        public Parachute(string name, bool isUsed) : base(name)
        {
            _isUsed = isUsed;
        }
    }
}