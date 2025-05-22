using System.Runtime.Versioning;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Browser;
using BeautyGuide_v2;

internal sealed partial class Program
{
    private static Task Main(string[] args) => AppBuilder
        .Configure<App>()
        .UseBrowser()
        .WithInterFont()
        .StartBrowserAppAsync("out");
}