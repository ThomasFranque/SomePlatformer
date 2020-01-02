using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GFXBlink
{
	int counter;
	bool toggle;

	//Instantiate this class and put the method on the Update()
	//private void Start()
	//{
	//	counter = 0;
	//}

	//void Update()
	//{
	//	DoBlink(mySpriteRenderer);
	//}

	public GFXBlink()
	{
		counter = 0;
		toggle = false;
	}

	public void DoBlink(SpriteRenderer spriteRen, int delay)
	{
		if (counter >= delay)
		{
			counter = 0;

			toggle = !toggle;
			if (toggle)
			{
				spriteRen.enabled = true;
			}
			else
			{
				spriteRen.enabled = false;
			}

		}
		else
		{
			counter++;
		}
	}
}
