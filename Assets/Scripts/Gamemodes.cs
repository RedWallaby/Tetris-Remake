using UnityEngine;

/// <summary>
/// <c>Gamemodes</c> Class controls the game and palletes to be chosen
/// </summary>
public class Gamemodes : MonoBehaviour
{
    [SerializeField] private GameObject palleteSelection;
	[SerializeField] private GameObject gamemodeSelection;

    public Game currentGame;
    public Block[] pallete;

    private Game previousGame;

    /// <summary>
    /// Sets pallete of new board, generates starting block and clears values of old board
    /// </summary>
    public void SetupGame()
    {
        if (previousGame != null) previousGame.ClearBoard();
		previousGame = currentGame;
        previousGame.enabled = false;

		currentGame.enabled = true;
		currentGame.gameObj.SetActive(true);
		currentGame.AvailableBlocks = pallete;
        currentGame.CreateNewBlock();
    }

    public void SetGame(Game game)
    {
        currentGame = game;
    }

    public void SetPallete(Transform palleteObj)
    {
        pallete = palleteObj.GetComponentsInChildren<Block>();
    }

    public void MoveGameSelector(Transform location)
    {
		gamemodeSelection.transform.position = location.position;
	}

	public void MovePalleteSelector(Transform location)
	{
		palleteSelection.transform.position = location.position;
	}

    public void RandomiseColourPallete()
    {
        foreach (Block block in pallete)
        {
            foreach (Tile tile in block.GetTiles())
            {
                tile.image.color = Random.ColorHSV(0, 1, 0, 1, 0.8f, 1);
            }
        }
    }
}
