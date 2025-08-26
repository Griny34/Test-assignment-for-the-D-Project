using System.Collections.Generic;

namespace SolutionDIcotaner
{
    public class Equipment : IEquipment
    {
        //C����� Item � ����� ����������
        List<Item> _items = new List<Item>();

        public void AddItem(Item item)
        {
            _items.Add(item);
        }

        public int GetCountItep()
        {
            return _items.Count;
        }
    }
}

