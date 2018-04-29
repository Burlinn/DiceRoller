using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class TestManager : MonoBehaviour {

    public GameManager _gameManager;
    private bool _isTesting = false;
    private int _testCount = 0;
    private Dictionary<int, int> _testItems = new Dictionary<int, int>();
    private bool isRolling;

    // Use this for initialization
    void Start () {
        _isTesting = _gameManager.GetIsTesting();
        if (_isTesting == true)
        {
            for (int i = 1; i <= 20; i++)
            {
                _testItems[i] = 0;
            }
            Test2();
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (_isTesting == true && isRolling == false)
        {
            
            if (_gameManager.IsDoneRolling() == true)
            {
                if (_testCount <= 40)
                {
                    GetTestResults();
                    _gameManager.DestroyDice();
                    Test2();
                    _testCount += 1;
                    StartCoroutine(StartRoll());
                }
                else
                {
                    _isTesting = false;
                    WriteResults();
                }

            }
        }
    }

    IEnumerator StartRoll()
    {
        isRolling = true;
        yield return new WaitForSeconds(5);
        isRolling = false;
    }


    void Test2()
    {
        List<DiceSetInfo> diceSetInfos = new List<DiceSetInfo>();
        DiceSetInfo currentDiceSetInfo = new DiceSetInfo();
        _isTesting = true;
        for (int i = 0; i < 8; i++)
        {
            currentDiceSetInfo = new DiceSetInfo();

            currentDiceSetInfo.numberOfDice = 8;

            currentDiceSetInfo.diceModifier = 0;

            currentDiceSetInfo.diceType = 20;
            diceSetInfos.Add(currentDiceSetInfo);


        }
        _gameManager.RollDice(diceSetInfos);
    }

    public void GetTestResults()
    {
        int currentValue = 0;

        for (int i = 0; i < _gameManager.GetDiceSets().Count; i++)
        {
            DiceSet currentDiceSet = _gameManager.GetDiceSets()[i];
            for (int j = 0; j < currentDiceSet.m_Dice.Count; j++)
            {
                DiceManager currentDice = currentDiceSet.m_Dice[j];
                currentValue = currentDice.GetValue();
                _testItems[currentValue] = _testItems[currentValue] + 1;
            }
        }


    }

    public void WriteResults()
    {
        string path = "Assets/Resources/TestLog.txt";

        //Write some text to the test.txt file
        StreamWriter writer = new StreamWriter(path, true);
        writer.WriteLine("");
        for (int i = 1; i <= 20; i++)
        {
            writer.WriteLine(i + ":" + _testItems[i]);
        }
        writer.Close();
    }

    void Test()
    {
        int currentDiceType = 20;
        int currentValue = 0;
        var testItems = new Dictionary<int, int>();
        DiceManager currentDice = new DiceManager();
        currentDice = _gameManager.SetDiceType(currentDice, currentDiceType);

        for (int i = 1; i <= 8; i++)
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
