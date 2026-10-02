using PetForge.Domain;

namespace PetForge.Creation;

public enum PetSpecies { Wolf, Fox, Dragon }

public abstract class PetFactory
{
    // Factory Method: cada fábrica concreta define su configuración de especie.
    public abstract Pet Create(string name);
}

public sealed class WolfFactory : PetFactory
{
    public override Pet Create(string name) => new(name, "Lobo", "Lobo", 92, 15, 3);
}

public sealed class FoxFactory : PetFactory
{
    public override Pet Create(string name) => new(name, "Zorro", "Zorro", 76, 18, 1);
}

public sealed class DragonFactory : PetFactory
{
    public override Pet Create(string name) => new(name, "Dragón", "Dragón", 84, 17, 2);
}

/// <summary>Builder para completar opciones de creación y validar el nombre.</summary>
public sealed class PetBuilder
{
    private PetSpecies? _species;
    private string _name = string.Empty;

    public PetBuilder OfSpecies(PetSpecies species) { _species = species; return this; }
    public PetBuilder Named(string name) { _name = name.Trim(); return this; }

    public Pet Build()
    {
        if (string.IsNullOrWhiteSpace(_name)) throw new InvalidOperationException("Escribe un nombre para tu mascota.");
        if (_name.Length > 18) throw new InvalidOperationException("El nombre puede tener hasta 18 caracteres.");
        PetFactory factory = _species switch
        {
            PetSpecies.Wolf => new WolfFactory(),
            PetSpecies.Fox => new FoxFactory(),
            PetSpecies.Dragon => new DragonFactory(),
            _ => throw new InvalidOperationException("Selecciona una especie.")
        };
        return factory.Create(_name);
    }
}
