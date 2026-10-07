using System.Drawing;
using System.Windows.Forms;

namespace MathSymbols;

internal sealed class SymbolGridControl : ScrollableControl
{
    private const int TileWidth = 69;
    private const int TileHeight = 70;
    private const int ColumnPitch = 74;
    private const int RowPitch = 75;
    private static readonly Font GlyphFont = new("Cambria Math", 21F, FontStyle.Regular, GraphicsUnit.Point);
    private static readonly Font NameFont = new("Segoe UI", 7.2F, FontStyle.Regular, GraphicsUnit.Point);
    private static readonly Font BadgeFont = new("Segoe UI Semibold", 8F, FontStyle.Bold, GraphicsUnit.Point);
    private static readonly Brush GlyphBrush = new SolidBrush(Color.White);
    private static readonly Brush TileBrush = new SolidBrush(Color.FromArgb(40, 42, 45));
    private static readonly Brush HoverBrush = new SolidBrush(Color.FromArgb(54, 58, 62));
    private static readonly Brush BadgeBrush = new SolidBrush(Color.FromArgb(85, 195, 239));
    private static readonly Brush BadgeTextBrush = new SolidBrush(Color.FromArgb(20, 31, 36));
    private static readonly Pen SelectionPen = new(Color.FromArgb(85, 195, 239), 1.5F);

    private IReadOnlyList<MathSymbol> _items = Array.Empty<MathSymbol>();
    private int _hovered = -1;
    private int _selected = -1;
    private ToolTip? _toolTip;

    public event Action<MathSymbol>? SymbolSelected;
    public event Action? SearchRequested;

    public SymbolGridControl()
    {
        AutoScroll = true;
        BackColor = Color.FromArgb(29, 30, 32);
        Cursor = Cursors.Hand;
        TabStop = true;
        AccessibleName = "Mathematics symbol results";
        AccessibleRole = AccessibleRole.List;
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.UserPaint | ControlStyles.Selectable, true);
    }

    public void SetItems(IReadOnlyList<MathSymbol> items)
    {
        _items = items;
        _hovered = -1;
        _selected = items.Count == 0 ? -1 : 0;
        AutoScrollPosition = Point.Empty;
        Reflow();
        Invalidate();
    }

    public void SetToolTip(ToolTip toolTip) => _toolTip = toolTip;

    public void InsertSelected()
    {
        if (_selected >= 0 && _selected < _items.Count)
            SymbolSelected?.Invoke(_items[_selected]);
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        Reflow();
    }

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);
        Reflow();
    }

    protected override bool IsInputKey(Keys keyData)
    {
        Keys key = keyData & Keys.KeyCode;
        if (key is Keys.Left or Keys.Right or Keys.Up or Keys.Down or Keys.Home or Keys.End or Keys.PageUp or Keys.PageDown or Keys.Enter or Keys.Space)
            return true;
        return base.IsInputKey(keyData);
    }

    protected override void OnGotFocus(EventArgs e)
    {
        base.OnGotFocus(e);
        if (_selected < 0 && _items.Count > 0) _selected = 0;
        Invalidate();
    }

    protected override void OnLostFocus(EventArgs e)
    {
        base.OnLostFocus(e);
        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        int columns = GetColumnCount();
        if (_items.Count == 0 || columns == 0) return;

        int scrollTop = Math.Max(0, -AutoScrollPosition.Y);
        int firstRow = Math.Max(0, (scrollTop + e.ClipRectangle.Top) / RowPitch);
        int lastRow = Math.Min((_items.Count - 1) / columns, (scrollTop + e.ClipRectangle.Bottom) / RowPitch);
        using var centered = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center, Trimming = StringTrimming.EllipsisCharacter, FormatFlags = StringFormatFlags.NoWrap };

        for (int row = firstRow; row <= lastRow; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                int index = row * columns + column;
                if (index >= _items.Count) break;
                int x = column * ColumnPitch;
                int y = row * RowPitch + AutoScrollPosition.Y;
                var tile = new Rectangle(x, y, TileWidth, TileHeight);
                e.Graphics.FillRectangle(index == _hovered ? HoverBrush : TileBrush, tile);
                if (Focused && index == _selected)
                    e.Graphics.DrawRectangle(SelectionPen, tile.X, tile.Y, tile.Width - 1, tile.Height - 1);
                var glyphRect = new Rectangle(tile.X + 2, tile.Y + 2, tile.Width - 4, 41);
                e.Graphics.DrawString(_items[index].Glyph, GlyphFont, GlyphBrush, glyphRect, centered);
                var nameRect = new Rectangle(tile.X + 2, tile.Y + 43, tile.Width - 4, 24);
                TextRenderer.DrawText(e.Graphics, _items[index].Name, NameFont, nameRect, Color.FromArgb(190, 193, 198), TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPadding);
                if (index < 4)
                {
                    var badge = new Rectangle(tile.X + 3, tile.Y + 3, 17, 17);
                    e.Graphics.FillEllipse(BadgeBrush, badge);
                    e.Graphics.DrawString((index + 1).ToString(), BadgeFont, BadgeTextBrush, badge, centered);
                }
            }
        }
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);
        int hit = HitTest(e.Location);
        if (_hovered != hit)
        {
            _hovered = hit;
            Invalidate();
        }
        if (_toolTip is not null)
        {
            var item = hit >= 0 ? _items[hit] : null;
            _toolTip.SetToolTip(this, item is null ? string.Empty : $"{item.Glyph}  {item.Name}\nSearch terms: {item.SearchTerms}");
        }
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        base.OnMouseLeave(e);
        _hovered = -1;
        Invalidate();
        _toolTip?.SetToolTip(this, string.Empty);
    }

    protected override void OnMouseClick(MouseEventArgs e)
    {
        base.OnMouseClick(e);
        if (e.Button != MouseButtons.Left) return;
        int hit = HitTest(e.Location);
        if (hit >= 0)
        {
            _selected = hit;
            Invalidate();
            SymbolSelected?.Invoke(_items[hit]);
        }
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        if (e.KeyCode == Keys.Up && _selected >= 0 && _selected < GetColumnCount())
        {
            SearchRequested?.Invoke();
            e.Handled = true;
            return;
        }
        if (_items.Count == 0) return;
        if (e.Control)
        {
            int number = e.KeyCode switch
            {
                Keys.D1 or Keys.NumPad1 => 1,
                Keys.D2 or Keys.NumPad2 => 2,
                Keys.D3 or Keys.NumPad3 => 3,
                Keys.D4 or Keys.NumPad4 => 4,
                _ => 0
            };
            if (number > 0 && TryInsertNumberedResult(number))
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
            return;
        }
        switch (e.KeyCode)
        {
            case Keys.Left: MoveSelection(-1); break;
            case Keys.Right: MoveSelection(1); break;
            case Keys.Up: MoveSelection(-GetColumnCount()); break;
            case Keys.Down: MoveSelection(GetColumnCount()); break;
            case Keys.Home: SetSelected(0); break;
            case Keys.End: SetSelected(_items.Count - 1); break;
            case Keys.PageUp: MoveSelection(-GetVisiblePageSize()); break;
            case Keys.PageDown: MoveSelection(GetVisiblePageSize()); break;
            case Keys.Enter:
            case Keys.Space: InsertSelected(); break;
            default: return;
        }
        e.Handled = true;
        e.SuppressKeyPress = true;
    }

    private int HitTest(Point point)
    {
        int columns = GetColumnCount();
        if (columns == 0 || point.X < 0 || point.Y < 0) return -1;
        int contentX = point.X - AutoScrollPosition.X;
        int contentY = point.Y - AutoScrollPosition.Y;
        int col = contentX / ColumnPitch;
        int row = contentY / RowPitch;
        if (col >= columns || contentX % ColumnPitch >= TileWidth || contentY % RowPitch >= TileHeight) return -1;
        int index = row * columns + col;
        return index >= 0 && index < _items.Count ? index : -1;
    }

    private int GetColumnCount() => Math.Max(1, ClientSize.Width / ColumnPitch);

    private int GetVisiblePageSize() => Math.Max(1, ClientSize.Height / RowPitch) * GetColumnCount();

    public bool TryInsertNumberedResult(int number)
    {
        int index = number - 1;
        if (index < 0 || index >= _items.Count) return false;
        _selected = index;
        SymbolSelected?.Invoke(_items[index]);
        return true;
    }

    private void MoveSelection(int delta) => SetSelected(Math.Clamp((_selected < 0 ? 0 : _selected) + delta, 0, _items.Count - 1));

    private void SetSelected(int index)
    {
        if (_items.Count == 0) return;
        _selected = Math.Clamp(index, 0, _items.Count - 1);
        int rowTop = (_selected / GetColumnCount()) * RowPitch;
        int scrollTop = -AutoScrollPosition.Y;
        if (rowTop < scrollTop)
            AutoScrollPosition = new Point(0, rowTop);
        else if (rowTop + TileHeight > scrollTop + ClientSize.Height)
            AutoScrollPosition = new Point(0, Math.Max(0, rowTop + TileHeight - ClientSize.Height));
        Invalidate();
    }

    private void Reflow()
    {
        if (!IsHandleCreated) return;
        int rows = (_items.Count + GetColumnCount() - 1) / GetColumnCount();
        AutoScrollMinSize = new Size(0, rows * RowPitch);
    }
}
