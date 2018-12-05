using doodLbot.Entities.CodeElements.ConditionElements;
using doodLbot.Logic;

using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;


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
