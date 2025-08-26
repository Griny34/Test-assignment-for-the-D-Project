using System.Collections.Generic;
using System;

namespace PlayerSingleton
{
    class Program
    {
        static void Main(string[] args)
        {
            Player player = Player.Instance; //����������������� ������ Player

            //�������������� ������
            player.Health = 100;
            player.Lives = 3;
            player.Nickname = "John";
            player.Skills = new string[] { "Skill1", "Skill2", "Skill3" };
            player.Equipment = new Equipment();

            Console.WriteLine("��������������: " + player.Health);
            Console.WriteLine("�������������: " + player.Nickname);

            Equipment equipment = player.Equipment;
            equipment.AddItem(new Weapon("��������", 50));
            equipment.AddItem(new Parachute(false));
            equipment.AddItem(new RocketPack(3)); //�������� ������ 3 ��������

            Console.ReadKey();
        }
    }

    //����� ������
    public class Player
    {
        private static Player _instance;
        public int Health { get; set; }
        public int Lives { get; set; }
        public string Nickname { get; set; }

        //����� ���������
        public string[] Skills { get; set; }

        //����������
        public Equipment Equipment { get; set; }

        public Player()
        {
            _instance = this;
        }

        public static Player Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Player();
                }
                return _instance;
            }
        }
    }

    //��������� ��� ����������
    interface IEquipment
    {
        void AddItem(Item item);
    }

    //���������� ����������
    public class Equipment : IEquipment
    {
        List<Item> items = new List<Item>();

        public void AddItem(Item item)
        {
            items.Add(item);
        }
    }

    //������� ����������
    public abstract class Item
    {
        protected string name;

        public Item(string name)
        {
            this.name = name;
        }
    }

    //������
    public class Weapon : Item
    {
        int ammo;

        public Weapon(string name, int ammo) : base(name)
        {
            this.ammo = ammo;
        }
    }

    //�������
    public class Parachute : Item
    {
        bool isOpen;

        public Parachute(bool isOpen) : base("Parachute")
        {
            this.isOpen = isOpen;
        }
    }

    //�������� �����
    public class RocketPack : Item
    {
        int charges;

        public RocketPack(int charges) : base("RocketPack")
        {
            this.charges = charges;
        }
    }
}