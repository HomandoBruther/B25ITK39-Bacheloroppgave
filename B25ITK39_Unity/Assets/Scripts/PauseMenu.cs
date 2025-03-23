using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    
    public GameObject pauseMenuUI;
    public GameObject controlsUI;
    public GameObject settingsUI;


    public static bool GameIsPaused = false;
    public static bool isShowControlActive = false;
    public static bool isSettingsActive = false;

    void Start() {

        Time.timeScale = 1f;

        pauseMenuUI.SetActive(false);
        controlsUI.SetActive(false);
        settingsUI.SetActive(false);
        

        GameIsPaused = false;
        isShowControlActive = false;
        isSettingsActive = false;

        
    
    }

    void Update(){

        if (Input.GetKeyDown(KeyCode.Escape)) {

            if (isSettingsActive){
                ReturnToPauseMenu();
            }

            else if (isShowControlActive){
                ReturnToPauseMenu();
            }

            else if (GameIsPaused) {
                Resume(); 
            }
            else{
                Pause();
            } 
        }
        
    }


    public void Resume(){

        Time.timeScale = 1f;
        
        pauseMenuUI.SetActive(false);
        
        GameIsPaused = false;

    }

    public void Pause(){

         Time.timeScale = 0f;

        pauseMenuUI.SetActive(true);
       
        GameIsPaused = true;
    }



    public void ShowControls() {

        Time.timeScale = 0f;

        pauseMenuUI.SetActive(false);
        controlsUI.SetActive(true);
        settingsUI.SetActive(false);

        isShowControlActive = true;
    }

    public void ReturnToPauseMenu() {

        Time.timeScale = 0f;

        pauseMenuUI.SetActive(true);
        controlsUI.SetActive(false);
        settingsUI.SetActive(false);

        isShowControlActive = false;
        isSettingsActive = false;

    }

    public void ShowSettings(){

        Time.timeScale = 0f;

        pauseMenuUI.SetActive(false);
        settingsUI.SetActive(true);
        controlsUI.SetActive(false);

        isSettingsActive = true;

    }

    public void RestartGame(){
        SceneManager.LoadScene("StageOne");
    }

    public void LoadMenu(){

        SceneManager.LoadScene("MainMenu");
    }


    public void QuitGame(){

        Application.Quit();
    }
}
