using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Source for code: https://www.sharpcoderblog.com/blog/unity-3d-create-main-menu-with-ui-canvas
public class SC_MainMenu : MonoBehaviour
{
    public GameObject MainMenu;
    public GameObject CreditsMenu;
    public GameObject CarMenu;

    // Start is called before the first frame update
    void Start()
    {
        MainMenuButton();
    }

    public void PlayNowButton()
    {
        // Play Now Button has been pressed, here you can initialize your game (For example Load a Scene called GameLevel etc.)
        MainMenu.SetActive(false);
        UnityEngine.SceneManagement.SceneManager.LoadScene("Tutorial");

    }


    public void CreditsButton()
    {
        // Show Credits Menu
        MainMenu.SetActive(false);
        CreditsMenu.SetActive(true);
    }

    public void MainMenuButton()
    {
        // Show Main Menu
        MainMenu.SetActive(true);
        CreditsMenu.SetActive(false);
    }


    public void QuitButton()
    {
        // Quit Game
        Application.Quit();
    }
}