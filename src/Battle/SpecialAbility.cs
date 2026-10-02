using PetForge.Domain;

namespace PetForge.Battle;

public interface ISpecialAbility
{
    string Name { get; }
    int Use(IPet pet);
}

/// <summary>API heredada de una biblioteca externa simulada.</summary>
public sealed class LegacySkillLibrary
{
    public int Cast(string skillCode, int power) => skillCode switch
    {
        "EMBER" => power + 12,
        _ => power
    };
}

/// <summary>Adapter: traduce la habilidad moderna al método y código de la biblioteca heredada.</summary>
public sealed class LegacySkillAdapter(LegacySkillLibrary library) : ISpecialAbility
{
    public string Name => "Ráfaga elemental";
    public int Use(IPet pet) => library.Cast("EMBER", pet.Attack);
}

public sealed class BattleService(ISpecialAbility ability)
{
    public async Task<bool> FightAsync(PetForge.Structure.PetTeam team, IProgress<string> log, CancellationToken cancellationToken = default)
    {
        int enemyHealth = 78;
        const int enemyAttack = 11;
        log.Report("¡Un Gólem de práctica aparece con 78 de salud!");
        foreach (IPet pet in team.Members) pet.Heal(pet.MaxHealth);
        bool usedSpecial = false;
        int turn = 0;

        while (enemyHealth > 0 && !team.IsDefeated && turn < 14)
        {
            cancellationToken.ThrowIfCancellationRequested();
            turn++;
            foreach (IPet pet in team.Members.Where(p => p.Health > 0).ToList())
            {
                int damage = pet.Attack;
                string attackName = "muerde";
                if (!usedSpecial)
                {
                    damage = ability.Use(pet);
                    attackName = ability.Name;
                    usedSpecial = true;
                }
                enemyHealth = Math.Max(0, enemyHealth - damage);
                log.Report($"Turno {turn}: {pet.Name} usa {attackName} e inflige {damage}. Gólem: {enemyHealth}/78.");
                await Task.Delay(250, cancellationToken);
                if (enemyHealth == 0) break;
            }
            if (enemyHealth > 0)
            {
                IPet? target = team.FirstStanding;
                if (target is not null)
                {
                    target.TakeDamage(Math.Max(1, enemyAttack - target.Defense));
                    log.Report($"El Gólem golpea a {target.Name}. Salud: {target.Health}/{target.MaxHealth}.");
                    await Task.Delay(250, cancellationToken);
                }
            }
        }

        bool victory = enemyHealth == 0;
        if (victory)
        {
            foreach (IPet pet in team.Members)
            {
                bool leveled = pet.GainExperience(35);
                log.Report($"{pet.Name} gana 35 XP{(leveled ? $" y sube al nivel {pet.Level}" : $" ({pet.Experience} XP) ")}.");
            }
            log.Report("¡Victoria! El equipo ha superado el entrenamiento.");
        }
        else log.Report("Derrota. Descansa y vuelve a intentarlo.");
        return victory;
    }
}
