using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;
using UnityEngine;

public class BallPhysics : MonoBehaviour
{

    private Vector3 m_vPreviousVelocity = Vector3.zero;
    public Rigidbody m_rb = null;
    //private float m_fSpeed = 0.0f;
    public bool m_bCanHitBullseye = true;

    // Start is called before the first frame update
    void Start()
    {
        //Ensures there is a rigid body to allow for physics on ball
        m_rb = GetComponent<Rigidbody>();
        Assert.IsNotNull(m_rb, "No rigid body found on ball");
    }

    // Update is called once per frame
    void Update()
    {
        m_vPreviousVelocity = m_rb.velocity;
    }
    private void OnCollisionEnter(Collision collision)
    {
        //This checks if ball has hit the back of the net. If so 
        //flips flag that does not allow it to bounce and hit targets
        if (collision.gameObject.name == "MissedTargetCollider")
        {
            m_bCanHitBullseye = false;
        }
        float m_fSpeed = m_vPreviousVelocity.magnitude;
        //This detects collision between walls and bounces ball off the wall losing 50% of its total speed
        var direction = Vector3.Reflect(m_vPreviousVelocity.normalized, collision.contacts[0].normal);
        m_rb.velocity = (direction * Mathf.Max(m_fSpeed, 0f)) * 0.5f;
    }
}
