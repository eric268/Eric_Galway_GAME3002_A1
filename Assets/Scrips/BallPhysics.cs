using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;
using UnityEngine;

public class BallPhysics : MonoBehaviour
{
    private float m_fKickStrength = 1.0f;
    private float m_fTheta = 1.0f;
    private Vector3 m_vDirection = Vector3.zero;
    private Rigidbody m_rb = null;

    // Start is called before the first frame update
    void Start()
    {
        m_rb = GetComponent<Rigidbody>();
        Assert.IsNotNull(m_rb, "No rigid body found on ball");
        m_vDirection = transform.forward;
    }

    // Update is called once per frame
    void Update()
    {
       
    }
}
