using System.Collections.Generic;
using UnityEngine;
using Zenject;


namespace SolutionZenject
{
    //Интерфейс для игрока, определяющий основные свойства и методы
    public interface IPlayer
    {
        public int Health { get; set; }
        public int Lives { get; set; }
        public string Nickname { get; set; }
        public string[] Skills { get; set; }
        public IEquipment Equipment { get; }
    }

    //Класс игрока, реализующий IPlayer
    public class Player : IPlayer
    {
        private static Player _instance; //Для обеспечения синглтона
        private readonly IEquipment _equipment; //Экипировка внедряется через DI

        //Свойства игрока
        public int Health { get; set; }
        public int Lives { get; set; }
        public string Nickname { get; set; }
        public string[] Skills { get; set; }

        //Экипировка доступна только для чтения
        public IEquipment Equipment => _equipment;

        //Конструктор для внедрения зависимостей
        public Player(IEquipment equipment)
        {
            if (_instance != null)
            {
                throw new System.InvalidOperationException("Только один экземпляр Player может существовать.");
            }

            _instance = this;
            _equipment = equipment ?? throw new System.ArgumentNullException(nameof(equipment));
        }

        //Статический метод для получения синглтона
        public static IPlayer Instance
        {
            get
            {
                if (_instance == null)
                {
                    throw new System.InvalidOperationException("Экземпляр Player должен быть создан через Zenject.");
                }

                return _instance;
            }
        }
    }

    //Интерфейс для экипировки
    public interface IEquipment
    {
        public void AddItem(IItem item);
        public IReadOnlyList<IItem> GetItems();
    }

    //Реализация экипировки
    public class Equipment : IEquipment
    {
        private readonly List<IItem> _items = new List<IItem>();

        public void AddItem(IItem item)
        {
            _items.Add(item);
        }

        public IReadOnlyList<IItem> GetItems()
        {
            return _items.AsReadOnly();
        }
    }

    //Интерфейс для предметов экипировки
    public interface IItem
    {
        string Name { get; }
    }

    //Базовый класс для предметов
    public abstract class Item : IItem
    {
        public string Name { get; }

        protected Item(string name)
        {
            Name = name;
        }
    }

    //Оружие
    public class Weapon : Item
    {
        public int Ammo { get; set; }

        public Weapon(string name, int ammo) : base(name)
        {
            Ammo = ammo;
        }
    }

    //Парашют
    public class Parachute : Item
    {
        public bool IsOpen { get; set; }

        public Parachute(string name, bool isOpen) : base(name)
        {
            IsOpen = isOpen;
        }
    }

    //Ракетный ранец
    public class RocketPack : Item
    {
        public int Charges { get; set; }

        public RocketPack(string name, int charges) : base(name)
        {
            Charges = charges;
        }
    }

    //Фабрика для создания предметов экипировки
    public interface IItemFactory
    {
        IItem CreateWeapon(string name, int ammo);
        IItem CreateParachute(string name, bool isOpen);
        IItem CreateRocketPack(string name, int charges);
    }

    //Реализация фабрики предметов
    public class ItemFactory : IItemFactory
    {
        public IItem CreateWeapon(string name, int ammo)
        {
            return new Weapon(name, ammo);
        }

        public IItem CreateParachute(string name, bool isOpen)
        {
            return new Parachute(name, isOpen);
        }

        public IItem CreateRocketPack(string name, int charges)
        {
            return new RocketPack(name, charges);
        }
    }

    //Класс для управления игрой
    public class GameManager
    {
        private readonly IPlayer _player;
        private readonly IItemFactory _itemFactory;

        public GameManager(IPlayer player, IItemFactory itemFactory)
        {
            _player = player;
            _itemFactory = itemFactory;
        }

        public void InitializePlayer()
        {
            //Инициализация игрока
            _player.Health = 100;
            _player.Lives = 3;
            _player.Nickname = "John";
            _player.Skills = new[] { "Skill1", "Skill2", "Skill3" };

            //Инициализация экипировки через фабрику
            _player.Equipment.AddItem(_itemFactory.CreateWeapon("Винтовка", 50));
            _player.Equipment.AddItem(_itemFactory.CreateParachute("Парашют", false));
            _player.Equipment.AddItem(_itemFactory.CreateRocketPack("Ракетный ранец", 3));

            //Вывод информации
            Debug.Log($"Игрок инициализирован: {_player.Nickname}, Здоровье: {_player.Health}, Предметы: {_player.Equipment.GetItems().Count}");
        }
    }

    //Класс для настройки DI в Unity через Zenject
    public class GameBootstrapper : MonoBehaviour
    {
        private void Awake()
        {
            //Настройка DI-контейнера выполняется через установщик
        }
    }

    //Установщик для Zenject
    public class GameInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            //Регистрация зависимостей как синглтонов
            Container.Bind<IEquipment>().To<Equipment>().AsSingle();
            Container.Bind<IPlayer>().To<Player>().AsSingle();
            Container.Bind<IItemFactory>().To<ItemFactory>().AsSingle();
            Container.Bind<GameManager>().AsSingle();
        }
    }

    //Класс для тестирования в рантайме
    public class RuntimeTester : MonoBehaviour
    {
        [Inject]
        private GameManager _gameManager;

        [Inject]
        private IPlayer _player;

        [Inject]
        private IItemFactory _itemFactory;

        private void Start()
        {
            //Инициализация игрока
            _gameManager.InitializePlayer();

            //Пример изменения экипировки в рантайме
            _player.Equipment.AddItem(_itemFactory.CreateWeapon("Дробовик", 20));
            Debug.Log($"Добавлено новое оружие. Всего предметов: {_player.Equipment.GetItems().Count}");
        }
    }
}