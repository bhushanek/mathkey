using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Text.Json;
using Microsoft.Win32;
using System.Windows.Forms;

namespace MathSymbols;

internal sealed class MainForm : Form
{
    private const int HotkeyId = 0x4D53;
    private const int WmHotkey = 0x0312;
    private const uint ModAlt = 0x0001, ModControl = 0x0002, ModShift = 0x0004, ModWin = 0x0008, ModNoRepeat = 0x4000;
    private static readonly Color Surface = Color.FromArgb(29, 30, 32);
    private static readonly Color Card = Color.FromArgb(40, 42, 45);
    private static readonly Color Muted = Color.FromArgb(159, 163, 168);
    private static readonly Color Accent = Color.FromArgb(85, 195, 239);
    private static readonly string SettingsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MathSymbols", "settings.json");
    private static readonly string RunKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Run";

    private AppSettings _settings;
    private readonly NotifyIcon _tray;
    private readonly ToolTip _toolTip = new();
    private readonly TextBox _search = new();
    private readonly FlowLayoutPanel _categories = new();
    private readonly SymbolGridControl _tiles = new();
    private readonly Panel _resultsViewport = new();
    private readonly Label _resultCount = new();
    private readonly Panel _pickerPanel = new();
    private readonly Panel _settingsPanel = new();
    private readonly Label _emptyState = new();
    private readonly Label _shortcutValue = new();
    private readonly Label _captureHint = new();
    private readonly Button _captureButton = new();
    private readonly Button _saveButton = new();
    private readonly CheckBox _startupCheck = new();
    private readonly System.Windows.Forms.Timer _insertTimer = new() { Interval = 120 };
    private readonly List<Button> _categoryButtons = new();
    private bool _capturingShortcut;
    private string _selectedCategory = "All";
    private nint _targetWindow;
    private bool _hotkeyRegistered;
    private Region? _windowRegion;
    private Keys _draftModifiers;
    private Keys _draftKey;
    private Label? _footerShortcut;

    public MainForm()
    {
        _settings = LoadSettings();
        _draftModifiers = _settings.Modifiers;
        _draftKey = _settings.Key;
        Text = "MathKey";
        ClientSize = new Size(406, 570);
        MinimumSize = new Size(360, 450);
        MaximumSize = new Size(600, 900);
        FormBorderStyle = FormBorderStyle.None;
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.Manual;
        BackColor = Surface;
        ForeColor = Color.White;
        Font = new Font("Segoe UI", 9F);
        KeyPreview = true;
        Opacity = 0;
        DoubleBuffered = true;
        BuildPicker();
        BuildSettings();
        Controls.Add(_settingsPanel);
        Controls.Add(_pickerPanel);
        _settingsPanel.Visible = false;
        _pickerPanel.Visible = true;

        _insertTimer.Tick += (_, _) =>
        {
            _insertTimer.Stop();
            if (_targetWindow != 0 && IsWindow(_targetWindow)) SetForegroundWindow(_targetWindow);
            SendUnicodeText(_pendingInsert);
            _pendingInsert = string.Empty;
        };

        var menu = new ContextMenuStrip();
        menu.Items.Add("Open symbol picker", null, (_, _) => ShowPickerFromTray());
        menu.Items.Add("Settings", null, (_, _) => ShowSettingsFromTray());
        menu.Items.Add(new ToolStripSeparator());
        menu.Items.Add("Quit", null, (_, _) => ExitApplication());
        _tray = new NotifyIcon
        {
            Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath) ?? SystemIcons.Application,
            Text = "MathKey",
            ContextMenuStrip = menu,
            Visible = true
        };
        _tray.DoubleClick += (_, _) => ShowPickerFromTray();
        _tray.BalloonTipTitle = "MathKey is ready";
        _tray.BalloonTipText = $"Press {FormatShortcut(_settings.Modifiers, _settings.Key)} to open the symbol picker.";

        Shown += (_, _) =>
        {
            Hide();
            Opacity = 1;
            if (!_hotkeyRegistered)
                _tray.ShowBalloonTip(3500);
        };
        Deactivate += (_, _) =>
        {
            if (Visible) BeginInvoke(new Action(Hide));
        };
        FormClosing += (_, e) =>
        {
            if (!_allowExit)
            {
                e.Cancel = true;
                Hide();
            }
        };
    }

    private bool _allowExit;
    private string _pendingInsert = string.Empty;

    protected override CreateParams CreateParams
    {
        get
        {
            var cp = base.CreateParams;
            cp.ClassStyle |= 0x00020000; // CS_DROPSHADOW
            return cp;
        }
    }

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);
        if (ClientSize.Width < 2 || ClientSize.Height < 2) return;
        const int radius = 18;
        using var path = new GraphicsPath();
        path.AddArc(0, 0, radius, radius, 180, 90);
        path.AddArc(ClientSize.Width - radius, 0, radius, radius, 270, 90);
        path.AddArc(ClientSize.Width - radius, ClientSize.Height - radius, radius, radius, 0, 90);
        path.AddArc(0, ClientSize.Height - radius, radius, radius, 90, 90);
        path.CloseFigure();
        var updated = new Region(path);
        Region = updated;
        _windowRegion?.Dispose();
        _windowRegion = updated;
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        RegisterCurrentHotkey();
    }

    protected override void OnHandleDestroyed(EventArgs e)
    {
        if (_hotkeyRegistered)
        {
            UnregisterHotKey(Handle, HotkeyId);
            _hotkeyRegistered = false;
        }
        base.OnHandleDestroyed(e);
    }

    protected override void WndProc(ref Message m)
    {
        if (m.Msg == WmHotkey && m.WParam.ToInt32() == HotkeyId)
        {
            if (Visible) Hide();
            else ShowPicker();
            return;
        }
        base.WndProc(ref m);
    }

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        if (!_capturingShortcut && Visible && _pickerPanel.Visible && (keyData & Keys.Control) != 0 && (keyData & (Keys.Alt | Keys.Shift)) == 0)
        {
            int number = (keyData & Keys.KeyCode) switch
            {
                Keys.D1 or Keys.NumPad1 => 1,
                Keys.D2 or Keys.NumPad2 => 2,
                Keys.D3 or Keys.NumPad3 => 3,
                Keys.D4 or Keys.NumPad4 => 4,
                _ => 0
            };
            if (number > 0 && _tiles.TryInsertNumberedResult(number)) return true;
        }
        if (!_capturingShortcut && Visible && _pickerPanel.Visible && keyData == (Keys.Control | Keys.F))
        {
            _search.Focus();
            _search.SelectAll();
            return true;
        }
        if (_capturingShortcut)
        {
            Keys key = keyData & Keys.KeyCode;
            if (key == Keys.Escape)
            {
                _capturingShortcut = false;
                _captureButton.Text = "Change shortcut";
                _captureHint.Text = "Shortcut capture cancelled.";
                _saveButton.Enabled = true;
                RegisterCurrentHotkey();
                return true;
            }
            if (key is Keys.ControlKey or Keys.ShiftKey or Keys.Menu or Keys.LWin or Keys.RWin) return true;

            Keys modifiers = keyData & Keys.Modifiers;
            if ((modifiers & (Keys.Control | Keys.Alt)) == 0)
            {
                _captureHint.Text = "Include Ctrl or Alt so the shortcut works in any app.";
                return true;
            }
            _draftKey = key;
            _draftModifiers = modifiers;
            _capturingShortcut = false;
            _shortcutValue.Text = FormatShortcut(_draftModifiers, _draftKey);
            _captureButton.Text = "Change shortcut";
            _captureHint.Text = "Save to activate this shortcut.";
            _saveButton.Enabled = true;
            return true;
        }

        if (keyData == Keys.Escape && Visible)
        {
            Hide();
            return true;
        }
        return base.ProcessCmdKey(ref msg, keyData);
    }

    private void BuildPicker()
    {
        _pickerPanel.Dock = DockStyle.Fill;
        _pickerPanel.Padding = new Padding(14, 10, 14, 10);
        _pickerPanel.BackColor = Surface;

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 6,
            BackColor = Surface,
            Margin = Padding.Empty
        };
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 43));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 61));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 32));
        _pickerPanel.Controls.Add(layout);

        var header = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 3, RowCount = 1, Margin = Padding.Empty };
        header.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 36));
        header.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        header.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 68));
        var logo = new Label { Text = "∑", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft, ForeColor = Accent, Font = new Font("Cambria Math", 23, FontStyle.Bold) };
        var title = new Label { Text = "MathKey", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft, Font = new Font("Segoe UI Semibold", 12, FontStyle.Bold) };
        var headerActions = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft, WrapContents = false, Margin = Padding.Empty, Padding = new Padding(0, 6, 0, 0) };
        var close = SmallButton("×", "Close picker");
        close.Click += (_, _) => Hide();
        var settings = SmallButton("⚙", "Settings");
        settings.Click += (_, _) => SetSettingsVisible(true);
        headerActions.Controls.Add(close); headerActions.Controls.Add(settings);
        header.Controls.Add(logo, 0, 0); header.Controls.Add(title, 1, 0); header.Controls.Add(headerActions, 2, 0);
        layout.Controls.Add(header, 0, 0);

        var searchHost = new Panel { Dock = DockStyle.Fill, BackColor = Card, Padding = new Padding(11, 0, 8, 0), Margin = new Padding(0, 2, 0, 3) };
        var magnifier = new Label { Text = "⌕", Dock = DockStyle.Left, Width = 25, TextAlign = ContentAlignment.MiddleCenter, ForeColor = Muted, Font = new Font("Segoe UI", 19) };
        _search.BorderStyle = BorderStyle.None;
        _search.Dock = DockStyle.Fill;
        _search.BackColor = Card;
        _search.ForeColor = Color.White;
        _search.Font = new Font("Segoe UI", 10.5F);
        _search.PlaceholderText = "Search by name (e.g. double integral)";
        _search.TextChanged += (_, _) => RefreshSymbols();
        _search.KeyDown += (_, e) =>
        {
            int quickNumber = e.KeyCode switch
            {
                Keys.D1 or Keys.NumPad1 => 1,
                Keys.D2 or Keys.NumPad2 => 2,
                Keys.D3 or Keys.NumPad3 => 3,
                Keys.D4 or Keys.NumPad4 => 4,
                _ => 0
            };
            if (e.Control && quickNumber > 0 && _tiles.TryInsertNumberedResult(quickNumber))
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == Keys.Down)
            {
                _tiles.Focus();
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == Keys.Enter)
            {
                _tiles.InsertSelected();
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        };
        searchHost.Controls.Add(_search); searchHost.Controls.Add(magnifier);
        layout.Controls.Add(searchHost, 0, 1);

        _categories.Dock = DockStyle.Fill;
        _categories.WrapContents = true;
        _categories.FlowDirection = FlowDirection.LeftToRight;
        _categories.Margin = new Padding(0, 2, 0, 1);
        _categories.Padding = new Padding(0, 2, 0, 0);
        _categories.BackColor = Surface;
        foreach (string category in new[] { "All", "Calculus", "Algebra", "Relations", "Geometry", "Sets & Logic", "Greek", "Arrows", "Other" })
        {
            var button = new Button
            {
                Text = category,
                Tag = category,
                AutoSize = true,
                Height = 25,
                FlatStyle = FlatStyle.Flat,
                BackColor = category == "All" ? Color.FromArgb(34, 82, 102) : Card,
                ForeColor = category == "All" ? Accent : Color.FromArgb(214, 216, 219),
                Font = new Font("Segoe UI", 8.1F),
                Padding = new Padding(8, 0, 8, 0),
                Margin = new Padding(0, 0, 5, 5),
                Cursor = Cursors.Hand
            };
            button.FlatAppearance.BorderSize = 0;
            button.Click += (_, _) => SelectCategory(category);
            _categoryButtons.Add(button);
            _categories.Controls.Add(button);
        }
        layout.Controls.Add(_categories, 0, 2);

        var resultsTitle = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, Margin = Padding.Empty };
        resultsTitle.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        resultsTitle.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        _resultCount.Dock = DockStyle.Fill;
        _resultCount.TextAlign = ContentAlignment.MiddleLeft;
        _resultCount.ForeColor = Muted;
        _resultCount.Font = new Font("Segoe UI", 8.5F);
        var hint = new Label { Text = "Ctrl+1–4 select · ↓ results · Enter inserts", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleRight, ForeColor = Color.FromArgb(145, 150, 155), Font = new Font("Segoe UI", 8) };
        resultsTitle.Controls.Add(_resultCount, 0, 0); resultsTitle.Controls.Add(hint, 1, 0);
        layout.Controls.Add(resultsTitle, 0, 3);

        _resultsViewport.Dock = DockStyle.Fill;
        _resultsViewport.AutoScroll = false;
        _resultsViewport.BackColor = Surface;
        _resultsViewport.Padding = new Padding(0, 3, 2, 2);
        _tiles.Dock = DockStyle.Fill;
        _tiles.BackColor = Surface;
        _tiles.SetToolTip(_toolTip);
        _tiles.SymbolSelected += item => InsertSymbol(item.Glyph);
        _tiles.SearchRequested += () => { _search.Focus(); _search.SelectAll(); };
        _resultsViewport.Controls.Add(_tiles);
        _emptyState.Text = "No matching symbols. Try another name or category.";
        _emptyState.ForeColor = Muted;
        _emptyState.Dock = DockStyle.Fill;
        _emptyState.Height = 80;
        _emptyState.TextAlign = ContentAlignment.MiddleCenter;
        _emptyState.Visible = false;
        _resultsViewport.Controls.Add(_emptyState);
        layout.Controls.Add(_resultsViewport, 0, 4);

        var footer = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, Margin = Padding.Empty };
        footer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100)); footer.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        var footerText = new Label { Text = "Works in the app you're typing in", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft, ForeColor = Color.FromArgb(125, 129, 134), Font = new Font("Segoe UI", 8) };
        _footerShortcut = new Label { Text = FormatShortcut(_settings.Modifiers, _settings.Key), Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleRight, ForeColor = Accent, Font = new Font("Segoe UI", 8.2F) };
        footer.Controls.Add(footerText, 0, 0); footer.Controls.Add(_footerShortcut, 1, 0);
        layout.Controls.Add(footer, 0, 5);
        RefreshSymbols();
    }

    private void BuildSettings()
    {
        _settingsPanel.Dock = DockStyle.Fill;
        _settingsPanel.BackColor = Surface;
        _settingsPanel.Padding = new Padding(17, 13, 17, 16);

        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 6, Margin = Padding.Empty };
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 105));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 73));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 75));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 41));
        _settingsPanel.Controls.Add(layout);

        var header = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, Margin = Padding.Empty };
        header.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100)); header.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        var title = new Label { Text = "Settings", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft, Font = new Font("Segoe UI Semibold", 13, FontStyle.Bold) };
        var back = SmallButton("‹", "Back to symbols");
        back.Click += (_, _) => SetSettingsVisible(false);
        header.Controls.Add(title, 0, 0); header.Controls.Add(back, 1, 0);
        layout.Controls.Add(header, 0, 0);

        var shortcutGroup = new Panel { Dock = DockStyle.Fill, BackColor = Card, Padding = new Padding(10, 7, 10, 6), Margin = new Padding(0, 3, 0, 4) };
        var shortcutLayout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 3, Margin = Padding.Empty };
        shortcutLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        shortcutLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        shortcutLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 24));
        shortcutLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 36));
        shortcutLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        var shortcutTitle = new Label { Text = "Global shortcut", Dock = DockStyle.Fill, ForeColor = Color.White, Font = new Font("Segoe UI Semibold", 9.3F, FontStyle.Bold), TextAlign = ContentAlignment.MiddleLeft };
        _shortcutValue.Text = FormatShortcut(_settings.Modifiers, _settings.Key);
        _shortcutValue.Dock = DockStyle.Fill; _shortcutValue.TextAlign = ContentAlignment.MiddleLeft; _shortcutValue.ForeColor = Accent; _shortcutValue.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
        _captureButton.Text = "Change shortcut"; _captureButton.Width = 122; StyleButton(_captureButton, true); _captureButton.Height = 29; _captureButton.Margin = new Padding(4, 2, 0, 2);
        _captureButton.Click += (_, _) =>
        {
            _capturingShortcut = true;
            _draftModifiers = _settings.Modifiers;
            _draftKey = _settings.Key;
            if (_hotkeyRegistered)
            {
                UnregisterHotKey(Handle, HotkeyId);
                _hotkeyRegistered = false;
            }
            _captureButton.Text = "Press keys…";
            _captureHint.Text = "Press Ctrl or Alt with another key. Esc cancels.";
            _saveButton.Enabled = false;
            Focus();
        };
        shortcutLayout.Controls.Add(shortcutTitle, 0, 0); shortcutLayout.SetColumnSpan(shortcutTitle, 2);
        shortcutLayout.Controls.Add(_shortcutValue, 0, 1); shortcutLayout.Controls.Add(_captureButton, 1, 1);
        _captureHint.Text = "Use Ctrl or Alt with another key.";
        _captureHint.Dock = DockStyle.Fill; _captureHint.ForeColor = Muted; _captureHint.Font = new Font("Segoe UI", 8); _captureHint.TextAlign = ContentAlignment.MiddleLeft; _captureHint.AutoEllipsis = true;
        shortcutLayout.Controls.Add(_captureHint, 0, 2); shortcutLayout.SetColumnSpan(_captureHint, 2);
        shortcutGroup.Controls.Add(shortcutLayout);
        layout.Controls.Add(shortcutGroup, 0, 1);

        var startupGroup = new Panel { Dock = DockStyle.Fill, BackColor = Card, Padding = new Padding(11, 8, 11, 8), Margin = new Padding(0, 2, 0, 4) };
        _startupCheck.Text = "Start MathKey when I sign in";
        _startupCheck.Checked = IsStartupEnabled();
        _startupCheck.AutoSize = true;
        _startupCheck.ForeColor = Color.White;
        _startupCheck.Font = new Font("Segoe UI", 9);
        _startupCheck.Location = new Point(10, 8);
        var startupHint = new Label { Text = "The tray app needs to be running to listen for its shortcut.", Dock = DockStyle.Bottom, Height = 27, ForeColor = Muted, Font = new Font("Segoe UI", 8), TextAlign = ContentAlignment.MiddleLeft };
        startupGroup.Controls.Add(startupHint); startupGroup.Controls.Add(_startupCheck);
        layout.Controls.Add(startupGroup, 0, 2);

        var info = new Label
        {
            Text = "Search updates as you type. Click a tile or use Enter to insert.\n\nKeyboard: Ctrl+1–4 inserts the first four results. ↓ enters results, arrows move, Ctrl+F returns to search.",
            Dock = DockStyle.Fill,
            ForeColor = Muted,
            Font = new Font("Segoe UI", 8.5F),
            TextAlign = ContentAlignment.TopLeft,
            Padding = new Padding(2, 9, 2, 0)
        };
        layout.Controls.Add(info, 0, 3);
        var spacer = new Panel { Dock = DockStyle.Fill, BackColor = Surface };
        layout.Controls.Add(spacer, 0, 4);
        var actions = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft, WrapContents = false, Margin = Padding.Empty, Padding = new Padding(0, 5, 0, 0) };
        _saveButton.Text = "Save"; _saveButton.Width = 86; _saveButton.Height = 30; StyleButton(_saveButton, true);
        _saveButton.Click += (_, _) => SaveSettings();
        var cancel = new Button { Text = "Cancel", Width = 78, Height = 30, Margin = new Padding(0, 0, 7, 0) };
        StyleButton(cancel, false); cancel.Click += (_, _) => { _settings = LoadSettings(); _draftModifiers = _settings.Modifiers; _draftKey = _settings.Key; _startupCheck.Checked = IsStartupEnabled(); UpdateShortcutDisplay(); SetSettingsVisible(false); };
        actions.Controls.Add(_saveButton); actions.Controls.Add(cancel);
        layout.Controls.Add(actions, 0, 5);
    }

    private void RefreshSymbols()
    {
        if (_tiles.IsDisposed) return;
        string query = Normalize(_search.Text);
        var filtered = SymbolCatalog.All.Where(s =>
            (_selectedCategory == "All" || s.Category == _selectedCategory) &&
            (query.Length == 0 || Normalize($"{s.Glyph} {s.Name} {s.SearchTerms} {s.Category}").Contains(query, StringComparison.Ordinal))
        ).ToList();
        _resultCount.Text = query.Length == 0 && _selectedCategory == "All"
            ? $"{filtered.Count} symbols"
            : $"{filtered.Count} result{(filtered.Count == 1 ? "" : "s")}";
        _tiles.SetItems(filtered);
        _emptyState.Visible = filtered.Count == 0;
        if (filtered.Count == 0) _emptyState.BringToFront();
    }

    private void SelectCategory(string category)
    {
        _selectedCategory = category;
        foreach (Button button in _categoryButtons)
        {
            bool active = (string)button.Tag! == category;
            button.BackColor = active ? Color.FromArgb(34, 82, 102) : Card;
            button.ForeColor = active ? Accent : Color.FromArgb(214, 216, 219);
        }
        RefreshSymbols();
    }

    private void ShowPickerFromTray()
    {
        _targetWindow = GetForegroundWindow();
        SetSettingsVisible(false);
        ShowPicker();
    }

    private void ShowSettingsFromTray()
    {
        _targetWindow = GetForegroundWindow();
        SetSettingsVisible(true);
        ShowAtCaretOrCursor();
    }

    private void ShowPicker()
    {
        _targetWindow = GetForegroundWindow();
        SetSettingsVisible(false);
        ShowAtCaretOrCursor();
        BeginInvoke(new Action(() => { _search.Focus(); _search.SelectAll(); }));
    }

    private void ShowAtCaretOrCursor()
    {
        Point anchor = TryGetCaretScreenPoint(_targetWindow, out var caret) ? caret : Cursor.Position;
        Screen screen = Screen.FromPoint(anchor);
        var work = screen.WorkingArea;
        int left = Math.Clamp(anchor.X, work.Left + 8, Math.Max(work.Left + 8, work.Right - Width - 8));
        int top = anchor.Y + 18;
        if (top + Height > work.Bottom - 8) top = anchor.Y - Height - 10;
        top = Math.Clamp(top, work.Top + 8, Math.Max(work.Top + 8, work.Bottom - Height - 8));
        Location = new Point(left, top);
        if (!Visible) Show();
        WindowState = FormWindowState.Normal;
        TopMost = true;
        Activate();
    }

    private void SetSettingsVisible(bool value)
    {
        _pickerPanel.Visible = !value;
        _settingsPanel.Visible = value;
        if (value)
        {
            _draftModifiers = _settings.Modifiers;
            _draftKey = _settings.Key;
            _shortcutValue.Text = FormatShortcut(_settings.Modifiers, _settings.Key);
            _captureButton.Text = "Change shortcut";
            _captureHint.Text = "Use Ctrl or Alt with another key.";
        }
        else
        {
            _capturingShortcut = false;
            _draftModifiers = _settings.Modifiers;
            _draftKey = _settings.Key;
            _shortcutValue.Text = FormatShortcut(_settings.Modifiers, _settings.Key);
            if (IsHandleCreated && !_hotkeyRegistered) RegisterCurrentHotkey();
        }
    }

    private void InsertSymbol(string glyph)
    {
        _pendingInsert = glyph;
        Hide();
        _insertTimer.Stop();
        _insertTimer.Start();
    }

    private void SaveSettings()
    {
        if (_capturingShortcut)
        {
            _captureHint.Text = "Press a shortcut or Esc to cancel first.";
            return;
        }

        bool oldWasRegistered = _hotkeyRegistered;
        if (oldWasRegistered)
        {
            UnregisterHotKey(Handle, HotkeyId);
            _hotkeyRegistered = false;
        }
        if (!TryRegisterHotkey(_draftModifiers, _draftKey))
        {
            _hotkeyRegistered = RegisterHotKey(Handle, HotkeyId, GetNativeModifiers(_settings.Modifiers) | ModNoRepeat, (uint)_settings.Key);
            _captureHint.Text = "That shortcut is already in use. Choose another one.";
            return;
        }
        _hotkeyRegistered = true;

        var previousSettings = _settings;
        _settings = new AppSettings { Modifiers = _draftModifiers, Key = _draftKey };
        try
        {
            SetStartupEnabled(_startupCheck.Checked);
            SaveSettingsFile(_settings);
        }
        catch (Exception ex)
        {
            UnregisterHotKey(Handle, HotkeyId);
            _settings = previousSettings;
            _hotkeyRegistered = RegisterHotKey(Handle, HotkeyId, GetNativeModifiers(_settings.Modifiers) | ModNoRepeat, (uint)_settings.Key);
            _captureHint.Text = $"Couldn't save settings: {ex.Message}";
            return;
        }
        UpdateShortcutDisplay();
        _captureHint.Text = "Settings saved.";
        SetSettingsVisible(false);
    }

    private void UpdateShortcutDisplay()
    {
        _shortcutValue.Text = FormatShortcut(_settings.Modifiers, _settings.Key);
        if (_footerShortcut is not null) _footerShortcut.Text = FormatShortcut(_settings.Modifiers, _settings.Key);
    }

    private void RegisterCurrentHotkey()
    {
        _hotkeyRegistered = TryRegisterHotkey(_settings.Modifiers, _settings.Key);
    }

    private bool TryRegisterHotkey(Keys modifiers, Keys key) =>
        RegisterHotKey(Handle, HotkeyId, GetNativeModifiers(modifiers) | ModNoRepeat, (uint)key);

    private static uint GetNativeModifiers(Keys modifiers)
    {
        uint result = 0;
        if ((modifiers & Keys.Control) != 0) result |= ModControl;
        if ((modifiers & Keys.Alt) != 0) result |= ModAlt;
        if ((modifiers & Keys.Shift) != 0) result |= ModShift;
        if ((modifiers & (Keys.LWin | Keys.RWin)) != 0) result |= ModWin;
        return result;
    }

    private static string FormatShortcut(Keys modifiers, Keys key)
    {
        var parts = new List<string>();
        if ((modifiers & Keys.Control) != 0) parts.Add("Ctrl");
        if ((modifiers & Keys.Alt) != 0) parts.Add("Alt");
        if ((modifiers & Keys.Shift) != 0) parts.Add("Shift");
        if ((modifiers & (Keys.LWin | Keys.RWin)) != 0) parts.Add("Win");
        string keyName = key switch { Keys.Oemcomma => ",", Keys.OemPeriod => ".", Keys.OemMinus => "-", Keys.Oemplus => "=", Keys.Space => "Space", _ => key.ToString() };
        parts.Add(keyName);
        return string.Join(" + ", parts);
    }

    private static string Normalize(string value) => string.Join(" ", value.Trim().ToLowerInvariant().Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries));

    private static AppSettings LoadSettings()
    {
        try
        {
            if (File.Exists(SettingsPath))
            {
                var loaded = JsonSerializer.Deserialize<AppSettings>(File.ReadAllText(SettingsPath));
                if (loaded is not null && loaded.Key != Keys.None && (loaded.Modifiers & (Keys.Control | Keys.Alt)) != 0) return loaded;
            }
        }
        catch { }
        return new AppSettings { Modifiers = Keys.Control | Keys.Alt, Key = Keys.M };
    }

    private static void SaveSettingsFile(AppSettings settings)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(SettingsPath)!);
        File.WriteAllText(SettingsPath, JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true }));
    }

    private static bool IsStartupEnabled()
    {
        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(RunKeyPath, false);
            return key?.GetValue("MathSymbols") is string;
        }
        catch { return false; }
    }

    private static void SetStartupEnabled(bool enabled)
    {
        using var key = Registry.CurrentUser.CreateSubKey(RunKeyPath, true);
        if (enabled) key.SetValue("MathSymbols", $"\"{Application.ExecutablePath}\"");
        else key.DeleteValue("MathSymbols", false);
    }

    private void ExitApplication()
    {
        _allowExit = true;
        _tray.Visible = false;
        _tray.Dispose();
        Close();
    }

    private static Button SmallButton(string text, string accessibleName)
    {
        var button = new Button { Text = text, Width = 29, Height = 27, Margin = new Padding(3, 0, 0, 0), FlatStyle = FlatStyle.Flat, BackColor = Card, ForeColor = Color.FromArgb(211, 214, 218), Font = new Font("Segoe UI", 10), Cursor = Cursors.Hand, AccessibleName = accessibleName };
        button.FlatAppearance.BorderSize = 0;
        return button;
    }

    private static void StyleButton(Button button, bool primary)
    {
        button.FlatStyle = FlatStyle.Flat;
        button.FlatAppearance.BorderSize = 0;
        button.BackColor = primary ? Color.FromArgb(34, 82, 102) : Card;
        button.ForeColor = primary ? Accent : Color.FromArgb(215, 217, 220);
        button.Font = new Font("Segoe UI", 8.5F);
        button.Cursor = Cursors.Hand;
        button.Padding = new Padding(8, 1, 8, 1);
    }

    private static bool TryGetCaretScreenPoint(nint target, out Point point)
    {
        point = Point.Empty;
        if (target == 0) return false;
        uint threadId = GetWindowThreadProcessId(target, out _);
        var info = new GuiThreadInfo { CbSize = (uint)Marshal.SizeOf<GuiThreadInfo>() };
        if (!GetGUIThreadInfo(threadId, ref info) || info.HwndCaret == 0) return false;
        point = new Point(info.Caret.Left, info.Caret.Bottom);
        if (!ClientToScreen(info.HwndCaret, ref point)) return false;
        return true;
    }

    private static void SendUnicodeText(string value)
    {
        if (string.IsNullOrEmpty(value)) return;
        var inputs = new Input[value.Length * 2];
        for (int i = 0; i < value.Length; i++)
        {
            ushort unit = value[i];
            inputs[i * 2] = Input.Unicode(unit, keyUp: false);
            inputs[i * 2 + 1] = Input.Unicode(unit, keyUp: true);
        }
        SendInput((uint)inputs.Length, inputs, Marshal.SizeOf<Input>());
    }

    private sealed class AppSettings
    {
        public AppSettings() { }
        public Keys Modifiers { get; set; } = Keys.Control | Keys.Alt;
        public Keys Key { get; set; } = Keys.M;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct Rect { public int Left, Top, Right, Bottom; }
    [StructLayout(LayoutKind.Sequential)]
    private struct GuiThreadInfo
    {
        public uint CbSize, Flags;
        public nint HwndActive, HwndFocus, HwndCapture, HwndMenuOwner, HwndMoveSize, HwndCaret;
        public Rect Caret;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct Input
    {
        public uint Type;
        public InputUnion Union;
        public static Input Unicode(ushort character, bool keyUp) => new()
        {
            Type = 1,
            Union = new InputUnion { Keyboard = new KeyboardInput { VirtualKey = 0, ScanCode = character, Flags = 0x0004 | (keyUp ? 0x0002u : 0u), Time = 0, ExtraInfo = 0 } }
        };
    }
    [StructLayout(LayoutKind.Explicit)]
    private struct InputUnion
    {
        [FieldOffset(0)] public KeyboardInput Keyboard;
        [FieldOffset(0)] public MouseInput Mouse;
        [FieldOffset(0)] public HardwareInput Hardware;
    }
    [StructLayout(LayoutKind.Sequential)]
    private struct KeyboardInput { public ushort VirtualKey, ScanCode; public uint Flags, Time; public nuint ExtraInfo; }
    [StructLayout(LayoutKind.Sequential)]
    private struct MouseInput { public int Dx, Dy; public uint MouseData, Flags, Time; public nuint ExtraInfo; }
    [StructLayout(LayoutKind.Sequential)]
    private struct HardwareInput { public uint Message; public ushort ParamL, ParamH; }

    [DllImport("user32.dll", SetLastError = true)] private static extern bool RegisterHotKey(nint hWnd, int id, uint modifiers, uint virtualKey);
    [DllImport("user32.dll", SetLastError = true)] private static extern bool UnregisterHotKey(nint hWnd, int id);
    [DllImport("user32.dll")] private static extern nint GetForegroundWindow();
    [DllImport("user32.dll")] private static extern uint GetWindowThreadProcessId(nint hWnd, out uint processId);
    [DllImport("user32.dll", SetLastError = true)] private static extern bool GetGUIThreadInfo(uint threadId, ref GuiThreadInfo info);
    [DllImport("user32.dll", SetLastError = true)] private static extern bool ClientToScreen(nint hWnd, ref Point point);
    [DllImport("user32.dll")] private static extern bool SetForegroundWindow(nint hWnd);
    [DllImport("user32.dll")] private static extern bool IsWindow(nint hWnd);
    [DllImport("user32.dll", SetLastError = true)] private static extern uint SendInput(uint count, Input[] inputs, int size);
}
