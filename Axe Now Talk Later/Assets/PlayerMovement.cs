using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    //Call PlayerMovement.Instance for easy get PlayerMovement script.
    public static PlayerMovement instance;

    Vector3 moveVector;
    Vector3 rotateVector;
    Vector3 currentAngle;

    Rigidbody m_Rigidbody;
    //Placed on player gameobject with Animator.
    PlayerAnimation s_PlayerAnimationEvents;

    public Transform lineStart;
    public Transform lineEnd;
    bool grounded;

    float axisX;
    float axisZ;
    //double distToGround;
    public float jumpForce;
    public float moveSpeed;
    float m_FallMultiplier;
    float m_LowJumpMultiplier;

    public float distToGround;

    public static PlayerMovement Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<PlayerMovement>();
            }
            return instance;
        }


    }

    void Start () {
        m_Rigidbody = GetComponent<Rigidbody>();
        if (!m_Rigidbody)
            m_Rigidbody = gameObject.AddComponent<Rigidbody>();

        s_PlayerAnimationEvents = transform.GetChild(0).GetComponent<PlayerAnimation>();
        if (!s_PlayerAnimationEvents)
            Debug.LogError("PlayerAnimation not found");

        moveSpeed = 1.5f;
        jumpForce = 1500;
        m_FallMultiplier = 6.25f;
        m_LowJumpMultiplier = 1.5f;

        currentAngle = s_PlayerAnimationEvents.transform.eulerAngles;
        distToGround = GetComponent<Collider>().bounds.extents.y;

    }

    void Update()
    {
        Raycasting();

        axisX = Input.GetAxis("Horizontal");
        axisZ = Input.GetAxis("Vertical");

        if (Input.GetKeyDown(KeyCode.Space) && grounded)
            m_Rigidbody.AddForce(Vector3.up * jumpForce);

        if (Input.GetKey(KeyCode.LeftShift))
        {
            moveSpeed = 2.25f;
        }
        else
            moveSpeed = 1.5f;

        Rotation();

    }

    void FixedUpdate ()
    {
        if (m_Rigidbody)
        {
            moveVector = new Vector3(axisX, 0, axisZ) * moveSpeed;
            rotateVector = new Vector3(axisX, 0, axisZ) * moveSpeed; ;

            Vector3 clampVel = m_Rigidbody.velocity;
            clampVel.x = Mathf.Clamp(clampVel.x, -moveSpeed, moveSpeed);
            clampVel.z = Mathf.Clamp(clampVel.z, -moveSpeed, moveSpeed);

            m_Rigidbody.velocity = clampVel;
            m_Rigidbody.AddForce(moveVector, ForceMode.VelocityChange);
        }

        if (m_Rigidbody.velocity.y <= -.2f)
        {
            m_Rigidbody.velocity += Vector3.up * Physics.gravity.y * (m_FallMultiplier - 1) * Time.deltaTime;
        }

        //if (m_Rigidbody.velocity.y < 0)
        //{
        //    m_Rigidbody.velocity += Vector3.up * Physics.gravity.y * (m_FallMultiplier - 1) * Time.deltaTime;
        //}
        //else if (m_Rigidbody.velocity.y > 0 && !Input.GetButton("Jump"))
        //{
        //    m_Rigidbody.velocity += Vector3.up * Physics.gravity.y * (m_LowJumpMultiplier - 1) * Time.deltaTime;
        //}
    }

    void Rotation()
    {
        currentAngle = new Vector3(
            Mathf.LerpAngle(currentAngle.x, rotateVector.x, Time.deltaTime * 7),
            Mathf.LerpAngle(currentAngle.y, rotateVector.y, Time.deltaTime * 7),
            Mathf.LerpAngle(currentAngle.z, rotateVector.z, Time.deltaTime * 7));

        if (axisX != 0 || axisZ != 0)
            s_PlayerAnimationEvents.transform.rotation = Quaternion.LookRotation(currentAngle);
    }

    void Raycasting()
    {
        Debug.DrawLine(lineStart.position, lineEnd.position, Color.green);
        grounded = Physics.Linecast(lineStart.position, lineEnd.position);
        Debug.Log(grounded);
    }
}
