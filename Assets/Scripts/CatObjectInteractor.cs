using System.Collections.Generic;
using UnityEngine;

public sealed class CatObjectInteractor : MonoBehaviour
{
    public static CatObjectInteractor Instance { get; private set; }

    [SerializeField] private float interactDistance = 5f;
    [SerializeField] private float holdDistance = 2.4f;
    [SerializeField] private float holdStrength = 12f;
    [SerializeField] private float maxHoldVelocity = 9f;
    [SerializeField] private float throwImpulse = 3.2f;
    [SerializeField] private LayerMask interactionMask = ~0;

    private readonly List<CacheEntry> cache = new List<CacheEntry>();
    private CacheableObject heldObject;
    private bool heldObjectUsedGravity;

    public int CacheCount => cache.Count;
    public string HeldObjectName => heldObject != null ? heldObject.DisplayName : string.Empty;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            ToggleHold();
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            DropHeldObjectInFront();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            CacheTargetOrHeldObject();
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            RestoreCachedObject();
        }
    }

    private void FixedUpdate()
    {
        if (heldObject == null)
        {
            return;
        }

        Rigidbody body = heldObject.Body;
        Vector3 target = GetAimOrigin() + GetAimDirection() * holdDistance;
        Vector3 desiredVelocity = (target - body.position) * holdStrength;
        body.linearVelocity = Vector3.ClampMagnitude(desiredVelocity, maxHoldVelocity);
        body.angularVelocity *= 0.85f;
    }

    private void ToggleHold()
    {
        if (heldObject != null)
        {
            ReleaseHeldObject(false);
            return;
        }

        CacheableObject target = FindTargetObject();
        if (target == null)
        {
            GameUI.ShowMessage("Рядом нет предмета, который можно схватить.");
            return;
        }

        heldObject = target;
        Rigidbody body = heldObject.Body;
        heldObjectUsedGravity = body.useGravity;
        body.useGravity = false;
        body.linearDamping = 4f;
        body.angularDamping = 6f;
        GameUI.ShowMessage("Кот держит " + heldObject.DisplayName + ". Отойди в сторону, чтобы аккуратно положить его.");
        StoryDirector.NotifyObjectMoved(heldObject.DisplayName);
    }

    private void DropHeldObjectInFront()
    {
        if (heldObject == null)
        {
            return;
        }

        CacheableObject dropped = heldObject;
        Rigidbody body = dropped.Body;
        ReleaseHeldObject(false);
        body.linearVelocity = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized * throwImpulse * Mathf.Min(dropped.ThrowMultiplier, 1.2f);
        body.angularVelocity = Vector3.zero;
        ProceduralAudio.PlayPurr(transform.position);
        GameUI.ShowMessage("Предмет отодвинут: " + dropped.DisplayName);
        StoryDirector.NotifyObjectMoved(dropped.DisplayName);
    }

    private void CacheTargetOrHeldObject()
    {
        CacheableObject target = heldObject != null ? heldObject : FindTargetObject();
        if (target == null)
        {
            GameUI.ShowMessage("Встань рядом, повернись к предмету, чтобы убрать его.");
            return;
        }

        if (target == heldObject)
        {
            ReleaseHeldObject(true);
        }

        Rigidbody body = target.Body;
        cache.Add(new CacheEntry(target.gameObject, body.useGravity));
        body.linearVelocity = Vector3.zero;
        body.angularVelocity = Vector3.zero;
        target.gameObject.SetActive(false);
        ProceduralAudio.PlayPurr(transform.position);
        GameUI.ShowMessage("В кэш убран предмет: " + target.DisplayName + ". T - восстановить.");
        StoryDirector.NotifyObjectCached(target.DisplayName);
    }

    private void RestoreCachedObject()
    {
        if (cache.Count == 0)
        {
            GameUI.ShowMessage("Кеш пуст: сначала убери предмет клавишей R.");
            return;
        }

        CacheEntry entry = cache[cache.Count - 1];
        cache.RemoveAt(cache.Count - 1);

        Vector3 position = FindRestorePosition();
        entry.Object.SetActive(true);
        entry.Object.transform.SetPositionAndRotation(position, Quaternion.LookRotation(GetAimDirection(), Vector3.up));

        Rigidbody body = entry.Object.GetComponent<Rigidbody>();
        if (body != null)
        {
            body.useGravity = entry.UsedGravity;
            body.linearVelocity = Vector3.zero;
            body.angularVelocity = Vector3.zero;
        }

        ProceduralAudio.PlayBell(position, 4);
        GameUI.ShowMessage("Предмет восстановлен из кэша. Осталось в кэше: " + cache.Count);
    }

    private void ReleaseHeldObject(bool keepSleeping)
    {
        if (heldObject == null)
        {
            return;
        }

        Rigidbody body = heldObject.Body;
        body.useGravity = heldObjectUsedGravity;
        body.linearDamping = 0.2f;
        body.angularDamping = 0.05f;
        if (keepSleeping)
        {
            body.linearVelocity = Vector3.zero;
            body.angularVelocity = Vector3.zero;
        }

        heldObject = null;
    }

    private CacheableObject FindTargetObject()
    {
        Ray ray = new Ray(GetAimOrigin(), GetAimDirection());
        if (Physics.SphereCast(ray, 0.45f, out RaycastHit hit, interactDistance, interactionMask, QueryTriggerInteraction.Ignore))
        {
            CacheableObject aimedObject = hit.collider.GetComponentInParent<CacheableObject>();
            if (aimedObject != null && IsReachableAndInFront(aimedObject))
            {
                return aimedObject;
            }
        }

        return null;
    }

    private bool IsReachableAndInFront(CacheableObject candidate)
    {
        if (candidate == null || !candidate.gameObject.activeInHierarchy)
        {
            return false;
        }

        Vector3 toCandidate = candidate.transform.position - transform.position;
        toCandidate.y = 0f;
        if (toCandidate.magnitude > interactDistance)
        {
            return false;
        }

        Vector3 forward = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
        return Vector3.Dot(forward, toCandidate.normalized) > 0.35f;
    }

    private Vector3 FindRestorePosition()
    {
        Ray ray = new Ray(GetAimOrigin(), GetAimDirection());
        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance + 2f, interactionMask, QueryTriggerInteraction.Ignore))
        {
            return hit.point + hit.normal * 0.65f;
        }

        return transform.position + transform.forward * holdDistance + Vector3.up * 0.8f;
    }

    private Vector3 GetAimOrigin()
    {
        return transform.position + Vector3.up * 0.8f;
    }

    private Vector3 GetAimDirection()
    {
        if (Camera.main == null)
        {
            return (transform.forward + Vector3.up * 0.15f).normalized;
        }

        return (transform.forward + Vector3.up * 0.1f).normalized;
    }

    private readonly struct CacheEntry
    {
        public readonly GameObject Object;
        public readonly bool UsedGravity;

        public CacheEntry(GameObject cachedObject, bool usedGravity)
        {
            Object = cachedObject;
            UsedGravity = usedGravity;
        }
    }
}
