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
        if (collision.gameObject.name == "Soccer Ball" && soccerBall.GetComponent<BallPhysics>().m_bCanHitBullseye)
        {
            ArrowDirection.m_iBullseyesRemaining -= 1;
            transform.parent.gameObject.transform.position = new Vector3(0.0f, 0.0f, 0.0f);
            Debug.Log("Hit");
            soccerBall.GetComponent<BallPhysics>().m_bCanHitBullseye = false;

            kickLocation.GetComponent<ArrowDirection>().m_fTimer = 1.0f;
        }
    }
}
