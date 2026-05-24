using System.Collections.Generic;
using UnityEngine;

public sealed class HarmonyGate : MonoBehaviour
{
    private static readonly List<HarmonyGate> Gates = new List<HarmonyGate>();

    [SerializeField] private int requiredNote = 2;
    [SerializeField] private int requiredKittens = 2;
    [SerializeField] private Vector3 openOffset = new Vector3(0f, -4.8f, 0f);
    [SerializeField] private float openSpeed = 9f;
    [SerializeField] private string[] legacyPassageBlockerNames =
    {
        "Roof_Final_House_Block",
        "Roof_Final_Parapet_W",
        "Final_Passage_Wall_Seal"
    };

    private Vector3 closedPosition;
    private Vector3 openPosition;
    private bool isOpen;
    private bool collidersStripped;
    private Collider[] gateColliders;

    public int RequiredNote
    {
        get => requiredNote;
        set => requiredNote = Mathf.Abs(value) % ProceduralAudio.NoteNames.Length;
    }

    public int RequiredKittens
    {
        get => requiredKittens;
        set => requiredKittens = Mathf.Max(0, value);
    }

    public static void ReportMeow(Vector3 source, int note, float range)
    {
        for (int i = Gates.Count - 1; i >= 0; i--)
        {
            if (Gates[i] == null)
            {
                Gates.RemoveAt(i);
                continue;
            }

            Gates[i].ReceiveMeow(source, note, range);
        }
    }

    private void Awake()
    {
        closedPosition = transform.position;
        openPosition = closedPosition + openOffset;
        gateColliders = GetComponentsInChildren<Collider>();
    }

    private void OnEnable()
    {
        if (!Gates.Contains(this))
        {
            Gates.Add(this);
        }
    }

    private void OnDisable()
    {
        Gates.Remove(this);
    }

    private void Update()
    {
        Vector3 target = isOpen ? openPosition : closedPosition;
        transform.position = Vector3.MoveTowards(transform.position, target, Time.deltaTime * openSpeed);
        if (isOpen && !collidersStripped)
        {
            StripGateColliders();
        }
    }

    private void ReceiveMeow(Vector3 source, int note, float range)
    {
        if (isOpen || Vector3.Distance(source, transform.position) > range)
        {
            return;
        }

        if (note != requiredNote)
        {
            GameUI.ShowMessage("Финальным воротам нужна нота 3.", 4f);
            return;
        }

        int rescued = CatController.Instance != null ? CatController.Instance.RescuedKittens : 0;
        if (rescued < requiredKittens)
        {
            GameUI.ShowMessage("Нужно доверие: " + rescued + "/" + requiredKittens + " котят.");
            return;
        }

        OpenGate();
        ProceduralAudio.PlaySuccess(transform.position);
        GameUI.ShowMessage("Пройди через ворота к большому оранжевому окну... Дом близко...", 7f);
        StoryDirector.NotifyFinalGateOpened();
    }

    private void OpenGate()
    {
        isOpen = true;
        StripGateColliders();
        ClearLegacyPassageBlockers();
    }

    private void ClearLegacyPassageBlockers()
    {
        if (legacyPassageBlockerNames == null)
        {
            return;
        }

        for (int i = 0; i < legacyPassageBlockerNames.Length; i++)
        {
            string objectName = legacyPassageBlockerNames[i];
            if (string.IsNullOrWhiteSpace(objectName))
            {
                continue;
            }

            GameObject blocker = GameObject.Find(objectName);
            if (blocker == null)
            {
                continue;
            }

            DisableColliders(blocker);
        }
    }

    private static void DisableColliders(GameObject target)
    {
        Collider[] colliders = target.GetComponentsInChildren<Collider>(true);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i] == null)
            {
                continue;
            }

            colliders[i].enabled = false;
            Object.Destroy(colliders[i]);
        }
    }

    private void StripGateColliders()
    {
        DisableColliders(gameObject);

        Rigidbody[] bodies = GetComponentsInChildren<Rigidbody>(true);
        for (int i = 0; i < bodies.Length; i++)
        {
            if (bodies[i] == null)
            {
                continue;
            }

            bodies[i].isKinematic = true;
            Destroy(bodies[i]);
        }

        gateColliders = System.Array.Empty<Collider>();
        collidersStripped = true;
    }
}
