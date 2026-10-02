using System.Drawing.Drawing2D;

namespace PetForge.UI;

public enum VectorIconKind
{
    Paw, Workshop, Collection, Arena, Health, Attack, Defense,
    Wings, Armor, Amulet, Wolf, Fox, Dragon
}

/// <summary>Icono vectorial dibujado con GDI+: conserva nitidez al cambiar de tamaño.</summary>
public sealed class VectorIcon : Control
{
    public VectorIconKind Kind { get; set; }
    public Color IconColor { get; set; } = Color.FromArgb(126, 231, 194);

    public VectorIcon()
    {
        SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                 ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw |
                 ControlStyles.SupportsTransparentBackColor, true);
        BackColor = Color.Transparent;
        Size = new Size(32, 32);
        TabStop = false;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        if (Width <= 0 || Height <= 0) return;
        Render(e.Graphics, Kind, IconColor, new Rectangle(0, 0, Width, Height));
    }

    public static void Render(Graphics g, VectorIconKind kind, Color color, Rectangle bounds)
    {
        if (bounds.Width <= 0 || bounds.Height <= 0) return;
        GraphicsState state = g.Save();
        g.TranslateTransform(bounds.X, bounds.Y);
        g.ScaleTransform(bounds.Width / 32f, bounds.Height / 32f);
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.PixelOffsetMode = PixelOffsetMode.HighQuality;
        using var pen = new Pen(color, 2.2f) { StartCap = LineCap.Round, EndCap = LineCap.Round, LineJoin = LineJoin.Round };
        using var fill = new SolidBrush(color);

        switch (kind)
        {
            case VectorIconKind.Paw: DrawPaw(g, fill); break;
            case VectorIconKind.Workshop: DrawWorkshop(g, pen); break;
            case VectorIconKind.Collection: DrawCollection(g, pen); break;
            case VectorIconKind.Arena: DrawArena(g, pen); break;
            case VectorIconKind.Health: DrawHeart(g, pen); break;
            case VectorIconKind.Attack: DrawSword(g, pen); break;
            case VectorIconKind.Defense:
            case VectorIconKind.Armor: DrawShield(g, pen); break;
            case VectorIconKind.Wings: DrawWings(g, pen); break;
            case VectorIconKind.Amulet: DrawAmulet(g, pen); break;
            case VectorIconKind.Wolf: DrawCreature(g, pen, dragon: false, fox: false); break;
            case VectorIconKind.Fox: DrawCreature(g, pen, dragon: false, fox: true); break;
            case VectorIconKind.Dragon: DrawCreature(g, pen, dragon: true, fox: false); break;
        }
        g.Restore(state);
    }

    private static void DrawPaw(Graphics g, Brush fill)
    {
        g.FillEllipse(fill, 10, 14, 12, 11);
        g.FillEllipse(fill, 5, 8, 6, 7);
        g.FillEllipse(fill, 12, 4, 6, 7);
        g.FillEllipse(fill, 20, 7, 6, 7);
        g.FillEllipse(fill, 24, 13, 5, 7);
    }

    private static void DrawWorkshop(Graphics g, Pen p)
    {
        g.DrawLine(p, 8, 24, 22, 10);
        g.DrawLine(p, 19, 7, 25, 13);
        g.DrawLine(p, 7, 25, 10, 27);
        g.DrawArc(p, 17, 3, 12, 12, 205, 210);
        g.DrawLine(p, 5, 27, 8, 24);
    }

    private static void DrawCollection(Graphics g, Pen p)
    {
        g.DrawEllipse(p, 4, 6, 10, 10);
        g.DrawEllipse(p, 18, 6, 10, 10);
        g.DrawArc(p, 2, 17, 14, 13, 190, 160);
        g.DrawArc(p, 16, 17, 14, 13, 190, 160);
        g.DrawLine(p, 11, 21, 21, 21);
    }

    private static void DrawArena(Graphics g, Pen p)
    {
        g.DrawLine(p, 7, 7, 24, 24);
        g.DrawLine(p, 5, 12, 12, 5);
        g.DrawLine(p, 4, 15, 15, 4);
        g.DrawLine(p, 8, 8, 5, 11);
        g.DrawLine(p, 25, 7, 8, 24);
        g.DrawLine(p, 27, 12, 20, 5);
        g.DrawLine(p, 28, 15, 17, 4);
        g.DrawLine(p, 24, 8, 27, 11);
        g.DrawLine(p, 5, 26, 8, 23);
        g.DrawLine(p, 24, 23, 27, 26);
    }

    private static void DrawHeart(Graphics g, Pen p)
    {
        using var path = new GraphicsPath();
        path.AddBezier(16, 27, 13, 24, 3, 17, 3, 11);
        path.AddBezier(3, 11, 3, 5, 11, 3, 16, 10);
        path.AddBezier(16, 10, 21, 3, 29, 5, 29, 11);
        path.AddBezier(29, 11, 29, 17, 19, 24, 16, 27);
        g.DrawPath(p, path);
    }

    private static void DrawSword(Graphics g, Pen p)
    {
        g.DrawLine(p, 8, 25, 24, 7);
        g.DrawLine(p, 19, 7, 25, 6);
        g.DrawLine(p, 24, 7, 25, 13);
        g.DrawLine(p, 5, 21, 12, 27);
        g.DrawLine(p, 7, 18, 14, 24);
        g.DrawLine(p, 5, 25, 9, 21);
    }

    private static void DrawShield(Graphics g, Pen p)
    {
        using var path = new GraphicsPath();
        path.AddPolygon(new[] { new PointF(16, 3), new PointF(27, 7), new PointF(26, 18), new PointF(16, 28), new PointF(6, 18), new PointF(5, 7) });
        g.DrawPath(p, path);
        g.DrawLine(p, 16, 7, 16, 24);
        g.DrawLine(p, 10, 14, 16, 18);
        g.DrawLine(p, 22, 14, 16, 18);
    }

    private static void DrawWings(Graphics g, Pen p)
    {
        g.DrawBezier(p, 16, 26, 11, 16, 4, 13, 3, 5);
        g.DrawBezier(p, 16, 26, 21, 16, 28, 13, 29, 5);
        g.DrawLine(p, 5, 9, 13, 15);
        g.DrawLine(p, 7, 16, 14, 20);
        g.DrawLine(p, 27, 9, 19, 15);
        g.DrawLine(p, 25, 16, 18, 20);
    }

    private static void DrawAmulet(Graphics g, Pen p)
    {
        g.DrawArc(p, 9, 2, 14, 19, 180, 180);
        using var path = new GraphicsPath();
        path.AddPolygon(new[] { new PointF(16, 13), new PointF(24, 21), new PointF(16, 29), new PointF(8, 21) });
        g.DrawPath(p, path);
        g.DrawLine(p, 12, 21, 20, 21);
    }

    private static void DrawCreature(Graphics g, Pen p, bool dragon, bool fox)
    {
        using var ears = new GraphicsPath();
        if (dragon)
            ears.AddPolygon(new[] { new PointF(7, 14), new PointF(8, 3), new PointF(14, 9), new PointF(18, 9), new PointF(24, 3), new PointF(25, 14) });
        else
            ears.AddPolygon(new[] { new PointF(5, 15), new PointF(7, fox ? 2 : 5), new PointF(14, 10), new PointF(18, 10), new PointF(25, fox ? 2 : 5), new PointF(27, 15) });
        g.DrawPath(p, ears);
        g.DrawEllipse(p, 6, 9, 20, 20);
        g.DrawEllipse(p, 11, 17, 2, 2);
        g.DrawEllipse(p, 19, 17, 2, 2);
        g.DrawArc(p, 13, 19, 6, 5, 20, 140);
        if (dragon)
        {
            g.DrawLine(p, 11, 10, 8, 5);
            g.DrawLine(p, 21, 10, 24, 5);
        }
        if (fox)
        {
            g.DrawLine(p, 12, 22, 16, 25);
            g.DrawLine(p, 20, 22, 16, 25);
        }
    }
}

public sealed class NavigationButton : Control
{
    public VectorIconKind IconKind { get; set; }
    public bool Selected { get; set; }
    private bool _hovered;

    public NavigationButton(string text, VectorIconKind iconKind)
    {
        Text = text;
        IconKind = iconKind;
        Size = new Size(176, 50);
        Font = new Font("Segoe UI", 9, FontStyle.Bold);
        Cursor = Cursors.Hand;
        AccessibleRole = AccessibleRole.PageTab;
        AccessibleName = text;
        TabStop = true;
        SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                 ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw |
                 ControlStyles.Selectable, true);
    }

    protected override void OnMouseEnter(EventArgs e) { base.OnMouseEnter(e); _hovered = true; Invalidate(); }
    protected override void OnMouseLeave(EventArgs e) { base.OnMouseLeave(e); _hovered = false; Invalidate(); }
    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        if (e.KeyCode is Keys.Enter or Keys.Space)
        {
            OnClick(EventArgs.Empty);
            e.Handled = true;
        }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        Graphics g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        Color mint = Color.FromArgb(126, 231, 194);
        Color bg = Selected ? Color.FromArgb(35, 67, 63) : (_hovered ? Color.FromArgb(35, 45, 62) : Color.FromArgb(24, 31, 45));
        Color fg = Selected ? mint : Color.FromArgb(177, 190, 210);
        using var path = RoundedRectangle(new Rectangle(0, 0, Width - 1, Height - 1), 10);
        using var brush = new SolidBrush(bg);
        using var border = new Pen(Selected ? Color.FromArgb(61, 139, 119) : bg, 1);
        g.FillPath(brush, path);
        g.DrawPath(border, path);
        DrawInlineIcon(g, IconKind, fg, new Rectangle(15, 9, 30, 30));
        TextRenderer.DrawText(g, Text, Font, new Rectangle(54, 0, Width - 62, Height), fg,
            TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.EndEllipsis);
    }

    private static GraphicsPath RoundedRectangle(Rectangle bounds, int radius)
    {
        var path = new GraphicsPath();
        int d = radius * 2;
        path.AddArc(bounds.X, bounds.Y, d, d, 180, 90);
        path.AddArc(bounds.Right - d, bounds.Y, d, d, 270, 90);
        path.AddArc(bounds.Right - d, bounds.Bottom - d, d, d, 0, 90);
        path.AddArc(bounds.X, bounds.Bottom - d, d, d, 90, 90);
        path.CloseFigure();
        return path;
    }

    private static void DrawInlineIcon(Graphics g, VectorIconKind kind, Color color, Rectangle rect)
    {
        VectorIcon.Render(g, kind, color, rect);
    }
}
