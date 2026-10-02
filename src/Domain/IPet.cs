namespace PetForge.Domain;

/// <summary>Contrato común de una mascota individual y de un equipo (Composite).</summary>
public interface IBattleComponent
{
    string Name { get; }
    int Health { get; }
    int MaxHealth { get; }
    int Attack { get; }
}

public interface IPet : IBattleComponent
{
    string Name { get; }
    string Species { get; }
    string Appearance { get; }
    int Level { get; }
    int MaxHealth { get; }
    int Health { get; }
    int Attack { get; }
    int Defense { get; }
    int Experience { get; }
    void TakeDamage(int amount);
    void Heal(int amount);
    bool GainExperience(int amount);
}

public sealed class Pet : IPet
{
    public string Name { get; }
    public string Species { get; }
    public string Appearance { get; }
    public int Level { get; private set; } = 1;
    public int MaxHealth { get; private set; }
    public int Health { get; private set; }
    public int Attack { get; private set; }
    public int Defense { get; private set; }
    public int Experience { get; private set; }

    public Pet(string name, string species, string appearance, int health, int attack, int defense)
    {
        Name = name;
        Species = species;
        Appearance = appearance;
        MaxHealth = health;
        Health = health;
        Attack = attack;
        Defense = defense;
    }

    public void TakeDamage(int amount) => Health = Math.Max(0, Health - Math.Max(1, amount));
    public void Heal(int amount) => Health = Math.Min(MaxHealth, Health + amount);

    public bool GainExperience(int amount)
    {
        Experience += amount;
        if (Experience < Level * 50) return false;
        Experience -= Level * 50;
        Level++;
        MaxHealth += 8;
        Attack++;
        Health = MaxHealth;
        return true;
    }

    public override string ToString() => $"{Appearance}  {Name} · {Species} · Nv. {Level} · ATQ {Attack} · DEF {Defense}";
}
