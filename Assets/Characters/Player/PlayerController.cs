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

    // 
    private Vector3 _currentMovement;
    private float _rotationMultiplier = 5.0f;
    [SerializeField] private float _walkMultiplier = 1.5f;
    [SerializeField] private float _runMultiplier = 3.0f;

    private bool _isPlayerMoving;
    private bool _isPlayerRunning;
    
    private int _walkingHash;
    private int _runningHash;
    private int _attackingHash;
    private int _blockingHash;

    private void Awake()
    {
        playerInput = new PlayerInput();
        playerController = GetComponent<CharacterController>();
        SetUpAnimationFlags();
    }

    // Start is called before the first frame update
    void Start()
    {
        // Basic movement handling
        playerInput.PlayerControls.Movement.started += OnMovement;
        playerInput.PlayerControls.Movement.canceled += OnMovement;
        playerInput.PlayerControls.Movement.performed += OnMovement;

        // Run modifier handling
        playerInput.PlayerControls.Run.started += OnRun;
        playerInput.PlayerControls.Run.canceled += OnRun;
    }

    void Update()
    {
        OnRotate();
        playerController.Move(_currentMovement * Time.deltaTime);
        AnimatePlayer();
    }

    // 
    void SetUpAnimationFlags()
    {
        animator = GetComponent<Animator>();
        _walkingHash = Animator.StringToHash("walking");
        _runningHash = Animator.StringToHash("running");
        _attackingHash = Animator.StringToHash("attacking");
        _blockingHash = Animator.StringToHash("blocking");
    }

    //
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

    //
    public void OnMovement(InputAction.CallbackContext value)
    {
        Vector2 inputMovement = value.ReadValue<Vector2>();
        if (_isPlayerRunning)
        {
            _currentMovement = new Vector3(inputMovement.x, 0, inputMovement.y) * _runMultiplier;
        }

        else
        {
            _currentMovement = new Vector3(inputMovement.x, 0, inputMovement.y) * _walkMultiplier;
            _isPlayerMoving = _currentMovement.x != 0 || _currentMovement.z != 0;
        }  
    }

    // 
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

    public void OnRun(InputAction.CallbackContext context)
    {
        _isPlayerRunning = context.ReadValueAsButton();
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
