namespace doodLbot.Entities.CodeElements
{
    abstract public class BaseCodeElement
    {
        abstract public void Execute(Hero hero);

        virtual public void Reset()
        {
            
        }
    }
}
