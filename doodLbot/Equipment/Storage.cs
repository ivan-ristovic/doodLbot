using doodLbot.Entities.CodeElements.ConditionElements;
using doodLbot.Logic;

using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;


namespace doodLbot.Equipment
{
    ///<summary> 
    ///Generic class for shop and storage of items 
    ///</summary>
    public abstract class Storage<T> where T : class, IStorageItem
    {
        public class ShopEntry
        {
            public T Element;
            public int Count;
        }

        public static Entities.CodeElements.BaseCodeElement getCodeElement(T element)
        {
            if (typeof(T).Equals(typeof(Entities.CodeElements.BaseCodeElement)))
                return (Entities.CodeElements.BaseCodeElement)(object)element;
            return new Entities.CodeElements.IdleElement(); //TODO throw error
        }

        [JsonProperty("items")]
        public List<ShopEntry> Items;


        // returns bought element
        public T BuyItem(string name)
        {
            var item = FindItemFromName(name);
            item.Count++;
            return item.Element;
        }

        public T SellItem(string name)
        {
            var item = FindItemFromName(name);
            if (item.Count <= 0)
            {
                throw new System.Exception("Cannot sell when item.count is 0");
            }
            item.Count--;
            return item.Element;
        }

        public bool ItemExists(string name, out int cost)
        {
            try
            {
                var item = FindItemFromName(name);
                cost = item.Element.Cost;
                return true;
            }
            catch
            {
                cost = 0;
                return false;
            }
        }

        public void EquipItem(string name, Entities.CodeElements.BehaviourAlgorithm algorithm)
        {
            try
            {
                var item = FindItemFromName(name).Element;
                algorithm.Insert(getCodeElement(item));
            }
            catch
            {
                ;
            }
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
    }
}
