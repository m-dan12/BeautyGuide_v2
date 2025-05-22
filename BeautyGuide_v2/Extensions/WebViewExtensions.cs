using System;
using System.Linq;
using System.Threading.Tasks;
using WebViewControl;

namespace BeautyGuide_v2.Extensions;

internal static class WebViewExtensions
{
    private static readonly string[] CandidateNames =
    {
        "ExecuteScriptAsync",
        "ExecuteScript",
        "EvaluateScriptAsync",
        "EvaluateScript",
        "RunScriptAsync",
        "RunScript",
        "EvaluateJavaScriptAsync",
        "EvaluateJavaScript",
        "ExecuteJavaScriptAsync",
        "ExecuteJavaScript"
    };

    public static Task InvokeScriptAsync(this WebView webView, string script)
    {
        if (webView == null) throw new ArgumentNullException(nameof(webView));

        var type = webView.GetType();

        // ищем любой метод из списка, у которого первый параметр string
        var mi = type.GetMethods()
            .FirstOrDefault(m =>
                CandidateNames.Contains(m.Name) &&
                m.GetParameters().Length >= 1 &&
                m.GetParameters()[0].ParameterType == typeof(string));

        if (mi != null)
        {
            var result = mi.Invoke(webView, new object[] { script });
            return result switch
            {
                Task t         => t,
                ValueTask vt   => vt.AsTask(),
                _              => Task.CompletedTask
            };
        }

        Console.WriteLine("Предупреждение: WebView не поддерживает выполнение скриптов.");
        return Task.CompletedTask;
    }
}