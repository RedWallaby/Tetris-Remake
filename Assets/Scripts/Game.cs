using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Class <c>Game</c> controls all aspects of board including movement of falling blocks and various
/// row checking aspects
/// </summary>
public class Game : MonoBehaviour
{
    [SerializeField] protected Transform backgroundPanel;
	public Transform tilePanel;
	protected List<Block> ActiveFallingBlocks = new();

    public Tile[,] board = new Tile[10, 20];
	public int maxBlockIndex;
	[HideInInspector] public Block[] AvailableBlocks;

	protected float horizontalMoveCooldown = 0.1f;
	protected float horizontalMoveTimer;

	protected bool keySpaceWasPressed;
	protected bool rotationWasPressed;

    protected int points = 0;

    public GameObject dt; //dt = duplicatable tile
    public TMP_Text pointsText;

    public GameObject gameObj;

    public GameObject endScreen;
    public TMP_Text endScore;

	public float moveCooldown = 0.5f;
	public float moveTimer = 0.5f;

    /// <summary>
    /// Creates a new block from a given range of preset blocks
    /// </summary>
    public void CreateNewBlock()
    {
        Block newBlock = Instantiate(AvailableBlocks[UnityEngine.Random.Range(0, maxBlockIndex)], transform);
        newBlock.SetLocation((int)Math.Floor(board.GetLength(0) / 2f), 0);
		newBlock.transform.position = IndexPosition(5, 0);
        newBlock.game = this;
        ActiveFallingBlocks.Add(newBlock);

        if (newBlock.GetType() == typeof(Swapper))
        {
            newBlock.Rotate();
        }
        else if (newBlock.GetType() == typeof(Morph))
        {
            ((Morph)newBlock).MorphBlock();
        }
    }

    /// <summary>
    /// Generates a new tile and sets its position on the board
    /// </summary>
    /// <param name="x"> the new x coordinate</param>
    /// <param name="y"> the new y coordinate</param>
    public void AddTile(int x, int y)
    {
        if (!TileIsFree(x, y)) return;
        GameObject newObj = Instantiate(dt, tilePanel);
        Tile newTile = newObj.GetComponent<Tile>();
        newTile.x = x;
        newTile.y = y;
        board[x, y] = newTile;
        newObj.transform.position = IndexPosition(x, y);
    }


    /// <summary>
    /// Removes a tile from the board
    /// </summary>
    /// <param name="x">the x coordinate of the position to remove</param>
    /// <param name="y">the y coordinate of the position to remove</param>
    public void RemoveTile(int x, int y)
    {
        if (!TileIsTaken(x, y)) return;
		Destroy(board[x, y].gameObject);
	}

	/// <summary>
	/// Checks if a position on the board is empty
	/// </summary>
	/// <param name="x">the x coordinate of the position to check</param>
	/// <param name="y">the y coordinate of the position to check</param>
	/// <returns>A bool dependant on the board's tile at the x and y position and the boards bounds</returns>
	public bool TileIsFree(int x, int y)
    {
        if (x < 0 || y < 0 || x >= board.GetLength(0) || y >= board.GetLength(1)) return false;
        return board[x, y] == null;
    }

	/// <summary>
	/// Checks if a position on the board is taken
	/// </summary>
	/// <param name="x">the x coordinate of the position to check</param>
	/// <param name="y">the y coordinate of the position to check</param>
	/// <returns>A bool dependant on the board's tile at the x and y position and the boards bounds</returns>
    /// <remarks>
    /// Not the opposite of <c>TileIsFree</c> as it will also return false if the given position is outside the bounds of the board
    /// </remarks>
	public bool TileIsTaken(int x, int y)
    {
        if (x < 0 || y < 0 || x >= board.GetLength(0) || y >= board.GetLength(1)) return false;
        return board[x, y] != null;
    }

	/// <summary>
	/// Gets the unity position of a board's coordinate
	/// </summary>
	/// <param name="x">the desired x coordinate</param>
	/// <param name="y">the desired y coordinate</param>
	/// <returns>A 2D Vector for the Unity position</returns>
	public Vector2 IndexPosition(int x, int y)
    {
        Transform obj = backgroundPanel.GetChild(x + board.GetLength(0) * y);
        return obj.transform.position;
    }

    /// <summary>
    /// Processes the new block placement and updates values of the game
    /// </summary>
    protected virtual void ProcessBlockPlacement()
    {
        if (Gameover()) return;
		CreateNewBlock();
		CheckForFill();

		moveCooldown = 0.5f / (1 + points / 50000); //doubles speed every 50k points
	}

    /// <summary>
    /// Checks for and completes gameover operations
    /// </summary>
    /// <returns>A bool if the game is completed</returns>
    private bool Gameover()
    {
		for (int i = 0; i < 2; i++)
		{
			for (int j = 0; j < board.GetLength(0); j++)
			{
                if (board[j, i] == null) continue;
				foreach (Block block in ActiveFallingBlocks)
				{
                    Destroy(block);
				}
                ActiveFallingBlocks.Clear();
                SetupGameoverMenu();
				return true;
			}
		}
        return false;
	}

    /// <summary>
    /// Removes all elements of the board
    /// </summary>
    public void ClearBoard()
    {
		for (int i = 0; i < board.GetLength(1); i++)
		{
			for (int j = 0; j < board.GetLength(0); j++)
			{
				if (board[j, i] != null) Destroy(board[j, i].gameObject);
			}
		}
	}

    /// <summary>
    /// Setup for end screen, awakens menu and updates point score
    /// </summary>
    private void SetupGameoverMenu()
    {
        endScreen.SetActive(true);
        endScore.text = points.ToString();
    }

	/// <summary>
    /// The main loop of the game, controls the activate blocks dependant on user input
    /// </summary>
	protected virtual void Update()
    {
        if (ActiveFallingBlocks.Count == 0) return;

        Block block = ActiveFallingBlocks[0];
        horizontalMoveTimer = Math.Max(horizontalMoveTimer - Time.deltaTime, 0);
        if (horizontalMoveTimer == 0)
        {
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                horizontalMoveTimer = horizontalMoveCooldown;
                block.MoveHorizontally(-1);
            }
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                horizontalMoveTimer = horizontalMoveCooldown;
                block.MoveHorizontally(1);
            }
        }

        //acceleration down
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            moveTimer -= 10 * Time.deltaTime;
        }
        else
        {
            moveTimer -= Time.deltaTime;
        }

        //instant drop
        if (Input.GetKey(KeyCode.Space))
        {
            if (!keySpaceWasPressed) //cannot be conjoined
            {
                keySpaceWasPressed = true;
                while (block.MoveDown()) ; //shortcut
                ActiveFallingBlocks.RemoveAt(0);
                ProcessBlockPlacement();
                return;
            }
        }
        else
        {
            keySpaceWasPressed = false;
        }

        //rotation
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            if (!rotationWasPressed)
            {
                rotationWasPressed = true;
                block.Rotate();
            }
        }
        else
        {
            rotationWasPressed = false;
        }

        //auto move-down
        if (moveTimer <= 0)
        {
            moveTimer += moveCooldown;
            if (!block.MoveDown())
            {
                ActiveFallingBlocks.RemoveAt(0);
                ProcessBlockPlacement();
            }
        }
    }

    /// <summary>
    /// Checks for any completed rows on the board
    /// </summary>
    public void CheckForFill()
    {
        int rowsInOne = 0;
        for (int i = 0; i < board.GetLength(1); i++)
        {
            bool completeRow = true;
            for (int j = 0; j < board.GetLength(0); j++)
            {
                if (board[j, i] == null) completeRow = false;
            }
            if (completeRow)
            {
                rowsInOne++;
                RemoveRow(i);
            }
        }

        if (rowsInOne == 0) return;
        points += (int)Math.Pow(rowsInOne, 2f) * 1000;
        UpdatePoints();
    }

    /// <summary>
    /// Removes a desired row
    /// </summary>
    /// <param name="y">The y coordinate of the row to be removed</param>
    public void RemoveRow(int y)
    {
        for (int i = 0; i < board.GetLength(0); i++)
        {
            Destroy(board[i, y].gameObject);
            board[i, y] = null;
        }

        for (int i = y; i > 0; i--)
        {
            for (int j = 0; j < board.GetLength(0); j++)
            {
                MovePlacedTile(j, i - 1, j, i);
			}
        }
    }

	/// <summary>
    /// Moves a placed tile accounting for updating board
    /// </summary>
    /// <param name="x">the original x coordinate</param>
    /// <param name="y">the original y coordinate</param>
    /// <param name="x2">the new x coordinate</param>
    /// <param name="y2">the new y coordinate</param>
	public void MovePlacedTile(int x, int y, int x2, int y2)
	{
		if (board[x, y] == null || board[x2, y2] != null) return;
		Tile tile = board[x, y];
        (board[x2, y2], board[x, y]) = (board[x, y], board[x2, y2]); //swap positions to avoid linking
		tile.transform.position = IndexPosition(x2, y2);
	}

    public void UpdatePoints()
    {
        pointsText.text = points.ToString();
    }
}
