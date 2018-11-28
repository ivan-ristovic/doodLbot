using doodLbot.Entities.CodeElements.ConditionElements;
using doodLbot.Logic;

using System.Collections.Generic;
using Newtonsoft.Json;



namespace doodLbot.Equipment
{
    public class ShopEntry
    {
        public Gear Element;
        public int Count;
    }

    public class EquipmentStorage
    {
        [JsonProperty("items")]
        public List<ShopEntry> Items;

        public void BuyItem(Gear e)
        {

        }

        public void SellItem(Gear e)
        {

        }

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
