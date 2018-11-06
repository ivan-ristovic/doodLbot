namespace doodLbot.Entities.CodeElements
{
    abstract public class BaseCodeElement
    {
        abstract public bool Execute(Hero hero);

        virtual public void Reset()
        {
            
        }
    }
}
