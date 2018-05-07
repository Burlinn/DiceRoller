﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceManager : MonoBehaviour {

    [HideInInspector]
    public GameObject[] m_values;
    [HideInInspector]
    public GameObject m_Instance;
    public bool isMoving;
    public Vector3 lastPosition;
    public Vector3 secondLastPosition;
    public List<string> _diceColors;




    // Use this for initialization
    void Start() {
       
    }

    private void SetUpColors()
    {
        _diceColors = new List<string>();
        _diceColors.Add("Yellow");
        _diceColors.Add("Black");
        _diceColors.Add("BlueGrey");
        _diceColors.Add("DarkGreen");
        _diceColors.Add("DarkRed");
        _diceColors.Add("LightBlue");
    }

    public void RandomizeDice()
    {
        m_Instance.transform.rotation = UniformRandomRotation();
    }

    public void SetTexture(int diceType, int diceColorIndex)
    {

        string diceColor;
        string path;
        Texture texture;

        if (_diceColors == null)
        {
            SetUpColors();
        }
        diceColor = _diceColors[diceColorIndex];
        if(diceType == 2)
        {
            path = "Dice_Textures/D" + diceType + "/D" + diceType + "_Texture";
        }
        else
        {
            path = "Dice_Textures/D" + diceType + "/D" + diceType + "_Texture_" + diceColor;
        }
        
        texture = Resources.Load(path, typeof(Texture)) as Texture;

        m_Instance.gameObject.GetComponent<Renderer>().material.mainTexture = texture;

    }

    public DiceManager CopyDiceForView(DiceManager dice)
    {
        DiceManager newDice = new DiceManager();
        newDice.m_values = dice.m_values;
        return newDice;
    }

    Quaternion UniformRandomRotation()
    {
        float x0 = UnityEngine.Random.value;
        float theta1 = UnityEngine.Random.Range(0f, Mathf.PI * 2f);
        float theta2 = UnityEngine.Random.Range(0f, Mathf.PI * 2f);
        // Make this theta2 in the range (0, PI * 1) if you want w > 0

        float r1 = Mathf.Sqrt(1f - x0);
        float r2 = Mathf.Sqrt(x0);

        return new Quaternion(
            Mathf.Cos(theta2) * r2,
            Mathf.Sin(theta1) * r1,
            Mathf.Cos(theta1) * r1,
            Mathf.Sin(theta2) * r2);
    }

    public void UnRandomizeDice()
    {
        m_Instance.transform.rotation = Quaternion.Euler(270, 270, 270);
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
