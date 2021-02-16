using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalBullseyes : MonoBehaviour
{
    private int m_iNumberBullseyes = 5;
    static public bool m_bResetBullseyePosition = false;

    public GameObject[] bullsEyes;
    public GameObject prefab;

    // Start is called before the first frame update
    void Start()
    {
        //Creates a pool of bullseye objects and stores them in an array
        for (int i = 0; i < m_iNumberBullseyes; i++)
        {
            bullsEyes[i] = Instantiate(prefab) as GameObject;

            bullsEyes[i].name = "Bullseye" + i;
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (m_bResetBullseyePosition)
        {
            //This moves the bullseyes from their starting positon of (0,0,0) to positons in foal
            m_bResetBullseyePosition = false;
            bullsEyes[0].transform.position = new Vector3(85.0f, 48.0f, 12.0f);
            bullsEyes[1].transform.position = new Vector3(85.0f, 39.0f, 12.0f);
            bullsEyes[2].transform.position = new Vector3(85.0f, 42.5f, 0.0f);
            bullsEyes[3].transform.position = new Vector3(85.0f, 48.0f, -12.0f);
            bullsEyes[4].transform.position = new Vector3(85.0f, 39.0f, -12.0f);


        }
    }

    public void ResetBullseyePositionto0()
    {
        bullsEyes[0].transform.position = new Vector3(0.0f, 0.0f, 0.0f);
        bullsEyes[1].transform.position = new Vector3(0.0f, 0.0f, 0.0f);
        bullsEyes[2].transform.position = new Vector3(0.0f, 0.0f, 0.0f);
        bullsEyes[3].transform.position = new Vector3(0.0f, 0.0f, 0.0f);
        bullsEyes[4].transform.position = new Vector3(0.0f, 0.0f, 0.0f);
    }
}
