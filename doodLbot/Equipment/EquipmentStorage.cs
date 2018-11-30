using doodLbot.Entities.CodeElements.ConditionElements;
using doodLbot.Logic;

using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;


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

        public void BuyItem(string name)
        {
            var item = FindItemFromName(name);
            item.Count++;
            // TODO add to hero
        }

        public void SellItem(string name)
        {
            var item = FindItemFromName(name);
            if (item.Count <= 0)
            {
                throw new System.Exception("Cannot sell when item.count is 0");
            }
            item.Count--;
        }

        private ShopEntry FindItemFromName(string name)
        {
            var candidate = Items.Where(e => e.Element.Name == name);
            var count = candidate.Count();
            if (count == 0)
            {
                throw new System.Exception("No such item");
            }
            else if (count > 1)
            {
                throw new System.Exception("Multiple items with same name");
            }

            return candidate.First();
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
