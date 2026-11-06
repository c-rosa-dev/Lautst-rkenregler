using Laermampel.Models;

namespace Laermampel.Services;

/// <summary>
/// Reine Logik ohne Browser-Abhängigkeit: Pegel rein, Zustand + "Psst ja/nein" raus.
/// Dadurch lässt sie sich später gut mit Unit-Tests prüfen.
/// </summary>
public class MoodEvaluator(AmpelSettings settings)
{
    private double? _smoothed;
    private Mood _candidate = Mood.Happy;
    private double _candidateSince;
    private double _unhappySince = -1;
    private double _lastPsst = double.NegativeInfinity;

    public Mood Current { get; private set; } = Mood.Happy;
    public double SmoothedLevel => _smoothed ?? 0;

    /// <summary>Rechnet den RMS-Wert vom Mikro in einen Pegel von 0 bis 100 um.</summary>
    public static double ToLevel(double rms)
    {
        if (rms <= 0) return 0;
        var dbfs = 20 * Math.Log10(rms);          // -100 (leise) … 0 (max)
        return Math.Clamp(dbfs + 100, 0, 100);
    }

    /// <param name="level">Aktueller Pegel 0–100</param>
    /// <param name="now">Zeit in Sekunden seit Start</param>
    /// <param name="dt">Sekunden seit dem letzten Aufruf</param>
    /// <returns>true, wenn jetzt ein Psst abgespielt werden soll</returns>
    public bool Update(double level, double now, double dt)
    {
        // 1. Glätten (exponentieller gleitender Durchschnitt)
        var alpha = settings.SmoothingSeconds <= 0 ? 1 : 1 - Math.Exp(-dt / settings.SmoothingSeconds);
        _smoothed = _smoothed is null ? level : _smoothed + alpha * (level - _smoothed.Value);

        // 2. Zustand erst wechseln, wenn der neue Zustand HoldSeconds lang anliegt
        var target = Classify(_smoothed.Value);
        if (target != _candidate)
        {
            _candidate = target;
            _candidateSince = now;
        }
        if (_candidate != Current && now - _candidateSince >= settings.HoldSeconds)
            Current = _candidate;

        // 3. Psst, wenn es lange genug unglücklich laut ist und der Cooldown vorbei ist
        if (Current != Mood.Unhappy)
        {
            _unhappySince = -1;
            return false;
        }

        if (_unhappySince < 0) _unhappySince = now;

        var psst = settings.PsstEnabled
                   && now - _unhappySince >= settings.PsstAfterSeconds
                   && now - _lastPsst >= settings.PsstCooldownSeconds;
        if (psst) _lastPsst = now;
        return psst;
    }

    private Mood Classify(double level) =>
        level <= settings.HappyMax ? Mood.Happy :
        level <= settings.NeutralMax ? Mood.Neutral :
        Mood.Unhappy;
}
