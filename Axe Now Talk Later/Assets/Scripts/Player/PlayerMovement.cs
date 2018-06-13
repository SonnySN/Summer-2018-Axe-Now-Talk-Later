using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// CREATED BY SONNY
/// Movement for a GameObject and Rotation for a child Player Model
/// Inherits from PlayerStats
/// </summary>

public class PlayerMovement : PlayerStats {

    //Call PlayerMovement.Instance for easy get PlayerMovement script.
    public static PlayerMovement instance;
    public PlayerController s_PlayerController;

    //On player model with Animator.
    public PlayerAnimation s_PlayerAnimation;
    public Animator m_Animator;

    //Contains Mode/Paradigm information
    public ModeParadigm s_ModeParadigm;
    public Transform lineStart;
    public Transform lineEnd;

    public Rigidbody m_Rigidbody;

    Vector3 moveVector;
    Vector3 newAngle;
    Vector3 currentAngle;

    bool grounded;

    float axisX;
    float axisZ;
    float originalSpeed;
    float rotationSpeed;
    bool attacking;
    bool dashing;
    bool rolling;
    bool blocking;

    public static PlayerMovement Instance
    {
        get
        {
            if (instance == null)
                instance = GameObject.FindObjectOfType<PlayerMovement>();
            return instance;
        }
    }

    void Awake()
    {
        InitialValues();
    }

    void Start ()
    {
        s_ModeParadigm = GetComponent<ModeParadigm>();
        if(!s_ModeParadigm)
            Debug.LogError("ModeParadigm Script not found");
        else
            s_ModeParadigm.Change(ModeParadigm.NeutralMode, this);

        m_Rigidbody = GetComponent<Rigidbody>();
        if (!m_Rigidbody)
        {
            m_Rigidbody = gameObject.AddComponent<Rigidbody>();
            m_Rigidbody.mass = 3;
            m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        }

        s_PlayerAnimation = transform.GetChild(0).GetComponent<PlayerAnimation>();
        if (!s_PlayerAnimation)
            Debug.LogError("PlayerAnimation not found");

        m_Animator = s_PlayerAnimation.gameObject.GetComponent<Animator>();
        if (!m_Animator)
            Debug.LogError("Animator not found");

        rotationSpeed = 7;
        currentAngle = s_PlayerAnimation.transform.eulerAngles;
    }

    void Update()
    {
        JumpLineCast();

        axisX = Input.GetAxis("Horizontal");
        axisZ = Input.GetAxis("Vertical");

        //Must tag all combat animations for this bool to work.
        attacking = m_Animator.GetCurrentAnimatorStateInfo(0).IsTag("Combat");

        if (Input.GetKeyDown(KeyCode.F1))
        {
            s_ModeParadigm.Change(ModeParadigm.AttackMode, this);
            m_Animator.speed = 1.66f;
        }
        else if (Input.GetKeyDown(KeyCode.F2))
        {
            s_ModeParadigm.Change(ModeParadigm.NeutralMode, this);
            m_Animator.speed = 1f;
        }
        else if (Input.GetKeyDown(KeyCode.F3))
        {
            s_ModeParadigm.Change(ModeParadigm.DefenceMode, this);
            m_Animator.speed = .66f;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            originalSpeed = moveSpeed;
            moveSpeed *= 1.25f;
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            moveSpeed = originalSpeed;
        }

        //This timing will change. Combos etc.
        if (Input.GetMouseButtonDown(0) && !attacking && !dashing && !rolling && !blocking)
        {
            m_Animator.SetTrigger("Attack");
        }

        if (Input.GetMouseButtonDown(1))
        {
            //m_Animator.ResetTrigger("Attack");
            m_Animator.Play("standing walk forward 0");
            if (ModeParadigm.currentMode == ModeParadigm.AttackMode && !dashing)
                StartCoroutine(DashCoroutine());
            else if (ModeParadigm.currentMode == ModeParadigm.NeutralMode && !rolling)
                StartCoroutine(RollCoroutine());
            else if (ModeParadigm.currentMode == ModeParadigm.DefenceMode)
                Block();
        }
        else if (Input.GetMouseButtonDown(1))
            if (ModeParadigm.currentMode == ModeParadigm.DefenceMode)
                Block();
        Rotation();
    }

    IEnumerator DashCoroutine()
    {
        dashing = true;
        yield return new WaitForSeconds(.15f);
        dashing = false;
    }
    IEnumerator RollCoroutine()
    {
        rolling = true;
        yield return new WaitForSeconds(.55f);
        rolling = false;
    }
    void Block()
    {
        blocking = !blocking;
    }

    void FixedUpdate ()
    {
        if (Input.GetKeyDown(KeyCode.Space) && grounded)
            m_Rigidbody.AddForce(Vector3.up * jumpForce);

        if (dashing)
            m_Rigidbody.velocity = (s_PlayerAnimation.transform.forward * dashForce * 3);
        else if (rolling)
            m_Rigidbody.AddForce(s_PlayerAnimation.transform.forward * dashForce * 3, ForceMode.Acceleration);
        else if (blocking)
            m_Rigidbody.velocity = Vector3.zero;
        else if (attacking)
            m_Rigidbody.velocity = (s_PlayerAnimation.transform.forward);
        else if (m_Rigidbody)
        {
            moveVector = new Vector3(axisX, 0, axisZ) * moveSpeed;
            newAngle = new Vector3(axisX, 0, axisZ) * moveSpeed; ;

            //Clamp the movement so that its not faster when moving normally.
            //Clamping only occurs during normal movement. So dashing is not affected.
            Vector3 clampVel = m_Rigidbody.velocity;
            clampVel.x = Mathf.Clamp(clampVel.x, -moveSpeed, moveSpeed);
            clampVel.z = Mathf.Clamp(clampVel.z, -moveSpeed, moveSpeed);

            m_Rigidbody.velocity = clampVel;
            m_Rigidbody.AddForce(moveVector, ForceMode.VelocityChange);
        }

        if (m_Rigidbody.velocity.y <= -.2f) //Faster Fall
        {
            m_Rigidbody.velocity += Vector3.up * Physics.gravity.y * (m_FallMultiplier - 1) * Time.deltaTime;
        }
        //else if (m_Rigidbody.velocity.y > 0 && !Input.GetButton("Jump")) //Hold Spacebar to jump higher
        //{
        //    m_Rigidbody.velocity += Vector3.up * Physics.gravity.y * (m_LowJumpMultiplier - 1) * Time.deltaTime;
        //}
    }

    //Rotate Player Model
    void Rotation()
    {
        currentAngle = new Vector3(
            Mathf.LerpAngle(currentAngle.x, newAngle.x, Time.deltaTime * 7),
            Mathf.LerpAngle(currentAngle.y, newAngle.y, Time.deltaTime * 7),
            Mathf.LerpAngle(currentAngle.z, newAngle.z, Time.deltaTime * 7));

        //if no input
        if (axisX != 0 || axisZ != 0)
            s_PlayerAnimation.transform.rotation = Quaternion.LookRotation(currentAngle);
    }

    void JumpLineCast()
    {
        Debug.DrawLine(lineStart.position, lineEnd.position, Color.green);
        grounded = Physics.Linecast(lineStart.position, lineEnd.position);
    }
}
