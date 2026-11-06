using Laermampel.Models;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;

namespace Laermampel.Services;

public class MediaService(IJSRuntime js)
{
    private const string KeyPrefix = "laermampel.face.";
    private readonly Dictionary<Mood, string> _custom = new();

    public string PsstSound => "sounds/psst.mp3";

    // Beim Start einmal die gespeicherten Bilder aus dem Browser holen
    public async Task LoadAsync()
    {
        foreach (var mood in Enum.GetValues<Mood>())
        {
            try
            {
                var data = await js.InvokeAsync<string?>("localStorage.getItem", KeyPrefix + mood);
                if (!string.IsNullOrEmpty(data)) _custom[mood] = data;
            }
            catch (JSException) { }
        }
    }

    public string GetFace(Mood mood) =>
        _custom.TryGetValue(mood, out var url) ? url : DefaultFace(mood);

    public bool HasCustomFace(Mood mood) => _custom.ContainsKey(mood);

    /// <returns>Fehlermeldung oder null, wenn alles geklappt hat</returns>
    public async Task<string?> SetFaceAsync(Mood mood, IBrowserFile file)
    {
        if (!file.ContentType.StartsWith("image/")) return "Das ist kein Bild.";

        try
{
    // PNG/WebP/GIF können Transparenz haben -> als PNG speichern, Fotos weiter als JPEG (kleiner)
    var keepAlpha = file.ContentType is "image/png" or "image/webp" or "image/gif";
    var format = keepAlpha ? "image/png" : "image/jpeg";

    var resized = await file.RequestImageFileAsync(format, 600, 600);
    await using var stream = resized.OpenReadStream(maxAllowedSize: 5 * 1024 * 1024);
    using var ms = new MemoryStream();
    await stream.CopyToAsync(ms);

    var dataUrl = $"data:{format};base64," + Convert.ToBase64String(ms.ToArray());
    await js.InvokeVoidAsync("localStorage.setItem", KeyPrefix + mood, dataUrl);
    _custom[mood] = dataUrl;
    return null;
}
        catch (JSException) { return "Bild konnte nicht gespeichert werden (Speicher voll?)."; }
        catch (IOException) { return "Das Bild ist zu groß."; }
    }

    public async Task ResetFaceAsync(Mood mood)
    {
        _custom.Remove(mood);
        try { await js.InvokeVoidAsync("localStorage.removeItem", KeyPrefix + mood); }
        catch (JSException) { }
    }

    private static string DefaultFace(Mood mood) => mood switch
    {
        Mood.Happy => "img/happy.svg",
        Mood.Neutral => "img/neutral.svg",
        _ => "img/unhappy.svg"
    };
}