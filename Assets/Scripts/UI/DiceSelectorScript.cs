using UnityEngine;

public class DiceSelectorScript : MonoBehaviour
{
    #region Global Variables

    private int _diceSelectorId = 0;

    #endregion

    #region Generic

    private void Start()
    {

    }

    private void Update()
    {

    }

    #endregion

    #region Getters & Setters

    public int SetDiceSelectorId(int diceSelectorId)
    {
        _diceSelectorId = diceSelectorId;
        return _diceSelectorId;
    }

    public int GetDiceSelectorId()
    {
        return _diceSelectorId;
    }

    #endregion
}
