using UnityEngine;

public class PlayerCurrency : MonoBehaviour
{
    public int currency = 0; // ---- Variable to keep track of collected blobs ----

    public void AddCurrency(int amount)
    {
        currency += amount;
    }

    public int GetCurrency()
    {
        return currency;
    }
    
    public void AddRandomCurrency()
    {
        int randomAmount = Random.Range(5, 21); // ---- Random amount between 5 and 20 ----
        AddCurrency(randomAmount);
    }
    
    
}
