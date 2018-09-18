using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using Pokega;

public class TitleScreenManager : MonoBehaviour {

    // Use this for initialization
    //public GameObject settingsPrefab;
    //public UIButton newGameBtn;
    public UIButton continueBtn;
    //public UIButton settingsBtn; 
    private static bool firstTime=true;
    public static TitleScreenManager instance;
    public static bool newGame;
    //public static bool colorBlindMode = false;
    private GameObject settingsPopUp;


    private bool settingsOpened;
    

    void OnEnable () {
        /*int x = 47;
        for (int i = 0; i < 20; i++)
        {
            Debug.Log(i+" = " + x);
            x = Mathf.RoundToInt(x * 1.05f);
        }*/
        //Screen.SetResolution(Screen.height*9/16, Screen.height, true);

        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
            Destroy(this);

        if (!firstTime)
        {
            //StartCoroutine(Transition.instance.EndTransition());
        }
        else
        {
            SaveAndLoadState.SetLoadable();
        }

        firstTime = false;
        settingsOpened = false;
		Tutorial.instance.showTutorial = false;

        if (!SoundManager.instance.musicSource.isPlaying)
        {
            SoundManager.instance.PlayMusic(SoundManager.instance.bgMusic);
            SoundManager.instance.musicSource.volume = 0.3f;
        }
        else
            StartCoroutine(SoundManager.FadeOut(SoundManager.instance.musicSource, 0.3f));

        
       
        continueBtn.isEnabled = SaveAndLoadState.loadable;
		if (Tutorial.instance!=null)
			Tutorial.instance.gameObject.SetActive (false);
		if (MessagePopup.instance!=null)
			MessagePopup.instance.gameObject.SetActive (false);

    }
		

    public void ContinueGame()
    {
        newGame = false;
    }

    public void StartNewGame()
    {
        newGame = true;
    }

    
    
}
