using Game.RuleBook.Character;

namespace Game.Common.Rules
{
    public interface IRuleBook
    {
        List<NameDescription> GetRaces();
        List<NameDescription> GetClasses(string race);
    }
}
