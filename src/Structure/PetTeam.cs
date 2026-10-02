using PetForge.Domain;

namespace PetForge.Structure;

/// <summary>Composite: trata a una colección de mascotas como un equipo de combate.</summary>
public sealed class PetTeam : IBattleComponent
{
    private readonly List<IPet> _members = new();
    public IReadOnlyList<IPet> Members => _members;
    public int TotalAttack => _members.Where(p => p.Health > 0).Sum(p => p.Attack);
    public string Name => "Equipo PetForge";
    public int Attack => TotalAttack;
    public int Health => _members.Sum(p => p.Health);
    public int MaxHealth => _members.Sum(p => p.MaxHealth);
    public bool IsDefeated => _members.Count == 0 || _members.All(p => p.Health <= 0);

    public bool Add(IPet pet)
    {
        if (_members.Count >= 3 || _members.Contains(pet)) return false;
        _members.Add(pet);
        return true;
    }

    public bool Remove(IPet pet) => _members.Remove(pet);
    public IPet? FirstStanding => _members.FirstOrDefault(p => p.Health > 0);
}
