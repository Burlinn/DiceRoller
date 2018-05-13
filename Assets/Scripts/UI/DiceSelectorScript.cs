using UnityEngine;

public class DiceSelectorScript : MonoBehaviour
{
    private int _diceSelectorId = 0;  

  


    private void Start()
    {
        
    }


    private void Update()
    {
        
    }

    public int SetDiceSelectorId(int diceSelectorId)
    {
        _diceSelectorId = diceSelectorId;
        return _diceSelectorId;
    }

    public int GetDiceSelectorId()
    {
        return _diceSelectorId;
    }
}
