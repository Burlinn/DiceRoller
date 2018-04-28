using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using System.IO;

public class GameManager : MonoBehaviour {

    public float m_StartDelay = 3.0f;
    public CameraControl m_CameraControl;
    public Canvas m_MessageCanvas;
    public GameObject DiceSelector;
    public GameObject D2;
    public GameObject D4;
    public GameObject D6;
    public GameObject D8;
    public GameObject D10;
    public GameObject D12;
    public GameObject D20;
    public GameObject D100;
    public int diceSelectionAdjustment;
    public int rollSpeed;

    private WaitForSeconds _startWait;
    private List<DiceSet> _diceSets;
    private bool _doneRolling = false;
    private int _diceSelectorsTotal = 1;
    private bool _rollingStart = false;



    // Use this for initialization
    void Start () {
        //Test();
        _startWait = new WaitForSeconds(m_StartDelay);
        GameLoop();
    }

    private void GameLoop()
    {
        //canvasManager.SetUp();

        //canvasManager.ShowIntroScreen();


    }

    
    public void RollDice(List<DiceSetInfo> diceSetInfos)
    {
        int yCoordinate = 10;
        int xCoordinate = 0;
        int zCoordinate = 0;
        bool isPositive = true;

        zCoordinate = diceSetInfos.Count;
        _diceSets = new List<DiceSet>();
        for (int i = 0; i < diceSetInfos.Count; i++)
        {
            yCoordinate = 10;
            DiceSetInfo diceSetInfo = diceSetInfos[i];
            DiceSet currentDiceSet = new DiceSet();
           

            currentDiceSet.m_Dice = new List<DiceManager>();
            currentDiceSet.modifier = diceSetInfo.diceModifier;
            if (diceSetInfo.numberOfDice < 10)
            {
                xCoordinate = -1 * (diceSetInfo.numberOfDice / 2);
            }
            else
            {
                xCoordinate = -8;
            }


            for (int j = 0; j < diceSetInfo.numberOfDice; j++)
            {
                DiceManager currentDice = new DiceManager();
                currentDice = SetDiceType(currentDice, diceSetInfo.diceType);
                currentDice.RandomizeDice();

                currentDice.m_Instance.transform.position = new Vector3(xCoordinate, yCoordinate, zCoordinate);

                currentDice.m_Instance.transform.GetComponent<Rigidbody>().AddForce(0, 0, rollSpeed);
                currentDiceSet.m_Dice.Add(currentDice);
                if (isPositive)
                {
                    xCoordinate += 1;
                }
                else
                {
                    xCoordinate -= 1;
                }


                if (xCoordinate >= 9)
                {
                    xCoordinate -= 1;
                    yCoordinate += 4;
                    isPositive = false;
                }
                if (xCoordinate <= -9)
                {
                    xCoordinate += 1;
                    yCoordinate += 4;
                    isPositive = true;
                }

            }
            zCoordinate -= 2;
            _diceSets.Add(currentDiceSet);
        }
        StartCoroutine(StartRoll());

    }


    public List<DiceSet> GetDiceSets()
    {
        return _diceSets;
    }

    IEnumerator StartRoll()
    {
        _rollingStart = true;
        yield return new WaitForSeconds(m_StartDelay);
        _rollingStart = false;
    }

    public void DestroyDice()
    {
        _diceSets.Clear();
        GameObject[] allDice = GameObject.FindGameObjectsWithTag("Dice");
        foreach (GameObject dice in allDice)
        {
            Destroy(dice);
        }


    }

    


    public DiceManager SetDiceType(DiceManager dice, int diceType)
    {
        switch (diceType)
        {
            case 2:
                dice.m_Instance = GameObject.Instantiate(D2);
                break;
            case 4:
                dice.m_Instance = GameObject.Instantiate(D4);
                break;
            case 6:
                dice.m_Instance = GameObject.Instantiate(D6);
                break;
            case 8:
                dice.m_Instance = GameObject.Instantiate(D8);
                break;
            case 10:
                dice.m_Instance = GameObject.Instantiate(D10);
                break;
            case 12:
                dice.m_Instance = GameObject.Instantiate(D12);
                break;
            case 20:
                dice.m_Instance = GameObject.Instantiate(D20);
                break;
        }
        return dice;
    }

    // Update is called once per frame
    void Update () {
        if(_diceSets != null) { 
            if(_rollingStart == false)
            {
            
                _doneRolling = IsDoneRolling();
      
                if (_doneRolling == true)
                {
                    GetTotals();
                }
            }
        }
    }

    bool IsDoneRolling()
    {
        bool noMovement = false;
        for (int i = 0; i < _diceSets.Count; i++)
        {
            DiceSet currentDiceSet = _diceSets[i];
            for (int j = 0; j < currentDiceSet.m_Dice.Count; j++)
            {
                DiceManager currentDice = currentDiceSet.m_Dice[j];
                if (currentDice.GetIsMoving() == false)
                {
                    noMovement = true;
                    break;
                }
            }
            if (noMovement == true)
            {
                break;
            }
        }
        return noMovement;
    }

    public bool GetDoneRolling()
    {
        return _doneRolling;
    }

    public void SetDoneRolling(bool doneRolling)
    {
        _doneRolling = doneRolling;
    }



    public void GetTotals()
    {
        int diceSetTotal = 0;
        for (int i = 0; i < _diceSets.Count; i++)
        {
            diceSetTotal = 0;
            DiceSet currentDiceSet = _diceSets[i];
            for (int j = 0; j < currentDiceSet.m_Dice.Count; j++)
            {
                DiceManager currentDice = currentDiceSet.m_Dice[j];
                diceSetTotal += currentDice.GetValue();
            }
            diceSetTotal += currentDiceSet.modifier;
            currentDiceSet.total = diceSetTotal;
            _diceSets[i] = currentDiceSet;
        }
    }

    void Test()
    {
        int currentDiceType = 20;
        int currentValue = 0;
        var testItems = new Dictionary<int, int>();
        DiceManager currentDice = new DiceManager();
        currentDice = SetDiceType(currentDice, currentDiceType);

        for (int i = 1; i <= 20; i++)
        {
            testItems[i] = 0;
        }

        for (int i = 0; i < 1000000; i++)
        {
          
            
            currentDice.RandomizeDice();

            currentValue = currentDice.GetValue();

            currentDice.UnRandomizeDice();

            var resetValue = currentDice.GetValue();

            testItems[currentValue] = testItems[currentValue] + 1;
            
            
        }
        string path = "Assets/Resources/TestLog.txt";

        //Write some text to the test.txt file
        StreamWriter writer = new StreamWriter(path, true);
        writer.WriteLine("");
        for (int i = 1; i <= 20; i++)
        {
            writer.WriteLine(i + ":" + testItems[i]);
        }
        writer.Close();
    }


}

public class DiceSetInfo
{
    public int numberOfDice;
    public int diceType;
    public int diceModifier;
}

public class DiceSet
{
    public List<DiceManager> m_Dice;
    public int modifier;
    public int total;
}
