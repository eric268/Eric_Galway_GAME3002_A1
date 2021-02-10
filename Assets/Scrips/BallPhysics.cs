using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;
using UnityEngine;

public class BallPhysics : MonoBehaviour
{

    private Vector3 m_vPreviousVelocity = Vector3.zero;
    public Rigidbody m_rb = null;
    private float m_fSpeed = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
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
        float m_fSpeed = m_vPreviousVelocity.magnitude;
        var direction = Vector3.Reflect(m_vPreviousVelocity.normalized, collision.contacts[0].normal);
        m_rb.velocity = (direction * Mathf.Max(m_fSpeed, 0f)) * 0.5f;
    }
}
