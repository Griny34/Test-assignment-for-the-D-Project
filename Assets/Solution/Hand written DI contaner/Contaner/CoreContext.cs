using UnityEngine;

namespace SolutionDIcotaner
{
    public class CoreContext : MonoBehaviour
    {
        private IPlayer _player;

        private void Awake()
        {
            var contaner = new DIContaner();  //������ ���������

            _player = new Player(           //������� ������
                new HealthPlayer(100),      //�������� 100
                new LivesPlayer(3),         //����� 3
                new[] { "skill 1" },        //�����
                new Equipment());           //����������

            _player.GetEquipment().AddItem(new Weapon("��������", 50));         //�������� ��������
            _player.GetEquipment().AddItem(new Parachute("�������", false));    //��������� �������
            _player.GetEquipment().AddItem(new RocketPack("������", 3));        //��������� ������

            contaner.Bind<�hest>(Lifetime.Singleton);       //���������� ��� ���� � ������ ������ Chest
            contaner.Bind(_player);                         //����������� Chest � ����� Player � � ������ GiveItem ��������� ������ �� �������

            Debug.Log("������ � ������ ���������� " + _player.GetEquipment().GetCountItep() + "��������");      //� ���������� ����� 4 ��������
                                                                                                                //���������� �������� 3 � 1 �� �������
            contaner.InjectAll();
        }
    }
}

