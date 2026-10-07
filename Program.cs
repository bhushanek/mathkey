using System.Threading;
using System.Windows.Forms;

namespace MathSymbols;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        using var singleInstance = new Mutex(true, "MathSymbols.SingleInstance", out bool createdNew);
        if (!createdNew) return;

        ApplicationConfiguration.Initialize();
        Application.Run(new MainForm());
    }
}
