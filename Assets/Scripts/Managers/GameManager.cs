using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour {

    public float _startDelay;
    private float _checkDelay;
    public CameraControl _cameraControl;
    public Canvas _messageCanvas;
    public GameObject _diceSelectorPrefab;
    public GameObject _d2Prefab;
    public GameObject _d4Prefab;
    public GameObject _d6Prefab;
    public GameObject _d8Prefab;
    public GameObject _d10Prefab;
    public GameObject _d12Prefab;
    public GameObject _d20Prefab;
    public GameObject _d100Prefab;
    public int _diceSelectionAdjustment;
    public bool _isTesting = false;

    private WaitForSeconds _startWait;
    private List<DiceSet> _diceSets;
    private List<DiceSet> _viewDiceSets;
    private bool _doneRolling = false;
    private bool _checkRollingAgain = true;
    private int _totalDiceCount;


    // Use this for initialization
    void Start () {
        
        if (_isTesting == false)
        {
            _startWait = new WaitForSeconds(_startDelay);
        }
        Screen.orientation = ScreenOrientation.Portrait;
        

    }

    private void GameLoop()
    {

    }

    public bool GetIsTesting()
    {
        return _isTesting;
    }

    public void SortDiceSets()
    {
        foreach (DiceSet diceSet in _viewDiceSets)
        {
            diceSet._diceManagers = SortDiceManagersList(diceSet._diceManagers);
        }
        foreach (DiceSet diceSet in _diceSets)
        {
            diceSet._diceManagers = SortDiceManagersList(diceSet._diceManagers);
        }
    }

    private List<DiceManager> SortDiceManagersList(List<DiceManager> diceManagers)
    {
        for(int i = 0; i < diceManagers.Count; i++)
        {
            for (int j = i; j > 0 && diceManagers[j - 1].GetValue() < diceManagers[j].GetValue(); j--)
            {
                DiceManager tempDice = diceManagers[j - 1];
                diceManagers[j - 1] = diceManagers[j];
                diceManagers[j] = tempDice;
            }
        }
        return diceManagers;
    }

    private void SetCameraTargets()
    {
        Transform[] targets = new Transform[_totalDiceCount];
        int currentCount = 0;

        for (int i = 0; i < _diceSets.Count; i++)
        {
            List<DiceManager> currentDice = _diceSets[i]._diceManagers;
            for(int j = 0; j < currentDice.Count; j++)
            {
                targets[currentCount] = currentDice[j]._instance.transform;
                currentCount += 1;
            }
            
        }

        _cameraControl._targets = targets;
        _cameraControl.SetMoveToDefault(false);
    }

    public void RollDice(List<DiceSetInfo> diceSetInfos)
    {
        int yCoordinate = 10;
        float xCoordinate = 0;
        int zCoordinate = 0;
        bool isPositive = true;
        int diceColorIndex = 0;
        int viewXCoordinate = -50;
        float xCoordinateModifier = 1f;
        int currentZCoordinate = 0;
        
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
                       

            currentDiceSet._diceManagers = new List<DiceManager>();
            currentViewDiceSet._diceManagers = new List<DiceManager>();
            currentDiceSet._modifier = diceSetInfo._diceModifier;
            currentDiceSet._diceType = diceSetInfo._diceType;
            currentViewDiceSet._diceType = diceSetInfo._diceType;

            diceColorIndex = UnityEngine.Random.Range(0, 6);
            
            if (diceSetInfo._diceType == 6)
            {
                xCoordinateModifier = 1.3f;
            }
            else
            {
                xCoordinateModifier = 1.2f;
            }


            if (diceSetInfo._numberOfDice < 10)
            {
                xCoordinate = - xCoordinateModifier * (diceSetInfo._numberOfDice / 2);
            }
            else
            {
                xCoordinate = -8;
            }


            currentZCoordinate = zCoordinate;
            for (int j = 0; j < diceSetInfo._numberOfDice; j++)
            {
                DiceManager currentDice = new DiceManager();
                DiceManager currentViewDice = new DiceManager();
                currentDice = SetDiceType(currentDice, diceSetInfo._diceType);
                currentViewDice = SetDiceType(currentViewDice, diceSetInfo._diceType);
                if(diceSetInfo._diceType == 2)
                {
                    currentViewDice._instance.gameObject.GetComponent<DiceScript>().SetIsD2(true);
                    currentDice._instance.gameObject.GetComponent<DiceScript>().SetIsD2(true);
                }
                currentViewDice._instance.gameObject.GetComponent<DiceScript>().SetIsRolling(false);
                currentDice.SetTexture(diceSetInfo._diceType, diceColorIndex);
                currentViewDice.SetTexture(diceSetInfo._diceType, diceColorIndex);
                currentDice.RandomizeDice();

                currentDice._instance.transform.position = new Vector3(xCoordinate, yCoordinate, currentZCoordinate);
                currentViewDice._instance.transform.position = new Vector3(xCoordinate + viewXCoordinate, yCoordinate, currentZCoordinate);
                currentViewDice._instance.GetComponent<Rigidbody>().useGravity = false;
                currentDiceSet._diceManagers.Add(currentDice);
                currentViewDiceSet._diceManagers.Add(currentViewDice);
                _totalDiceCount++;


                if (isPositive)
                {
                    xCoordinate += xCoordinateModifier;
                }
                else
                {
                    xCoordinate -= xCoordinateModifier;
                }


                if (xCoordinate >= 9)
                {
                    xCoordinate -= 5;
                    yCoordinate += 4;
                    isPositive = false;
                    currentZCoordinate -= 1;
                }
                if (xCoordinate <= -9)
                {
                    xCoordinate += 5;
                    yCoordinate += 4;
                    isPositive = true;
                    currentZCoordinate -= 1;
                }

            }
            zCoordinate -= 2;
            _diceSets.Add(currentDiceSet);
            _viewDiceSets.Add(currentViewDiceSet);
        }
        if (_totalDiceCount <= 5)
        {
            _checkDelay = .5f;
        }
        else if(_totalDiceCount > 5 && _totalDiceCount <= 20)
        {
            _checkDelay = 1f;
        }
        else if (_totalDiceCount > 20 && _totalDiceCount <= 40)
        {
            _checkDelay = 1.5f;
        }
        else
        {
            _checkDelay = 2f;
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
        yield return new WaitForSeconds(_checkDelay);
        _checkRollingAgain = true;
    }

    public void DestroyDice()
    {
        if(_diceSets != null)
        {
            foreach (DiceSet diceSet in _diceSets)
            {
                foreach(DiceManager dice in diceSet._diceManagers)
                {
                    Destroy(dice._instance.gameObject);
                }
            }
            _diceSets.Clear();
        }
        if(_viewDiceSets != null)
        {
            foreach (DiceSet diceSet in _viewDiceSets)
            {
                foreach (DiceManager dice in diceSet._diceManagers)
                {
                    Destroy(dice._instance.gameObject);
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
                dice._instance = GameObject.Instantiate(_d2Prefab);
                break;
            case 4:
                dice._instance = GameObject.Instantiate(_d4Prefab);
                break;
            case 6:
                dice._instance = GameObject.Instantiate(_d6Prefab);
                break;
            case 8:
                dice._instance = GameObject.Instantiate(_d8Prefab);
                break;
            case 10:
                dice._instance = GameObject.Instantiate(_d10Prefab);
                break;
            case 12:
                dice._instance = GameObject.Instantiate(_d12Prefab);
                break;
            case 20:
                dice._instance = GameObject.Instantiate(_d20Prefab);
                break;
            case 100:
                dice._instance = GameObject.Instantiate(_d100Prefab);
                break;
        }
        return dice;
    }

    // Update is called once per frame
    void Update () {
        if(_diceSets != null && _isTesting == false) { 
            _doneRolling = IsDoneRolling();

            if (_doneRolling == true)
            {
                _cameraControl.SetMoveToDefault(true);
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
                for (int j = 0; j < currentDiceSet._diceManagers.Count; j++)
                {
                    DiceManager currentDice = currentDiceSet._diceManagers[j];
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
        return _doneRolling && _cameraControl.GetAtDefaultPosition();
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
            DiceSet currentViewSet = _viewDiceSets[i];
            for (int j = 0; j < currentDiceSet._diceManagers.Count; j++)
            {
                DiceManager currentDice = currentDiceSet._diceManagers[j];
                diceSetTotal += currentDice.CalculateValue();
                currentViewSet._diceManagers[j].SetValue(currentDice.GetValue());
            }
            diceSetTotal += currentDiceSet._modifier;
            currentDiceSet._total = diceSetTotal;
            _diceSets[i] = currentDiceSet;
        }
    }
    

}

public class DiceSetInfo
{
    public int _numberOfDice;
    public int _diceType;
    public int _diceModifier;
}

public class DiceSet
{
    public List<DiceManager> _diceManagers;
    public int _modifier;
    public int _diceType;
    public int _total;
}
