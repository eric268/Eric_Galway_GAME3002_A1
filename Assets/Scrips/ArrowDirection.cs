using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowDirection : MonoBehaviour
{
    //Float variables
    private float m_fRotationSpeed = 0.05f;
    private float m_fDistanceToTarget = 0.0f;
    public float m_fTimer = 0.0f;

    //Boolean variables
    private bool m_bTurnLeft = false;
    private bool m_bTurnRight = false;
    private bool m_bMoveForward = false;
    private bool m_bMoveBackwards = false;
    private bool m_bRotateUp = false;
    private bool m_bRotateDown = false;
    private bool m_bBallKicked = false;
    static public bool m_bStartGame = false;

    //Integer variables
    static public int m_iBullseyesRemaining = 0;

    //Vector 3 variables
    private Vector3 m_vDirection = Vector3.zero;
    private Vector3 m_vStartingPosition = Vector3.zero;
    private Vector3 m_vPosition = Vector3.zero;
    private Vector3 m_vStartVel = Vector3.zero;
    [SerializeField]
    private Vector3 m_vTargetPosition = Vector3.zero;

    //Quaternion variables
    private Quaternion m_qStartRotation = Quaternion.identity;
    private Quaternion m_TargetStartingRotation;

    //Gameobject/component variables
    private GameObject m_soccerBall = null;
    private GameObject m_Target = null;
    private GameObject m_bullseye = null;
    private GoalBullseyes m_BullseyePool = null;
    private CountdownTimer m_TimerObject = null;

    // Start is called before the first frame update
    void Start()
    {
        //Creates the target the player will use to aim their penalty shot
        CreateLandingDisplay(); 
        //Number of targets the player has to destroy to win
        m_iBullseyesRemaining = 10;
        //This saves the inital rotation so that when resetting position can reset rotation to starting value
        m_qStartRotation = transform.rotation;
        //This saves the position so that when resetting position can reset to starting value
        m_vStartingPosition = transform.position;
        //This starts the targets positon 5 units in front of kicking location
        m_vTargetPosition += transform.position + transform.forward * 5;
        //This starts the target positon slightly above the kicking location to avoid divison by 0 
        m_vTargetPosition.y = transform.position.y + 2.0f;
        //Saves targets starting position
        m_vPosition = m_vTargetPosition;
        //Sets targets position
        m_Target.transform.position = m_vTargetPosition;
        //Instantiates bullseye component variable
        m_BullseyePool = this.GetComponent<GoalBullseyes>();
        m_soccerBall = GameObject.Find("Soccer Ball");
        m_TimerObject = GameObject.Find("Canvas").GetComponent<CountdownTimer>();
        //Saves the inital target rotation
        m_TargetStartingRotation = m_Target.transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        //Checks for key inputs
        HandleEvents();
        Move();
        RotateCamera();
        CreateRandomBullseye();
        calculateDistanceToTarget();
    }
    private void HandleEvents()
    {
        //Keydown
        //This allows players to hold down buttons and move an object instead of repeatidly pressing buttons
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
            //Stops being able to kick the ball while it is in air etc.
            if (!m_bBallKicked)
            {
                m_bBallKicked = true;
                OnKick();
            }
        }
        if (Input.GetKeyUp(KeyCode.Q))
        {
            //This resets the ball to the starting postion along with camera rotation, target positon etc 
            //And allows for players to shoot the ball again
            ResetSceneObjects();
        }   

    }
    public void StartGame()
    {
        //Resets the targets to their origonal positons in goal
        if (!m_bStartGame)
        {
            GoalBullseyes.m_bResetBullseyePosition = true;
        }
        ResetSceneObjects();
        m_iBullseyesRemaining = 10;
        m_bStartGame = true;
        //Starts game timer
        m_TimerObject.m_bStartTimer = true;
        
    }

    public void ResetScene()
    {
        //Starts new game
        m_bStartGame = false;
        //Moves targets to (0,0,0) position awaiting for game to start
        m_BullseyePool.ResetBullseyePositionto0();
        ResetSceneObjects();
        m_iBullseyesRemaining = 10;
        m_TimerObject.m_fCurrentTime = 60.0f;
        m_TimerObject.m_Victory.text = "";
        m_TimerObject.m_CountdownText.color = Color.white;
        //Ensures the countdown value is reset to 60 and only shows integer
        m_TimerObject.m_CountdownText.text = m_TimerObject.m_fCurrentTime.ToString("0");
    }

    private void OnKick()
    { 
        //Gets the height of target, or how high the player wants to kick the ball.
        //Have to subtract the kicking location y value to ensure correct height is found
        float fMaxHeight = (m_Target.transform.position.y - transform.position.y);

        //Since the peak or middle of the balls trajectory will occur when it reaches target total range is double the target distance
        float fRange = (m_fDistanceToTarget * 2);

        //Only care about the distance on x and z axis because since field is flat the final y positon will be equal
        //To the starting y positon
        float totalXDistance = (m_Target.transform.position.x - transform.position.x) * 2;
        float totalZDistance = (m_Target.transform.position.z - transform.position.z) * 2;

        //Formula used to determine angle the ball must be kicked given the range and height
        float fTheta = Mathf.Atan((4 * fMaxHeight) / (fRange));
        //Formula for magnitude or starting velocity to reach max height at a given angle
        float fInitVelMag = Mathf.Sqrt((2 * Mathf.Abs(Physics.gravity.y) * fMaxHeight)) / Mathf.Sin(fTheta);

        //The x and z axis can be used as a single vector because the hypotenus of these two values is the trajectory we need
        //our soccer ball to travel
        Vector2 xzAxis = new Vector2(totalXDistance, totalZDistance);

        //Need to normalize this new vector to get the direction we need to travel in both axis to reach the target
        //This allows multiplying this vector later by the total velocity 
        xzAxis.Normalize();
        
        //using formula such as Vy = V * sin(theta) to determine starting y velocity
        m_vStartVel.y = fInitVelMag * Mathf.Sin(fTheta);
        //Do the same thing except sub cos for sin for x and z values. We know the direction because of the normalized Vector2 and we have the magnitude
        //of the velocity vector. Just need to sub in values to find inital veloctity on z and x to reach target
        m_vStartVel.x = xzAxis.x * fInitVelMag * Mathf.Cos(fTheta);
        m_vStartVel.z = xzAxis.y * fInitVelMag * Mathf.Cos(fTheta);

        //Finally, adding the force to the balls rigid body to make it move
        m_soccerBall.GetComponent<BallPhysics>().m_rb.velocity = m_vStartVel;
    }
    private void calculateDistanceToTarget()
    {
        m_Target.transform.position = m_vTargetPosition;

        Vector3 m_vFinalRange = new Vector3(m_Target.transform.position.x, transform.position.y, m_Target.transform.position.z);

        m_fDistanceToTarget = Vector3.Distance(transform.position, m_vFinalRange);
    }
    
    private void RotateCamera()
    {
        //Sets a position variable eqial to the targets positon but sets the y value equal to the kicking locations y value.
        //This is because we only care about rotating on the y axis
        m_vPosition = m_vTargetPosition;
        m_vPosition.y = transform.position.y;

        //Gets the direction vector from the kicking location to the target location
        Vector3 m_vLandingPosDir = m_vPosition - transform.position;

        //This calculates the angle between the forward vector of the kicking location and the direction vector
        float angle = Vector3.Angle(m_vLandingPosDir, transform.forward);

         //The dot product will tell which side the target is. 
        float dotP = Vector3.Dot(m_vLandingPosDir, transform.right);

        //If it is positive the target is right of the kicking location
        if (dotP > 0)
        {
            //This rotates the kicking location that has the camera as a child so it rotates as well
            transform.Rotate(0.0f, angle, 0.0f);
            //This rotates the target disk as well so it always has the flat side facing the camera
            m_Target.transform.Rotate(angle, 0, 0.0f);
        }
        //Otherwise the target is left of the kicking location
        else
        {
            //This rotates the kicking location that has the camera as a child so it rotates as well
            transform.Rotate(0.0f, -angle, 0.0f);
            m_Target.transform.Rotate(-angle, 0, 0.0f);
        }
    }
    private void CreateLandingDisplay()
    {
        //Creates the target the player will use to aim their shots
        m_Target = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        m_Target.transform.position = transform.position + (transform.forward *5);
        m_Target.transform.localScale = new Vector3(2.5f, 0.1f, 2.5f);
        m_Target.transform.Rotate(0.0f, 0.0f, 90.0f);
        m_Target.GetComponent<Renderer>().material.color = Color.blue;
        //Stops the ball from being able to hit the target
        m_Target.GetComponent<Collider>().enabled = false;
    }


    private void MoveTargetLeft(float speed)
    {
        m_vTargetPosition -= (transform.right * speed);
    }
    private void MoveTargetRight(float speed)
    {
        m_vTargetPosition += (transform.right * speed);
    }
    private void MoveTargetForward(float speed)
    {
        m_vTargetPosition += (transform.forward * speed);
    }
    private void MoveTargetBackwards(float speed)
    {
        //Stops the target from being able to get too close to the kicking location
        //Otherwise rotation can happen too fast
        float distance = Vector3.Distance(transform.position, m_Target.transform.position);
        if (distance > 5.0f)
        {
           m_vTargetPosition -= (transform.forward * speed);
        }
    }
    private void MoveTargetUp(float speed)
    {
        m_vTargetPosition.y += speed;
        //= new Vector3(m_Target.transform.position.x, val, m_Target.transform.position.z);
        //m_vTargetPosition += (transform.up * speed);
    }

    private void MoveTargetDown(float speed)
    {
        if (m_Target.transform.position.y > 39.0f)
        {
            m_vTargetPosition.y -= speed;
        }
        //m_vTargetPosition += (transform.up * speed);
    }

    private void Move()
    {
            if (m_bTurnLeft)
            {
                MoveTargetLeft(m_fRotationSpeed);
            }
            if (m_bTurnRight)
            {
                MoveTargetRight(m_fRotationSpeed);
            }
            if (m_bMoveForward)
            {
                MoveTargetForward(m_fRotationSpeed * 1.1f);
            }
            if (m_bMoveBackwards)
            {
                MoveTargetBackwards(m_fRotationSpeed * 1.1f);
            }
            if (m_bRotateUp)
            {
                MoveTargetUp(m_fRotationSpeed);

            }
            if (m_bRotateDown)
            {
                MoveTargetDown(m_fRotationSpeed);
            }
        

    }

    private void ResetSceneObjects()
    {
        //This resets objects and variables in game to starting values for next shot isnt affected by last
        transform.rotation = m_qStartRotation;
        m_soccerBall.transform.position = m_vStartingPosition;
        //Stops all ball velocity so it is at rest for next shot
        m_soccerBall.GetComponent<BallPhysics>().m_rb.velocity = Vector3.zero;
        m_soccerBall.GetComponent<BallPhysics>().m_rb.angularVelocity = Vector3.zero;
        m_soccerBall.GetComponent<BallPhysics>().m_bCanHitBullseye = true;
        //Resets target values
        m_vTargetPosition = Vector3.zero;
        m_vTargetPosition += transform.position + transform.forward * 5;
        m_vTargetPosition.y = transform.position.y + 2.0f;
        m_vPosition = m_vTargetPosition;
        m_Target.transform.position = m_vTargetPosition;

        m_Target.transform.rotation = m_TargetStartingRotation;
        //Resets scene booleans so that if a button is held while scene is reset it won't affect new kicking positon
        ResetSceneVariables();

    }

    private void ResetSceneVariables()
    {
        m_bTurnLeft = false;
        m_bTurnRight = false;
        m_bRotateUp = false;
        m_bRotateDown = false;
        m_bBallKicked = false;
    }

    private void CreateRandomBullseye()
    {
        //This function creates a bullseye at a random location inside the goal when the intial bullseyes have been hit
        if (m_iBullseyesRemaining <= 5 && m_iBullseyesRemaining > 0)
        {
            //Only need to move one target for random as they arent destroyed
            m_bullseye = this.GetComponent<GoalBullseyes>().bullsEyes[0];

            if (m_fTimer <= 0.0f)
            {
                Vector3 position = new Vector3(85.0f, Random.Range(39.0f, 49.0f), Random.Range(-14.0f, 12.5f));
                //When a bullseye is hit it is not destroyed it is moved to (0, 0, 0) so that I do not have to constantly create and destory bullseye objects
                m_bullseye.transform.position = position;
                //A timer starts when a random bullseye is created and the player has 6.0 seconds to hit it or it moves
                m_fTimer = 6.0f;
            }
            //This decrements the timer 1 per second. Ensuring that it is consistent with 
            m_fTimer -= 1 * Time.deltaTime;
        }
    }
    
    private void OnDrawGizmos()
    {
        
    }
}
