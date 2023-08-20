using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Primary control class to handle player inputs and move/animate the player character
/// </summary>
public class PlayerController : MonoBehaviour
{
    // Unity systems references
    Animator animator;
    PlayerInput playerInput;
    CharacterController playerController;

    private Vector3 _currentMovement;
    private Vector2 _inputMovement;

    [SerializeField] private float _walkMultiplier = 1.5f;
    [SerializeField] private float _runMultiplier = 3.0f;
    [SerializeField] private float _rotationMultiplier = 5.0f;

    private bool _isPlayerMoving;
    private bool _isPlayerRunning;
    private bool _isPlayerBlocking;

    private int _walkingHash;
    private int _runningHash;
    private int _attackHash;
    private int _blockHash;

    // Sets player input and controller objects
    // Sets up animator hashes
    private void Awake()
    {
        playerInput = new PlayerInput();
        playerController = GetComponent<CharacterController>();
        SetUpAnimatorFlags();
    }

    // Start is called before the first frame update
    // Subscribes to Unity Input System events
    void Start()
    {
        // Player movement
        playerInput.PlayerControls.Movement.started += OnMovement;
        playerInput.PlayerControls.Movement.canceled += OnMovement;
        playerInput.PlayerControls.Movement.performed += OnMovement;

        // Run modifier handling
        playerInput.PlayerControls.Run.started += OnRun;
        playerInput.PlayerControls.Run.canceled += OnRun;

        // Player attack
        playerInput.PlayerControls.Attack.started += OnAttack;
        playerInput.PlayerControls.Attack.canceled += OnAttack;

        // Player block
        playerInput.PlayerControls.Attack.started += OnBlock;
        playerInput.PlayerControls.Attack.canceled += OnBlock;
    }

    // Update player state
    void Update()
    {
        playerController.Move(_currentMovement * Time.deltaTime);
        OnRotate();
        AnimatePlayer();
    }

    // Sets up hashes for animator optimization
    void SetUpAnimatorFlags()
    {
        animator = GetComponent<Animator>();
        _walkingHash = Animator.StringToHash("walking");
        _runningHash = Animator.StringToHash("running");
        _attackHash = Animator.StringToHash("attack");
        _blockHash = Animator.StringToHash("block");
    }

    // Gets the current state of the Animator parameters and animates the player
    void AnimatePlayer()
    {
        bool playerWalking = animator.GetBool(_walkingHash);
        bool playerRunning = animator.GetBool(_runningHash);

        if (_isPlayerMoving && !playerWalking)
        {
            animator.SetBool(_walkingHash, true);
        }

        if (!_isPlayerMoving && playerWalking)
        {
            animator.SetBool(_walkingHash, false);
        }

        if ((_isPlayerMoving && _isPlayerRunning) && !playerRunning)
        {
            animator.SetBool(_runningHash, true);
        }

        if ((!_isPlayerMoving && !_isPlayerRunning) && playerRunning)
        {
            animator.SetBool(_runningHash, false);
        }
    }

    // Receives event context from the Unity Input System and moves the player
    public void OnMovement(InputAction.CallbackContext value)
    {
        _inputMovement = value.ReadValue<Vector2>();
        _currentMovement = new Vector3(_inputMovement.x, 0, _inputMovement.y) * _walkMultiplier;
        _isPlayerMoving = _currentMovement.x != 0 || _currentMovement.z != 0;
    }

    // Handles rotation of the player character
    void OnRotate()
    {
        // Get current player rotation
        Quaternion currentRotation = transform.rotation;

        // Set new direction to turn towards
        Vector3 turnDirection = new Vector3(_currentMovement.x, 0, _currentMovement.z);

        // Verify movement has been pressed and rotate the character towards that new direction
        if (_isPlayerMoving)
        {
            Quaternion endRotation = Quaternion.LookRotation(turnDirection);
            transform.rotation = Quaternion.Slerp(currentRotation, endRotation, _rotationMultiplier * Time.deltaTime);
        }
    }

    // Receives event context from the Unity Input System and engages the run modifier
    public void OnRun(InputAction.CallbackContext context)
    {
        _isPlayerRunning = context.ReadValueAsButton();

        if (_isPlayerRunning)
        {
            _currentMovement = new Vector3(_inputMovement.x, 0, _inputMovement.y) * _runMultiplier;
            _isPlayerMoving = _currentMovement.x != 0 || _currentMovement.z != 0;
        }
    }

    // Receives event context from the Unity Input System and triggers attack animation
    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            animator.SetTrigger(_attackHash);
        }
    }

    // Receives event context from the Unity Input System and triggers blocking animation/state
    public void OnBlock(InputAction.CallbackContext context)
    {
        _isPlayerBlocking = animator.GetBool(_blockHash);

        if (context.started && !_isPlayerBlocking)
        {
            animator.SetBool(_blockHash, true);
        }

        else if (context.canceled && _isPlayerBlocking)
        {
            animator.SetBool(_blockHash, false);
        }
    }

    private void OnEnable()
    {
        playerInput.PlayerControls.Enable();
    }

    private void OnDisable()
    {
        playerInput.PlayerControls.Disable();
    }

}
