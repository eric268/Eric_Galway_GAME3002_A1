using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowDirection : MonoBehaviour
{
    private float m_fRotationSpeed = 0.05f;
    private float m_fRotationLimit = 0.0f;
    private float m_fKickingStrength = 1.0f;
    private bool m_bTurnLeft = false;
    private bool m_bTurnRight = false;
    private bool m_bRotateUp = false;
    private bool m_bRotateDown = false;
    private bool m_bChargeKickStrength = false;
    private bool m_bBallKicked = false;

    private bool m_bDebugDirection = false;

    private Vector3 m_vDirection = Vector3.zero;
    private Vector3 m_vStartingPosition = Vector3.zero;
    private Quaternion m_qStartRotation = Quaternion.identity;



    private Vector3 m_vStartingArrowPosition = Vector3.zero;
    private Vector3 m_vStartingArrow2Position = Vector3.zero;
    private Vector3 m_vStartingArrowScale = Vector3.zero;
    private Vector3 m_vStartingArrow2Scale = Vector3.zero; 
    private Quaternion m_qArrowRotation = Quaternion.identity;
    private Quaternion m_qArrow2Rotation = Quaternion.identity;

    GameObject m_arrow;
    GameObject m_soccerBall;
    GameObject m_arrowBox;
    GameObject m_arrowBox2;

    // Start is called before the first frame update
    void Start()
    {
        m_arrow = GameObject.Find("arrow");
        if (!m_arrow)
        {
            Debug.LogError("Can't find arrow");
        }

        m_soccerBall = GameObject.Find("Soccer Ball");
        if (!m_soccerBall)
        {
            Debug.LogError("Can't find soccer ball");
        }

        m_arrowBox = GameObject.Find("Box");
        if (!m_arrowBox)
        {
            Debug.LogError("Can't find first part of arrow");
        }

        m_arrowBox2 = GameObject.Find("Box1");
        if (!m_arrowBox2)
        {
            Debug.LogError("Can't find 2nd part of arrow");
        }


        m_qStartRotation = transform.rotation;
        m_vStartingPosition = transform.position;

        m_vStartingArrowPosition = m_arrowBox.transform.position;
        m_vStartingArrow2Position = m_arrowBox2.transform.position;

        m_vStartingArrowScale = m_arrowBox.transform.localScale;
        m_vStartingArrow2Scale = m_arrowBox2.transform.localScale;

        m_qArrowRotation = m_arrowBox.transform.rotation;
        m_qArrow2Rotation = m_arrowBox2.transform.rotation;

        m_arrowBox.GetComponent<Renderer>().material.color = Color.grey;
        m_arrowBox2.GetComponent<Renderer>().material.color = Color.grey;

    }

    // Update is called once per frame
    void Update()
    {
        HandleEvents();
        Move();

    }
    private void HandleEvents()
    {
        //Keydown
        if (Input.GetKeyDown(KeyCode.A))
        {
            m_bTurnLeft = true;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            m_bTurnRight = true;
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            m_bRotateUp = true;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            m_bRotateDown = true;
        }
        if(Input.GetKeyDown(KeyCode.Space))
        {
            m_bChargeKickStrength = true;
        }

        //Keyup
        if (Input.GetKeyUp(KeyCode.A))
        {
            m_bTurnLeft = false;
        }
        if (Input.GetKeyUp(KeyCode.D))
        {
            m_bTurnRight = false;
        }
        if (Input.GetKeyUp(KeyCode.W))
        {
            m_bRotateUp = false;
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            m_bRotateDown = false;
        }
        if(Input.GetKeyUp(KeyCode.Space))
        {
            if (!m_bBallKicked)
            { 
            m_vDirection = new Vector3(transform.forward.x, m_arrow.transform.forward.y, transform.forward.z);
            m_soccerBall.GetComponent<BallPhysics>().m_rb.AddForce(m_vDirection * m_fKickingStrength, ForceMode.Impulse);
            m_bChargeKickStrength = false;
            m_bBallKicked = true;
            m_fKickingStrength = 1.0f;
            }
        }
        if (Input.GetKeyUp(KeyCode.R))
        {
            Debug.LogError("R Pressed");
            ResetSceneObjects();

        }   

    }

    private void RotateLeft(float speed)
    {
        transform.Rotate(Vector3.up * -speed);
    }
    private void RotateRight(float speed)
    {
        transform.Rotate(Vector3.up * speed);
    }
    private void RotateUp(float speed)
    {
        m_arrow.transform.Rotate(Vector3.right * -speed);
    }

    private void RotateDown(float speed)
    {
        m_arrow.transform.Rotate(Vector3.right * speed);
    }

    private void Move()
    {
        //if (!m_bBallKicked)
        {
            if (m_bTurnLeft)
            {
                RotateLeft(m_fRotationSpeed);
            }
            if (m_bTurnRight)
            {
                RotateRight(m_fRotationSpeed);
            }
            if (m_bRotateUp)
            {
                m_fRotationLimit -= m_fRotationSpeed;
                m_fRotationLimit = Mathf.Clamp(m_fRotationLimit, -30.0f, 5.0f);

                if (m_fRotationLimit > -30.0f)
                    RotateUp(m_fRotationSpeed);

            }
            if (m_bRotateDown)
            {
                m_fRotationLimit += m_fRotationSpeed;
                m_fRotationLimit = Mathf.Clamp(m_fRotationLimit, -30.0f, 5.0f);

                if (m_fRotationLimit < 5.0f)
                    RotateDown(m_fRotationSpeed);
            }

            if (m_bChargeKickStrength)
            {
                Debug.LogError(m_fKickingStrength);
                m_fKickingStrength += 0.03f;
                m_fKickingStrength = Mathf.Clamp(m_fKickingStrength, 1.0f, 50.0f);
                UpdateArrowSizeandColor();
            }
        }

    }

    private void ResetSceneObjects()
    {
        transform.rotation = m_qStartRotation;
        m_soccerBall.transform.position = m_vStartingPosition;
        m_soccerBall.GetComponent<BallPhysics>().m_rb.velocity = Vector3.zero;
        m_soccerBall.GetComponent<BallPhysics>().m_rb.angularVelocity = Vector3.zero;

        m_arrowBox.transform.localScale = m_vStartingArrowScale;
        m_arrowBox2.transform.localScale = m_vStartingArrow2Scale;

        m_arrowBox.transform.position = m_vStartingArrowPosition;
        m_arrowBox2.transform.position = m_vStartingArrow2Position;

        m_arrowBox.transform.rotation = m_qArrowRotation;
        m_arrowBox2.transform.rotation = m_qArrow2Rotation;

        m_arrowBox.GetComponent<Renderer>().material.color = Color.grey;
        m_arrowBox2.GetComponent<Renderer>().material.color = Color.grey;

        ResetSceneVariables();

    }

    private void ResetSceneVariables()
    {
        m_bTurnLeft = false;
        m_bTurnRight = false;
        m_bRotateUp = false;
        m_bRotateDown = false;
        m_bChargeKickStrength = false;
        m_bBallKicked = false;

        m_fRotationLimit = 0.0f;
    }
    private void UpdateArrowSizeandColor()
    {
        if (!m_bBallKicked)
        {
            if (m_fKickingStrength < 50.0f)
            {
                m_arrowBox.transform.localScale += new Vector3(0.0001f, 0.0f, 0.0008f);
                m_arrowBox2.transform.localScale += new Vector3(0.00005f, 0.0f, 0.0005f);
                m_arrowBox.transform.position += (transform.forward * 0.0015f);
                m_arrowBox2.transform.position += (transform.forward * 0.001f);
            }

            if (m_fKickingStrength > 20.0f && m_fKickingStrength <= 35.0f)
            {
                m_arrowBox.GetComponent<Renderer>().material.color = Color.yellow;
                m_arrowBox2.GetComponent<Renderer>().material.color = Color.yellow;
            }
            else if (m_fKickingStrength > 35.0f)
            {
                m_arrowBox.GetComponent<Renderer>().material.color = Color.red;
                m_arrowBox2.GetComponent<Renderer>().material.color = Color.red;
            }
        }
    }
    
    private void OnDrawGizmos()
    {
        
    }
}
