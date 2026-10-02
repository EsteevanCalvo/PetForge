# PetForge — taller de mascotas y arena

PetForge es una aplicación de escritorio Windows Forms para crear compañeros de aventura, equiparlos, formar un grupo y probarlo en una batalla por turnos. Las victorias otorgan experiencia y permiten subir de nivel.

La interfaz incluye iconos vectoriales dibujados con `Graphics`/`GraphicsPath` de Windows Forms. Se escalan nítidamente sin depender de emojis, archivos de imagen o paquetes externos.

## Problema

En un videojuego, cada especie, accesorio y habilidad aporta una variación a la mascota. Crear una clase por cada combinación vuelve difícil ampliar el juego y equilibrar sus reglas. PetForge separa la creación de especies, los accesorios y el combate para que se puedan combinar de manera independiente.

## Patrones de diseño

- **Factory Method:** `PetFactory` y sus fábricas concretas configuran los valores base de lobo, zorro y dragón.
- **Builder:** `PetBuilder` recibe la especie y el nombre, valida los datos y produce la mascota.
- **Decorator:** `WingsDecorator`, `ArmorDecorator` y `AmuletDecorator` agregan apariencia y mejoras sin crear una clase para cada combinación.
- **Adapter:** `LegacySkillAdapter` adapta una biblioteca ficticia de habilidades al contrato que espera el combate.
- **Composite:** `IBattleComponent` permite consultar a una mascota o a `PetTeam` con el mismo contrato; el equipo administra hasta tres integrantes y agrega sus estadísticas.

## Cómo jugar

1. En **Taller**, elige una especie, escribe un nombre y marca accesorios opcionales.
2. En **Colección y equipo**, selecciona mascotas del establo y añádelas al equipo (máximo tres).
3. En **Arena**, inicia el combate contra el Gólem. Sigue los turnos en el registro; una victoria concede 35 XP a cada integrante.

Las partidas viven en memoria y se reinician al cerrar la aplicación. El proyecto no requiere conexión ni servicios externos.

## Especificación

La SDD con problema, requisitos comprobables, diseño y criterios de aceptación está en [`openspec/SDD-petforge.md`](openspec/SDD-petforge.md).

## Ejecutar en VS Code

Requisitos: Windows y SDK de .NET 8 o compatible con `net8.0-windows`.

Abre la terminal integrada en la carpeta del proyecto y ejecuta:

```powershell
cd src
dotnet run
```

Para compilar sin iniciar la ventana:

```powershell
dotnet build
```

## Estructura

```text
openspec/SDD-petforge.md       Especificación escrita antes de implementar
src/PetForge.csproj           Proyecto Windows Forms
src/Creation/                 Factory Method y Builder
src/Domain/                   Estado y reglas de mascota
src/Structure/                Decorator y Composite
src/Battle/                   Combate y Adapter de habilidad
src/UI/MainForm.cs            Taller, colección y arena
```
