using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class GameManager : MonoBehaviour {

    public float m_StartDelay = .01f;
    public CameraControl m_CameraControl;
    public Canvas m_MessageCanvas;
    public Button btnRoll;
    public Button btnBack;
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

    private WaitForSeconds m_StartWait;
    private List<DiceSet> m_DiceSets;
    private bool doneRolling = false;
    private int diceSelectorsTotal = 1;

    public struct DiceSet
    {
        public List<DiceManager> m_Dice;
        public int modifier;
        public int total;
    }

    // Use this for initialization
    void Start () {
        m_StartWait = new WaitForSeconds(m_StartDelay);
        StartCoroutine(GameLoop());
    }

    private IEnumerator GameLoop()
    {
        yield return StartCoroutine(ShowIntroScreen());
        ShowSelectScreen();
        

    }

    private IEnumerator ShowIntroScreen()
    {
        GameObject introScreen = m_MessageCanvas.transform.Find("IntroScreen").gameObject;
        GameObject selectionScreen = m_MessageCanvas.transform.Find("SelectionScreen").gameObject;
        introScreen.SetActive(true);
        selectionScreen.SetActive(false);
        yield return m_StartWait;
    }

    private void ShowSelectScreen()
    {
        GameObject introScreen = m_MessageCanvas.transform.Find("IntroScreen").gameObject;
        GameObject selectionScreen = m_MessageCanvas.transform.Find("SelectionScreen").gameObject;
        GameObject diceSelectors = selectionScreen.transform.Find("DiceSelectors").gameObject;
        introScreen.SetActive(false);
        selectionScreen.SetActive(true);
        GameObject diceSelector = diceSelectors.transform.Find("DiceSelector").gameObject;
        GameObject btnAddNew = diceSelector.transform.Find("btnAddNew").gameObject;
        GameObject btnRemove = diceSelector.transform.Find("btnRemove").gameObject;
        diceSelector.transform.Find("lblTotal").gameObject.SetActive(false);
        btnAddNew.GetComponent<Button>().onClick.AddListener(delegate { AddNewClick(); });
        btnRemove.GetComponent<Button>().onClick.AddListener(delegate { RemoveClick(0); });
        btnRoll.GetComponent<Button>().onClick.AddListener(delegate { RollClick(); });
        btnBack.GetComponent<Button>().onClick.AddListener(delegate { BackClick(); });
        diceSelector.name = "DiceSelector1";
        btnBack.gameObject.SetActive(false);
        
    }


    void AddNewClick()
    {
        GameObject selectionScreen = m_MessageCanvas.transform.Find("SelectionScreen").gameObject;
        GameObject diceSelectors = selectionScreen.transform.Find("DiceSelectors").gameObject;
        GameObject lastSelector = FindLastSelector(diceSelectors);
        GameObject diceSelector;
        GameObject btnAddNew;
        GameObject btnRemove;

        if (diceSelectors.transform.childCount < 6) {
            diceSelector = GameObject.Instantiate(DiceSelector);
            diceSelector.transform.parent = diceSelectors.transform;
            btnRoll.transform.position = new Vector3(btnRoll.transform.position.x, btnRoll.transform.position.y - diceSelectionAdjustment, btnRoll.transform.position.z);
            btnBack.transform.position = new Vector3(btnBack.transform.position.x, btnBack.transform.position.y - diceSelectionAdjustment, btnBack.transform.position.z);
            diceSelector.transform.position = new Vector3(lastSelector.transform.position.x, lastSelector.transform.position.y - diceSelectionAdjustment, lastSelector.transform.position.z);
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
        if (diceSelectors.transform.childCount > 1) { 
            Destroy(selectorToRemove);
            for(int i = id; i < diceSelectors.transform.childCount; i++)
            {
                currentSelector = diceSelectors.transform.GetChild(i).gameObject;
                currentSelector.transform.position = new Vector3(currentSelector.transform.position.x, currentSelector.transform.position.y + diceSelectionAdjustment, currentSelector.transform.position.z);
                currentSelector.name = "DiceSelector" + i;
            }

            btnRoll.transform.position = new Vector3(btnRoll.transform.position.x, btnRoll.transform.position.y + diceSelectionAdjustment, btnRoll.transform.position.z);
            btnBack.transform.position = new Vector3(btnBack.transform.position.x, btnBack.transform.position.y + diceSelectionAdjustment, btnBack.transform.position.z);
        }
    }

    void RollClick()
    {
        GameObject selectionScreen = m_MessageCanvas.transform.Find("SelectionScreen").gameObject;
        GameObject diceSelectors = selectionScreen.transform.Find("DiceSelectors").gameObject;
        int currentDiceCount = 0;
        int currentModifier = 0;
        int currentDiceType = 0;
        int selectedDiceTypeValue = 0;
        m_DiceSets = new List<DiceSet>();
        for (int i = 0; i < diceSelectors.transform.childCount; i++)
        {
            GameObject diceSelector = diceSelectors.transform.GetChild(i).gameObject;
            DiceSet currentDiceSet = new DiceSet();

            GameObject txtDiceCount = diceSelector.transform.Find("txtDiceCount").gameObject;
            GameObject txtModifier = diceSelector.transform.Find("txtModifier").gameObject;
            GameObject ddlDiceSelector = diceSelector.transform.Find("ddlDiceSelector").gameObject;
            
            if (txtDiceCount.transform.Find("Text").GetComponent<Text>().text == "")
            {
                currentDiceCount = 1;
            }
            else
            {
                currentDiceCount = Convert.ToInt32(txtDiceCount.transform.Find("Text").GetComponent<Text>().text);
            }
            if (txtModifier.transform.Find("Text").GetComponent<Text>().text == "")
            {
                currentModifier = 0;
            }
            else
            {
                currentModifier = Convert.ToInt32(txtModifier.transform.Find("Text").GetComponent<Text>().text);
            }
            
            selectedDiceTypeValue = Convert.ToInt32(ddlDiceSelector.GetComponent<Dropdown>().value);
            currentDiceType = Convert.ToInt32(ddlDiceSelector.GetComponent<Dropdown>().options[selectedDiceTypeValue].text);

            currentDiceSet.m_Dice = new List<DiceManager>();
            currentDiceSet.modifier = currentModifier;

            for(int j = 0; j < currentDiceCount; j++)
            {
                DiceManager currentDice = new DiceManager();
                currentDice = SetDiceType(currentDice, currentDiceType);
                currentDice.RandomizeDice();
                int xCoordinate = 0;
                int zCoordinate = 0;
                if (j > currentDiceCount / 2)
                {
                    xCoordinate = -1 * (j + 1) + 2;
                }
                else
                {
                    xCoordinate = (j + 1) + 2;
                }

                if (i > diceSelectors.transform.childCount / 2)
                {
                    zCoordinate = -1 * (i + 1) + 2;
                }
                else
                {
                    zCoordinate = (i + 1) + 2;
                }
                

                currentDice.m_Instance.transform.position = new Vector3(xCoordinate, 10, zCoordinate);
                currentDiceSet.m_Dice.Add(currentDice);
            }
            m_DiceSets.Add(currentDiceSet);
        }
        selectionScreen.SetActive(false);
    }

    void BackClick()
    {
        m_DiceSets = new List<DiceSet>();
        GameObject selectionScreen = m_MessageCanvas.transform.Find("SelectionScreen").gameObject;
        GameObject diceSelectors = selectionScreen.transform.Find("DiceSelectors").gameObject;
        if (diceSelectors.transform.childCount > 1)
        {
            
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
                else { 
                    GameObject currentSelector = diceSelectors.transform.GetChild(i).gameObject;
                    Destroy(currentSelector);
                }
            }

            btnRoll.transform.position = new Vector3(btnRoll.transform.position.x, btnRoll.transform.position.y + diceSelectionAdjustment, btnRoll.transform.position.z);
            btnBack.transform.position = new Vector3(btnBack.transform.position.x, btnBack.transform.position.y + diceSelectionAdjustment, btnBack.transform.position.z);
        }
        GameObject[] allDice = GameObject.FindGameObjectsWithTag("Dice");
        foreach (GameObject dice in allDice)
        {
            Destroy(dice);
        }
        btnRoll.gameObject.SetActive(true);
        btnBack.gameObject.SetActive(false);

    }

    GameObject FindLastSelector(GameObject diceSelectors)
    {
        GameObject lastSelector;
        lastSelector = diceSelectors.transform.GetChild(diceSelectors.transform.childCount - 1).gameObject;

        return lastSelector;
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
        if(m_DiceSets != null) { 
            doneRolling = IsDoneRolling();
      
            if (doneRolling == true)
            {
                GetTotals();
                ReShowSelectScreen();
            }
        }
    }

    bool IsDoneRolling()
    {
        bool noMovement = false;
        for (int i = 0; i < m_DiceSets.Count; i++)
        {
            DiceSet currentDiceSet = m_DiceSets[i];
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

    void ReShowSelectScreen()
    {
        
        GameObject selectionScreen = m_MessageCanvas.transform.Find("SelectionScreen").gameObject;
        GameObject diceSelectors = selectionScreen.transform.Find("DiceSelectors").gameObject;

        selectionScreen.SetActive(true);
        
        for (int i = 0; i < diceSelectors.transform.childCount; i++)
        {
            GameObject diceSelector = diceSelectors.transform.GetChild(i).gameObject;
            DiceSet currentDiceSet = m_DiceSets[i];

            diceSelector.transform.Find("btnAddNew").gameObject.SetActive(false);
            diceSelector.transform.Find("btnRemove").gameObject.SetActive(false);
            diceSelector.transform.Find("lblTotal").gameObject.SetActive(true);
            diceSelector.transform.Find("lblTotal").GetComponent<Text>().text = "Total: " + currentDiceSet.total;

        }
        
        btnBack.gameObject.SetActive(true);
        btnRoll.gameObject.SetActive(false);

    }

    void GetTotals()
    {
        int diceSetTotal = 0;
        for (int i = 0; i < m_DiceSets.Count; i++)
        {
            diceSetTotal = 0;
            DiceSet currentDiceSet = m_DiceSets[i];
            for (int j = 0; j < currentDiceSet.m_Dice.Count; j++)
            {
                DiceManager currentDice = currentDiceSet.m_Dice[j];
                diceSetTotal += currentDice.GetValue();
            }
            diceSetTotal += currentDiceSet.modifier;
            currentDiceSet.total = diceSetTotal;
            m_DiceSets[i] = currentDiceSet;
        }
    }
}
