# Lärmampel – Einrichtung

## 1. Projekt anlegen (einmalig)
```bash
dotnet new blazorwasm --empty -n Laermampel
```
Dann die Dateien aus diesem Paket in den neuen Ordner `Laermampel` kopieren
(gleiche Ordnerstruktur, vorhandene `Program.cs` und `Pages/Home.razor` überschreiben).

```
Laermampel/
├─ Program.cs                     <- überschreiben
├─ Models/        Mood.cs, AmpelSettings.cs
├─ Services/      MoodEvaluator.cs, SettingsService.cs, MediaService.cs
├─ Components/    SettingsPanel.razor (+ .css)
├─ Pages/         Home.razor (+ .css)          <- überschreiben
└─ wwwroot/
   ├─ js/mic.js
   ├─ img/happy.svg, neutral.svg, unhappy.svg  (Platzhalter)
   └─ sounds/psst.mp3                          (optional, selbst aufnehmen)
```

## 2. Starten
```bash
cd Laermampel
dotnet watch
```
Im Browser auf **Starten** klicken und das Mikro erlauben.

## 3. Eigene Bilder / eigenes Psst
- Fotos als `happy.jpg` usw. nach `wwwroot/img/` legen und in `MediaService.cs` die Endungen anpassen.
- Fehlt `wwwroot/sounds/psst.mp3`, sagt der Browser „Psst“ per Sprachausgabe.

## Wie es funktioniert
mic.js misst alle 100 ms → `OnLevel()` in Home.razor → `MoodEvaluator` glättet,
entscheidet den Zustand und ob ein Psst fällig ist → Gesicht + Hintergrundfarbe wechseln.
