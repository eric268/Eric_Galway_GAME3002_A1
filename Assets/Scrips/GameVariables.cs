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
        m_BullseyeRemainingText.text = ArrowDirection.m_iBullseyesRemaining.ToString();
        m_TimerObject = GameObject.Find("Canvas").GetComponent<CountdownTimer>();
        m_fFastestTime = 60.0f;
    }

    // Update is called once per frame
    void Update()
    {
        m_BullseyeRemainingText.text = ArrowDirection.m_iBullseyesRemaining.ToString();

        if (ArrowDirection.m_iBullseyesRemaining == 0 && m_TimerObject.m_fCurrentTime > 0)
        {
            m_fCurrentTime = 60.0f - m_TimerObject.m_fCurrentTime;
            if (m_fCurrentTime < m_fFastestTime)
            {
                m_fFastestTime = m_fCurrentTime;
                m_FastestTimeText.text = m_fFastestTime.ToString("0");
            }
        }
    }
}
