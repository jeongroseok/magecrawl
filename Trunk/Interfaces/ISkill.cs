namespace Magecrawl.Interfaces
{
    public interface ISkill
    {
        string Name { get; }
        int Cost { get; }
        string School { get; }
        string Description { get; }
        bool NewSpell { get; }
    }
}
