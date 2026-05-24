using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public sealed class AmbienceAudio : MonoBehaviour
{
    [SerializeField] private float volume = 0.16f;
    [SerializeField] private AudioClip rainThunderClip;

    public void Configure(AudioClip clip)
    {
        rainThunderClip = clip;
    }

    private void Awake()
    {
        AudioSource source = GetComponent<AudioSource>();
        source.clip = rainThunderClip != null ? rainThunderClip : CreateStormFallbackLoop();
        source.loop = true;
        source.volume = volume;
        source.spatialBlend = 0f;
        source.Play();
    }

    private static AudioClip CreateStormFallbackLoop()
    {
        const int sampleRate = 44100;
        const float duration = 12f;
        int samples = Mathf.RoundToInt(sampleRate * duration);
        float[] data = new float[samples];
        int seed = 19317;
        float lowRain = 0f;
        float midRain = 0f;

        for (int i = 0; i < samples; i++)
        {
            seed = seed * 1103515245 + 12345;
            float white = ((seed >> 16) & 0x7fff) / 16384f - 1f;
            float t = i / (float)sampleRate;
            lowRain = Mathf.Lerp(lowRain, white, 0.012f);
            midRain = Mathf.Lerp(midRain, white, 0.18f);
            float droplets = Mathf.Max(0f, Mathf.Sin(t * 83f) * Mathf.Sin(t * 137f)) * 0.05f;
            float thunder = Mathf.Exp(-Mathf.Repeat(t + 1.5f, 8.5f) * 1.2f) * Mathf.Sin(t * 31f) * 0.22f;
            data[i] = lowRain * 0.28f + midRain * 0.12f + droplets + thunder;
        }

        AudioClip clip = AudioClip.Create("Procedural_Storm_Fallback_Ambience", samples, 1, sampleRate, false);
        clip.SetData(data, 0);
        return clip;
    }
}
