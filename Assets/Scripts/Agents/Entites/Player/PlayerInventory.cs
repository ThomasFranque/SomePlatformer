using UnityEngine;

public class PlayerInventory
{
    public int StarDust { get; private set; }
    public int XP { get; private set; }

    public PlayerInventory()
    {

    }

    public void AddStardust(int dustToAdd)
    {
        StarDust += dustToAdd;
    }
    public void AddXP(int xpToAdd)
    {
        XP += xpToAdd;
    }

    public void SpendStardust(int dustToSpend)
    {
        StarDust -= dustToSpend;
        if (StarDust < 0) StarDust = 0;
    }
    public void SpendXP(int xpToSpend)
    {
        XP -= xpToSpend;
        if (XP < 0) XP = 0; 
    }
    
}
