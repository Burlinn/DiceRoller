using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceManager : MonoBehaviour {

    [HideInInspector]
    public GameObject[] m_values;
    [HideInInspector]
    public Texture m_texture;
    [HideInInspector]
    public GameObject m_Instance;
    public bool isMoving;
    public Vector3 lastPosition;
    public Vector3 secondLastPosition;






    // Use this for initialization
    void Start() {

    }

    public void RandomizeDice()
    {
        if (UnityEngine.Random.Range(0.0f, 10f) == 1)
        {
            m_Instance.transform.Rotate(UnityEngine.Random.Range(0.0f, 360.0f), UnityEngine.Random.Range(0.0f, 360.0f), UnityEngine.Random.Range(0.0f, 360.0f));
        }
        else { 
            m_Instance.transform.rotation = Quaternion.Euler(UnityEngine.Random.Range(0.0f, 360.0f), UnityEngine.Random.Range(0.0f, 360.0f), UnityEngine.Random.Range(0.0f, 360.0f));
        }
    }

    public int GetValue()
    {
        int selectedValue = 0;
        GameObject values = m_Instance.transform.Find("Values").gameObject;
        GameObject currentValue;
        float highestValue = 0;
        for (int i = 0; i < values.transform.childCount; i++)
        {
            currentValue = values.transform.GetChild(i).gameObject;
            if (currentValue.transform.position.y > highestValue)
            {
                highestValue = currentValue.transform.position.y;
                selectedValue = Convert.ToInt32(currentValue.name);
            }
        }
       
        return selectedValue;
    }

    public bool GetIsMoving()
    {
        bool isMoving = true;
        Vector3 currentPosition = m_Instance.transform.position;
        if (currentPosition.Equals(lastPosition) && currentPosition.Equals(secondLastPosition))
        {
            isMoving = false;
        }
        else
        {
            isMoving = true;
        }
        secondLastPosition = lastPosition;
        lastPosition = currentPosition;
        
        return isMoving;
    }
	
	// Update is called once per frame
	void Update () {
        
	}
}
