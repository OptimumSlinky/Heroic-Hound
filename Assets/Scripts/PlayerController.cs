using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;

/// <summary>
/// Primary control class to handle player inputs and move/animate the player character
/// </summary>
public class PlayerController : MonoBehaviour
{
    // Unity systems references
    Animator animator;
    PlayerInput input;
    CharacterController playerController;

    private Vector3 _currentMovement;
    private Vector2 _inputMovement;

    [SerializeField] private float _walkMultiplier;
    [SerializeField] private float _runMultiplier;
    [SerializeField] private float _blockModifier;
    [SerializeField] private float _rotationMultiplier;

    // Bools for tracking current player state
    private bool _playerIsMoving;
    private bool _playerIsRunning;
    private bool _playerIsBlocking;

    // Integers for animator hashing
    private int _walkingHash;
    private int _runningHash;
    private int _attackHash;
    private int _blockHash;

    // Sets player input and controller objects
    // Sets up animator hashes
    private void Awake()
    {
        input = new PlayerInput();
        playerController = GetComponent<CharacterController>();
        SetUpAnimatorFlags();
    }

    // Start is called before the first frame update
    // Subscribes to Unity Input System events
    void Start()
    {
        // Player movement
        input.PlayerControls.Movement.started += OnMovement;
        input.PlayerControls.Movement.canceled += OnMovement;
        input.PlayerControls.Movement.performed += OnMovement;

        // Run modifier handling
        input.PlayerControls.Run.started += OnRun;
        input.PlayerControls.Run.canceled += OnRun;

        // Player attack
        input.PlayerControls.Attack.started += OnAttack;

        // Player block
        input.PlayerControls.Attack.started += OnBlock;
        input.PlayerControls.Attack.canceled += OnBlock;

        // Player interact
        input.PlayerControls.Interact.started += OnInteract;
    }

    // Update player state
    void Update()
    {
        OnRotate();
        AnimatePlayer();

        if (_playerIsRunning)
        {
            playerController.Move((_currentMovement * _runMultiplier) * Time.deltaTime);
        }

        if (_playerIsBlocking)
        {
            playerController.Move((_currentMovement * _blockModifier) * Time.deltaTime);
        }

        else
        {
            playerController.Move((_currentMovement * _walkMultiplier) * Time.deltaTime);
        }

        HandleGravity();
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

        if (_playerIsMoving && !playerWalking)
        {
            animator.SetBool(_walkingHash, true);
        }

        if (!_playerIsMoving && playerWalking)
        {
            animator.SetBool(_walkingHash, false);
        }

        if ((_playerIsMoving && _playerIsRunning) && !playerRunning)
        {
            animator.SetBool(_runningHash, true);
        }

        if ((!_playerIsMoving && !_playerIsRunning) && playerRunning)
        {
            animator.SetBool(_runningHash, false);
        }
    }

    // Receives event context from the Unity Input System and moves the player
    public void OnMovement(InputAction.CallbackContext value)
    {
        _inputMovement = value.ReadValue<Vector2>();
        _currentMovement = new Vector3(_inputMovement.x, 0, _inputMovement.y);
        _playerIsMoving = _currentMovement.x != 0 || _currentMovement.z != 0;
    }

    // Handles rotation of the player character
    void OnRotate()
    {
        // Get current player rotation
        Quaternion currentRotation = transform.rotation;

        // Set new direction to turn towards
        Vector3 turnDirection = new Vector3(_currentMovement.x, 0, _currentMovement.z);

        // Verify movement has been pressed and rotate the character towards that new direction
        if (_playerIsMoving)
        {
            Quaternion endRotation = Quaternion.LookRotation(turnDirection);
            transform.rotation = Quaternion.Slerp(currentRotation, endRotation, _rotationMultiplier * Time.deltaTime);
        }
    }

    // Receives event context from the Unity Input System and engages the run modifier
    public void OnRun(InputAction.CallbackContext context)
    {
        _playerIsRunning = context.ReadValueAsButton();
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
        _playerIsBlocking = animator.GetBool(_blockHash);

        if (context.started && !_playerIsBlocking)
        {
            animator.SetBool(_blockHash, true);
        }

        else if (context.canceled && _playerIsBlocking)
        {
            animator.SetBool(_blockHash, false);
            _playerIsBlocking = false;
        }
    }

    // Receives event context from the Unity Input System and triggers attack animation
    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            // Interaction logic here
        }
    }

    // Make sure the player doesn't fly
    private void HandleGravity()
    {
        if (playerController.isGrounded)
        {
            float groundedGravity = -0.05f;
            _currentMovement.y = groundedGravity;
        }

        else
        {
            float actualGravity = -9.8f;
            _currentMovement.y = actualGravity;
        }
    }

    private void OnEnable()
    {
        input.PlayerControls.Enable();
    }

    private void OnDisable()
    {
        input.PlayerControls.Disable();
    }

}
