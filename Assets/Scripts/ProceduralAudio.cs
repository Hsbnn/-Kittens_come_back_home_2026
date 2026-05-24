using System.Collections.Generic;
using UnityEngine;

public static class ProceduralAudio
{
    public static readonly string[] NoteNames = { "C", "E", "G" };

    private static readonly float[] NoteFrequencies = { 261.63f, 329.63f, 392.00f };
    private static readonly Dictionary<string, AudioClip> Cache = new Dictionary<string, AudioClip>();

    public static string GetNoteName(int note)
    {
        return NoteNames[Mathf.Abs(note) % NoteNames.Length];
    }

    public static void PlayMeow(Vector3 position, int note)
    {
        float frequency = NoteFrequencies[Mathf.Abs(note) % NoteFrequencies.Length];
        PlayClip(position, CreateMeow("meow_v3_" + note, frequency), 0.72f, 13f);
    }

    public static void PlayBell(Vector3 position, int note)
    {
        float frequency = NoteFrequencies[Mathf.Abs(note) % NoteFrequencies.Length] * 2f;
        PlayClip(position, CreateBellTone("bell_v2_" + note, frequency), 0.9f, 18f);
    }

    public static void PlayPurr(Vector3 position)
    {
        PlayClip(position, CreatePurr("purr_v2"), 0.45f, 7f);
    }

    public static void PlaySuccess(Vector3 position)
    {
        PlayClip(position, CreateChord("success_v2", new[] { 392f, 523.25f, 659.25f, 783.99f }, 1.35f, 0.24f), 0.9f, 20f);
    }

    private static void PlayClip(Vector3 position, AudioClip clip, float volume, float maxDistance)
    {
        if (clip == null)
        {
            return;
        }

        GameObject audioObject = new GameObject("Procedural_Audio_" + clip.name);
        audioObject.transform.position = position;
        AudioSource source = audioObject.AddComponent<AudioSource>();
        source.clip = clip;
        source.volume = volume;
        source.spatialBlend = 1f;
        source.rolloffMode = AudioRolloffMode.Linear;
        source.minDistance = 1.2f;
        source.maxDistance = maxDistance;
        source.Play();
        Object.Destroy(audioObject, clip.length + 0.1f);
    }

    private static AudioClip CreateMeow(string key, float frequency)
    {
        if (Cache.TryGetValue(key, out AudioClip cached))
        {
            return cached;
        }

        const int sampleRate = 44100;
        const float duration = 0.74f;
        int samples = Mathf.CeilToInt(sampleRate * duration);
        float[] data = new float[samples];

        for (int i = 0; i < samples; i++)
        {
            float t = i / (float)sampleRate;
            float n = t / duration;
            float open = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(n * 8f));
            float close = 1f - Mathf.SmoothStep(0.72f, 1f, n);
            float envelope = open * close;
            float pitchCurve = Mathf.Lerp(1.18f, 0.82f, Mathf.SmoothStep(0f, 1f, n));
            pitchCurve += Mathf.Sin(n * Mathf.PI) * 0.18f;
            float pitch = frequency * pitchCurve;
            float vowel = Mathf.Sin(2f * Mathf.PI * pitch * t);
            vowel += 0.32f * Mathf.Sin(2f * Mathf.PI * pitch * 1.52f * t);
            vowel += 0.16f * Mathf.Sin(2f * Mathf.PI * pitch * 2.14f * t);
            float mouth = 0.76f + 0.24f * Mathf.Sin(2f * Mathf.PI * (5.2f + n * 1.6f) * t);
            data[i] = vowel * mouth * envelope * 0.32f;
        }

        AudioClip clip = AudioClip.Create(key, samples, 1, sampleRate, false);
        clip.SetData(data, 0);
        Cache[key] = clip;
        return clip;
    }

    private static AudioClip CreateBellTone(string key, float frequency)
    {
        if (Cache.TryGetValue(key, out AudioClip cached))
        {
            return cached;
        }

        const int sampleRate = 44100;
        const float duration = 1.25f;
        int samples = Mathf.CeilToInt(sampleRate * duration);
        float[] data = new float[samples];

        for (int i = 0; i < samples; i++)
        {
            float t = i / (float)sampleRate;
            float n = t / duration;
            float envelope = Mathf.Exp(-4.2f * n);
            float wave = Mathf.Sin(2f * Mathf.PI * frequency * t);
            wave += 0.62f * Mathf.Sin(2f * Mathf.PI * frequency * 2.41f * t);
            wave += 0.28f * Mathf.Sin(2f * Mathf.PI * frequency * 3.73f * t);
            wave += 0.12f * Mathf.Sin(2f * Mathf.PI * frequency * 5.12f * t);
            data[i] = wave * envelope * 0.23f;
        }

        AudioClip clip = AudioClip.Create(key, samples, 1, sampleRate, false);
        clip.SetData(data, 0);
        Cache[key] = clip;
        return clip;
    }

    private static AudioClip CreatePurr(string key)
    {
        if (Cache.TryGetValue(key, out AudioClip cached))
        {
            return cached;
        }

        const int sampleRate = 44100;
        const float duration = 0.55f;
        int samples = Mathf.CeilToInt(sampleRate * duration);
        float[] data = new float[samples];

        for (int i = 0; i < samples; i++)
        {
            float t = i / (float)sampleRate;
            float pulse = 0.55f + 0.45f * Mathf.Sin(2f * Mathf.PI * 18f * t);
            float wave = Mathf.Sin(2f * Mathf.PI * 58f * t) + 0.35f * Mathf.Sin(2f * Mathf.PI * 116f * t);
            data[i] = wave * pulse * 0.12f;
        }

        AudioClip clip = AudioClip.Create(key, samples, 1, sampleRate, false);
        clip.SetData(data, 0);
        Cache[key] = clip;
        return clip;
    }

    private static AudioClip CreateTone(string key, float frequency, float duration, float volume, bool bend)
    {
        if (Cache.TryGetValue(key, out AudioClip cached))
        {
            return cached;
        }

        const int sampleRate = 44100;
        int samples = Mathf.CeilToInt(sampleRate * duration);
        float[] data = new float[samples];

        for (int i = 0; i < samples; i++)
        {
            float t = i / (float)sampleRate;
            float normalized = t / duration;
            float envelope = Mathf.Clamp01(1f - normalized);
            envelope *= Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(normalized * 10f));
            float localFrequency = bend ? frequency * Mathf.Lerp(0.86f, 1.12f, normalized) : frequency;
            float wave = Mathf.Sin(2f * Mathf.PI * localFrequency * t);
            wave += 0.35f * Mathf.Sin(2f * Mathf.PI * localFrequency * 2f * t);
            data[i] = wave * envelope * volume;
        }

        AudioClip clip = AudioClip.Create(key, samples, 1, sampleRate, false);
        clip.SetData(data, 0);
        Cache[key] = clip;
        return clip;
    }

    private static AudioClip CreateChord(string key, IReadOnlyList<float> frequencies, float duration, float volume)
    {
        if (Cache.TryGetValue(key, out AudioClip cached))
        {
            return cached;
        }

        const int sampleRate = 44100;
        int samples = Mathf.CeilToInt(sampleRate * duration);
        float[] data = new float[samples];

        for (int i = 0; i < samples; i++)
        {
            float t = i / (float)sampleRate;
            float normalized = t / duration;
            float envelope = Mathf.Clamp01(1f - normalized);
            float wave = 0f;
            for (int f = 0; f < frequencies.Count; f++)
            {
                wave += Mathf.Sin(2f * Mathf.PI * frequencies[f] * t);
            }

            data[i] = wave / frequencies.Count * envelope * volume;
        }

        AudioClip clip = AudioClip.Create(key, samples, 1, sampleRate, false);
        clip.SetData(data, 0);
        Cache[key] = clip;
        return clip;
    }
}
