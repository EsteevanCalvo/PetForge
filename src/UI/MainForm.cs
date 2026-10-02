using PetForge.Battle;
using PetForge.Creation;
using PetForge.Domain;
using PetForge.Structure;
using PetForge.UI;

namespace PetForge;

public sealed class MainForm : Form
{
    private readonly List<IPet> _stable = new();
    private readonly PetTeam _team = new();
    private readonly ListBox _stableList = new();
    private readonly ListBox _teamList = new();
    private readonly RichTextBox _battleLog = new();
    private readonly ComboBox _species = new();
    private readonly TextBox _petName = new();
    private readonly CheckBox _wings = new();
    private readonly CheckBox _armor = new();
    private readonly CheckBox _amulet = new();
    private readonly Label _preview = new();
    private readonly Label _workshopStatus = new();
    private readonly Label _teamStatus = new();
    private readonly Label _arenaStatus = new();
    private readonly Button _fightButton = new();
    private readonly Button _cancelButton = new();
    private readonly Panel _contentHost = new();
    private readonly VectorIcon _previewCreature = new();
    private readonly Label _healthValue = new();
    private readonly Label _attackValue = new();
    private readonly Label _defenseValue = new();
    private Panel _workshopPage = new();
    private Panel _stablePage = new();
    private Panel _arenaPage = new();
    private NavigationButton _workshopNav = null!;
    private NavigationButton _stableNav = null!;
    private NavigationButton _arenaNav = null!;
    private CancellationTokenSource? _battleCancellation;

    public MainForm()
    {
        Text = "PETFORGE  |  Taller & Arena";
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(940, 680);
        Size = new Size(1120, 800);
        BackColor = Color.FromArgb(15, 20, 31);
        ForeColor = Color.White;
        Font = new Font("Segoe UI", 10);

        var root = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(24, 18, 24, 22), RowCount = 3, ColumnCount = 1, BackColor = BackColor };
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 78));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 66));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        Controls.Add(root);
        root.Controls.Add(BuildHeader(), 0, 0);
        root.Controls.Add(BuildNavigation(), 0, 1);

        _contentHost.Dock = DockStyle.Fill;
        _contentHost.BackColor = BackColor;
        _workshopPage = BuildWorkshopPage();
        _stablePage = BuildStablePage();
        _arenaPage = BuildArenaPage();
        _contentHost.Controls.AddRange(new Control[] { _workshopPage, _stablePage, _arenaPage });
        root.Controls.Add(_contentHost, 0, 2);
        ShowPage(_workshopPage, _workshopNav);

        _species.Items.AddRange(new object[] { "Lobo", "Zorro", "Dragón" });
        _species.SelectedIndex = 0;
        _petName.Text = "Chispa";
        _species.SelectedIndexChanged += (_, _) => UpdatePreview();
        _petName.TextChanged += (_, _) => UpdatePreview();
        _wings.CheckedChanged += (_, _) => UpdatePreview();
        _armor.CheckedChanged += (_, _) => UpdatePreview();
        _amulet.CheckedChanged += (_, _) => UpdatePreview();
        RefreshLists();
        UpdatePreview();
    }

    private Control BuildHeader()
    {
        var panel = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1, BackColor = BackColor };
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 58));
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        var mark = new VectorIcon { Kind = VectorIconKind.Paw, IconColor = Color.FromArgb(126, 231, 194), Size = new Size(42, 42), Anchor = AnchorStyles.None };
        panel.Controls.Add(mark, 0, 0);
        var brand = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 2, ColumnCount = 1, BackColor = BackColor };
        brand.RowStyles.Add(new RowStyle(SizeType.Percent, 60));
        brand.RowStyles.Add(new RowStyle(SizeType.Percent, 40));
        brand.Controls.Add(new Label { Text = "PETFORGE", Dock = DockStyle.Fill, Font = new Font("Segoe UI", 22, FontStyle.Bold), ForeColor = Color.FromArgb(126, 231, 194), TextAlign = ContentAlignment.BottomLeft });
        brand.Controls.Add(new Label { Text = "TALLER DE CRIATURAS  /  ARENA DE ENTRENAMIENTO", Dock = DockStyle.Fill, ForeColor = Color.FromArgb(139, 156, 180), Font = new Font("Segoe UI", 8, FontStyle.Bold), TextAlign = ContentAlignment.TopLeft });
        panel.Controls.Add(brand, 1, 0);
        return panel;
    }

    private Control BuildNavigation()
    {
        var bar = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight, WrapContents = false, Padding = new Padding(0, 5, 0, 7), BackColor = BackColor };
        _workshopNav = new NavigationButton("TALLER", VectorIconKind.Workshop);
        _stableNav = new NavigationButton("COLECCIÓN", VectorIconKind.Collection);
        _arenaNav = new NavigationButton("ARENA", VectorIconKind.Arena);
        _workshopNav.Click += (_, _) => ShowPage(_workshopPage, _workshopNav);
        _stableNav.Click += (_, _) => ShowPage(_stablePage, _stableNav);
        _arenaNav.Click += (_, _) => ShowPage(_arenaPage, _arenaNav);
        bar.Controls.AddRange(new Control[] { _workshopNav, _stableNav, _arenaNav });
        return bar;
    }

    private void ShowPage(Panel page, NavigationButton active)
    {
        _workshopPage.Visible = page == _workshopPage;
        _stablePage.Visible = page == _stablePage;
        _arenaPage.Visible = page == _arenaPage;
        _workshopNav.Selected = active == _workshopNav;
        _stableNav.Selected = active == _stableNav;
        _arenaNav.Selected = active == _arenaNav;
        _workshopNav.Invalidate();
        _stableNav.Invalidate();
        _arenaNav.Invalidate();
        page.BringToFront();
    }

    private Panel BuildWorkshopPage()
    {
        var page = MakePage();
        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(24), ColumnCount = 2, RowCount = 1, BackColor = page.BackColor };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 53));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 47));
        page.Controls.Add(layout);

        var form = new TableLayoutPanel { Dock = DockStyle.Top, ColumnCount = 1, RowCount = 9, AutoSize = true, Padding = new Padding(0, 4, 12, 0) };
        form.RowStyles.Add(new RowStyle(SizeType.Absolute, 54));
        for (int row = 1; row < form.RowCount; row++)
            form.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        form.Controls.Add(SectionTitle("Diseña tu mascota"));
        _species.DropDownStyle = ComboBoxStyle.DropDownList;
        _species.FlatStyle = FlatStyle.Flat;
        _species.BackColor = Color.FromArgb(31, 42, 59);
        _species.ForeColor = Color.White;
        _species.Dock = DockStyle.Fill;
        _petName.Dock = DockStyle.Fill;
        _petName.MaxLength = 18;
        _petName.BackColor = Color.FromArgb(31, 42, 59);
        _petName.ForeColor = Color.White;
        _petName.BorderStyle = BorderStyle.FixedSingle;
        form.Controls.Add(Labeled("Especie", _species));
        form.Controls.Add(Labeled("Nombre", _petName));
        form.Controls.Add(SectionTitle("Accesorios"));
        ConfigureCheck(_wings, "Alas  ·  +2 ataque");
        ConfigureCheck(_armor, "Armadura  ·  +3 defensa");
        ConfigureCheck(_amulet, "Amuleto  ·  +10 salud máxima");
        form.Controls.Add(AccessoryRow(_wings, VectorIconKind.Wings));
        form.Controls.Add(AccessoryRow(_armor, VectorIconKind.Armor));
        form.Controls.Add(AccessoryRow(_amulet, VectorIconKind.Amulet));
        var create = ActionButton("CREAR MASCOTA");
        create.Click += (_, _) => CreatePet();
        form.Controls.Add(create);
        _workshopStatus.ForeColor = Color.FromArgb(126, 231, 194);
        form.Controls.Add(_workshopStatus);
        layout.Controls.Add(form, 0, 0);

        var card = new Panel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(25, 34, 50), Padding = new Padding(22), Margin = new Padding(8, 6, 4, 6) };
        var cardLayout = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 5, ColumnCount = 1 };
        cardLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 35));
        cardLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 40));
        cardLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 58));
        cardLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 76));
        cardLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 60));
        cardLayout.Controls.Add(new Label { Text = "VISTA PREVIA", Dock = DockStyle.Fill, ForeColor = Color.FromArgb(126, 231, 194), Font = new Font("Segoe UI", 9, FontStyle.Bold), TextAlign = ContentAlignment.MiddleCenter });
        _previewCreature.Size = new Size(92, 92);
        _previewCreature.Anchor = AnchorStyles.None;
        _previewCreature.IconColor = Color.FromArgb(126, 231, 194);
        cardLayout.Controls.Add(_previewCreature, 0, 1);
        _preview.Dock = DockStyle.Fill;
        _preview.TextAlign = ContentAlignment.MiddleCenter;
        _preview.Font = new Font("Segoe UI", 14, FontStyle.Bold);
        _preview.ForeColor = Color.White;
        cardLayout.Controls.Add(_preview, 0, 2);
        cardLayout.Controls.Add(BuildStatStrip(), 0, 3);
        cardLayout.Controls.Add(new Label { Text = "Cada especie tiene fortalezas distintas.\nLos accesorios cambian sus atributos en combate.", Dock = DockStyle.Fill, TextAlign = ContentAlignment.TopCenter, ForeColor = Color.FromArgb(139, 156, 180) }, 0, 4);
        card.Controls.Add(cardLayout);
        layout.Controls.Add(card, 1, 0);
        return page;
    }

    private Panel BuildStablePage()
    {
        var page = MakePage();
        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(22, 20, 22, 20), ColumnCount = 3, RowCount = 3 };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 44));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 190));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 56));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 45));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 45));
        page.Controls.Add(layout);

        layout.Controls.Add(SectionHeading("ESTABLO", VectorIconKind.Collection), 0, 0);
        layout.Controls.Add(SectionHeading("EQUIPO DE ARENA", VectorIconKind.Arena), 2, 0);
        ConfigureList(_stableList);
        ConfigureList(_teamList);
        layout.Controls.Add(_stableList, 0, 1);
        layout.Controls.Add(_teamList, 2, 1);
        var actions = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, WrapContents = false, Padding = new Padding(5, 50, 5, 0) };
        var add = ActionButton("AÑADIR AL EQUIPO");
        add.Click += (_, _) => AddToTeam();
        var remove = ActionButton("QUITAR");
        remove.Click += (_, _) => RemoveFromTeam();
        actions.Controls.Add(add);
        actions.Controls.Add(remove);
        layout.Controls.Add(actions, 1, 1);
        _teamStatus.Dock = DockStyle.Fill;
        _teamStatus.TextAlign = ContentAlignment.MiddleLeft;
        _teamStatus.ForeColor = Color.FromArgb(177, 190, 210);
        layout.Controls.Add(_teamStatus, 0, 2);
        layout.SetColumnSpan(_teamStatus, 3);
        return page;
    }

    private Panel BuildArenaPage()
    {
        var page = MakePage();
        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(24), ColumnCount = 1, RowCount = 3 };
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 70));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 55));
        page.Controls.Add(layout);

        var top = new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = false, FlowDirection = FlowDirection.LeftToRight, Padding = new Padding(0, 8, 0, 0) };
        _fightButton.Text = "INICIAR COMBATE";
        _fightButton.Size = new Size(205, 42);
        StyleButton(_fightButton, true);
        _fightButton.Click += async (_, _) => await StartFightAsync();
        _cancelButton.Text = "CANCELAR";
        _cancelButton.Size = new Size(120, 42);
        StyleButton(_cancelButton, false);
        _cancelButton.Enabled = false;
        _cancelButton.Click += (_, _) => _battleCancellation?.Cancel();
        top.Controls.Add(_fightButton);
        top.Controls.Add(_cancelButton);
        top.Controls.Add(new Label { Text = "   EQUIPO vs. GÓLEM   ·   RECOMPENSA: 35 XP", AutoSize = true, ForeColor = Color.FromArgb(139, 156, 180), Font = new Font("Segoe UI", 8, FontStyle.Bold), Padding = new Padding(0, 14, 0, 0) });
        layout.Controls.Add(top, 0, 0);

        _battleLog.Dock = DockStyle.Fill;
        _battleLog.ReadOnly = true;
        _battleLog.BackColor = Color.FromArgb(10, 14, 22);
        _battleLog.ForeColor = Color.FromArgb(213, 225, 238);
        _battleLog.BorderStyle = BorderStyle.None;
        _battleLog.Font = new Font("Consolas", 10);
        layout.Controls.Add(_battleLog, 0, 1);
        _arenaStatus.Dock = DockStyle.Fill;
        _arenaStatus.ForeColor = Color.FromArgb(126, 231, 194);
        layout.Controls.Add(_arenaStatus, 0, 2);
        return page;
    }

    private void CreatePet()
    {
        try
        {
            var species = _species.SelectedIndex switch { 0 => PetSpecies.Wolf, 1 => PetSpecies.Fox, 2 => PetSpecies.Dragon, _ => throw new InvalidOperationException("Selecciona una especie.") };
            IPet pet = new PetBuilder().OfSpecies(species).Named(_petName.Text).Build();
            if (_wings.Checked) pet = new WingsDecorator(pet);
            if (_armor.Checked) pet = new ArmorDecorator(pet);
            if (_amulet.Checked) pet = new AmuletDecorator(pet);
            _stable.Add(pet);
            RefreshLists();
            _workshopStatus.Text = $"{pet.Name} ya está en tu establo. Ve a Colección para sumarlo al equipo.";
        }
        catch (InvalidOperationException ex) { _workshopStatus.Text = ex.Message; }
    }

    private void AddToTeam()
    {
        if (_stableList.SelectedItem is not IPet pet) { _teamStatus.Text = "Selecciona una mascota del establo."; return; }
        if (!_team.Add(pet)) { _teamStatus.Text = _team.Members.Count >= 3 ? "El equipo está completo (máximo 3)." : "Esa mascota ya pertenece al equipo."; return; }
        RefreshLists();
    }

    private void RemoveFromTeam()
    {
        if (_teamList.SelectedItem is IPet pet) _team.Remove(pet);
        RefreshLists();
    }

    private async Task StartFightAsync()
    {
        if (_team.Members.Count == 0) { _arenaStatus.Text = "Agrega al menos una mascota al equipo."; return; }
        _battleLog.Clear();
        foreach (IPet pet in _team.Members) pet.Heal(pet.MaxHealth);
        _fightButton.Enabled = false;
        _cancelButton.Enabled = true;
        _battleCancellation = new CancellationTokenSource();
        _arenaStatus.Text = "Combate en curso…";
        var progress = new Progress<string>(line => { _battleLog.AppendText(line + Environment.NewLine); _battleLog.ScrollToCaret(); });
        try
        {
            var service = new BattleService(new LegacySkillAdapter(new LegacySkillLibrary()));
            bool won = await service.FightAsync(_team, progress, _battleCancellation.Token);
            _arenaStatus.Text = won ? "VICTORIA  ·  Experiencia añadida a tu equipo." : "DERROTA  ·  Recupera fuerzas y vuelve a intentarlo.";
        }
        catch (OperationCanceledException) { _arenaStatus.Text = "Combate cancelado."; _battleLog.AppendText("Combate cancelado por el jugador." + Environment.NewLine); }
        finally
        {
            _fightButton.Enabled = true;
            _cancelButton.Enabled = false;
            _battleCancellation.Dispose();
            _battleCancellation = null;
            RefreshLists();
        }
    }

    private void UpdatePreview()
    {
        int health = _species.SelectedIndex switch { 0 => 92, 1 => 76, 2 => 84, _ => 0 } + (_amulet.Checked ? 10 : 0);
        int attack = _species.SelectedIndex switch { 0 => 15, 1 => 18, 2 => 17, _ => 0 } + (_wings.Checked ? 2 : 0);
        int defense = (_species.SelectedIndex switch { 0 => 3, 1 => 1, 2 => 2, _ => 0 }) + (_armor.Checked ? 3 : 0);
        _previewCreature.Kind = _species.SelectedIndex switch { 0 => VectorIconKind.Wolf, 1 => VectorIconKind.Fox, 2 => VectorIconKind.Dragon, _ => VectorIconKind.Paw };
        _previewCreature.IconColor = _species.SelectedIndex switch
        {
            0 => Color.FromArgb(126, 231, 194),
            1 => Color.FromArgb(255, 177, 108),
            2 => Color.FromArgb(152, 172, 255),
            _ => Color.White
        };
        _previewCreature.Invalidate();
        string species = _species.SelectedItem?.ToString() ?? "Mascota";
        string name = _petName.Text.Trim().Length == 0 ? "Tu mascota" : _petName.Text.Trim();
        _preview.Text = $"{name}\n{species}";
        _healthValue.Text = health.ToString();
        _attackValue.Text = attack.ToString();
        _defenseValue.Text = defense.ToString();
    }

    private void RefreshLists()
    {
        _stableList.DataSource = null;
        _stableList.DataSource = _stable.ToList();
        _teamList.DataSource = null;
        _teamList.DataSource = _team.Members.ToList();
        _teamStatus.Text = $"Establo: {_stable.Count} mascota(s)     Equipo: {_team.Members.Count}/3     Salud: {_team.Health}/{_team.MaxHealth}     ATQ: {_team.Attack}";
        _fightButton.Enabled = _team.Members.Count > 0 && _battleCancellation is null;
    }

    private static Panel MakePage() => new() { Dock = DockStyle.Fill, BackColor = Color.FromArgb(20, 27, 41), ForeColor = Color.White };
    private static Label SectionTitle(string text) => new() { Text = text, Dock = DockStyle.Fill, ForeColor = Color.FromArgb(126, 231, 194), Font = new Font("Segoe UI", 11, FontStyle.Bold), TextAlign = ContentAlignment.MiddleLeft };
    private static Control SectionHeading(string text, VectorIconKind iconKind)
    {
        var row = new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = false, FlowDirection = FlowDirection.LeftToRight, Padding = new Padding(0, 5, 0, 0) };
        row.Controls.Add(new VectorIcon { Kind = iconKind, IconColor = Color.FromArgb(126, 231, 194), Size = new Size(24, 24), Margin = new Padding(0, 2, 8, 0) });
        row.Controls.Add(new Label { Text = text, AutoSize = true, ForeColor = Color.White, Font = new Font("Segoe UI", 10, FontStyle.Bold), Padding = new Padding(0, 4, 0, 0) });
        return row;
    }

    private Control AccessoryRow(CheckBox checkbox, VectorIconKind iconKind)
    {
        var row = new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = false, FlowDirection = FlowDirection.LeftToRight, Padding = new Padding(3, 0, 0, 0), Margin = new Padding(0, 3, 0, 3) };
        row.Controls.Add(new VectorIcon { Kind = iconKind, IconColor = Color.FromArgb(177, 190, 210), Size = new Size(25, 25), Margin = new Padding(0, 3, 8, 0) });
        row.Controls.Add(checkbox);
        return row;
    }

    private Control BuildStatStrip()
    {
        var strip = new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = false, FlowDirection = FlowDirection.LeftToRight, Padding = new Padding(1, 3, 0, 0) };
        strip.Controls.Add(MetricTile(VectorIconKind.Health, "SALUD", _healthValue, Color.FromArgb(255, 130, 150)));
        strip.Controls.Add(MetricTile(VectorIconKind.Attack, "ATAQUE", _attackValue, Color.FromArgb(255, 177, 108)));
        strip.Controls.Add(MetricTile(VectorIconKind.Defense, "DEFENSA", _defenseValue, Color.FromArgb(139, 173, 255)));
        return strip;
    }

    private static Control MetricTile(VectorIconKind iconKind, string caption, Label value, Color accent)
    {
        var tile = new Panel { Size = new Size(102, 64), BackColor = Color.FromArgb(34, 46, 65), Margin = new Padding(0, 0, 5, 0) };
        tile.Controls.Add(new VectorIcon { Kind = iconKind, IconColor = accent, Size = new Size(23, 23), Location = new Point(8, 20) });
        value.Location = new Point(36, 7);
        value.Size = new Size(60, 25);
        value.ForeColor = Color.White;
        value.Font = new Font("Segoe UI", 12, FontStyle.Bold);
        value.TextAlign = ContentAlignment.MiddleLeft;
        var label = new Label { Text = caption, Location = new Point(37, 33), Size = new Size(62, 19), ForeColor = Color.FromArgb(139, 156, 180), Font = new Font("Segoe UI", 7, FontStyle.Bold), TextAlign = ContentAlignment.MiddleLeft };
        tile.Controls.Add(value);
        tile.Controls.Add(label);
        return tile;
    }

    private static Control Labeled(string title, Control input)
    {
        var panel = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1, Margin = new Padding(0, 6, 0, 6) };
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 32));
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 68));
        panel.Controls.Add(new Label { Text = title, Dock = DockStyle.Fill, ForeColor = Color.White, TextAlign = ContentAlignment.MiddleLeft });
        panel.Controls.Add(input, 1, 0);
        return panel;
    }
    private static void ConfigureCheck(CheckBox box, string text) { box.Text = text; box.AutoSize = true; box.ForeColor = Color.White; box.Margin = new Padding(3, 8, 3, 8); }
    private static void ConfigureList(ListBox list)
    {
        list.Dock = DockStyle.Fill;
        list.BackColor = Color.FromArgb(28, 37, 54);
        list.ForeColor = Color.White;
        list.BorderStyle = BorderStyle.None;
        list.IntegralHeight = false;
        list.DrawMode = DrawMode.OwnerDrawFixed;
        list.ItemHeight = 70;
        list.Font = new Font("Segoe UI", 10);
        list.DrawItem += (_, e) =>
        {
            if (e.Index < 0 || e.Index >= list.Items.Count) return;
            bool selected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;
            Color background = selected ? Color.FromArgb(48, 76, 76) : Color.FromArgb(28, 37, 54);
            using var backgroundBrush = new SolidBrush(background);
            e.Graphics.FillRectangle(backgroundBrush, e.Bounds);
            if (list.Items[e.Index] is IPet pet)
            {
                VectorIconKind kind = PetIcon(pet.Species);
                Color accent = PetAccent(pet.Species);
                VectorIcon.Render(e.Graphics, kind, accent, new Rectangle(e.Bounds.X + 12, e.Bounds.Y + 18, 34, 34));
                using var nameFont = new Font("Segoe UI", 10, FontStyle.Bold);
                using var detailFont = new Font("Segoe UI", 8, FontStyle.Regular);
                using var white = new SolidBrush(Color.White);
                using var muted = new SolidBrush(Color.FromArgb(163, 180, 201));
                e.Graphics.DrawString($"{pet.Name}   Nv. {pet.Level}", nameFont, white, e.Bounds.X + 58, e.Bounds.Y + 13);
                string details = $"{pet.Species} · {pet.Health}/{pet.MaxHealth} HP · ATQ {pet.Attack} · DEF {pet.Defense}";
                e.Graphics.DrawString(details, detailFont, muted, new RectangleF(e.Bounds.X + 58, e.Bounds.Y + 37, e.Bounds.Width - 65, 28));
            }
            if ((e.State & DrawItemState.Focus) == DrawItemState.Focus) e.DrawFocusRectangle();
        };
    }

    private static VectorIconKind PetIcon(string species) => species switch
    {
        "Lobo" => VectorIconKind.Wolf,
        "Zorro" => VectorIconKind.Fox,
        "Dragón" => VectorIconKind.Dragon,
        _ => VectorIconKind.Paw
    };

    private static Color PetAccent(string species) => species switch
    {
        "Lobo" => Color.FromArgb(126, 231, 194),
        "Zorro" => Color.FromArgb(255, 177, 108),
        "Dragón" => Color.FromArgb(152, 172, 255),
        _ => Color.White
    };
    private static Button ActionButton(string text)
    {
        var button = new Button { Text = text, Size = new Size(180, 42), FlatStyle = FlatStyle.Flat };
        StyleButton(button, true);
        return button;
    }
    private static void StyleButton(Button button, bool primary)
    {
        button.BackColor = primary ? Color.FromArgb(39, 183, 142) : Color.FromArgb(48, 60, 80);
        button.ForeColor = Color.White;
        button.FlatAppearance.BorderSize = 0;
        button.FlatAppearance.MouseOverBackColor = primary ? Color.FromArgb(53, 204, 160) : Color.FromArgb(62, 76, 98);
        button.FlatAppearance.MouseDownBackColor = primary ? Color.FromArgb(31, 156, 120) : Color.FromArgb(39, 50, 69);
        button.Font = new Font("Segoe UI", 9, FontStyle.Bold);
        button.Cursor = Cursors.Hand;
    }
}
