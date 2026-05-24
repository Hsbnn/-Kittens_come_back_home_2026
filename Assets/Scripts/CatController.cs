using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public sealed class CatController : MonoBehaviour
{
    public static CatController Instance { get; private set; }

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6.4f;
    [SerializeField] private float accelerationTime = 0.05f;
    [SerializeField] private float decelerationTime = 0.08f;
    [SerializeField] private float turnSpeed = 18f;
    [SerializeField] private float jumpHeight = 1.25f;
    [SerializeField] private float gravity = -24f;

    [Header("Sound Abilities")]
    [SerializeField] private float meowRange = 8f;
    [SerializeField] private float meowCooldown = 0.45f;
    [SerializeField] private float chorusRangeBonus = 1.6f;
    [SerializeField] private float chorusIntensityBonus = 0.28f;
    [SerializeField] private float purrRange = 4.2f;
    [SerializeField] private float purrBpm = 92f;

    private CharacterController controller;
    private Vector3 verticalVelocity;
    private Vector3 horizontalVelocity;
    private Vector3 horizontalVelocitySmoothing;
    private int currentNote;
    private float nextMeowTime;
    private float nextPurrTickTime;
    private int rescuedKittens;

    public int CurrentNote => currentNote;
    public string CurrentNoteName => ProceduralAudio.GetNoteName(currentNote);
    public int RescuedKittens => rescuedKittens;
    public float PurrBeatScore { get; private set; }
    public bool IsPurring { get; private set; }

    private void Awake()
    {
        Instance = this;
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        HandleMovement();
        HandleNotes();
        HandleMeow();
        HandlePurr();
    }

    public void RegisterRescuedKitten()
    {
        rescuedKittens++;
        GameUI.ShowMessage("Котёнок доверяет тебе.");
        StoryDirector.NotifyKittenRescued(rescuedKittens, 2);
    }

    private void HandleMovement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Transform cameraTransform = Camera.main != null ? Camera.main.transform : null;
        Vector3 forward = cameraTransform != null ? cameraTransform.forward : Vector3.forward;
        Vector3 right = cameraTransform != null ? cameraTransform.right : Vector3.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 inputMove = Vector3.ClampMagnitude(forward * vertical + right * horizontal, 1f);
        Vector3 targetHorizontalVelocity = inputMove * moveSpeed;
        float smoothingTime = inputMove.sqrMagnitude > 0.01f ? accelerationTime : decelerationTime;
        horizontalVelocity = Vector3.SmoothDamp(horizontalVelocity, targetHorizontalVelocity, ref horizontalVelocitySmoothing, smoothingTime);

        Vector3 facingDirection = horizontalVelocity.sqrMagnitude > 0.05f ? horizontalVelocity.normalized : inputMove;
        if (facingDirection.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(facingDirection, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        }

        if (controller.isGrounded && verticalVelocity.y < 0f)
        {
            verticalVelocity.y = -2f;
        }

        if (Input.GetButtonDown("Jump") && controller.isGrounded)
        {
            verticalVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        verticalVelocity.y += gravity * Time.deltaTime;
        controller.Move((horizontalVelocity + verticalVelocity) * Time.deltaTime);
    }

    private void HandleNotes()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetNote(0);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SetNote(1);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SetNote(2);
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            currentNote = (currentNote + ProceduralAudio.NoteNames.Length - 1) % ProceduralAudio.NoteNames.Length;
            ShowNoteHint();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            currentNote = (currentNote + 1) % ProceduralAudio.NoteNames.Length;
            ShowNoteHint();
        }
    }

    private void HandleMeow()
    {
        if (!Input.GetKeyDown(KeyCode.F))
        {
            return;
        }

        if (Time.time < nextMeowTime)
        {
            return;
        }

        nextMeowTime = Time.time + meowCooldown;
        float chorusRange = meowRange + rescuedKittens * chorusRangeBonus;
        float chorusIntensity = 1f + rescuedKittens * chorusIntensityBonus;
        ProceduralAudio.PlayMeow(transform.position, currentNote);
        BellRegistry.BroadcastMeow(transform.position, currentNote, chorusRange, chorusIntensity);
        GameUI.ShowMessage("Мяу: нота " + CurrentNoteName + ", x" + chorusIntensity.ToString("0.0"));
        StoryDirector.NotifyMeow(CurrentNoteName);
    }

    private void SetNote(int note)
    {
        currentNote = Mathf.Clamp(note, 0, ProceduralAudio.NoteNames.Length - 1);
        ShowNoteHint();
    }

    private void ShowNoteHint()
    {
        GameUI.ShowMessage("Выбрана нота " + CurrentNoteName);
    }

    private void HandlePurr()
    {
        IsPurring = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        if (!IsPurring)
        {
            PurrBeatScore = 0f;
            return;
        }

        float beatPhase = Mathf.Repeat(Time.time * purrBpm / 60f, 1f);
        float distanceToBeat = Mathf.Min(beatPhase, 1f - beatPhase);
        PurrBeatScore = Mathf.InverseLerp(0.24f, 0.02f, distanceToBeat);
        PurrRegistry.BroadcastPurr(transform.position, purrRange, PurrBeatScore);

        if (Time.time >= nextPurrTickTime)
        {
            nextPurrTickTime = Time.time + 0.35f;
            ProceduralAudio.PlayPurr(transform.position);
        }
    }
}
