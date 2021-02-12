using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BullsEyeCollision : MonoBehaviour
{
    private GameObject soccerBall;
    // Start is called before the first frame update
    void Start()
    {
        soccerBall = GameObject.Find("Soccer Ball");
    }

    // Update is called once per frame
    void Update()
    {
       
    }
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Hit");
        if (collision.gameObject.name == "Soccer Ball" && soccerBall.GetComponent<BallPhysics>().m_bCanHitBullseye)
        {
            transform.position = new Vector3(0.0f, 0.0f, 0.0f);
            soccerBall.GetComponent<BallPhysics>().m_bCanHitBullseye = false;
        }
    }
}
