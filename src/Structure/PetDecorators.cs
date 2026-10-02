using PetForge.Domain;

namespace PetForge.Structure;

/// <summary>Decorator base: conserva la interfaz y delega el comportamiento a la mascota.</summary>
public abstract class PetDecorator : IPet
{
    protected readonly IPet Inner;
    protected PetDecorator(IPet inner) => Inner = inner;
    public virtual string Name => Inner.Name;
    public virtual string Species => Inner.Species;
    public virtual string Appearance => Inner.Appearance;
    public virtual int Level => Inner.Level;
    public virtual int MaxHealth => Inner.MaxHealth;
    public virtual int Health => Inner.Health;
    public virtual int Attack => Inner.Attack;
    public virtual int Defense => Inner.Defense;
    public virtual int Experience => Inner.Experience;
    public virtual void TakeDamage(int amount) => Inner.TakeDamage(amount);
    public virtual void Heal(int amount) => Inner.Heal(amount);
    public virtual bool GainExperience(int amount) => Inner.GainExperience(amount);
    public override string ToString() => $"{Appearance}  {Name} · {Species} · Nv. {Level} · ATQ {Attack} · DEF {Defense}";
}

public sealed class WingsDecorator(IPet inner) : PetDecorator(inner)
{
    public override string Appearance => $"{Inner.Appearance}, alas";
    public override int Attack => Inner.Attack + 2;
}

public sealed class ArmorDecorator(IPet inner) : PetDecorator(inner)
{
    public override string Appearance => $"{Inner.Appearance}, armadura";
    public override int Defense => Inner.Defense + 3;
}

public sealed class AmuletDecorator : PetDecorator
{
    private int _bonusHealth = 10;

    public AmuletDecorator(IPet inner) : base(inner) { }
    public override string Appearance => $"{Inner.Appearance}, amuleto";
    public override int MaxHealth => Inner.MaxHealth + 10;
    public override int Health => Inner.Health + _bonusHealth;

    public override void TakeDamage(int amount)
    {
        int absorbed = Math.Min(_bonusHealth, Math.Max(1, amount));
        _bonusHealth -= absorbed;
        int remaining = amount - absorbed;
        if (remaining > 0) Inner.TakeDamage(remaining);
    }

    public override void Heal(int amount)
    {
        int recoveredBonus = Math.Min(10 - _bonusHealth, amount);
        _bonusHealth += recoveredBonus;
        Inner.Heal(amount - recoveredBonus);
    }
}
