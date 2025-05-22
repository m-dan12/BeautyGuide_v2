using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform;
using Avalonia.Xaml.Interactivity;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using WebViewControl;
using BeautyGuide_v2.Extensions;


namespace BeautyGuide_v2.Behaviors;

public sealed class WebViewMapBehavior : Behavior<WebView>
{
    /* --------------------------- Свойство -------------------------------- */
    public static readonly StyledProperty<string> SalonsJsonProperty =
        AvaloniaProperty.Register<WebViewMapBehavior, string>(
            nameof(SalonsJson), string.Empty);

    public string SalonsJson
    {
        get => GetValue(SalonsJsonProperty);
        set => SetValue(SalonsJsonProperty, value);
    }

    static WebViewMapBehavior()
    {
        SalonsJsonProperty.Changed.AddClassHandler<WebViewMapBehavior>(
            (b, _) => b.OnSalonsJsonChanged());
    }

    /* --------------------------- Поле WebView ---------------------------- */
    private WebView? _webView;

    /* ------------------------ Подключение поведения ---------------------- */
    protected override void OnAttached()
    {
        base.OnAttached();
        _webView = AssociatedObject;

        AssociatedObject.AttachedToVisualTree += async (_, _) =>
        {
            if (!string.IsNullOrEmpty(SalonsJson))
                await LoadMapAsync();
        };
    }

    protected override void OnDetaching()
    {
        _webView = null;
        base.OnDetaching();
    }

    /* --------------------- Колбэк изменения свойства --------------------- */
    private async void OnSalonsJsonChanged()
    {
        if (_webView is not null && !string.IsNullOrEmpty(SalonsJson))
            await LoadMapAsync();
    }

    /* --------------------------- Загрузка карты -------------------------- */
    private async Task LoadMapAsync()
    {
        if (_webView == null)
            return;

        try
        {
            // 1. HTML-страница с картой
            var htmlContent = await LoadHtmlContentAsync();
            _webView.LoadHtml(htmlContent);

            // 2. Передаём данные о салонах
            if (!string.IsNullOrEmpty(SalonsJson))
            {
                var escapedJson = SalonsJson
                    .Replace("\\", "\\\\")
                    .Replace("'", "\\'");

                var script = $"window.postMessage('{escapedJson}', '*');";
                await _webView.InvokeScriptAsync(script);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при загрузке карты: {ex.Message}");
        }
    }

    /* --------------------- Чтение встроенного ресурса -------------------- */
    private static async Task<string> LoadHtmlContentAsync()
    {
        // путь внутри проекта: Assets/map.html
        var uri = new Uri("avares://BeautyGuide_v2/Assets/map.html");

        await using var stream = AssetLoader.Open(uri);      // ← заменили reflection
        using var reader = new StreamReader(stream);
        return await reader.ReadToEndAsync();
    }
}

/* ----------------------------------------------------------------------- */
/*           Extension-метод, вызывающий нужный Script-API                  */
/* ----------------------------------------------------------------------- */
internal static class WebViewExtensions
{
    public static Task InvokeScriptAsync(this WebView webView, string script)
    {
        var mi = webView.GetType().GetMethod("ExecuteScriptAsync", new[] { typeof(string) }) ??
                 webView.GetType().GetMethod("EvaluateScriptAsync", new[] { typeof(string) });

        if (mi != null)
            return (Task)mi.Invoke(webView, new object[] { script })!;

        Console.WriteLine("Предупреждение: WebView не поддерживает выполнение скриптов.");
        return Task.CompletedTask;
    }
}