# SDD — PetForge: taller de mascotas y arena

## 1. Problema

En un videojuego de aventura, los jugadores necesitan crear compañeros que se sientan propios y llevarlos a desafíos. Si cada especie, accesorio y habilidad especial se programa como una combinación aparte, el número de clases crece rápido y cambiar reglas implica tocar el combate y la interfaz. PetForge propone un taller visual para crear mascotas, formar un equipo y probarlo en una arena de entrenamiento.

**Usuarios:** jugadores que crean y personalizan mascotas; el ejemplo sirve también para quien estudia patrones de diseño.

**Necesidad de crear objetos:** producir mascotas de distintas especies con nombre, apariencia y estadísticas; aplicar accesorios y habilidades sin codificar una clase por combinación.

## 2. Alcance

Aplicación Windows Forms con tres áreas: taller de creación, equipo/colección de la sesión y arena de combate por turnos. Incluye especies lobo, zorro y dragón; nombre editable; accesorios ala, armadura y amuleto; una habilidad de ataque; formar un equipo hasta de tres mascotas; combatir un rival de entrenamiento; ganar experiencia y subir nivel. No incluye red, tienda ni cuenta de usuario.

## 3. Requisitos comprobables

- **R1.** La aplicación abre una ventana gráfica con áreas diferenciadas para taller, equipo y arena.
- **R2.** En el taller se elige especie y nombre, y se crea una mascota con estadísticas base propias de su especie.
- **R3.** Se pueden aplicar accesorios opcionales que cambian visualmente la descripción y, cuando corresponde, las estadísticas.
- **R4.** El equipo admite hasta tres mascotas y presenta nombre, especie, nivel y estadísticas.
- **R5.** La arena permite iniciar un combate por turnos contra un rival de entrenamiento y ver el registro de acciones, salud y resultado.
- **R6.** La victoria concede experiencia y permite progresar de nivel.
- **R7.** La lógica de composición, estadísticas, combate y habilidades reside fuera del formulario.
- **R8.** El diseño demuestra Factory Method y Builder como patrones creacionales, más Decorator, Adapter y Composite como estructurales.
- **R9.** README explica alcance, patrones y cómo ejecutar con Windows y .NET.
- **R10.** La interfaz debe usar iconos vectoriales escalables para navegación, especies, accesorios y estadísticas, con un estilo visual coherente.

## 4. Patrones seleccionados

### Creacionales

- **Factory Method:** `PetFactory` y sus fábricas concretas crean el tipo/configuración base (lobo, zorro o dragón). Centraliza las diferencias de especie.
- **Builder:** `PetBuilder` combina el nombre validado, especie y configuración inicial para entregar una mascota lista. Evita constructores largos y la lógica de opciones en la interfaz.

### Estructurales

- **Decorator:** accesorios (`WingsDecorator`, `ArmorDecorator`, `AmuletDecorator`) envuelven una mascota y añaden apariencia o estadísticas sin multiplicar subclases por combinación.
- **Adapter:** `LegacySkillAdapter` adapta una biblioteca de habilidad heredada al contrato de ataque especial que consume el combate.
- **Composite:** `IBattleComponent` define operaciones comunes para una mascota o un equipo. `PetTeam` agrupa mascotas bajo ese mismo contrato, limita miembros y calcula salud y ataque colectivos para arena.

## 5. Diseño propuesto

```text
PetWorkshopForm
 ├── PetBuilder ──> PetFactory (Wolf / Fox / Dragon) ──> Pet
 ├── Decorators (Wings / Armor / Amulet) ───────────────> IPet
 └── PetTeam (Composite) ──> BattleService ──> rival
                          └── LegacySkillAdapter ──> LegacySkillLibrary
```

Clases principales: `IPet`/`Pet`, `PetFactory` y fábricas concretas, `PetBuilder`, decoradores de accesorios, `PetTeam`, `ISpecialAbility` y su adapter, `BattleService`, el formulario Windows Forms y `VectorIcon`, que dibuja iconos escalables con `Graphics` y `GraphicsPath` sin imágenes externas.

## 6. Criterios de aceptación

- **CA1.** La interfaz inicia sin consola y permite navegar visualmente por el taller, equipo y arena.
- **CA2.** Crear especies diferentes produce estadísticas base diferentes y conserva el nombre elegido.
- **CA3.** Marcar un accesorio cambia los detalles de la mascota; armadura y amuleto alteran estadísticas de combate.
- **CA4.** Se agregan hasta tres mascotas al equipo; el cuarto intento se rechaza con una explicación.
- **CA5.** Al iniciar arena se ejecuta un combate visible con turnos y ganador.
- **CA6.** Ganar otorga experiencia, y la mascota puede subir de nivel y mejorar sus estadísticas.
- **CA7.** El código contiene colaboraciones observables de los cinco patrones citados.
- **CA8.** README permite ejecutar el proyecto desde VS Code con `dotnet run`.
- **CA9.** La navegación, las especies, los accesorios y las estadísticas usan iconos vectoriales de un mismo estilo; no dependen de emojis ni de paquetes gráficos externos.

## 7. Supuestos

- Windows con SDK .NET 8 y Windows Forms.
- Batallas locales de demostración, sin persistencia entre ejecuciones.
- Balance numérico simple, orientado a mostrar el flujo y los patrones.
