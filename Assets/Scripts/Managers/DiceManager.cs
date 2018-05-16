using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceManager : MonoBehaviour {

    [HideInInspector]
    public GameObject[] m_values;
    [HideInInspector]
    public GameObject m_Instance;
    private bool isMoving;
    private Vector3 lastPosition;
    private List<string> _diceColors;
    public GameManager _gameManager;



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
        m_Instance.transform.rotation = Quaternion.LookRotation(UnityEngine.Random.onUnitSphere, UnityEngine.Random.onUnitSphere);
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
        if (currentPosition.ToString() == lastPosition.ToString())
        {
            isMoving = false;
        }
        else
        {
            isMoving = true;
        }
        lastPosition = currentPosition;
        
        return isMoving;
    }


    // Update is called once per frame
    void Update () {
         

    }
}
