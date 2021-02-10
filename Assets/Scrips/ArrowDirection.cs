using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowDirection : MonoBehaviour
{
    private float m_fRotationSpeed = 0.05f;
    private float m_fRotationLimit = 0.0f;
    private bool m_bTurnLeft = false;
    private bool m_bTurnRight = false;
    private bool m_bRotateUp = false;
    private bool m_bRotateDown = false;
    
    GameObject m_arrow;

    // Start is called before the first frame update
    void Start()
    {
        m_arrow = GameObject.Find("arrow");
        if (!m_arrow)
        {
            Debug.LogError("Can't find arrow");
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        HandleEvents();
        Move();
        //m_arrow.transform.position += transform.forward;
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
        Debug.Log("rotation limit:" + m_fRotationLimit);
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

    }
    private void OnDrawGizmos()
    {
        
    }
}
