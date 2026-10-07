# MathKey

A lightweight Windows tray app that opens a searchable math symbol picker near the current text caret. Search filters on every keystroke; selecting a tile inserts the Unicode character into the app you were using.

## Features

- Default global shortcut: **Ctrl + Alt + M** (change it in Settings).
- Live search across symbol names, aliases, and categories, including phrases such as “double integration”.
- Keyboard navigation: Ctrl+1–4 inserts the first four matches directly; Down enters results, arrows move, Enter inserts, and Ctrl+F returns to search.
- Categories for calculus, algebra, relations, geometry, sets and logic, Greek letters, arrows, and other notation.
- Optional start-at-sign-in setting. The app can also stay in the tray without enabling startup.
- Uses built-in Windows Forms and Win32 APIs; no third-party packages.

The catalog combines curated math vocabulary and aliases with 2,003 named entries from Unicode's math, technical, arrow, Greek math alphabet, and geometric symbol blocks. The grid only draws visible symbols, so the full catalog remains quick to search and scroll.

## Build and run

Install the .NET 10 SDK on Windows, then run these commands from this folder:

```powershell
dotnet run
```

To create a small framework-dependent app folder:

```powershell
dotnet publish -c Release -r win-x64 --self-contained false
```

The published app requires the matching .NET Desktop Runtime. For a self-contained single-file publish, use `--self-contained true -p:PublishSingleFile=true` (larger output, no separate runtime install).

When first launched, MathKey starts in the notification area. Right-click its tray icon for Settings or Quit. Its shortcut only works while the app is running; enable “Start MathKey when I sign in” in Settings to keep it ready after sign-in.
