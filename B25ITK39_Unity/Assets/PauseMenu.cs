using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    
    public GameObject pauseMenuUI;
    public GameObject controlsUI;

    public static bool GameIsPaused = false;
    public static bool isShowControlActive = false;

    void Start() {
        pauseMenuUI.SetActive(false);
        controlsUI.SetActive(false);
        GameIsPaused = false;
        isShowControlActive = false;
        Time.timeScale = 1f;
    
    }

    void Update(){

        if (Input.GetKeyDown(KeyCode.Escape)) {

            if (isShowControlActive){
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
        
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;

    }

    public void Pause(){

        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }



    public void ShowControls() {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 0f;
        controlsUI.SetActive(true);
        isShowControlActive = true;
    }

    public void ReturnToPauseMenu() {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        controlsUI.SetActive(false);
        isShowControlActive = false;

    }

    public void LoadMenu(){
        SceneManager.LoadScene("MainMenu");
    }


    public void QuitGame(){
        Application.Quit();
    }
}
