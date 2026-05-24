using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public sealed class CacheableObject : MonoBehaviour
{
    [SerializeField] private string displayName = "предмет";
    [SerializeField] private float throwMultiplier = 1f;

    private Rigidbody body;

    public string DisplayName => displayName;
    public float ThrowMultiplier => throwMultiplier;
    public Rigidbody Body => body != null ? body : body = GetComponent<Rigidbody>();

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
    }

    public void Configure(string objectName, float multiplier)
    {
        displayName = objectName;
        throwMultiplier = Mathf.Max(0.2f, multiplier);
    }
}
