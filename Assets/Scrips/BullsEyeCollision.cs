using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BullsEyeCollision : MonoBehaviour
{
    private GameObject soccerBall;
    private GameObject kickLocation;
    // Start is called before the first frame update
    void Start()
    {
        soccerBall = GameObject.Find("Soccer Ball");
        kickLocation = GameObject.Find("KickLocation");

    }

    // Update is called once per frame
    void Update()
    {
       
    }
    private void OnCollisionEnter(Collision collision)
    {
        //Checks for collison between bullseyes and soccer ball
        //Also checks to make sure ball hasn't hit the back of the net before it hits target
        if (collision.gameObject.name == "Soccer Ball" && soccerBall.GetComponent<BallPhysics>().m_bCanHitBullseye)
        {
            //Decrements number of bullseyes remaining 
            ArrowDirection.m_iBullseyesRemaining -= 1;
            //Moves the target to far away positon (uses object pooling)
            transform.parent.gameObject.transform.position = new Vector3(0.0f, 0.0f, 0.0f);
            //Stops the ball from being able to hit multiple targets in one shot
            soccerBall.GetComponent<BallPhysics>().m_bCanHitBullseye = false;
            //Sets the timer for random bullseye generation to begin shortly 
            kickLocation.GetComponent<ArrowDirection>().m_fTimer = 1.0f;
        }
    }
}
