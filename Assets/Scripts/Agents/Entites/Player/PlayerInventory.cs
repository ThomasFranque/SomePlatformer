using UnityEngine;

public class PlayerInventory
{
    public int Coins { get; private set; }
    public int XP { get; private set; }

    public PlayerInventory()
    {
        Coins = 0;
        XP = 0;
    }

    public void AddCoins(int coinsToAdd)
    {
        Coins += coinsToAdd;
        Debug.Log($"Added {coinsToAdd} Coins.\nNow with {Coins} coins.");
    }
    public void AddXP(int xpToAdd)
    {
        XP += xpToAdd;
        Debug.Log($"Added {xpToAdd} XP.\nNow with {XP} XP.");
    }

    public void SpendStardust(int coinsToSpend)
    {
        Coins -= coinsToSpend;
        if (Coins < 0) Coins = 0;
    }
    public void SpendXP(int xpToSpend)
    {
        XP -= xpToSpend;
        if (XP < 0) XP = 0; 
    }
    
}
