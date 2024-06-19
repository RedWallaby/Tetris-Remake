using Unity.Mathematics;
using UnityEngine;

public class Swapper : Block
{
	[SerializeField] private float swapCooldown = 1;
	[SerializeField] private float swapTimer = 0;


	/// <summary>
	/// Overrides <c>Rotate</c> to replace it with an attempted morph
	/// </summary>
	public override void Rotate()
	{
		if (swapTimer != 0) return;
		swapTimer = swapCooldown;
		MorphBlock();
		for (int i = 0; i < UnityEngine.Random.Range(0, 3); i++)
		{
			base.Rotate();
		}
	}

	private void Update()
	{
		swapTimer = math.max(swapTimer - Time.deltaTime, 0);
	}
}
