using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class <c>Block</c> Controls falling motion and collision checking of new tiles
/// </summary>
public class Block : MonoBehaviour
{
    public Game game;

	[SerializeField] protected List<Tile> tiles = new();
	[SerializeField] protected int x = 0, y = 0;
    [SerializeField] protected float xRot = 0, yRot = 0;

    public List<Tile> GetTiles()
    {
        return tiles;
    }

	/// <summary>
	/// Determines if this block can exist without overlap and without exiting the board's bounds
	/// </summary>
	/// <returns> A bool stating if it can exist</returns>
	public bool CanExist()
    {
        foreach (Tile tile in tiles)
        {
			if (game.TileIsFree(tile.x, tile.y)) return true;
		}
        return true;
    }

    /// <summary>
    /// Moves block and all tiles to a specific location assuming no collision
    /// </summary>
    /// <param name="x">the new x coordinate</param>
    /// <param name="y">the new y coordinate</param>
    public void SetLocation(int x, int y)
    {
		int xChange = x - this.x;
        int yChange = y - this.y;
        foreach (Tile tile in tiles)
        {
            tile.x += xChange;
            tile.y += yChange;
        }
        this.x = x;
        this.y = y;
        transform.position = game.IndexPosition(x, y);
    }

	/// <summary>
	/// Moves a block horizontally accounting for collisions
	/// </summary>
	/// <param name="x"> amount to move right, negatives move left</param>
	/// <returns>A bool for if the movement was successful</returns>
	public bool MoveHorizontally(int x)
	{
		bool allValidMoves = true;
		foreach (Tile tile in tiles)
		{
			if (tile.x + x >= game.board.GetLength(0) || tile.x + x < 0 || game.board[tile.x + x, tile.y] != null)
			{
				allValidMoves = false;
				break;
			}
		}
		if (!allValidMoves)
		{
			return false;
		}
		SetLocation(this.x + x, y);
		return true;
	}

	/// <summary>
	/// Moves a block horizontally accounting for collisions
	/// </summary>
	/// <returns>A bool for if the movement was successful</returns>
	public virtual bool MoveDown()
    {
        bool allValidMoves = true;
        foreach (Tile tile in tiles)
        {
            if (tile.y + 1 >= game.board.GetLength(1) || game.board[tile.x, tile.y+1] != null)
            {
                allValidMoves = false;
                break;
            }
        }
        if (!allValidMoves)
        {
			EndFalling();
            return false;
		}
        SetLocation(x, y + 1);
        return true;
	}

    public void MoveTile(Tile tile, int x, int y)
    {
        tile.x = x;
        tile.y = y;
        tile.transform.position = game.IndexPosition(x, y);
    }

	/// <summary>
	/// Rotates a block 90 degrees clockwise
	/// </summary>
	/// <remarks>
	/// Method used flips numbers and makes right coordinate negative to find new rotation coordinates
	/// (1, 2) -> (2, -1) -> (-1, -2) -> (-2, 1) -> (1, 2)
	/// </remarks>
	public virtual void Rotate()
    {
        //check if rotation is valid
        foreach (Tile tile in tiles)
        {
			float xDiff = tile.x - x - xRot;
            float yDiff = tile.y - y - yRot;
            (xDiff, yDiff) = (-yDiff, xDiff); //this is now backwards due to the board's y axis being inverted (increase in y is down the screen)
			if (!game.TileIsFree((int)(x + xRot + xDiff), (int)(y + yRot + yDiff))) return;
        }

        //complete rotation
        foreach (Tile tile in tiles)
        {
			float xDiff = tile.x - x - xRot;
			float yDiff = tile.y - y - yRot;
			(xDiff, yDiff) = (-yDiff, xDiff);
			MoveTile(tile, (int)(x + xRot + xDiff), (int)(y + yRot + yDiff));
        }
    }

	/// <summary>
	/// Fill in game array and transfer tiles to board
	/// </summary>
	public virtual void EndFalling()
    {
        foreach (Tile tile in tiles)
        {
            game.board[tile.x, tile.y] = tile;
            tile.gameObject.transform.SetParent(game.tilePanel);
        }
        Destroy(gameObject);
    }

	/// <summary>
	/// Attempts to morph the block into a new one
	/// </summary>
	public void MorphBlock()
	{
		Block newBlock = Instantiate(game.AvailableBlocks[Random.Range(0, 7)], gameObject.transform);
		newBlock.SetLocation(x, y);

		if (!newBlock.CanExist())
		{
			Destroy(newBlock.gameObject);
			return;
		}

		foreach (Tile tile in tiles)
		{
			Destroy(tile.gameObject);
		}
		tiles.Clear();
		foreach (Tile tile in newBlock.GetTiles())
		{
			tile.transform.SetParent(transform, true);
			tiles.Add(tile);
		}
		Destroy(newBlock.gameObject);
	}
}
