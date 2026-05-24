using UnityEngine;

public sealed class KittenRescue : MonoBehaviour
{
    public static float VisibleCalmingProgress { get; private set; }
    public static float VisibleCalmingUntil { get; private set; }

    [SerializeField] private float trustRequired = 3.2f;
    [SerializeField] private float rescueRange = 3.2f;
    [SerializeField] private float followDistance = 1.4f;
    [SerializeField] private float followSpeed = 3.8f;
    [SerializeField] private Color scaredColor = new Color(0.55f, 0.55f, 0.65f);
    [SerializeField] private Color rescuedColor = new Color(1f, 0.86f, 0.55f);

    private Renderer[] renderers;
    private float trust;
    private bool rescued;
    private Vector3 followOffset;
    private float nextHintTime;

    private void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>();
        followOffset = new Vector3(Random.Range(-1.2f, 1.2f), 0f, Random.Range(-1.2f, 1.2f)).normalized * followDistance;
        ApplyColor(scaredColor);
    }

    private void OnEnable()
    {
        PurrRegistry.Register(this);
    }

    private void OnDisable()
    {
        PurrRegistry.Unregister(this);
    }

    private void Update()
    {
        if (!rescued || CatController.Instance == null)
        {
            return;
        }

        Vector3 target = CatController.Instance.transform.position + followOffset;
        target.y = transform.position.y;
        transform.position = Vector3.MoveTowards(transform.position, target, followSpeed * Time.deltaTime);

        Vector3 direction = CatController.Instance.transform.position - transform.position;
        direction.y = 0f;
        if (direction.sqrMagnitude > 0.05f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 8f);
        }
    }

    public void ReceivePurr(Vector3 source, float range, float beatScore)
    {
        if (rescued)
        {
            return;
        }

        float distance = Vector3.Distance(source, transform.position);
        float effectiveRange = Mathf.Min(range, rescueRange);
        if (distance > effectiveRange)
        {
            return;
        }

        float distanceFactor = Mathf.Clamp01(1f - distance / effectiveRange);
        float closeBonus = Mathf.SmoothStep(0.35f, 1f, distanceFactor);
        trust = Mathf.Min(trustRequired, trust + Time.deltaTime * Mathf.Lerp(0.85f, 2.45f, closeBonus));
        VisibleCalmingProgress = Mathf.Clamp01(trust / trustRequired);
        VisibleCalmingUntil = Time.time + 0.35f;

        if (Time.time >= nextHintTime)
        {
            nextHintTime = Time.time + 0.75f;
            GameUI.ShowMessage("Котёнок успокаивается: " + Mathf.RoundToInt(VisibleCalmingProgress * 100f) + "%. Стой рядом...", 1f);
        }

        ApplyColor(Color.Lerp(scaredColor, rescuedColor, VisibleCalmingProgress));

        if (trust >= trustRequired)
        {
            rescued = true;
            VisibleCalmingProgress = 1f;
            VisibleCalmingUntil = Time.time + 2f;
            ApplyColor(rescuedColor);
            ProceduralAudio.PlaySuccess(transform.position);
            if (CatController.Instance != null)
            {
                CatController.Instance.RegisterRescuedKitten();
            }
        }
    }

    private void ApplyColor(Color color)
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] != null && renderers[i].material != null)
            {
                renderers[i].material.color = color;
            }
        }
    }
}
