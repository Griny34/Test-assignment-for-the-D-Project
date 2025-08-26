using UnityEngine;

namespace SolutionDIcotaner
{
    public class Player : IPlayer
    {
        private static Player _instance;

        //��������
        private HealthPlayer _healthPlayer;
        //�����
        private LivesPlayer _livesPlayer;

        //������� �������
        private string[] _skills;

        //����������
        private Equipment _equipment;

        public Player(HealthPlayer healthPlayer, LivesPlayer livesPlayer, string[] skills, Equipment equipment)
        {
            //���� ��� ���� �����, �� ���������� ������
            if (_instance != null)
            {
                throw new System.InvalidOperationException("������ ���� ��������� Player ����� ������������.");
            }

            _healthPlayer = healthPlayer;
            _livesPlayer = livesPlayer;
            _skills = skills;
            _equipment = equipment;
        }

        //�������� ������ ����������
        public Equipment GetEquipment()
        {
            return _equipment;
        }

        //�������� ����� ����������
        [Inject]
        public void GiveItem(�hest �hest)
        {
            _equipment.AddItem(�hest.TakeReward());
        }
    }
}