using doodLbot.Entities.CodeElements.ConditionElements;
using doodLbot.Logic;
using doodLbot.Entities.CodeElements;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace doodLbot.Equipment
{

    public class CodeStorage : Storage<BaseCodeElement>
    {
        public CodeStorage()
        {
            Items = new List<ShopEntry>();
            Items.Add(new ShopEntry { Element = new ShootElement(), Count = 0 });
            Items.Add(new ShopEntry { Element = new BranchingElement(), Count = 0 });
            //Items.Add(new ShopEntry { Element = new IdleElement(), Count = 0 });
            Items.Add(new ShopEntry { Element = new TargetElement(), Count = 0 });
            Items.Add(new ShopEntry { Element = new IsEnemyNearCondition(), Count = 0 });
            Items.Add(new ShopEntry { Element = new MoveAwayFromElement(), Count = 0 });
        }
    }
}
