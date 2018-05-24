using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

public class CanvasManager : MonoBehaviour
{



    #region Global Variables

    public CameraControl _cameraControl;
    public Canvas _messageCanvas;
    public GameObject _diceSelector;
    public GameManager _gameManager;
    public float _diceSelectionAdjustmentY;
    public float _diceSelectionAdjustmentZ;

    private GameObject _diceSelectors;
    private GameObject _diceView;

    private GameObject _selectionScreen;
    private GameObject _introScreen;
    private GameObject _specialSelectScreen;

    private Button _btnSelectDice;
    private Button _btnSelectD20;
    private Button _btnSelectD100;
    private Button _btnSelectCoinFlip;
    private Button _btnRoll;
    private Button _btnSelectBack;
    private Button _btnResultsBack;
    private Button _btnSpecialResultsBack;
    private Button _btnQuit;
    private Button _btnFlipOrientation;
    private Button _btnRight;
    private Button _btnLeft;


    private string _selection = string.Empty;
    private int _currentResultScreen = -1;
    private int _diceSelectorId = 1;

    #endregion

    #region Generic
    // Use this for initialization
    void Start()
    {
        SetUp();

    }

    void Update()
    {
        if (_gameManager.GetDoneRollingAndCameraDefaulted())
        {
            if (_selection == "CustomRoll")
            {
                _currentResultScreen = -1;
                ShowResultsScreen();
                SetDiceView();
            }
            else if (_selection == "RollD20" || _selection == "RollCoin" || _selection == "RollD100")
            {
                ShowSpecialResultsScreen();
            }

        }
    }
    #endregion

    #region SetUp

    public void SetUp()
    {
        _selectionScreen = _messageCanvas.transform.Find("SelectionScreen").gameObject;
        _introScreen = _messageCanvas.transform.Find("IntroScreen").gameObject;
        _specialSelectScreen = _messageCanvas.transform.Find("SpecialSelectScreen").gameObject;

        _diceSelectors = _selectionScreen.transform.Find("DiceSelectors").gameObject;

        _btnRight = _selectionScreen.transform.Find("btnRight").GetComponent<Button>();
        _btnLeft = _selectionScreen.transform.Find("btnLeft").GetComponent<Button>();
        _btnSelectDice = _introScreen.transform.Find("btnSelectDice").GetComponent<Button>();
        _btnSelectD20 = _introScreen.transform.Find("btnSelectD20").GetComponent<Button>();
        _btnSelectD100 = _introScreen.transform.Find("btnSelectD100").GetComponent<Button>();
        _btnSelectCoinFlip = _introScreen.transform.Find("btnSelectCoinFlip").GetComponent<Button>();
        _btnQuit = _introScreen.transform.Find("btnQuit").GetComponent<Button>();
        _btnFlipOrientation = _introScreen.transform.Find("btnFlipOrientation").GetComponent<Button>();
        _btnRoll = _selectionScreen.transform.Find("btnRoll").GetComponent<Button>();
        _btnResultsBack = _selectionScreen.transform.Find("btnResultsBack").GetComponent<Button>();
        _btnSelectBack = _selectionScreen.transform.Find("btnSelectBack").GetComponent<Button>();
        _btnSpecialResultsBack = _specialSelectScreen.transform.Find("btnSpecialResultsBack").GetComponent<Button>();

        _btnSelectDice.onClick.AddListener(delegate { ShowSelectScreen(); });
        _btnSelectD20.onClick.AddListener(delegate { RollD20Click(); });
        _btnSelectD100.onClick.AddListener(delegate { RollD100Click(); });
        _btnSelectCoinFlip.onClick.AddListener(delegate { FlipCoinClick(); });
        _btnQuit.onClick.AddListener(delegate { QuitClick(); });
        _btnRoll.onClick.AddListener(delegate { RollClick(); });
        _btnResultsBack.onClick.AddListener(delegate { ResultsBackClick(); });
        _btnSpecialResultsBack.onClick.AddListener(delegate { SpecialResultsBackClick(); });
        _btnSelectBack.onClick.AddListener(delegate { SelectBackClick(); });
        _btnFlipOrientation.onClick.AddListener(delegate { FlipOrientation(); });

        _btnRight.onClick.AddListener(delegate { ResultsRightClick(); });
        _btnLeft.onClick.AddListener(delegate { ResultsLeftClick(); });



    }
    GameObject SetUpDiceSelector(GameObject diceSelector, int currentId)
    {
        GameObject txtDiceCount;
        GameObject txtModifier;
        GameObject btnAddNew;
        GameObject btnRemove;
        btnAddNew = diceSelector.transform.Find("btnAddNew").gameObject;
        btnRemove = diceSelector.transform.Find("btnRemove").gameObject;
        txtDiceCount = diceSelector.transform.Find("txtDiceCount").gameObject;
        txtModifier = diceSelector.transform.Find("txtModifier").gameObject;
        diceSelector.transform.Find("lblTotal").gameObject.SetActive(false);
        btnAddNew.GetComponent<Button>().onClick.AddListener(delegate { AddNewClick(); });
        btnRemove.GetComponent<Button>().onClick.AddListener(delegate { RemoveClick(currentId); });
        txtDiceCount.GetComponent<InputField>().onValueChanged.AddListener(delegate { CheckMaxDiceCount(currentId); });
        txtModifier.GetComponent<InputField>().onValueChanged.AddListener(delegate { CheckMaxModifier(currentId); });
        diceSelector.gameObject.GetComponent<DiceSelectorScript>().SetDiceSelectorId(currentId);
        _selectionScreen.transform.Find("lblDiceSelectionTotal").gameObject.SetActive(false);
        return diceSelector;
    }

    public void SetDiceView()
    {
        _diceView.gameObject.SetActive(true);
        List<DiceSet> diceSets = _gameManager.GetDiceSets();
        DiceSet diceSet;
        List<DiceManager> diceList;
        List<DiceSet> viewDiceSets = _gameManager.GetViewDiceSets();
        DiceSet viewDiceSet;
        List<DiceManager> viewDiceList;
        List<GameObject> dicePanels = GameObject.FindGameObjectsWithTag("DicePanel").ToList();

        if (_diceView != null)
        {
            for (int i = 0; i < diceSets.Count; i++)
            {
                diceSet = diceSets[i];
                viewDiceSet = viewDiceSets[i];
                diceList = diceSet._diceManagers;
                viewDiceList = viewDiceSet._diceManagers;
                for (int j = 0; j < diceList.Count; j++)
                {


                    viewDiceList[j]._instance.transform.SetParent(dicePanels[j].transform, false);
                    viewDiceList[j]._instance.transform.rotation = diceList[j]._instance.transform.rotation;
                    viewDiceList[j]._instance.GetComponent<MeshCollider>().enabled = false;
                    viewDiceList[j]._instance.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                    switch (viewDiceSet._diceType)
                    {
                        case 4:
                            viewDiceList[j]._instance.transform.localScale = new Vector3(100, 100, 100);
                            break;
                        case 6:
                            viewDiceList[j]._instance.transform.localScale = new Vector3(7000, 7000, 7000);
                            break;
                        case 8:
                            viewDiceList[j]._instance.transform.localScale = new Vector3(8000, 8000, 8000);
                            break;
                        case 10:
                        case 100:
                            viewDiceList[j]._instance.transform.localScale = new Vector3(9000, 9000, 9000);
                            break;
                        case 12:
                        case 20:
                            viewDiceList[j]._instance.transform.localScale = new Vector3(10000, 10000, 10000);
                            break;
                    }

                    viewDiceList[j]._instance.transform.position = new Vector3(viewDiceList[j]._instance.transform.position.x + .25f, viewDiceList[j]._instance.transform.position.y + .25f, viewDiceList[j]._instance.transform.position.z);

                    if (viewDiceSet._diceType != 4)
                    {
                        viewDiceList[j]._instance.transform.Rotate(new Vector3(-45, 0, 0), Space.World);
                    }
                    viewDiceList[j]._instance.SetActive(false);


                }
            }
        }
        _diceView.gameObject.SetActive(false);

    }
    #endregion
    
    #region ClickEvents
    void ResultsRightClick()
    {
        HideDiceView();
        _currentResultScreen += 1;
        ShowResultsScreen();
    }

    void ResultsLeftClick()
    {
        HideDiceView();
        _currentResultScreen -= 1;
        ShowResultsScreen();
    }


    void AddNewClick()
    {
        GameObject lastSelector = FindLastSelector();

        GameObject diceSelector;


        if (_diceSelectors.transform.childCount < 6)
        {
            int currentId = _diceSelectorId + 1;
            diceSelector = GameObject.Instantiate(_diceSelector);
            diceSelector.transform.SetParent(_diceSelectors.transform, false);
            _btnRoll.transform.position = new Vector3(_btnRoll.transform.position.x, _btnRoll.transform.position.y - _diceSelectionAdjustmentY, _btnRoll.transform.position.z - _diceSelectionAdjustmentZ);
            _btnResultsBack.transform.position = new Vector3(_btnResultsBack.transform.position.x, _btnResultsBack.transform.position.y - _diceSelectionAdjustmentY, _btnResultsBack.transform.position.z - _diceSelectionAdjustmentZ);
            _btnSelectBack.transform.position = new Vector3(_btnSelectBack.transform.position.x, _btnSelectBack.transform.position.y - _diceSelectionAdjustmentY, _btnSelectBack.transform.position.z - _diceSelectionAdjustmentZ);
            diceSelector.transform.position = new Vector3(lastSelector.transform.position.x, lastSelector.transform.position.y - _diceSelectionAdjustmentY, lastSelector.transform.position.z - _diceSelectionAdjustmentZ);
            diceSelector = SetUpDiceSelector(diceSelector, currentId);
            diceSelector.name = "DiceSelector" + _diceSelectors.transform.childCount;
            _diceSelectorId = currentId;
        }

    }

    void RemoveClick(int id)
    {
        if (_diceSelectors.transform.childCount > 1)
        {
            GameObject currentSelector;
            GameObject selectorToDestroy;
            bool selectorToDestroyFound = false;

            for (int i = 0; i < _diceSelectors.transform.childCount; i++)
            {
                currentSelector = _diceSelectors.transform.GetChild(i).gameObject;
                if (selectorToDestroyFound == false)
                {
                    if (currentSelector.gameObject.GetComponent<DiceSelectorScript>().GetDiceSelectorId() == id)
                    {
                        selectorToDestroy = currentSelector;
                        Destroy(selectorToDestroy);
                        break;
                    }
                }
            }
            for (int i = 0; i < _diceSelectors.transform.childCount; i++)
            {
                currentSelector = _diceSelectors.transform.GetChild(i).gameObject;
                currentSelector.name = "DiceSelector" + (i + 1);
                if (currentSelector.gameObject.GetComponent<DiceSelectorScript>().GetDiceSelectorId() > id)
                {
                    currentSelector.transform.position = new Vector3(currentSelector.transform.position.x, currentSelector.transform.position.y + _diceSelectionAdjustmentY, currentSelector.transform.position.z + _diceSelectionAdjustmentZ);
                }
            }

            _btnRoll.transform.position = new Vector3(_btnRoll.transform.position.x, _btnRoll.transform.position.y + _diceSelectionAdjustmentY, _btnRoll.transform.position.z + _diceSelectionAdjustmentZ);
            _btnResultsBack.transform.position = new Vector3(_btnResultsBack.transform.position.x, _btnResultsBack.transform.position.y + _diceSelectionAdjustmentY, _btnResultsBack.transform.position.z + _diceSelectionAdjustmentZ);
            _btnSelectBack.transform.position = new Vector3(_btnSelectBack.transform.position.x, _btnSelectBack.transform.position.y + _diceSelectionAdjustmentY, _btnSelectBack.transform.position.z + _diceSelectionAdjustmentZ);
        }
        else
        {
            _diceSelectors.transform.GetChild(0).gameObject.name = "DiceSelector1";
        }
    }
    void RollClick()
    {
        List<DiceSetInfo> diceSetInfos = new List<DiceSetInfo>();
        DiceSetInfo currentDiceSetInfo = new DiceSetInfo();
        int selectedDiceTypeValue = 0;

        for (int i = 0; i < _diceSelectors.transform.childCount; i++)
        {
            currentDiceSetInfo = new DiceSetInfo();

            GameObject diceSelector = _diceSelectors.transform.GetChild(i).gameObject;

            GameObject txtDiceCount = diceSelector.transform.Find("txtDiceCount").gameObject;
            GameObject txtModifier = diceSelector.transform.Find("txtModifier").gameObject;
            GameObject ddlDiceSelector = diceSelector.transform.Find("ddlDiceSelector").gameObject;

            if (txtDiceCount.transform.Find("Text").GetComponent<Text>().text == "")
            {
                currentDiceSetInfo._numberOfDice = 1;
            }
            else
            {
                currentDiceSetInfo._numberOfDice = Convert.ToInt32(txtDiceCount.transform.Find("Text").GetComponent<Text>().text);
            }

            if (txtModifier.transform.Find("Text").GetComponent<Text>().text == "")
            {
                currentDiceSetInfo._diceModifier = 0;
            }
            else
            {
                currentDiceSetInfo._diceModifier = Convert.ToInt32(txtModifier.transform.Find("Text").GetComponent<Text>().text);
            }

            selectedDiceTypeValue = Convert.ToInt32(ddlDiceSelector.GetComponent<Dropdown>().value);
            currentDiceSetInfo._diceType = Convert.ToInt32(ddlDiceSelector.GetComponent<Dropdown>().options[selectedDiceTypeValue].text);
            diceSetInfos.Add(currentDiceSetInfo);

        }
        _gameManager.RollDice(diceSetInfos);
        _selectionScreen.SetActive(false);
        _selection = "CustomRoll";

    }

    private void RollD20Click()
    {
        List<DiceSetInfo> diceSetInfos = new List<DiceSetInfo>();
        DiceSetInfo currentDiceSetInfo = new DiceSetInfo();

        currentDiceSetInfo._diceType = 20;
        currentDiceSetInfo._diceModifier = 0;
        currentDiceSetInfo._numberOfDice = 1;

        diceSetInfos.Add(currentDiceSetInfo);

        _gameManager.RollDice(diceSetInfos);

        _selectionScreen.SetActive(false);
        _introScreen.SetActive(false);
        _selection = "RollD20";
    }

    private void RollD100Click()
    {
        List<DiceSetInfo> diceSetInfos = new List<DiceSetInfo>();
        DiceSetInfo currentDiceSetInfo = new DiceSetInfo();

        currentDiceSetInfo._diceType = 10;
        currentDiceSetInfo._diceModifier = 0;
        currentDiceSetInfo._numberOfDice = 1;

        diceSetInfos.Add(currentDiceSetInfo);

        currentDiceSetInfo = new DiceSetInfo();
        currentDiceSetInfo._diceType = 100;
        currentDiceSetInfo._diceModifier = 0;
        currentDiceSetInfo._numberOfDice = 1;

        diceSetInfos.Add(currentDiceSetInfo);

        _gameManager.RollDice(diceSetInfos);

        _selectionScreen.SetActive(false);
        _introScreen.SetActive(false);
        _selection = "RollD100";
    }

    private void FlipCoinClick()
    {

        List<DiceSetInfo> diceSetInfos = new List<DiceSetInfo>();
        DiceSetInfo currentDiceSetInfo = new DiceSetInfo();

        currentDiceSetInfo._diceType = 2;
        currentDiceSetInfo._diceModifier = 0;
        currentDiceSetInfo._numberOfDice = 1;

        diceSetInfos.Add(currentDiceSetInfo);

        _gameManager.RollDice(diceSetInfos);

        _selectionScreen.SetActive(false);
        _introScreen.SetActive(false);
        _selection = "RollCoin";

    }


    void ResultsBackClick()
    {
        GameObject btnRight = _selectionScreen.transform.Find("btnRight").gameObject;
        GameObject btnLeft = _selectionScreen.transform.Find("btnLeft").gameObject;

        for (int i = 0; i < _diceSelectors.transform.childCount; i++)
        {

            GameObject currentSelector = _diceSelectors.transform.GetChild(i).gameObject;
            currentSelector.transform.Find("btnAddNew").gameObject.SetActive(true);
            currentSelector.transform.Find("btnRemove").gameObject.SetActive(true);
            currentSelector.transform.Find("lblTotal").gameObject.SetActive(false);

            currentSelector.transform.Find("txtDiceCount").gameObject.GetComponent<InputField>().enabled = true;
            currentSelector.transform.Find("txtModifier").gameObject.GetComponent<InputField>().enabled = true;
            currentSelector.transform.Find("ddlDiceSelector").gameObject.GetComponent<Dropdown>().enabled = true;

        }
        _selectionScreen.transform.Find("lblDiceSelectionTotal").gameObject.SetActive(false);
        _gameManager.DestroyDice();
        _btnRoll.gameObject.SetActive(true);
        _btnResultsBack.gameObject.SetActive(false);
        _btnSelectBack.gameObject.SetActive(true);
        btnLeft.gameObject.SetActive(false);
        btnRight.gameObject.SetActive(false);

    }

    void SpecialResultsBackClick()
    {

        ShowIntroScreen();

        _gameManager.DestroyDice();

    }

    void SelectBackClick()
    {
        _diceSelectorId = 1;

        for (int i = 0; i < _diceSelectors.transform.childCount; i++)
        {
            GameObject currentSelector = _diceSelectors.transform.GetChild(i).gameObject;
            if (i == 0)
            {

                currentSelector.transform.Find("btnAddNew").gameObject.GetComponent<Button>().onClick.RemoveAllListeners();
                currentSelector.transform.Find("btnRemove").gameObject.GetComponent<Button>().onClick.RemoveAllListeners();
                currentSelector.transform.Find("txtDiceCount").gameObject.GetComponent<InputField>().onValueChanged.RemoveAllListeners();
                currentSelector.transform.Find("txtModifier").gameObject.GetComponent<InputField>().onValueChanged.RemoveAllListeners();

                currentSelector.name = "DiceSelector";
                currentSelector.transform.Find("btnAddNew").gameObject.SetActive(true);
                currentSelector.transform.Find("btnRemove").gameObject.SetActive(true);
                currentSelector.transform.Find("lblTotal").gameObject.SetActive(false);
                
                currentSelector.gameObject.GetComponent<DiceSelectorScript>().SetDiceSelectorId(1);
            }
            else
            {
                Destroy(currentSelector);
                _btnRoll.transform.position = new Vector3(_btnRoll.transform.position.x, _btnRoll.transform.position.y + _diceSelectionAdjustmentY, _btnRoll.transform.position.z + _diceSelectionAdjustmentZ);
                _btnResultsBack.transform.position = new Vector3(_btnResultsBack.transform.position.x, _btnResultsBack.transform.position.y + _diceSelectionAdjustmentY, _btnResultsBack.transform.position.z + _diceSelectionAdjustmentZ);
                _btnSelectBack.transform.position = new Vector3(_btnSelectBack.transform.position.x, _btnSelectBack.transform.position.y + _diceSelectionAdjustmentY, _btnSelectBack.transform.position.z + _diceSelectionAdjustmentZ);

            }
        }
        _selectionScreen.transform.Find("lblDiceSelectionTotal").gameObject.SetActive(false);
        _gameManager.DestroyDice();
        ShowIntroScreen();

    }

    void FlipOrientation()
    {
        if (Screen.orientation == ScreenOrientation.Portrait)
        {
            Screen.orientation = ScreenOrientation.LandscapeLeft;
        }
        else
        {
            Screen.orientation = ScreenOrientation.Portrait;
        }

    }

    void QuitClick()
    {
        Application.Quit();
    }

    #endregion

    #region ShowHide
    public void ShowIntroScreen()
    {

        _introScreen.SetActive(true);
        _selectionScreen.SetActive(false);
        _specialSelectScreen.SetActive(false);

    }

    public void ShowSelectScreen()
    {
        int currentId = 1;
        _diceView = _selectionScreen.transform.Find("DiceView").gameObject;
        _introScreen.SetActive(false);
        _selectionScreen.SetActive(true);
        GameObject diceSelector = GameObject.FindGameObjectsWithTag("DiceSelector")[0];
        SetUpDiceSelector(diceSelector, currentId);

        _btnResultsBack.gameObject.SetActive(false);
        _btnSelectBack.gameObject.SetActive(true);
        _diceView.gameObject.SetActive(false);
        _btnRight.gameObject.SetActive(false);
        _btnLeft.gameObject.SetActive(false);

    }
    public void HideDiceView()
    {
        List<GameObject> dicePanels = GameObject.FindGameObjectsWithTag("DicePanel").ToList();

        DiceSet diceSet;
        List<DiceManager> diceList;
        DiceSet viewDiceSets;
        List<DiceManager> viewDiceList;

        if (_currentResultScreen >= 0)
        {
            diceSet = _gameManager.GetDiceSets()[_currentResultScreen];
            diceList = diceSet._diceManagers;
            viewDiceSets = _gameManager.GetViewDiceSets()[_currentResultScreen];
            viewDiceList = viewDiceSets._diceManagers;
            for (int i = 0; i < diceList.Count; i++)
            {

                viewDiceList[i]._instance.SetActive(false);
                dicePanels[i].gameObject.transform.Find("InputField").gameObject.SetActive(false);
            }

        }
    }


    public void ShowResultsScreen()
    {
        if (Screen.orientation == ScreenOrientation.Portrait)
        {
            _btnRight.transform.localPosition = new Vector3(200, -310, 681);
            _btnLeft.transform.localPosition = new Vector3(-30, -310, 681);
        }
        else
        {
            _btnRight.transform.localPosition = new Vector3(250, -140, 681);
            _btnLeft.transform.localPosition = new Vector3(-83, -140, 681);
        }
        if (_currentResultScreen < 0)
        {
            ShowTotals();
        }
        else
        {
            ShowDiceView();
        }
    }

    private void ShowTotals()
    {
        _gameManager.GetTotals();
        _gameManager.SortDiceSets();
        List<DiceSet> diceSets = _gameManager.GetDiceSets();
        _selectionScreen.SetActive(true);
        int allDiceTotal = 0;

        for (int i = 0; i < _diceSelectors.transform.childCount; i++)
        {
            GameObject diceSelector = _diceSelectors.transform.GetChild(i).gameObject;
            DiceSet currentDiceSet = diceSets[i];

            diceSelector.transform.Find("btnAddNew").gameObject.SetActive(false);
            diceSelector.transform.Find("btnRemove").gameObject.SetActive(false);
            diceSelector.transform.Find("lblTotal").gameObject.SetActive(true);
            diceSelector.transform.Find("lblTotal").GetComponent<Text>().text = "= " + currentDiceSet._total;
            allDiceTotal += currentDiceSet._total;
            diceSelector.transform.Find("txtDiceCount").gameObject.GetComponent<InputField>().enabled = false;
            diceSelector.transform.Find("txtModifier").gameObject.GetComponent<InputField>().enabled = false;
            diceSelector.transform.Find("ddlDiceSelector").gameObject.GetComponent<Dropdown>().enabled = false;
        }

        _selectionScreen.transform.Find("lblDiceSelectionTotal").gameObject.SetActive(true);
        _selectionScreen.transform.Find("lblDiceSelectionTotal").GetComponent<Text>().text = "Total = " + allDiceTotal;

        _btnResultsBack.gameObject.SetActive(true);
        _btnRoll.gameObject.SetActive(false);
        _btnSelectBack.gameObject.SetActive(false);
        _diceView.gameObject.SetActive(false);
        _btnLeft.gameObject.SetActive(false);
        _btnRight.gameObject.SetActive(true);
        _diceSelectors.gameObject.SetActive(true);
        _selection = String.Empty;
    }

    private void ShowDiceView()
    {
        _diceView.gameObject.SetActive(true);

        DiceSet diceSet = _gameManager.GetDiceSets()[_currentResultScreen];
        List<DiceManager> diceList = diceSet._diceManagers;
        DiceSet viewDiceSets = _gameManager.GetViewDiceSets()[_currentResultScreen];
        List<DiceManager> viewDiceList = viewDiceSets._diceManagers;
        List<GameObject> dicePanels = GameObject.FindGameObjectsWithTag("DicePanel").ToList();

        for (int i = 0; i < diceList.Count; i++)
        {

            if (_diceView != null)
            {
                viewDiceList[i]._instance.SetActive(true);
                dicePanels[i].gameObject.transform.Find("InputField").gameObject.SetActive(true);
                dicePanels[i].gameObject.transform.Find("InputField").GetComponent<InputField>().text = diceList[i].GetValue().ToString();
            }
        }

        _diceSelectors.gameObject.SetActive(false);
        _btnResultsBack.gameObject.SetActive(false);

        _btnLeft.gameObject.SetActive(true);
        if (_currentResultScreen < _diceSelectors.transform.childCount - 1)
        {
            _btnRight.gameObject.SetActive(true);
        }
        else
        {
            _btnRight.gameObject.SetActive(false);
        }
    }
    public void ShowSpecialResultsScreen()
    {
        _gameManager.GetTotals();
        List<DiceSet> diceSets = _gameManager.GetDiceSets();
        string results = string.Empty;
        _specialSelectScreen.SetActive(true);

        if (_selection == "RollD20")
        {
            results = diceSets[0]._total.ToString();
        }
        if (_selection == "RollD100")
        {
            results = (diceSets[0]._total + diceSets[1]._total).ToString();
        }
        else if (_selection == "RollCoin")
        {
            if (diceSets[0]._total == 1)
            {
                results = "Heads";
            }
            else if (diceSets[0]._total == 2)
            {
                results = "Tails";
            }
        }

        _specialSelectScreen.transform.Find("btnSpecialResultsBack").gameObject.SetActive(true);
        _specialSelectScreen.transform.Find("lblResults").gameObject.SetActive(true);
        _specialSelectScreen.transform.Find("lblResults").GetComponent<Text>().text = "Result: " + results;


        _btnSpecialResultsBack.gameObject.SetActive(true);
        _selection = string.Empty;
    }

    #endregion
    

    #region Helpers


    void CheckMaxDiceCount(int id)
    {

        GameObject currentSelector;
        GameObject txtDiceCount;
        for (int i = 0; i < _diceSelectors.transform.childCount; i++)
        {
            currentSelector = _diceSelectors.transform.GetChild(i).gameObject;

            if (currentSelector.gameObject.GetComponent<DiceSelectorScript>().GetDiceSelectorId() == id)
            {
                txtDiceCount = currentSelector.transform.Find("txtDiceCount").gameObject;
                if (Convert.ToInt32(txtDiceCount.GetComponent<InputField>().text) > 25)
                {
                    txtDiceCount.GetComponent<InputField>().text = "25";
                }
                break;
            }

        }

    }

    void CheckMaxModifier(int id)
    {

        GameObject currentSelector;
        GameObject txtModifier;
        for (int i = 0; i < _diceSelectors.transform.childCount; i++)
        {
            currentSelector = _diceSelectors.transform.GetChild(i).gameObject;

            if (currentSelector.gameObject.GetComponent<DiceSelectorScript>().GetDiceSelectorId() == id)
            {
                txtModifier = currentSelector.transform.Find("txtModifier").gameObject;
                if (Convert.ToInt32(txtModifier.GetComponent<InputField>().text) > 100)
                {
                    txtModifier.GetComponent<InputField>().text = "100";
                }
                break;
            }

        }

    }


    GameObject FindLastSelector()
    {
        GameObject lastSelector;
        lastSelector = _diceSelectors.transform.GetChild(_diceSelectors.transform.childCount - 1).gameObject;

        return lastSelector;
    }



    #endregion


















}
