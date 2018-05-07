using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using System.IO;
using System.Linq;

public class CanvasManager : MonoBehaviour
{

    public CameraControl m_CameraControl;
    public Canvas m_MessageCanvas;
    public Button btnIntroRollDice;
    public Button btnRollD20;
    public Button btnRollD100;
    public Button btnFlipCoin;
    public Button btnRoll;
    public Button btnSelectBack;
    public Button btnResultsBack;
    public Button btnSpecialResultsBack;
    public Button btnQuit;
    public GameObject DiceSelector;
    public GameManager _gameManager;
    public float diceSelectionAdjustmentY;
    public float diceSelectionAdjustmentZ;
    private string _selection = string.Empty;
    private int currentResultScreen = -1;
    


    // Use this for initialization
    void Start()
    {
        SetUp();

        ShowIntroScreen();
    }

    void Update()
    {
        if (_gameManager.GetDoneRolling())
        {
            if (_selection == "RollMisc")
            {
                currentResultScreen = -1;
                ShowResultsScreen();
            }
            else if(_selection == "RollD20" || _selection == "RollCoin" || _selection == "RollD100")
            {
                ShowSpecialResultsScreen();
            }
            
        }
    }

    public void SetUp()
    {
        GameObject selectionScreen = m_MessageCanvas.transform.Find("SelectionScreen").gameObject;
        GameObject btnRight = selectionScreen.transform.Find("btnRight").gameObject;
        GameObject btnLeft = selectionScreen.transform.Find("btnLeft").gameObject;
        btnIntroRollDice.GetComponent<Button>().onClick.AddListener(delegate { ShowSelectScreen(); });
        btnRollD20.GetComponent<Button>().onClick.AddListener(delegate { RollD20Click(); });
        btnRollD100.GetComponent<Button>().onClick.AddListener(delegate { RollD100Click(); });
        btnFlipCoin.GetComponent<Button>().onClick.AddListener(delegate { FlipCoinClick(); });
        btnQuit.GetComponent<Button>().onClick.AddListener(delegate { QuitClick(); });
        btnRoll.GetComponent<Button>().onClick.AddListener(delegate { RollClick(); });
        btnResultsBack.GetComponent<Button>().onClick.AddListener(delegate { ResultsBackClick(); });
        btnSpecialResultsBack.GetComponent<Button>().onClick.AddListener(delegate { SpecialResultsBackClick(); });
        btnSelectBack.GetComponent<Button>().onClick.AddListener(delegate { SelectBackClick(); });


        btnRight.GetComponent<Button>().onClick.AddListener(delegate { ResultsRightClick(); });
        btnLeft.GetComponent<Button>().onClick.AddListener(delegate { ResultsLeftClick(); });

        ShowIntroScreen();


    }


    public void ShowIntroScreen()
    {
        GameObject introScreen = m_MessageCanvas.transform.Find("IntroScreen").gameObject;
        GameObject selectionScreen = m_MessageCanvas.transform.Find("SelectionScreen").gameObject;
        GameObject specialSelectScreen = m_MessageCanvas.transform.Find("SpecialSelectScreen").gameObject;

        introScreen.SetActive(true);
        selectionScreen.SetActive(false);
        specialSelectScreen.SetActive(false);

    }

    public void ShowSelectScreen()
    {
        GameObject introScreen = m_MessageCanvas.transform.Find("IntroScreen").gameObject;
        GameObject selectionScreen = m_MessageCanvas.transform.Find("SelectionScreen").gameObject;
        GameObject diceSelectors = selectionScreen.transform.Find("DiceSelectors").gameObject;
        GameObject diceView = selectionScreen.transform.Find("DiceView").gameObject;
        GameObject btnRight = selectionScreen.transform.Find("btnRight").gameObject;
        GameObject btnLeft = selectionScreen.transform.Find("btnLeft").gameObject;
        introScreen.SetActive(false);
        selectionScreen.SetActive(true);
        GameObject diceSelector = GameObject.FindGameObjectsWithTag("DiceSelector")[0];
        GameObject btnAddNew = diceSelector.transform.Find("btnAddNew").gameObject;
        GameObject btnRemove = diceSelector.transform.Find("btnRemove").gameObject;
        diceSelector.transform.Find("lblTotal").gameObject.SetActive(false);
        
        btnAddNew.GetComponent<Button>().onClick.AddListener(delegate { AddNewClick(); });
        btnRemove.GetComponent<Button>().onClick.AddListener(delegate { RemoveClick(0); });
        diceSelector.name = "DiceSelector1";
        btnResultsBack.gameObject.SetActive(false);
        btnSelectBack.gameObject.SetActive(true);
        diceView.gameObject.SetActive(false);
        btnRight.gameObject.SetActive(false);
        btnLeft.gameObject.SetActive(false);

    }

    void ResultsRightClick()
    {
        currentResultScreen += 1;
        ShowResultsScreen();
    }

    void ResultsLeftClick()
    {
        currentResultScreen -= 1;
        ShowResultsScreen();
    }

    void AddNewClick()
    {
        GameObject selectionScreen = m_MessageCanvas.transform.Find("SelectionScreen").gameObject;
        GameObject diceSelectors = selectionScreen.transform.Find("DiceSelectors").gameObject;
        GameObject lastSelector = FindLastSelector(diceSelectors);
        GameObject diceSelector;
        GameObject btnAddNew;
        GameObject btnRemove;

        if (diceSelectors.transform.childCount < 6)
        {
            diceSelector = GameObject.Instantiate(DiceSelector);
            diceSelector.transform.SetParent(diceSelectors.transform, false);
            btnRoll.transform.position = new Vector3(btnRoll.transform.position.x, btnRoll.transform.position.y - diceSelectionAdjustmentY, btnRoll.transform.position.z - diceSelectionAdjustmentZ);
            btnResultsBack.transform.position = new Vector3(btnResultsBack.transform.position.x, btnResultsBack.transform.position.y - diceSelectionAdjustmentY, btnResultsBack.transform.position.z - diceSelectionAdjustmentZ);
            btnSelectBack.transform.position = new Vector3(btnSelectBack.transform.position.x, btnSelectBack.transform.position.y - diceSelectionAdjustmentY, btnSelectBack.transform.position.z - diceSelectionAdjustmentZ);
            btnQuit.transform.position = new Vector3(btnQuit.transform.position.x, btnQuit.transform.position.y - diceSelectionAdjustmentY, btnQuit.transform.position.z - diceSelectionAdjustmentZ);
            diceSelector.transform.position = new Vector3(lastSelector.transform.position.x, lastSelector.transform.position.y - diceSelectionAdjustmentY, lastSelector.transform.position.z - diceSelectionAdjustmentZ);
            btnAddNew = diceSelector.transform.Find("btnAddNew").gameObject;
            btnRemove = diceSelector.transform.Find("btnRemove").gameObject;
            diceSelector.transform.Find("lblTotal").gameObject.SetActive(false);
            btnAddNew.GetComponent<Button>().onClick.AddListener(delegate { AddNewClick(); });
            btnRemove.GetComponent<Button>().onClick.AddListener(delegate { RemoveClick(diceSelectors.transform.childCount - 1); });
            diceSelector.name = "DiceSelector" + diceSelectors.transform.childCount;
        }
    }

    void RemoveClick(int id)
    {
        GameObject selectionScreen = m_MessageCanvas.transform.Find("SelectionScreen").gameObject;
        GameObject diceSelectors = selectionScreen.transform.Find("DiceSelectors").gameObject;
        GameObject selectorToRemove = diceSelectors.transform.GetChild(id).gameObject;
        GameObject currentSelector;
        if (diceSelectors.transform.childCount > 1)
        {
            Destroy(selectorToRemove);
            for (int i = id; i < diceSelectors.transform.childCount; i++)
            {
                currentSelector = diceSelectors.transform.GetChild(i).gameObject;
                currentSelector.transform.position = new Vector3(currentSelector.transform.position.x, currentSelector.transform.position.y + diceSelectionAdjustmentY, currentSelector.transform.position.z + diceSelectionAdjustmentZ);
                currentSelector.name = "DiceSelector" + i;
            }

            btnRoll.transform.position = new Vector3(btnRoll.transform.position.x, btnRoll.transform.position.y + diceSelectionAdjustmentY, btnRoll.transform.position.z + diceSelectionAdjustmentZ);
            btnResultsBack.transform.position = new Vector3(btnResultsBack.transform.position.x, btnResultsBack.transform.position.y + diceSelectionAdjustmentY, btnResultsBack.transform.position.z + diceSelectionAdjustmentZ);
            btnSelectBack.transform.position = new Vector3(btnSelectBack.transform.position.x, btnSelectBack.transform.position.y + diceSelectionAdjustmentY, btnSelectBack.transform.position.z + diceSelectionAdjustmentZ);
            btnQuit.transform.position = new Vector3(btnQuit.transform.position.x, btnQuit.transform.position.y + diceSelectionAdjustmentY, btnQuit.transform.position.z + diceSelectionAdjustmentZ);
        }
    }

    void RollClick()
    {
        GameObject selectionScreen = m_MessageCanvas.transform.Find("SelectionScreen").gameObject;
        GameObject diceSelectors = selectionScreen.transform.Find("DiceSelectors").gameObject;
        List<DiceSetInfo> diceSetInfos = new List<DiceSetInfo>();
        DiceSetInfo currentDiceSetInfo = new DiceSetInfo();
        int selectedDiceTypeValue = 0;

        for (int i = 0; i < diceSelectors.transform.childCount; i++)
        {
            currentDiceSetInfo = new DiceSetInfo();

            GameObject diceSelector = diceSelectors.transform.GetChild(i).gameObject;

            GameObject txtDiceCount = diceSelector.transform.Find("txtDiceCount").gameObject;
            GameObject txtModifier = diceSelector.transform.Find("txtModifier").gameObject;
            GameObject ddlDiceSelector = diceSelector.transform.Find("ddlDiceSelector").gameObject;

            if (txtDiceCount.transform.Find("Text").GetComponent<Text>().text == "")
            {
                currentDiceSetInfo.numberOfDice = 1;
            }
            else
            {
                currentDiceSetInfo.numberOfDice = Convert.ToInt32(txtDiceCount.transform.Find("Text").GetComponent<Text>().text);
            }
            
            if (txtModifier.transform.Find("Text").GetComponent<Text>().text == "")
            {
                currentDiceSetInfo.diceModifier = 0;
            }
            else
            {
                currentDiceSetInfo.diceModifier = Convert.ToInt32(txtModifier.transform.Find("Text").GetComponent<Text>().text);
            }

            selectedDiceTypeValue = Convert.ToInt32(ddlDiceSelector.GetComponent<Dropdown>().value);
            currentDiceSetInfo.diceType = Convert.ToInt32(ddlDiceSelector.GetComponent<Dropdown>().options[selectedDiceTypeValue].text);
            diceSetInfos.Add(currentDiceSetInfo);
            
            
        }
        _gameManager.RollDice(diceSetInfos);
        selectionScreen.SetActive(false);
        _selection = "RollMisc";

    }

    private void RollD20Click()
    {
        List<DiceSetInfo> diceSetInfos = new List<DiceSetInfo>();
        DiceSetInfo currentDiceSetInfo = new DiceSetInfo();
        GameObject selectionScreen = m_MessageCanvas.transform.Find("SelectionScreen").gameObject;
        GameObject introScreen = m_MessageCanvas.transform.Find("IntroScreen").gameObject;

        currentDiceSetInfo.diceType = 20;
        currentDiceSetInfo.diceModifier = 0;
        currentDiceSetInfo.numberOfDice = 1;

        diceSetInfos.Add(currentDiceSetInfo);

        _gameManager.RollDice(diceSetInfos);
        
        selectionScreen.SetActive(false);
        introScreen.SetActive(false);
        _selection = "RollD20";
    }

    private void RollD100Click()
    {
        List<DiceSetInfo> diceSetInfos = new List<DiceSetInfo>();
        DiceSetInfo currentDiceSetInfo = new DiceSetInfo();
        GameObject selectionScreen = m_MessageCanvas.transform.Find("SelectionScreen").gameObject;
        GameObject introScreen = m_MessageCanvas.transform.Find("IntroScreen").gameObject;

        currentDiceSetInfo.diceType = 10;
        currentDiceSetInfo.diceModifier = 0;
        currentDiceSetInfo.numberOfDice = 1;

        diceSetInfos.Add(currentDiceSetInfo);

        currentDiceSetInfo = new DiceSetInfo();
        currentDiceSetInfo.diceType = 100;
        currentDiceSetInfo.diceModifier = 0;
        currentDiceSetInfo.numberOfDice = 1;

        diceSetInfos.Add(currentDiceSetInfo);

        _gameManager.RollDice(diceSetInfos);

        selectionScreen.SetActive(false);
        introScreen.SetActive(false);
        _selection = "RollD100";
    }

    private void FlipCoinClick()
    {

        List<DiceSetInfo> diceSetInfos = new List<DiceSetInfo>();
        DiceSetInfo currentDiceSetInfo = new DiceSetInfo();
        GameObject selectionScreen = m_MessageCanvas.transform.Find("SelectionScreen").gameObject;
        GameObject introScreen = m_MessageCanvas.transform.Find("IntroScreen").gameObject;

        currentDiceSetInfo.diceType = 2;
        currentDiceSetInfo.diceModifier = 0;
        currentDiceSetInfo.numberOfDice = 1;

        diceSetInfos.Add(currentDiceSetInfo);

        _gameManager.RollDice(diceSetInfos);
        
        selectionScreen.SetActive(false);
        introScreen.SetActive(false);
        _selection = "RollCoin";

    }
    

    void ResultsBackClick()
    {
        GameObject selectionScreen = m_MessageCanvas.transform.Find("SelectionScreen").gameObject;
        GameObject diceSelectors = selectionScreen.transform.Find("DiceSelectors").gameObject;
        GameObject btnRight = selectionScreen.transform.Find("btnRight").gameObject;
        GameObject btnLeft = selectionScreen.transform.Find("btnLeft").gameObject;

        for (int i = 0; i < diceSelectors.transform.childCount; i++)
        {
            if (i == 0)
            {
                GameObject currentSelector = diceSelectors.transform.GetChild(i).gameObject;
                currentSelector.name = "DiceSelector";
                currentSelector.transform.Find("btnAddNew").gameObject.SetActive(true);
                currentSelector.transform.Find("btnRemove").gameObject.SetActive(true);
                currentSelector.transform.Find("lblTotal").gameObject.SetActive(false);
            }
            else
            {
                GameObject currentSelector = diceSelectors.transform.GetChild(i).gameObject;
                Destroy(currentSelector);
                btnRoll.transform.position = new Vector3(btnRoll.transform.position.x, btnRoll.transform.position.y + diceSelectionAdjustmentY, btnRoll.transform.position.z + diceSelectionAdjustmentZ);
                btnResultsBack.transform.position = new Vector3(btnResultsBack.transform.position.x, btnResultsBack.transform.position.y + diceSelectionAdjustmentY, btnResultsBack.transform.position.z + diceSelectionAdjustmentZ);
                btnSelectBack.transform.position = new Vector3(btnSelectBack.transform.position.x, btnSelectBack.transform.position.y + diceSelectionAdjustmentY, btnSelectBack.transform.position.z + diceSelectionAdjustmentZ);
                btnQuit.transform.position = new Vector3(btnQuit.transform.position.x, btnQuit.transform.position.y + diceSelectionAdjustmentY, btnQuit.transform.position.z + diceSelectionAdjustmentZ);
            }
        }

        _gameManager.DestroyDice();
        _gameManager.SetDoneRolling(false);
        btnRoll.gameObject.SetActive(true);
        btnResultsBack.gameObject.SetActive(false);
        btnSelectBack.gameObject.SetActive(true);
        btnLeft.gameObject.SetActive(false);
        btnRight.gameObject.SetActive(false);

    }

    void SpecialResultsBackClick()
    {

        ShowIntroScreen();

        _gameManager.DestroyDice();
        _gameManager.SetDoneRolling(false);

    }

    void SelectBackClick()
    {
        GameObject selectionScreen = m_MessageCanvas.transform.Find("SelectionScreen").gameObject;
        GameObject diceSelectors = selectionScreen.transform.Find("DiceSelectors").gameObject;
        GameObject btnAddNew;
        GameObject btnRemove;

        for (int i = 0; i < diceSelectors.transform.childCount; i++)
        {
            if (i == 0)
            {
                GameObject currentSelector = diceSelectors.transform.GetChild(i).gameObject;
                btnAddNew = currentSelector.transform.Find("btnAddNew").gameObject;
                btnRemove = currentSelector.transform.Find("btnRemove").gameObject;
                btnAddNew.GetComponent<Button>().onClick.RemoveAllListeners();
                btnRemove.GetComponent<Button>().onClick.RemoveAllListeners();
                currentSelector.name = "DiceSelector";
                currentSelector.transform.Find("btnAddNew").gameObject.SetActive(true);
                currentSelector.transform.Find("btnRemove").gameObject.SetActive(true);
                currentSelector.transform.Find("lblTotal").gameObject.SetActive(false);
            }
            else
            {
                GameObject currentSelector = diceSelectors.transform.GetChild(i).gameObject;
                Destroy(currentSelector);
                btnRoll.transform.position = new Vector3(btnRoll.transform.position.x, btnRoll.transform.position.y + diceSelectionAdjustmentY, btnRoll.transform.position.z + diceSelectionAdjustmentZ);
                btnResultsBack.transform.position = new Vector3(btnResultsBack.transform.position.x, btnResultsBack.transform.position.y + diceSelectionAdjustmentY, btnResultsBack.transform.position.z + diceSelectionAdjustmentZ);
                btnSelectBack.transform.position = new Vector3(btnSelectBack.transform.position.x, btnSelectBack.transform.position.y + diceSelectionAdjustmentY, btnSelectBack.transform.position.z + diceSelectionAdjustmentZ);
                btnQuit.transform.position = new Vector3(btnQuit.transform.position.x, btnQuit.transform.position.y + diceSelectionAdjustmentY, btnQuit.transform.position.z + diceSelectionAdjustmentZ);
                
            }
        }

        _gameManager.DestroyDice();
        _gameManager.SetDoneRolling(false);
        ShowIntroScreen();

    }


    void QuitClick()
    {
        Application.Quit();
    }

    GameObject FindLastSelector(GameObject diceSelectors)
    {
        GameObject lastSelector;
        lastSelector = diceSelectors.transform.GetChild(diceSelectors.transform.childCount - 1).gameObject;

        return lastSelector;
    }

   


   

    public void ShowResultsScreen()
    {
        if (currentResultScreen < 0)
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
        GameObject selectionScreen = m_MessageCanvas.transform.Find("SelectionScreen").gameObject;
        GameObject diceSelectors = selectionScreen.transform.Find("DiceSelectors").gameObject;
        GameObject diceView = selectionScreen.transform.Find("DiceView").gameObject;
        GameObject btnRight = selectionScreen.transform.Find("btnRight").gameObject;
        GameObject btnLeft = selectionScreen.transform.Find("btnLeft").gameObject;
        _gameManager.GetTotals();
        List<DiceSet> diceSets = _gameManager.GetDiceSets();
        selectionScreen.SetActive(true);

        for (int i = 0; i < diceSelectors.transform.childCount; i++)
        {
            GameObject diceSelector = diceSelectors.transform.GetChild(i).gameObject;
            DiceSet currentDiceSet = diceSets[i];

            diceSelector.transform.Find("btnAddNew").gameObject.SetActive(false);
            diceSelector.transform.Find("btnRemove").gameObject.SetActive(false);
            diceSelector.transform.Find("lblTotal").gameObject.SetActive(true);
            diceSelector.transform.Find("lblTotal").GetComponent<Text>().text = "Total: " + currentDiceSet.total;

        }

        btnResultsBack.gameObject.SetActive(true);
        btnRoll.gameObject.SetActive(false);
        btnSelectBack.gameObject.SetActive(false);
        diceView.gameObject.SetActive(false);
        btnLeft.gameObject.SetActive(false);
        btnRight.gameObject.SetActive(true);
        diceSelectors.gameObject.SetActive(true);
        _selection = string.Empty;
    }

    private void ShowDiceView()
    {
        GameObject selectionScreen = m_MessageCanvas.transform.Find("SelectionScreen").gameObject;
        GameObject diceSelectors = selectionScreen.transform.Find("DiceSelectors").gameObject;
        GameObject diceView = selectionScreen.transform.Find("DiceView").gameObject;
        diceView.gameObject.SetActive(true);

        GameObject btnRight = selectionScreen.transform.Find("btnRight").gameObject;
        GameObject btnLeft = selectionScreen.transform.Find("btnLeft").gameObject;
        DiceSet diceSet = _gameManager.GetDiceSets()[currentResultScreen];
        List<DiceManager> diceList = diceSet.m_Dice;
        DiceSet viewDiceSets = _gameManager.GetViewDiceSets()[currentResultScreen];
        List<DiceManager> viewDiceList = viewDiceSets.m_Dice;
        List<GameObject> dicePanels = GameObject.FindGameObjectsWithTag("DicePanel").ToList();

        for (int i = 0; i < diceList.Count; i++)
        {

            if (diceView != null)
            {
                viewDiceList[i].m_Instance.transform.SetParent(dicePanels[i].transform, false);
                viewDiceList[i].m_Instance.transform.rotation = diceList[i].m_Instance.transform.rotation;
                viewDiceList[i].m_Instance.GetComponent<MeshCollider>().enabled = false;
                viewDiceList[i].m_Instance.transform.localScale = new Vector3(10000, 10000, 10000);
                viewDiceList[i].m_Instance.transform.position = new Vector3(viewDiceList[i].m_Instance.transform.position.x + .5f, viewDiceList[i].m_Instance.transform.position.y + .5f, viewDiceList[i].m_Instance.transform.position.z);
                viewDiceList[i].m_Instance.transform.Rotate(new Vector3(-45, 0, 0), Space.World);
                dicePanels[i].gameObject.transform.Find("InputField").gameObject.SetActive(true);
                dicePanels[i].gameObject.transform.Find("InputField").GetComponent<InputField>().text = diceList[i].GetValue().ToString();
            }
        }

        diceSelectors.gameObject.SetActive(false);
        btnResultsBack.gameObject.SetActive(false);
        
        btnLeft.gameObject.SetActive(true);
        if (currentResultScreen < diceSelectors.transform.childCount - 1)
        { 
            btnRight.gameObject.SetActive(true);
        }
        else
        {
            btnRight.gameObject.SetActive(false);
        }
    }

    public void ShowSpecialResultsScreen()
    {
        GameObject selectionScreen = m_MessageCanvas.transform.Find("SpecialSelectScreen").gameObject;
        _gameManager.GetTotals();
        List<DiceSet> diceSets = _gameManager.GetDiceSets();
        string results = string.Empty;
        selectionScreen.SetActive(true);

        if (_selection == "RollD20")
        {
            results = diceSets[0].total.ToString();
        }
        if (_selection == "RollD100")
        {
            results = (diceSets[0].total + diceSets[1].total).ToString();
        }
        else if (_selection == "RollCoin")
        {
            if (diceSets[0].total == 1)
            {
                results = "Heads";
            }
            else if (diceSets[0].total == 2)
            {
                results = "Tails";
            }
        }

        selectionScreen.transform.Find("btnSpecialResultsBack").gameObject.SetActive(true);
        selectionScreen.transform.Find("lblResults").gameObject.SetActive(true);
        selectionScreen.transform.Find("lblResults").GetComponent<Text>().text = "Result: " + results;


        btnSpecialResultsBack.gameObject.SetActive(true);
        _selection = string.Empty;
    }







}
