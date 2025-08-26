using UnityEngine;

namespace SolutionDIcotaner
{
    public class Player : IPlayer
    {
        private static Player _instance;

        //Здоровье
        private HealthPlayer _healthPlayer;
        //Жизни
        private LivesPlayer _livesPlayer;

        //Таблица навыков
        private string[] _skills;

        //Экипировка
        private Equipment _equipment;

        public Player(HealthPlayer healthPlayer, LivesPlayer livesPlayer, string[] skills, Equipment equipment)
        {
            //Если уже есть игрок, то выкидываем ошибку
            if (_instance != null)
            {
                throw new System.InvalidOperationException("Только один экземпляр Player может существовать.");
            }

            _healthPlayer = healthPlayer;
            _livesPlayer = livesPlayer;
            _skills = skills;
            _equipment = equipment;
        }

        //Получаем список экиперовки
        public Equipment GetEquipment()
        {
            return _equipment;
        }

        //Получаем новое снаряжение
        [Inject]
        public void GiveItem(Сhest сhest)
        {
            _equipment.AddItem(сhest.TakeReward());
        }
    }
}