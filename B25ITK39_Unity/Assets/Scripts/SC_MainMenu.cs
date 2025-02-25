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
        CarMenu.SetActive(true);
    }

    public void vehicleSportsCarButton()
    {
    https://www.reddit.com/r/Unity2D/comments/14dpepg/how_to_get_a_variable_from_another_gameobject_and/
        PlayerData.PD.SetCarChoiceToSportsCar();
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameLevel");
    }

    public void vehicleBusButton()
    {
    https://www.reddit.com/r/Unity2D/comments/14dpepg/how_to_get_a_variable_from_another_gameobject_and/
        PlayerData.PD.SetCarChoiceToBus();
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameLevel");
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