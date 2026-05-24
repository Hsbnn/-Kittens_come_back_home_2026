using System.Collections.Generic;
using UnityEngine;

public sealed class ResonantPlatform : MonoBehaviour
{
    private static readonly List<ResonantPlatform> Platforms = new List<ResonantPlatform>();

    [SerializeField] private int triggerNote;
    [SerializeField] private Vector3 activeOffset = new Vector3(0f, 0f, 3f);
    [SerializeField] private Vector3 activeRotation = new Vector3(0f, 0f, 0f);
    [SerializeField] private float activeDuration = 6f;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private Color idleColor = new Color(0.22f, 0.24f, 0.32f);
    [SerializeField] private Color activeColor = new Color(0.45f, 0.72f, 1f);

    private Vector3 idlePosition;
    private Quaternion idleRotation;
    private Renderer[] renderers;
    private float activeUntil;

    public int TriggerNote
    {
        get => triggerNote;
        set => triggerNote = Mathf.Abs(value) % ProceduralAudio.NoteNames.Length;
    }

    public Vector3 ActiveOffset
    {
        get => activeOffset;
        set => activeOffset = value;
    }

    public Vector3 ActiveRotation
    {
        get => activeRotation;
        set => activeRotation = value;
    }

    public static void ReportBell(int note)
    {
        for (int i = Platforms.Count - 1; i >= 0; i--)
        {
            if (Platforms[i] == null)
            {
                Platforms.RemoveAt(i);
                continue;
            }

            Platforms[i].ReceiveBell(note);
        }
    }

    private void Awake()
    {
        idlePosition = transform.position;
        idleRotation = transform.rotation;
        renderers = GetComponentsInChildren<Renderer>();
        ApplyColor(idleColor);
    }

    private void OnEnable()
    {
        if (!Platforms.Contains(this))
        {
            Platforms.Add(this);
        }
    }

    private void OnDisable()
    {
        Platforms.Remove(this);
    }

    private void Update()
    {
        bool active = Time.time < activeUntil;
        Vector3 targetPosition = active ? idlePosition + activeOffset : idlePosition;
        Quaternion targetRotation = active ? idleRotation * Quaternion.Euler(activeRotation) : idleRotation;

        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * moveSpeed);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * moveSpeed);
        ApplyColor(Color.Lerp(idleColor, activeColor, active ? 0.85f : 0f));
    }

    private void ReceiveBell(int note)
    {
        if (note != triggerNote)
        {
            activeUntil = Mathf.Min(activeUntil, Time.time + 0.5f);
            return;
        }

        activeUntil = Time.time + activeDuration;
        GameUI.ShowMessage("Мир настроился на " + ProceduralAudio.GetNoteName(note));
    }

    private void ApplyColor(Color color)
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] == null || renderers[i].material == null)
            {
                continue;
            }

            renderers[i].material.color = color;
            if (renderers[i].material.HasProperty("_EmissionColor"))
            {
                renderers[i].material.SetColor("_EmissionColor", color * 0.8f);
            }
        }
    }
}
