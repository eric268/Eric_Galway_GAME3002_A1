using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowDirection : MonoBehaviour
{
    private float m_fRotationSpeed = 0.05f;
    private float m_fRotationLimit = 0.0f;
    private float m_fKickingStrength = 1.0f;
    private float m_fDistanceToTarget = 0.0f;

    private bool m_bTurnLeft = false;
    private bool m_bTurnRight = false;
    private bool m_bMoveForward = false;
    private bool m_bMoveBackwards = false;
    private bool m_bRotateUp = false;
    private bool m_bRotateDown = false;
    private bool m_bChargeKickStrength = false;
    private bool m_bBallKicked = false;

    private bool m_bDebugDirection = false;

    private Vector3 m_vDirection = Vector3.zero;
    private Vector3 m_vStartingPosition = Vector3.zero;
    [SerializeField]
    private Vector3 m_vInitialVelocity = Vector3.zero;
    private Quaternion m_qStartRotation = Quaternion.identity;

    private Vector3 m_vPosition = Vector3.zero;

    private Vector3 m_vStartVel = Vector3.zero;


    private GameObject m_soccerBall = null;
    private GameObject m_LandingPosition = null;

    // Start is called before the first frame update
    void Start()
    {
        CreateLandingDisplay(); 

        m_soccerBall = GameObject.Find("Soccer Ball");
        if (!m_soccerBall)
        {
            Debug.LogError("Can't find soccer ball");
        }


        m_qStartRotation = transform.rotation;
        m_vStartingPosition = transform.position;
        m_vInitialVelocity += transform.position + transform.forward * 5;
        m_vInitialVelocity.y = transform.position.y;
        m_vPosition = m_vInitialVelocity;
        m_LandingPosition.transform.position = m_vInitialVelocity;

    }

    // Update is called once per frame
    void Update()
    {
        HandleEvents();
        Move();
        //UpdateTargetPosition();
        RotateCamera();
        m_LandingPosition.transform.position = m_vInitialVelocity;
        m_fDistanceToTarget = Vector3.Distance(transform.position, m_LandingPosition.transform.position);

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
            m_bMoveForward = true;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            m_bMoveBackwards = true;
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            m_bRotateUp = true;
        }
        if (Input.GetKeyDown(KeyCode.F))
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
            m_bMoveForward = false;
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            m_bMoveBackwards = false;
        }
        if (Input.GetKeyUp(KeyCode.R))
        {
            m_bRotateUp = false;
        }
        if (Input.GetKeyUp(KeyCode.F))
        {
            m_bRotateDown = false;
        }
        if(Input.GetKeyUp(KeyCode.Space))
        {
            if (!m_bBallKicked)
            {
                 //m_LandingPosition.transform.position = GetLandingPosition();
                 m_bBallKicked = true;
                //transform.LookAt(m_LandingPosition.transform.position, Vector3.up);
                OnKick();
                
                 //m_soccerBall.GetComponent<BallPhysics>().m_rb.velocity = new Vector3(m_vInitialVelocity.x, 5.0f, m_vInitialVelocity.z);

            }
        }
        if (Input.GetKeyUp(KeyCode.Q))
        {
            Debug.LogError("R Pressed");
            ResetSceneObjects();

        }   

    }
    private void OnKick()
    {
  
        float fMaxHeight = (m_LandingPosition.transform.position.y - transform.position.y);
        float fRange = (m_fDistanceToTarget * 2);
        float totalXDistance = m_LandingPosition.transform.position.x - transform.position.x;
        float totalZDistance = m_LandingPosition.transform.position.z - transform.position.z;
        float fTheta = Mathf.Atan((4 * fMaxHeight) / (fRange));
        float fInitVelMag = Mathf.Sqrt((2 * Mathf.Abs(Physics.gravity.y) * fMaxHeight)) / Mathf.Sin(fTheta);

        Vector2 xzAxis = new Vector2(totalXDistance, totalZDistance);
        xzAxis.Normalize();

        m_vStartVel.y = fInitVelMag * Mathf.Sin(fTheta);
        m_vStartVel.x = xzAxis.x * fInitVelMag;
        m_vStartVel.z = xzAxis.y * fInitVelMag;

        m_soccerBall.GetComponent<BallPhysics>().m_rb.velocity = m_vStartVel;
    }

    
    private void RotateCamera()
    {
        m_vPosition = m_vInitialVelocity;
        m_vPosition.y = transform.position.y;

        Vector3 m_vLandingPosDir = m_vPosition - transform.position;
        float angle = Vector3.Angle(m_vLandingPosDir, transform.forward);
         
        float dotP = Vector3.Dot(m_vLandingPosDir, transform.right);

        if (dotP > 0)
        {
            transform.Rotate(0.0f, angle, 0.0f);
        }
        else
        {
            transform.Rotate(0.0f, -angle, 0.0f);
        }
    }
    private void CreateLandingDisplay()
    {
        m_LandingPosition = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        m_LandingPosition.transform.position = transform.position + (transform.forward *5);
        m_LandingPosition.transform.localScale = new Vector3(1.0f, 0.1f, 1.0f);
        m_LandingPosition.GetComponent<Renderer>().material.color = Color.red;
        m_LandingPosition.GetComponent<Collider>().enabled = false;
    }

    private void UpdateTargetPosition()
    {
        if (m_LandingPosition && !m_bBallKicked)
        {
            //m_LandingPosition.transform.position = GetLandingPosition();
        }
    }

    private Vector3 GetLandingPosition()
    {
        //float fTime = 2f * (0f - m_vInitialVelocity.y / Physics.gravity.y);
        //Vector3 vFlatVel = m_vInitialVelocity;
        //vFlatVel.y = 0;
        //vFlatVel *= fTime;
        return transform.position; // + vFlatVel;
    }

    private void RotateLeft(float speed)
    {
        m_vInitialVelocity -= (transform.right * speed);
    }
    private void RotateRight(float speed)
    {
        m_vInitialVelocity += (transform.right * speed);
    }
    private void MoveForward(float speed)
    {
        m_vInitialVelocity += (transform.forward * speed);
    }
    private void MoveBackwards(float speed)
    {
        float distance = Vector3.Distance(transform.position, m_LandingPosition.transform.position);
        if (distance > 5.0f)
        {
           m_vInitialVelocity -= (transform.forward * speed);
        }
    }
    private void RotateUp(float speed)
    {
        m_vInitialVelocity.y += speed;
        //= new Vector3(m_LandingPosition.transform.position.x, val, m_LandingPosition.transform.position.z);
        //m_vInitialVelocity += (transform.up * speed);
    }

    private void RotateDown(float speed)
    {
        if (m_LandingPosition.transform.position.y > 38.0f)
        {
            m_vInitialVelocity.y -= speed;
        }
        //m_vInitialVelocity += (transform.up * speed);
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
            if (m_bMoveForward)
            {
                MoveForward(m_fRotationSpeed);
            }
            if (m_bMoveBackwards)
            {
                MoveBackwards(m_fRotationSpeed);
            }
            if (m_bRotateUp)
            {
                RotateUp(m_fRotationSpeed);

            }
            if (m_bRotateDown)
            {
                RotateDown(m_fRotationSpeed);
            }
        }

    }

    private void ResetSceneObjects()
    {
        transform.rotation = m_qStartRotation;
        m_soccerBall.transform.position = m_vStartingPosition;
        m_soccerBall.GetComponent<BallPhysics>().m_rb.velocity = Vector3.zero;
        m_soccerBall.GetComponent<BallPhysics>().m_rb.angularVelocity = Vector3.zero;

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
    
    private void OnDrawGizmos()
    {
        
    }
}
