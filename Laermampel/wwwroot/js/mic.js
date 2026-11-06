// Der einzige JavaScript-Teil: Mikrofon abgreifen und regelmäßig den RMS-Wert an C# schicken.
let ctx, stream, timer;

export async function start(dotnetRef, intervalMs) {
    stop();
    stream = await navigator.mediaDevices.getUserMedia({
        // Automatik aus, sonst "regelt" der Browser die Lautstärke weg
        audio: { echoCancellation: false, noiseSuppression: false, autoGainControl: false }
    });
    ctx = new AudioContext();
    const analyser = ctx.createAnalyser();
    analyser.fftSize = 2048;
    ctx.createMediaStreamSource(stream).connect(analyser);

    const buf = new Float32Array(analyser.fftSize);
    timer = setInterval(() => {
        analyser.getFloatTimeDomainData(buf);
        let sum = 0;
        for (const v of buf) sum += v * v;
        dotnetRef.invokeMethodAsync('OnLevel', Math.sqrt(sum / buf.length));
    }, intervalMs);
}

export function stop() {
    if (timer) clearInterval(timer);
    stream?.getTracks().forEach(t => t.stop());
    ctx?.close();
    timer = stream = ctx = undefined;
}

export function playPsst(url) {
    const speak = () => {
        const u = new SpeechSynthesisUtterance('Psssst');
        u.lang = 'de-DE';
        speechSynthesis.speak(u);
    };
    const audio = new Audio(url);
    audio.onerror = speak;                  // Datei fehlt -> Sprachausgabe
    audio.play().catch(speak);
}

export function toggleFullscreen() {
    if (document.fullscreenElement) document.exitFullscreen();
    else document.documentElement.requestFullscreen?.();
}
