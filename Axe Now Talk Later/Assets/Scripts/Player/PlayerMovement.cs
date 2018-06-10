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
            m_Rigidbody = gameObject.AddComponent<Rigidbody>();

        s_PlayerAnimation = transform.GetChild(0).GetComponent<PlayerAnimation>();
        if (!s_PlayerAnimation)
            Debug.LogError("PlayerAnimation not found");

        currentAngle = s_PlayerAnimation.transform.eulerAngles;
    }

    void Update()
    {
        Raycasting();

        axisX = Input.GetAxis("Horizontal");
        axisZ = Input.GetAxis("Vertical");

        if (Input.GetKeyDown(KeyCode.F1))
            s_ModeParadigm.Change(ModeParadigm.AttackMode, this);
        else if (Input.GetKeyDown(KeyCode.F2))
            s_ModeParadigm.Change(ModeParadigm.NeutralMode, this);
        else if(Input.GetKeyDown(KeyCode.F3))
            s_ModeParadigm.Change(ModeParadigm.DefenceMode, this);

        if (Input.GetKeyDown(KeyCode.Space) && grounded)
            m_Rigidbody.AddForce(Vector3.up * jumpForce);

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            originalSpeed = moveSpeed;
            moveSpeed *= 1.25f;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            moveSpeed = originalSpeed;

        }

        Rotation();
    }

    void FixedUpdate ()
    {
        if (m_Rigidbody)
        {
            moveVector = new Vector3(axisX, 0, axisZ) * moveSpeed;
            newAngle = new Vector3(axisX, 0, axisZ) * moveSpeed; ;

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

        if (axisX != 0 || axisZ != 0)
            s_PlayerAnimation.transform.rotation = Quaternion.LookRotation(currentAngle);
    }

    void Raycasting()
    {
        Debug.DrawLine(lineStart.position, lineEnd.position, Color.green);
        grounded = Physics.Linecast(lineStart.position, lineEnd.position);
        Debug.Log(grounded);
    }
}
