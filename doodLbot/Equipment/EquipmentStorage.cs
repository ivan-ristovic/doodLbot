using System.Collections.Generic;
using doodLbot.Logic;


namespace doodLbot.Equipment
{
    public class EquipmentStorage : Storage<Gear>
    {
        public EquipmentStorage()
        {
            Items = new List<ShopEntry>();
            foreach(var e in Design.GearDict)
            {
                Items.Add(new ShopEntry{Element = e.Value, Count = 0});
            }
        }
    }
}
