using System.Text.Json;
using Laermampel.Models;
using Microsoft.JSInterop;

namespace Laermampel.Services;

/// <summary>Speichert die Einstellungen im Browser (localStorage) – bleiben also am Board erhalten.</summary>
public class SettingsService(IJSRuntime js)
{
    private const string Key = "laermampel.settings";

    public async Task<AmpelSettings> LoadAsync()
    {
        try
        {
            var json = await js.InvokeAsync<string?>("localStorage.getItem", Key);
            if (!string.IsNullOrWhiteSpace(json))
                return JsonSerializer.Deserialize<AmpelSettings>(json) ?? new AmpelSettings();
        }
        catch (JsonException) { /* kaputte Daten -> Standardwerte */ }
        catch (JSException) { /* localStorage gesperrt -> Standardwerte */ }

        return new AmpelSettings();
    }

    public async Task SaveAsync(AmpelSettings settings)
    {
        try
        {
            await js.InvokeVoidAsync("localStorage.setItem", Key, JsonSerializer.Serialize(settings));
        }
        catch (JSException) { /* Speichern nicht möglich – App läuft trotzdem weiter */ }
    }
}
