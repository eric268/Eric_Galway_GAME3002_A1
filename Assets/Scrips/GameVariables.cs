using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameVariables : MonoBehaviour
{
    [SerializeField]
    Text m_BullseyeRemainingText;

    [SerializeField]
    Text m_FastestTimeText;

    private float m_fFastestTime;

    CountdownTimer m_TimerObject = null;
     
    private float m_fCurrentTime;
    // Start is called before the first frame update
    void Start()
    {
        //This sets the remaining bullseye text to inital value (10)
        m_BullseyeRemainingText.text = ArrowDirection.m_iBullseyesRemaining.ToString();
        m_TimerObject = GameObject.Find("Canvas").GetComponent<CountdownTimer>();
        //Fastest time is set to 60 because this way when game is completed for first time no way
        //for that time to be higher than fastest time check if game has never been beaten
        m_fFastestTime = 60.0f;
    }

    // Update is called once per frame
    void Update()
    {
        m_BullseyeRemainingText.text = ArrowDirection.m_iBullseyesRemaining.ToString();

        //This checks to see if the game has been won
        if (ArrowDirection.m_iBullseyesRemaining == 0 && m_TimerObject.m_fCurrentTime > 0)
        {
            //If thats the case want to check if the time it took to beat that game is the fastest
            //If so then set current time to fastest time and display it on canvas
            m_fCurrentTime = 60.0f - m_TimerObject.m_fCurrentTime;
            if (m_fCurrentTime < m_fFastestTime)
            {
                m_fFastestTime = m_fCurrentTime;
                m_FastestTimeText.text = m_fFastestTime.ToString("0");
            }
        }
    }
}
