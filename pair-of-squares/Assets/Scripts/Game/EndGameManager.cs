using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using Pokega;

public class EndGameManager : MonoBehaviour {

    // Use this for initialization
    //public Text scoreText;
    //public Text bestText;
    //public Text levelText;
    public GameObject scoreCircle;
	public UILabel levelTxt;
	public UILabel bestTxt;

	private GameObject container;
	private Coroutine scoreCoroutine;
	private Coroutine bestScoreCoroutine;


	void OnDisable()
	{
		if (scoreCoroutine != null) 
		{
			StopCoroutine (scoreCoroutine);
			scoreCoroutine = null;
		}

		if (bestScoreCoroutine != null) 
		{
			StopCoroutine (bestScoreCoroutine);
			bestScoreCoroutine = null;
		}
		
		Destroy(container);
	}

	void OnEnable () {

        //StartCoroutine(Transition.instance.EndTransition());
        //App.score.SetAndSaveBestScore();
        #if UNITY_ANDROID
        App.score.SaveScoresToGP();
        #endif
        #if UNITY_IOS
        App.score.SaveScoresToGC();
        #endif

		container = new GameObject ();
		if (RecordControl.IsRecording ()) 
		{
			RecordControl.StopRecording ();
			RecordControl.SetMetadata ("score", App.score.GetScore ("Points"));
		}
        //App.score.SetScore(GameManager.level, "Level");
		scoreCoroutine=StartCoroutine(AnimateScore());
		if (!GameManager.newHighscore) 
		{
			bestTxt.text = "BEST " + GameManager.bestScore;
		} 
		else 
		{
			bestTxt.text = "NEW RECORD";
			bestScoreCoroutine = StartCoroutine (AnimateBestScore ());
		}
		levelTxt.text = "LEVEL " + GameManager.level;

        //App.score.SetScore(GameManager.level,"Level");
        if (SoundManager.instance.musicSource.isPlaying)
        {
            StartCoroutine(SoundManager.FadeOut(SoundManager.instance.musicSource, 0.3f));
        }

		App.appRate.CheckRated ();

		MessagePopup.instance.gameObject.SetActive (false);
		Tutorial.instance.gameObject.SetActive (false);

		RecordControl.StopRecording ();

    }

    public void StartNewGame()
    {
        TitleScreenManager.newGame = true;
    }

    private IEnumerator AnimateScore()
    {

        
        int score = 0;
        int k = 0;
        //scoreText.text = "" + score;
		int finalScore=(int) App.score.GetScore("Points");
		App.score.SetScore(0, "Points");
		yield return new WaitForSeconds(1f);
		while (score < finalScore)
        {
            scoreCircle.transform.localScale = Vector3.one * Random.Range(0.9f, 1.1f);
            score += Mathf.CeilToInt(GameManager.score / 60f);
            //scoreText.text = "" + score;
            App.score.SetScore(score,"Points");
            k++;
            if (k%5==1)
                SoundManager.instance.PlaySound(SoundManager.instance.scoreCountSfx);

            yield return null;
        }
        scoreCircle.transform.localScale = Vector3.one;
		App.score.SetScore(finalScore, "Points");
       
        if (GameManager.newHighscore)
        {
			
            for (int i = 0; i < 30; i++)
			{
				Vector3 pos = scoreCircle.transform.position + new Vector3(Random.Range(-2.5f, 2.5f), Random.Range(-2.5f, 2.5f), 0f);
                float radius = Random.Range(0.5f, 1.5f);
				int numOfP = Random.Range(10, 20);
				yield return new WaitForSeconds(Random.Range(0.1f, 0.4f));
				StartCoroutine(Explosion.Explode(pos, Color.clear, numOfP, radius, 0.5f,0f,container.transform));
				SoundManager.instance.PlayFireworksSound ();
             }
        }
    
        yield break;
    }

	public IEnumerator AnimateBestScore()
	{
		Color col = bestTxt.color;
		while (true) 
		{
			float dur = 30f;
			for (int i = 1; i <= dur; i++) 
			{
				bestTxt.color = new Color (col.r, col.g, col.b, Easing.EaseInCubic (1f, 0f, i / dur));
				yield return null;
			}

			for (int i = 1; i <= dur; i++) 
			{
				bestTxt.color = new Color (col.r, col.g, col.b, Easing.EaseOutCubic (0f, 1f, i / dur));
				yield return null;
			}
		}
	}
    

}
