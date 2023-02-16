using UnityEngine;
using System.Collections;

public class PlayerScripts : MonoBehaviour
{
    private Transform ball;

    // movement config
    private float runSpeed = 5f;
    private float jumpHeight = 1.5f;
    private float gravity = -30f;
    private float groundDamping = 20f; // how fast do we change direction? higher means faster
	private float inAirDamping = 5f;

    [HideInInspector]
	private float normalizedHorizontalSpeed = 0;

	private CharacterController2D _controller;
	private Animator _animator;
	private RaycastHit2D _lastControllerColliderHit;
	private Vector3 _velocity;

    [HideInInspector]
    public Sprite[] myFace;
    [HideInInspector]
    public Sprite myBody, myShoes;
    private SpriteRenderer myHeadSp, myBodySp, myRShoe, myLShoe;

    private GameManager gm;

    public bool isPlayer2;
    public bool isAi;

    private LastAction lastAction;
    public enum LastAction
    {
        STAY,
        MOVE,
        JUMP,
        KICK,
        HEAD
    };

    void Awake()
    {
        _animator = GetComponent<Animator>();
        _controller = GetComponent<CharacterController2D>();
        ball = GameObject.FindGameObjectWithTag("Ball").transform;
        gm = FindObjectOfType<GameManager>();
    }

    void Start()
	{
        // listen to some events for illustration purposes
        _controller.onControllerCollidedEvent += onControllerCollider;
		_controller.onTriggerEnterEvent += onTriggerEnterEvent;
		_controller.onTriggerExitEvent += onTriggerExitEvent;

        _controller.SetKickingFalse();

        if (isAi)
        {
            runSpeed += 4 + PlayerPrefs.GetInt(VariablesName.AILevel, 1);
        }
    }

    public void Init()
    {
        myHeadSp = transform.GetChild(0).GetComponent<SpriteRenderer>();
        myBodySp = transform.GetChild(2).GetComponent<SpriteRenderer>();
        myRShoe = transform.GetChild(3).GetComponent<SpriteRenderer>();
        myLShoe = transform.GetChild(4).GetComponent<SpriteRenderer>();

        myHeadSp.sprite = myFace[0];
        myBodySp.sprite = myBody;
        myRShoe.sprite = myLShoe.sprite = myShoes;
    }


	#region Event Listeners

	void onControllerCollider( RaycastHit2D hit )
	{
        // bail out on plain old ground hits cause they arent very interesting
        if (hit.normal.y == 1f)
            return;

        // logs any collider hits if uncommented. it gets noisy so it is commented out for the demo
        // Debug.Log( "flags: " + _controller.collisionState + ", hit.normal: " + hit.normal );

        //if (_controller.collisionState.above)
        //{
        //    print(hit.collider.name);
        //}
	}


	void onTriggerEnterEvent( Collider2D col )
	{
        //Debug.Log( "onTriggerEnterEvent: " + col.gameObject.name );

        if (col.name == "Ball" && gm.gameMode == GameMode.PLAY)
        {
            StartCoroutine(ChangeFace());
        }
    }

    void onTriggerExitEvent( Collider2D col )
	{
        //Debug.Log( "onTriggerExitEvent: " + col.gameObject.name );
    }

    #endregion

    // the Update loop contains a very simple example of moving the character around and controlling the animation
    void LateUpdate()
	{
		// grab our current _velocity to use as a base for all calculations
		_velocity = _controller.velocity;

        if ( _controller.isGrounded )
			_velocity.y = 0;

        if (!isAi)
        {
            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                int touchCount = Input.touchCount;

                if (touchCount > 0 && gm.gameMode != GameMode.END)
                {
                    for (int i = 0; i < touchCount; i++)
                    {
                        if (Input.GetTouch(i).phase == TouchPhase.Began ||
                            Input.GetTouch(i).phase == TouchPhase.Stationary ||
                            Input.GetTouch(i).phase == TouchPhase.Moved)
                        {
                            RaycastHit2D hit = Physics2D.Raycast(gm.cameraButton.ScreenToWorldPoint(Input.GetTouch(i).position), Vector2.zero);

                            if (hit.collider != null)
                            {
                                if (hit.collider.gameObject.name.Contains("RightBtn"))
                                {
                                    MoveRight();
                                }
                                else if (hit.collider.gameObject.name.Contains("LeftBtn"))
                                {
                                    MoveLeft();
                                }
                                else
                                {
                                    Stay();
                                }

                                if (hit.collider.gameObject.name.Contains("JumpBtn"))
                                {
                                    Jump();
                                }
                                if (hit.collider.gameObject.name.Contains("ActionBtn") && gm.gameMode != GameMode.END)
                                {
                                    Action();
                                }
                            }
                        }
                    }
                }
                else
                {
                    Stay();
                }
            }
            else
            {
                if (Input.GetMouseButton(0) && gm.gameMode != GameMode.END)
                {
                    RaycastHit2D hit = Physics2D.Raycast(gm.cameraButton.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

                    if (hit.collider != null)
                    {
                        if (hit.collider.gameObject.name.Contains("RightBtn"))
                        {
                            MoveRight();
                        }
                        else if (hit.collider.gameObject.name.Contains("LeftBtn"))
                        {
                            MoveLeft();
                        }
                        else
                        {
                            Stay();
                        }

                        if (hit.collider.gameObject.name.Contains("JumpBtn"))
                        {
                            Jump();
                        }
                        if (hit.collider.gameObject.name.Contains("ActionBtn") && gm.gameMode != GameMode.END)
                        {
                            Action();
                        }
                    }
                }
                else
                {
                    Stay();
                }
            }
        }
        else
        {
            // ai controller
            AiController();
        }
            
        // apply horizontal speed smoothing it
        var smoothedMovementFactor = _controller.isGrounded ? groundDamping : inAirDamping; // how fast do we change direction?
		_velocity.x = Mathf.Lerp( _velocity.x, normalizedHorizontalSpeed * runSpeed, Time.deltaTime * smoothedMovementFactor );

		// apply gravity before moving
		_velocity.y += gravity * Time.deltaTime;

		_controller.move( _velocity * Time.deltaTime );
	}

    void AiController()
    {
        if (gm.gameMode == GameMode.PLAY)
        {
            Vector3 tmp = ball.transform.position;

            if (tmp.x > 0 && Mathf.Abs(tmp.x - transform.position.x) > 0.3f)
            {
                if (tmp.x > transform.position.x && transform.position.x < 7f)
                {
                    MoveRight();
                }
                else if (tmp.x < transform.position.x)
                {
                    MoveLeft();
                }

                if (Mathf.Abs(tmp.y - transform.position.y) > 1.5f &&
                    Mathf.Abs(tmp.y - transform.position.y) < 4f && tmp.x < 1.75f)
                {
                    Jump();
                }
            }
            else if ((tmp.x > 0 && Mathf.Abs(tmp.x - transform.position.x) <= 0.3f) || (tmp.x <= 0 && tmp.x >= -0.6f))
            {
                Stay();
            }
            else if (tmp.x < -0.6f)
            {
                if (transform.position.x < 3.75f)
                {
                    MoveRight();
                }
                else if (transform.position.x > 4.1f)
                {
                    MoveLeft();
                }
                else
                {
                    Stay();
                }
            }
        }
        else
        {
            Stay();
        }
    }

    public void MoveRight()
    {
        normalizedHorizontalSpeed = 1;

        if (_controller.isGrounded && !_controller.GetKicking() && lastAction != LastAction.MOVE)
        {
            _animator.Play(Animator.StringToHash("Run"));

            lastAction = LastAction.MOVE;
        }

        //if( transform.localScale.x < 0f )
        //transform.localScale = new Vector3( -transform.localScale.x, transform.localScale.y, transform.localScale.z );
    }

    public void MoveLeft()
    {
        normalizedHorizontalSpeed = -1;

        if (_controller.isGrounded && !_controller.GetKicking() && lastAction != LastAction.MOVE)
        {
            _animator.Play(Animator.StringToHash("Run"));

            lastAction = LastAction.MOVE;
        }

        //if( transform.localScale.x > 0f )
        //transform.localScale = new Vector3( transform.localScale.x, transform.localScale.y, transform.localScale.z );
    }

    public void Stay()
    {
        normalizedHorizontalSpeed = 0;

        if (_controller.isGrounded && !_controller.GetKicking())
        {
            _animator.Play(Animator.StringToHash("Idle"));

            lastAction = LastAction.STAY;
        }
    }

    public void Jump()
    {
        // we can only jump whilst grounded
        if (_controller.isGrounded && !_controller.GetKicking() && gm.gameMode != GameMode.END)
        {
            _velocity.y = Mathf.Sqrt(2f * jumpHeight * -gravity);
            _animator.Play(Animator.StringToHash("Jump"));

            lastAction = LastAction.JUMP;
        }
    }

    public void Action()
    {
        if (!_controller.GetKicking())
        {
            if (_controller.isGrounded)
            {
                _animator.Play(Animator.StringToHash("Kick"));

                lastAction = LastAction.KICK;

                StartCoroutine(ChangeFace());

                if (Vector3.Distance(transform.position, ball.transform.position) < 2.5f && gm.gameMode == GameMode.PLAY)
                {
                    Rigidbody2D rig = ball.GetComponent<Rigidbody2D>();

                    Vector3 tmp = ball.transform.position;
                    tmp.y -= 0.5f;
                    gm.CreateHitEffect(tmp);

                    if (!isPlayer2)
                    {
                        rig.AddForce(new Vector2(2, 6) * 9, ForceMode2D.Impulse);
                    }
                    else
                    {
                        rig.AddForce(new Vector2(-2, 6) * 4, ForceMode2D.Impulse);
                    }

                    //print("kick");
                    if (Random.Range(0, 2.1f) > 1)
                        AssetManager.Use.PlaySound(4);
                    else
                        AssetManager.Use.PlaySound(5);
                }
            }
            else
            {
                _animator.Play(Animator.StringToHash("Head"));

                lastAction = LastAction.HEAD;

                StartCoroutine(ChangeFace());

                if (Vector3.Distance(transform.position, ball.transform.position) < 2.5f && gm.gameMode == GameMode.PLAY)
                {
                    Rigidbody2D rig = ball.GetComponent<Rigidbody2D>();

                    gm.CreateHitEffect(ball.transform.position);

                    if (!isPlayer2)
                    {
                        rig.AddForce(new Vector2(3, -1) * 9, ForceMode2D.Impulse);
                    }
                    else
                    {
                        rig.AddForce(new Vector2(-3, -1) * 4, ForceMode2D.Impulse);
                    }

                    //print("Head");
                    if (Random.Range(0, 2.1f) > 1)
                        AssetManager.Use.PlaySound(4);
                    else
                        AssetManager.Use.PlaySound(5);
                }
            }
        }
    }

    public IEnumerator ChangeFace()
    {
        myHeadSp.sprite = myFace[1];
        yield return new WaitForSeconds(0.3f);
        myHeadSp.sprite = myFace[0];
    }
}
