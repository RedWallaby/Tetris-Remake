using System;
using UnityEngine;

/// <summary>
/// Class <c>MemoryGame</c> inherits <c>Game</c> and adds memory functions to board
/// </summary>
public class MemoryGame : Game
{
	private float colourTime = 1.5f;
	private float alpha = 0;
	private bool finishedDecreasing = true;
	private int revealProgress;
	private int gs = 1;

	/// <summary>
	/// Overides orginal function to change alpha values of board
	/// </summary>
	protected override void ProcessBlockPlacement()
	{
		base.ProcessBlockPlacement();
		colourTime = 1.5f / (1 + (points % 20000 / 12000) + points/120000); //increase difficulty (for every 12k points it is +1x quicker so 2x -> 3x)
		revealProgress--;
		if (revealProgress > 0)
		{
			alpha = 0;
		}
		else
		{
			alpha = 1;
			revealProgress = points / 20000 + 1;
		}
		finishedDecreasing = false;
		foreach (Tile tile in board)
		{
			if (tile == null) continue;
			tile.image.color = new Color(gs, gs, gs, 255);
		}
	}

	protected override void Update()
	{
		base.Update();
		if (finishedDecreasing) return;
		alpha = Math.Max(0, alpha - Time.deltaTime/ colourTime);
		foreach (Tile tile in board)	
		{
			if (tile == null) continue;
			tile.image.color = new Color(gs, gs, gs, alpha);
		}
		if (alpha == 0) finishedDecreasing = true;
	}
}
