using System.Collections.Generic;
using doodLbot.Entities.CodeElements;
using doodLbot.Entities.CodeElements.ConditionElements;

namespace doodLbot.Equipment
{

    public class CodeStorage : Storage<BaseCodeElement>
    {
        public CodeStorage()
        {
            Items = new List<ShopEntry>
            {
                new ShopEntry { Element = new ShootElement(), Count = 0 },
                new ShopEntry { Element = new BranchingElement(), Count = 0 },
                //new ShopEntry { Element = new IdleElement(), Count = 0 },
                new ShopEntry { Element = new TargetElement(), Count = 0 },
                new ShopEntry { Element = new IsEnemyNearCondition(), Count = 0 },
                new ShopEntry { Element = new MoveAwayFromElement(), Count = 0 }
            };
        }
    }
}
