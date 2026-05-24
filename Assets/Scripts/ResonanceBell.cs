using UnityEngine;

public sealed class ResonanceBell : MonoBehaviour
{
    [SerializeField] private int bellNote;
    [SerializeField] private Color idleColor = new Color(0.9f, 0.78f, 0.35f);
    [SerializeField] private Color ringingColor = new Color(1f, 0.95f, 0.55f);
    [SerializeField] private float cooldown = 0.35f;
    [SerializeField] private float resonanceRequired = 1.15f;
    [SerializeField] private float resonanceDecay = 0.45f;
    [SerializeField] private float detunePenalty = 0.55f;
    [SerializeField] private float relayRange = 7f;
    [SerializeField] private float relayIntensity = 0.62f;
    [SerializeField] private LayerMask occlusionMask = ~0;

    private Renderer bellRenderer;
    private ParticleSystem particles;
    private Vector3 baseScale;
    private float ringTimer;
    private float nextRingTime;
    private float resonanceEnergy;
    private int lastSignalId = -1;

    public int BellNote
    {
        get => bellNote;
        set => bellNote = Mathf.Abs(value) % ProceduralAudio.NoteNames.Length;
    }

    private void Awake()
    {
        bellRenderer = GetComponentInChildren<Renderer>();
        particles = GetComponentInChildren<ParticleSystem>();
        baseScale = transform.localScale;
        ApplyColor(idleColor);
    }

    private void OnEnable()
    {
        BellRegistry.Register(this);
    }

    private void OnDisable()
    {
        BellRegistry.Unregister(this);
    }

    private void Update()
    {
        resonanceEnergy = Mathf.Max(0f, resonanceEnergy - resonanceDecay * Time.deltaTime);

        if (ringTimer <= 0f)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, baseScale, Time.deltaTime * 8f);
            ApplyColor(Color.Lerp(idleColor, ringingColor, Mathf.Clamp01(resonanceEnergy / resonanceRequired) * 0.6f));
            return;
        }

        ringTimer -= Time.deltaTime;
        float pulse = 1f + Mathf.Sin(Time.time * 34f) * 0.08f;
        transform.localScale = baseScale * pulse;
        ApplyColor(ringingColor);
    }

    public void ReceiveMeow(Vector3 source, int note, float range)
    {
        ReceiveMeow(source, note, range, 1f, -1);
    }

    public void ReceiveMeow(Vector3 source, int note, float range, float intensity, int signalId)
    {
        if (signalId == lastSignalId)
        {
            return;
        }

        lastSignalId = signalId;
        float distance = Vector3.Distance(source, transform.position);
        if (distance > range || Time.time < nextRingTime)
        {
            return;
        }

        float distanceGain = Mathf.Clamp01(1f - distance / range);
        bool clearSoundPath = HasLineOfSound(source);
        float obstructionGain = clearSoundPath ? 1f : 0.18f;
        float signalPower = intensity * Mathf.Lerp(0.25f, 1f, distanceGain) * obstructionGain;

        if (note != bellNote)
        {
            resonanceEnergy = Mathf.Max(0f, resonanceEnergy - detunePenalty * signalPower);
            GameUI.ShowMessage("Этот колокольчик ждёт ноту " + ProceduralAudio.GetNoteName(bellNote) + ". Выбери её цифрой 1/2/3 и нажми F.", 3f);
            return;
        }

        resonanceEnergy += signalPower;
        if (!clearSoundPath && signalPower < 0.22f)
        {
            GameUI.ShowMessage("Звук почти глохнет за предметом. Убери его.", 3.5f);
        }

        if (resonanceEnergy >= resonanceRequired)
        {
            Ring(signalId);
        }
    }

    private void Ring(int signalId)
    {
        nextRingTime = Time.time + cooldown;
        ringTimer = 0.55f;
        resonanceEnergy = 0f;
        ProceduralAudio.PlayBell(transform.position, bellNote);
        MelodyGate.ReportBell(bellNote);
        ResonantPlatform.ReportBell(bellNote);

        if (relayIntensity > 0f && relayRange > 0f)
        {
            BellRegistry.BroadcastMeow(transform.position, bellNote, relayRange, relayIntensity, signalId);
        }

        if (particles != null)
        {
            particles.Play();
        }
    }

    private void ApplyColor(Color color)
    {
        if (bellRenderer == null || bellRenderer.material == null)
        {
            return;
        }

        bellRenderer.material.color = color;
        if (bellRenderer.material.HasProperty("_EmissionColor"))
        {
            bellRenderer.material.SetColor("_EmissionColor", color * 1.4f);
        }
    }

    private bool HasLineOfSound(Vector3 source)
    {
        Vector3 target = transform.position + Vector3.up * 0.25f;
        Vector3 origin = source + Vector3.up * 0.25f;
        if (!Physics.Linecast(origin, target, out RaycastHit hit, occlusionMask, QueryTriggerInteraction.Ignore))
        {
            return true;
        }

        return hit.transform == transform || hit.transform.IsChildOf(transform);
    }
}
