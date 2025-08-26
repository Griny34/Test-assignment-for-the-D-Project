using UnityEngine;

namespace SolutionDIcotaner
{
    public class HealthPlayer
    {
        private int _volue;
        private int _maxHealth;

        public int Volue => _volue;

        public HealthPlayer(int health)
        {
            _volue = health;
        }

        public void TakeDameg(int damage)
        {
            _volue = Mathf.Clamp(_volue - damage, 0, _maxHealth);
        }
    }
}

