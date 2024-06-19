using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    public Image image;
    public int x, y;

	private void Awake()
	{
		image = GetComponent<Image>();
	}
}
