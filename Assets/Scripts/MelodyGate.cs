using System.Collections.Generic;
using UnityEngine;

public sealed class MelodyGate : MonoBehaviour
{
    private static readonly int[] DefaultMelody = { 0, 1, 2 };
    private static readonly List<MelodyGate> Gates = new List<MelodyGate>();

    [SerializeField] private int[] melody = { 0, 1, 2 };
    [SerializeField] private Vector3 openOffset = new Vector3(0f, -3.6f, 0f);
    [SerializeField] private float openSpeed = 9f;
    [SerializeField] private string[] legacyPassageBlockerNames =
    {
        "Roof_Melody_Parapet_E",
        "Roof_Garden_Parapet_W",
        "Melody_Passage_Wall_Seal"
    };

    private Vector3 closedPosition;
    private Vector3 openPosition;
    private int melodyIndex;
    private bool isOpen;
    private bool collidersStripped;
    private Collider[] gateColliders;

    public int[] Melody
    {
        get => melody;
        set
        {
            melody = value;
            NormalizeMelody();
        }
    }

    public static void ReportBell(int note)
    {
        for (int i = Gates.Count - 1; i >= 0; i--)
        {
            if (Gates[i] == null)
            {
                Gates.RemoveAt(i);
                continue;
            }

            Gates[i].ReceiveBell(note);
        }
    }

    private void Awake()
    {
        NormalizeMelody();
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

    private void ReceiveBell(int note)
    {
        if (isOpen || melody == null || melody.Length == 0)
        {
            return;
        }

        note = NormalizeNote(note);
        if (melodyIndex >= melody.Length)
        {
            return;
        }

        if (note == NormalizeNote(melody[melodyIndex]))
        {
            melodyIndex++;
            string next = melodyIndex < melody.Length ? ". Следующая: " + ProceduralAudio.GetNoteName(melody[melodyIndex]) : ".";
            GameUI.ShowMessage("Верная нота ворот: " + melodyIndex + "/" + melody.Length + next);
        }
        else
        {
            melodyIndex = note == NormalizeNote(melody[0]) ? 1 : 0;
            GameUI.ShowMessage("Мелодия сбилась. Нужен порядок... Начни с 1.");
        }

        if (melodyIndex >= melody.Length)
        {
            OpenGate();
            ProceduralAudio.PlaySuccess(transform.position);
            GameUI.ShowMessage("Мелодия 1 -> 2 -> 3 собрана. Ворота открылись.");
            StoryDirector.NotifyMelodyGateOpened();
        }
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

    private void NormalizeMelody()
    {
        if (melody == null || melody.Length == 0)
        {
            melody = (int[])DefaultMelody.Clone();
            return;
        }

        for (int i = 0; i < melody.Length; i++)
        {
            melody[i] = NormalizeNote(melody[i]);
        }
    }

    private static int NormalizeNote(int note)
    {
        return Mathf.Abs(note) % ProceduralAudio.NoteNames.Length;
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
