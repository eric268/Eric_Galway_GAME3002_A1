using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CountdownTimer : MonoBehaviour
{
    public float m_fCurrentTime = 60.0f;
    private float m_fStartingTime = 60.0f;

    public bool m_bStartTimer = false;

    GameObject kickLocation;

    [SerializeField]
    public Text m_CountdownText;

    [SerializeField]
    public Text m_Victory;
    // Start is called before the first frame update
    void Start()
    {
        m_fCurrentTime = m_fStartingTime;
        kickLocation = GameObject.Find("KickLocation");
        m_CountdownText.text = m_fCurrentTime.ToString("0");
    }

    // Update is called once per frame
    void Update()
    {
        //Check to make sure we want to start the game and run timer
        if (m_bStartTimer && ArrowDirection.m_bStartGame)
        {
            //If the timer is 0 then the game has been lost and display it to screen and stop timer
            if (m_fCurrentTime <=0.0f)
            {
                m_fCurrentTime = 0.0f;
                m_Victory.text = "Defeat!";
                m_Victory.color = Color.red;
                m_bStartTimer = false;
            }
            //If timer < 5 then display timer in red to show close to end
            if (m_fCurrentTime <= 5.0f)
            {
                m_CountdownText.color = Color.red;
            }
            //Decrement the timer ensursing it is frame dependant
            m_fCurrentTime -= 1 * Time.deltaTime;
            m_CountdownText.text = m_fCurrentTime.ToString("0");

            //Checks if game has been won. Displays victory and stops timer
            if (ArrowDirection.m_iBullseyesRemaining == 0)
            {
                m_Victory.text = "Success!";
                m_Victory.color = Color.green;
                m_bStartTimer = false;
            }
        }
    }
}
