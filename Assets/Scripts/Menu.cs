using UnityEngine;

/// <summary>
/// Basic <c>Menu</c> class allows easy manipulation of on-screen interfaces
/// </summary>
public class Menu : MonoBehaviour
{
    public void GotoMenu(Menu menu)
    {
        menu.ShowMenu();
        HideMenu();
    }

    public void ShowMenu()
    {
        gameObject.SetActive(true);
    }

    public void HideMenu()
    {
        gameObject.SetActive(false);
    }

    public void CloseGame()
    {
        Application.Quit();
    }
}
