public class InvertedBomb : Block
{
	public override void EndFalling()
	{
		for (int i = -1; i <= 1; i++)
		{
			for (int j = -1; j <= 1; j++)
			{
				game.AddTile(x + i, y + j);
			}
		}
		Destroy(gameObject);
	}
}
