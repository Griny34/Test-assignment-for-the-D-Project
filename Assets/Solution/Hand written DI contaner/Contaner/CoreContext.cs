using UnityEngine;

namespace SolutionDIcotaner
{
    public class CoreContext : MonoBehaviour
    {
        private IPlayer _player;

        private void Awake()
        {
            var contaner = new DIContaner();  //Создаём контейнер

            _player = new Player(           //Создаем игрока
                new HealthPlayer(100),      //здоровье 100
                new LivesPlayer(3),         //жизни 3
                new[] { "skill 1" },        //скилы
                new Equipment());           //экиперовка

            _player.GetEquipment().AddItem(new Weapon("Дробовик", 50));         //обавляем дробовик
            _player.GetEquipment().AddItem(new Parachute("Парашут", false));    //добавляем парашют
            _player.GetEquipment().AddItem(new RocketPack("Ракеты", 3));        //добавляем ракеты

            contaner.Bind<Сhest>(Lifetime.Singleton);       //Записываем все поля и методы класса Chest
            contaner.Bind(_player);                         //Прокидываем Chest в класс Player и в методе GiveItem добавляем пулемёт из сундука

            Debug.Log("Сейчас в списке экиперовки " + _player.GetEquipment().GetCountItep() + "предмета");      //В экиперовке будет 4 предмета
                                                                                                                //изначально довавили 3 и 1 из сундука
            contaner.InjectAll();
        }
    }
}

