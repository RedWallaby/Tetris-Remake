using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InversionBomb : Block
{
	public override void EndFalling()
	{
		for (int i = -1; i <= 1; i++)
		{
			for (int j = -1; j <= 1; j++)
			{
				if (game.TileIsFree(x + i, y + j))
				{
					game.AddTile(x + i, y + j);
				}
				else if (game.TileIsTaken(x + i, y + j))
				{
					game.RemoveTile(x + i, y + j);
				}
			}
		}
		Destroy(gameObject);
	}
}
