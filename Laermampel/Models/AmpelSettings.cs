namespace Laermampel.Models;

/// <summary>
/// Alle einstellbaren Werte. Pegel-Skala: 0 (Stille) bis 100 (Maximum des Mikros).
/// Das sind keine echten Dezibel, sondern ein relativer Pegel – deshalb gibt es "Ruhe kalibrieren".
/// </summary>
public class AmpelSettings
{
    /// <summary>Bis zu diesem Pegel ist er glücklich.</summary>
    public double HappyMax { get; set; } = 50;

    /// <summary>Bis zu diesem Pegel ist er mittelmäßig, darüber unglücklich.</summary>
    public double NeutralMax { get; set; } = 65;

    /// <summary>So lange muss ein neuer Zustand anliegen, bevor das Gesicht wechselt (gegen Flackern).</summary>
    public double HoldSeconds { get; set; } = 1.0;

    /// <summary>Glättung: wie träge die Anzeige reagiert. Höher = ruhiger.</summary>
    public double SmoothingSeconds { get; set; } = 1.5;

    public bool PsstEnabled { get; set; } = true;

    /// <summary>So lange muss es "unglücklich" laut sein, bevor das Psst kommt.</summary>
    public double PsstAfterSeconds { get; set; } = 3;

    /// <summary>Mindestabstand zwischen zwei Psst.</summary>
    public double PsstCooldownSeconds { get; set; } = 10;

    public void ResetToDefaults()
    {
        var d = new AmpelSettings();
        HappyMax = d.HappyMax;
        NeutralMax = d.NeutralMax;
        HoldSeconds = d.HoldSeconds;
        SmoothingSeconds = d.SmoothingSeconds;
        PsstEnabled = d.PsstEnabled;
        PsstAfterSeconds = d.PsstAfterSeconds;
        PsstCooldownSeconds = d.PsstCooldownSeconds;
    }
}
