using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour {

    public float m_StartDelay = 3.0f;
    public float m_CheckDelay = 0.5f;
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
    public bool _isTesting = false;

    private WaitForSeconds _startWait;
    private List<DiceSet> _diceSets;
    private List<DiceSet> _viewDiceSets;
    private bool _doneRolling = false;
    private bool _checkRollingAgain = true;
    private int _diceSelectorsTotal = 1;
    private int _totalDiceCount;


    // Use this for initialization
    void Start () {
        
        if (_isTesting == false)
        {
            _startWait = new WaitForSeconds(m_StartDelay);
        }
        


    }

    private void GameLoop()
    {

    }

    public bool GetIsTesting()
    {
        return _isTesting;
    }

    private void SetCameraTargets()
    {
        Transform[] targets = new Transform[_totalDiceCount];
        int currentCount = 0;

        for (int i = 0; i < _diceSets.Count; i++)
        {
            List<DiceManager> currentDice = _diceSets[i].m_Dice;
            for(int j = 0; j < currentDice.Count; j++)
            {
                targets[currentCount] = currentDice[j].m_Instance.transform;
                currentCount += 1;
            }
            
        }

        m_CameraControl.m_Targets = targets;
        m_CameraControl.SetMoveToDefault(false);
    }

    public void RollDice(List<DiceSetInfo> diceSetInfos)
    {
        int yCoordinate = 10;
        int xCoordinate = 0;
        int zCoordinate = 0;
        bool isPositive = true;
        int diceColorIndex = 0;
        int viewXCoordinate = -50;
        
        zCoordinate = diceSetInfos.Count;
        _diceSets = new List<DiceSet>();
        _viewDiceSets = new List<DiceSet>();
        _totalDiceCount = 0;
        for (int i = 0; i < diceSetInfos.Count; i++)
        {
            yCoordinate = 10;
            DiceSetInfo diceSetInfo = diceSetInfos[i];
            DiceSet currentDiceSet = new DiceSet();
            DiceSet currentViewDiceSet = new DiceSet();
            diceColorIndex = UnityEngine.Random.Range(0, 6);

            currentDiceSet.m_Dice = new List<DiceManager>();
            currentViewDiceSet.m_Dice = new List<DiceManager>();
            currentDiceSet.modifier = diceSetInfo.diceModifier;
            currentDiceSet.diceType = diceSetInfo.diceType;
            currentViewDiceSet.diceType = diceSetInfo.diceType;
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
                DiceManager currentViewDice = new DiceManager();
                currentDice = SetDiceType(currentDice, diceSetInfo.diceType);
                currentViewDice = SetDiceType(currentViewDice, diceSetInfo.diceType);
                currentViewDice.m_Instance.gameObject.GetComponent<DiceScript>().SetIsRolling(false);
                currentDice.SetTexture(diceSetInfo.diceType, diceColorIndex);
                currentViewDice.SetTexture(diceSetInfo.diceType, diceColorIndex);
                currentDice.RandomizeDice();

                currentDice.m_Instance.transform.position = new Vector3(xCoordinate, yCoordinate, zCoordinate);
                currentViewDice.m_Instance.transform.position = new Vector3(xCoordinate + viewXCoordinate, yCoordinate, zCoordinate);
                currentViewDice.m_Instance.GetComponent<Rigidbody>().useGravity = false;
                currentDiceSet.m_Dice.Add(currentDice);
                currentViewDiceSet.m_Dice.Add(currentViewDice);
                _totalDiceCount++;
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
                    xCoordinate -= 5;
                    yCoordinate += 4;
                    isPositive = false;
                }
                if (xCoordinate <= -9)
                {
                    xCoordinate += 5;
                    yCoordinate += 4;
                    isPositive = true;
                }

            }
            zCoordinate -= 2;
            _diceSets.Add(currentDiceSet);
            _viewDiceSets.Add(currentViewDiceSet);
        }
        SetCameraTargets();
    }


    public List<DiceSet> GetDiceSets()
    {
        return _diceSets;
    }

    public List<DiceSet> GetViewDiceSets()
    {
        return _viewDiceSets;
    }
    
    IEnumerator ResetCheckAgain()
    {
        _checkRollingAgain = false;
        yield return new WaitForSeconds(m_CheckDelay);
        _checkRollingAgain = true;
    }

    public void DestroyDice()
    {
        List<DiceManager> diceList;
        if(_diceSets != null)
        {
            foreach (DiceSet diceSet in _diceSets)
            {
                foreach(DiceManager dice in diceSet.m_Dice)
                {
                    Destroy(dice.m_Instance.gameObject);
                }
            }
            _diceSets.Clear();
        }
        if(_viewDiceSets != null)
        {
            foreach (DiceSet diceSet in _viewDiceSets)
            {
                foreach (DiceManager dice in diceSet.m_Dice)
                {
                    Destroy(dice.m_Instance.gameObject);
                }
            }
            _viewDiceSets.Clear();
        }
        GameObject[] allDice = GameObject.FindGameObjectsWithTag("Dice");
        foreach (GameObject dice in allDice)
        {
            Destroy(dice);
        }
        _totalDiceCount = 0;

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
            case 100:
                dice.m_Instance = GameObject.Instantiate(D100);
                break;
        }
        return dice;
    }

    // Update is called once per frame
    void Update () {
        if(_diceSets != null && _isTesting == false) { 
            _doneRolling = IsDoneRolling();
      
            if (_doneRolling == true && m_CameraControl.GetAtDefaultPosition() == true)
            {
                GetTotals();
            }
            else if(_doneRolling == true){
                m_CameraControl.SetMoveToDefault(true);
            }
  
        }
    }

    public bool IsDoneRolling()
    {
        bool noMovement = false;
        if( _checkRollingAgain == true) { 
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
                else
                {
                    StartCoroutine(ResetCheckAgain());
                }
            }
        }
        return noMovement;
    }

    public bool GetDoneRolling()
    {
        return _doneRolling;
    }

    public bool GetDoneRollingAndCameraDefaulted()
    {
        return _doneRolling && m_CameraControl.GetAtDefaultPosition();
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
    public int diceType;
    public int total;
}
