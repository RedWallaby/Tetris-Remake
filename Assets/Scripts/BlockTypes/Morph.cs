using UnityEngine;

public class Morph : Block
{
	/// <summary>
	/// Overides <c>MoveDown</c> to check if tile is at required y value
	/// </summary>
	/// <returns>A bool if the move was successful</returns>
    public override bool MoveDown()
    {
		print("Sup");
        if (!base.MoveDown()) return false;
        if (GetHighestTile() - y == 3)
        {
			MorphBlock();
		}

        return true;
    }

	/// <summary>
	/// Gets and returns the highest y-value of any tile
	/// </summary>
	/// <returns>the y coordinate of the highest tile</returns>
	public int GetHighestTile()
	{
		for (int i = 0; i < game.board.GetLength(1); i++)
		{
			for (int j = 0; j < game.board.GetLength(0); j++)
			{
				if (game.board[j, i] != null) return i;
			}
		}
		return 0;
	}
}
