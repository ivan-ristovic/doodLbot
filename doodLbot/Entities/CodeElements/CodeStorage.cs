using doodLbot.Entities.CodeElements.ConditionElements;
using doodLbot.Logic;

using System.Collections.Generic;
using Newtonsoft.Json;



namespace doodLbot.Entities.CodeElements
{
    public class ShopEntry
    {
        public BaseCodeElement Element;
        public int Count;
    }

    public class CodeStorage
    {
        [JsonProperty("items")]
        public List<ShopEntry> Items;

        public void BuyItem(BaseCodeElement e)
        {

        }

        public void SellItem(BaseCodeElement e)
        {

        }

        public CodeStorage()
        {
            Items = new List<ShopEntry>();
            Items.Add(new ShopEntry{Element = new ShootElement(), Count = 0});
            Items.Add(new ShopEntry{Element = new BranchingElement(), Count = 0});
            Items.Add(new ShopEntry{Element = new IdleElement(), Count = 0});
            Items.Add(new ShopEntry{Element = new TargetElement(), Count = 0});
            Items.Add(new ShopEntry{Element = new IsEnemyNearCondition(), Count = 0});
        }
    }
}
