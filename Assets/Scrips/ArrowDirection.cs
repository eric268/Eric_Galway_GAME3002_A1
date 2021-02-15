using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowDirection : MonoBehaviour
{
    private float m_fRotationSpeed = 0.05f;
    //private float m_fRotationLimit = 0.0f;
    //private float m_fKickingStrength = 1.0f;
    private float m_fDistanceToTarget = 0.0f;
    public float m_fTimer = 0.0f;

    private bool m_bTurnLeft = false;
    private bool m_bTurnRight = false;
    private bool m_bMoveForward = false;
    private bool m_bMoveBackwards = false;
    private bool m_bRotateUp = false;
    private bool m_bRotateDown = false;
    //private bool m_bChargeKickStrength = false;
    private bool m_bBallKicked = false;
    private bool m_bGenerateRandomBullseye = false;

    static public int m_iBullseyesRemaining = 0;

    static public bool m_bStartGame = false;
    
    

    private Vector3 m_vDirection = Vector3.zero;
    private Vector3 m_vStartingPosition = Vector3.zero;
    [SerializeField]
    private Vector3 m_vInitialVelocity = Vector3.zero;
    private Quaternion m_qStartRotation = Quaternion.identity;

    private Quaternion m_TargetStartingRotation;

    private Vector3 m_vPosition = Vector3.zero;

    private Vector3 m_vStartVel = Vector3.zero;


    private GameObject m_soccerBall = null;
    private GameObject m_LandingPosition = null;
    private GameObject m_bullseye = null;
    private GoalBullseyes m_BullseyePool = null;
    private CountdownTimer m_TimerObject = null;

    // Start is called before the first frame update
    void Start()
    {
        CreateLandingDisplay(); 

        m_soccerBall = GameObject.Find("Soccer Ball");
        if (!m_soccerBall)
        {
            Debug.LogError("Can't find soccer ball");
        }
        m_iBullseyesRemaining = 10;

        m_qStartRotation = transform.rotation;
        m_vStartingPosition = transform.position;
        m_vInitialVelocity += transform.position + transform.forward * 5;
        m_vInitialVelocity.y = transform.position.y + 2.0f;
        m_vPosition = m_vInitialVelocity;
        m_LandingPosition.transform.position = m_vInitialVelocity;

        m_BullseyePool = this.GetComponent<GoalBullseyes>();

        m_TimerObject = GameObject.Find("Canvas").GetComponent<CountdownTimer>();

        m_TargetStartingRotation = m_LandingPosition.transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        HandleEvents();
        Move();
        //UpdateTargetPosition();
        RotateCamera();
        CreateRandomBullseye();

        m_LandingPosition.transform.position = m_vInitialVelocity;

        Vector3 m_vFinalRange = new Vector3(m_LandingPosition.transform.position.x, transform.position.y, m_LandingPosition.transform.position.z);

        m_fDistanceToTarget = Vector3.Distance(transform.position, m_vFinalRange);

        if (m_iBullseyesRemaining <= 5)
        {
            m_bGenerateRandomBullseye = true;
        }

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
            //m_bChargeKickStrength = true;
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

            ResetSceneObjects();

        }   

    }
    public void StartGame()
    {
        if (!m_bStartGame)
        {
            GoalBullseyes.m_bResetBullseyePosition = true;
        }
        ResetSceneObjects();
        m_iBullseyesRemaining = 10;
        m_bStartGame = true;
        m_TimerObject.m_bStartTimer = true;
    }

    public void ResetScene()
    {
        m_bStartGame = false;
        m_BullseyePool.ResetBullseyePositionto0();
        ResetSceneObjects();
        m_iBullseyesRemaining = 10;
        m_TimerObject.m_fCurrentTime = 60.0f;
        m_TimerObject.m_Victory.text = "";
        m_TimerObject.m_CountdownText.text = m_TimerObject.m_fCurrentTime.ToString("0");
        m_bGenerateRandomBullseye = false;
    }

    private void OnKick()
    {
  
        float fMaxHeight = (m_LandingPosition.transform.position.y - transform.position.y);

        float fRange = (m_fDistanceToTarget * 2);
        float totalXDistance = (m_LandingPosition.transform.position.x - transform.position.x) * 2;
        float totalZDistance = (m_LandingPosition.transform.position.z - transform.position.z) * 2;
        float fTheta = Mathf.Atan((4 * fMaxHeight) / (fRange));
        float fInitVelMag = Mathf.Sqrt((2 * Mathf.Abs(Physics.gravity.y) * fMaxHeight)) / Mathf.Sin(fTheta);

        //float fInitVelMag = (1.0f / Mathf.Cos(fTheta)) * (Mathf.Sqrt((0.5f * Mathf.Abs(Physics.gravity.y) * (fRange * fRange)) / (fRange * Mathf.Tan(fTheta) + fMaxHeight)));

        Vector2 xzAxis = new Vector2(totalXDistance, totalZDistance);
        xzAxis.Normalize();

        //Debug.LogError("Max Height: " + fMaxHeight);
        //Debug.LogError("Dist to target: " + fRange/2);
        //Debug.LogError("Total X Dist: " + totalXDistance);
        //Debug.LogError("Total Z Dist: " + totalZDistance);
        //Debug.LogError("Angle: " + fTheta * Mathf.Rad2Deg);
        //Debug.LogError("Magnitude: " + fInitVelMag);



        m_vStartVel.y = fInitVelMag * Mathf.Sin(fTheta);
        m_vStartVel.x = xzAxis.x * fInitVelMag * Mathf.Cos(fTheta);
        m_vStartVel.z = xzAxis.y * fInitVelMag * Mathf.Cos(fTheta);

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
            m_LandingPosition.transform.Rotate(angle, 0, 0.0f);
        }
        else
        {
            transform.Rotate(0.0f, -angle, 0.0f);
            m_LandingPosition.transform.Rotate(-angle, 0, 0.0f);
        }
    }
    private void CreateLandingDisplay()
    {
        m_LandingPosition = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        m_LandingPosition.transform.position = transform.position + (transform.forward *5);
        m_LandingPosition.transform.localScale = new Vector3(2.5f, 0.1f, 2.5f);
        m_LandingPosition.transform.Rotate(0.0f, 0.0f, 90.0f);
        m_LandingPosition.GetComponent<Renderer>().material.color = Color.blue;
        m_LandingPosition.GetComponent<Collider>().enabled = false;
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
        if (m_LandingPosition.transform.position.y > 39.0f)
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
        m_soccerBall.GetComponent<BallPhysics>().m_bCanHitBullseye = true;

        m_vInitialVelocity = Vector3.zero;
        m_vInitialVelocity += transform.position + transform.forward * 5;
        m_vInitialVelocity.y = transform.position.y + 2.0f;
        m_vPosition = m_vInitialVelocity;
        m_LandingPosition.transform.position = m_vInitialVelocity;

        m_LandingPosition.transform.rotation = m_TargetStartingRotation;

        ResetSceneVariables();

    }

    private void ResetSceneVariables()
    {
        m_bTurnLeft = false;
        m_bTurnRight = false;
        m_bRotateUp = false;
        m_bRotateDown = false;
        //m_bChargeKickStrength = false;
        m_bBallKicked = false;


        //m_fRotationLimit = 0.0f;
    }

    private void CreateRandomBullseye()
    {
 
        if (m_bGenerateRandomBullseye && m_iBullseyesRemaining > 0)
        {
            m_bullseye = this.GetComponent<GoalBullseyes>().bullsEyes[0];

            if (m_fTimer <= 0.0f)
            {
                Vector3 position = new Vector3(85.0f, Random.Range(39.0f, 49.0f), Random.Range(-2.3f, 12.5f));
                m_bullseye.transform.position = position;
                m_fTimer = 10.0f;
            }

            m_fTimer -= 1 * Time.deltaTime;
        }
    }
    
    private void OnDrawGizmos()
    {
        
    }
}
