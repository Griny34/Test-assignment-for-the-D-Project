using System.Collections.Generic;
using UnityEngine;
using Zenject;


namespace SolutionZenject
{
    //��������� ��� ������, ������������ �������� �������� � ������
    public interface IPlayer
    {
        public int Health { get; set; }
        public int Lives { get; set; }
        public string Nickname { get; set; }
        public string[] Skills { get; set; }
        public IEquipment Equipment { get; }
    }

    //����� ������, ����������� IPlayer
    public class Player : IPlayer
    {
        private static Player _instance; //��� ����������� ���������
        private readonly IEquipment _equipment; //���������� ���������� ����� DI

        //�������� ������
        public int Health { get; set; }
        public int Lives { get; set; }
        public string Nickname { get; set; }
        public string[] Skills { get; set; }

        //���������� �������� ������ ��� ������
        public IEquipment Equipment => _equipment;

        //����������� ��� ��������� ������������
        public Player(IEquipment equipment)
        {
            if (_instance != null)
            {
                throw new System.InvalidOperationException("������ ���� ��������� Player ����� ������������.");
            }

            _instance = this;
            _equipment = equipment ?? throw new System.ArgumentNullException(nameof(equipment));
        }

        //����������� ����� ��� ��������� ���������
        public static IPlayer Instance
        {
            get
            {
                if (_instance == null)
                {
                    throw new System.InvalidOperationException("��������� Player ������ ���� ������ ����� Zenject.");
                }

                return _instance;
            }
        }
    }

    //��������� ��� ����������
    public interface IEquipment
    {
        public void AddItem(IItem item);
        public IReadOnlyList<IItem> GetItems();
    }

    //���������� ����������
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

    //��������� ��� ��������� ����������
    public interface IItem
    {
        string Name { get; }
    }

    //������� ����� ��� ���������
    public abstract class Item : IItem
    {
        public string Name { get; }

        protected Item(string name)
        {
            Name = name;
        }
    }

    //������
    public class Weapon : Item
    {
        public int Ammo { get; set; }

        public Weapon(string name, int ammo) : base(name)
        {
            Ammo = ammo;
        }
    }

    //�������
    public class Parachute : Item
    {
        public bool IsOpen { get; set; }

        public Parachute(string name, bool isOpen) : base(name)
        {
            IsOpen = isOpen;
        }
    }

    //�������� �����
    public class RocketPack : Item
    {
        public int Charges { get; set; }

        public RocketPack(string name, int charges) : base(name)
        {
            Charges = charges;
        }
    }

    //������� ��� �������� ��������� ����������
    public interface IItemFactory
    {
        IItem CreateWeapon(string name, int ammo);
        IItem CreateParachute(string name, bool isOpen);
        IItem CreateRocketPack(string name, int charges);
    }

    //���������� ������� ���������
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

    //����� ��� ���������� �����
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
            //������������� ������
            _player.Health = 100;
            _player.Lives = 3;
            _player.Nickname = "John";
            _player.Skills = new[] { "Skill1", "Skill2", "Skill3" };

            //������������� ���������� ����� �������
            _player.Equipment.AddItem(_itemFactory.CreateWeapon("��������", 50));
            _player.Equipment.AddItem(_itemFactory.CreateParachute("�������", false));
            _player.Equipment.AddItem(_itemFactory.CreateRocketPack("�������� �����", 3));

            //����� ����������
            Debug.Log($"����� ���������������: {_player.Nickname}, ��������: {_player.Health}, ��������: {_player.Equipment.GetItems().Count}");
        }
    }

    //����� ��� ��������� DI � Unity ����� Zenject
    public class GameBootstrapper : MonoBehaviour
    {
        private void Awake()
        {
            //��������� DI-���������� ����������� ����� ����������
        }
    }

    //���������� ��� Zenject
    public class GameInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            //����������� ������������ ��� ����������
            Container.Bind<IEquipment>().To<Equipment>().AsSingle();
            Container.Bind<IPlayer>().To<Player>().AsSingle();
            Container.Bind<IItemFactory>().To<ItemFactory>().AsSingle();
            Container.Bind<GameManager>().AsSingle();
        }
    }

    //����� ��� ������������ � ��������
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
            //������������� ������
            _gameManager.InitializePlayer();

            //������ ��������� ���������� � ��������
            _player.Equipment.AddItem(_itemFactory.CreateWeapon("��������", 20));
            Debug.Log($"��������� ����� ������. ����� ���������: {_player.Equipment.GetItems().Count}");
        }
    }
}