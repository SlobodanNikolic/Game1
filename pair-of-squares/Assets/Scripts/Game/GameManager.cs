using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Pokega;


public class GameManager : MonoBehaviour {

    // Use this for initialization
    public int StartMovesToLevelUp = 48;
    public float levelUpFactor = 1.05f;
    public GameObject tilePrefab;
    public GameObject powerUpPrefab;
    public GameObject powUpPopUpPrefab;
    public float playTilesY=-6f;
	public float nextTilesY=-6.5f;
    public float powerUpsY = 6.2f;
    public float playTilesSpacing = 0.6f;
	public float nextTilesSpacing = 0.06f;
    public Image levelProgressBar;
    public int barMaxWidth;
	public UIButton pauseBtn;
	public GameObject multiplierMsgPrefab;
    //public Text scoreText;
    //public GameObject pauseBtn;
    public Color[] colors;
    


    public static GameManager instance;
    public static int score;
	public static float duration;
    public static int bestScore;
    public static int level;
    public static bool newHighscore;
	public static bool gameOver;
	public static bool showLevelUp;
	public static int lastPuType1, lastPuSubtype1, lastPuType2, lastPuSubtype2;

    [HideInInspector] public Tile[] playTiles;
	[HideInInspector] public Tile[] nextTiles;
    [HideInInspector] public Vector3[] playTilesStartPos;
    [HideInInspector] public List<GameObject> powerUpsList;
    [HideInInspector] public int movesToLevelUp;
    [HideInInspector] public bool pause;
	private Vector3[] nextTilesStartPos;
    private Vector3 tileFollowPos;
    private float tileTargetY;
    private int[,] tileBags;
	[HideInInspector] public int[] powerUpBag;
    private int[] drawCount;
    public const float TILE_SPEED= 8f;
    private int[] colorTileCount = new int[Tile.NUM_OF_COLORS];
    private int[] boardTileCount = new int[BoardManager.NUM_OF_BOARDS];
    private Tile[] highlightedTiles = new Tile[BoardManager.NUM_OF_BOARDS];
    private int lastMoveTileCount;
    [HideInInspector] public int numOfMoves;
    [HideInInspector] public int multipliedMove;
    private Vector3 tileStartOffset;
    [HideInInspector] public int multiplier;
	[HideInInspector] public int powDrawCount;
    private int numOfDifferentPowerUps;
    private bool addingPowerUp;
    private bool removingPowerUp;
    private GameObject powerUpPopUp;
    private float scoreY0;
    private float scoreY1;
	public static GameObject container;
	private bool showEraserWarning=true;
   

    void OnDisable()
    {
        Destroy(container);
    }

    void OnEnable () {

        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(this);
        //DontDestroyOnLoad(gameObject);

        if (SoundManager.instance.musicSource.isPlaying)
        {
            StartCoroutine(SoundManager.FadeIn(SoundManager.instance.musicSource, 1f));
        }
        InitTileBags();
        InitPoweUpBag();
        ResetTileCount();
        level = 1;
        movesToLevelUp = StartMovesToLevelUp;
        score = 0;
		duration = 0f;
        App.score.SetScore(0, "Points");
        App.score.SetScore(1, "Level");
        bestScore = SaveAndLoadState.GetBestScore();
        numOfMoves = 0;
        levelProgressBar.rectTransform.sizeDelta = new Vector2(0f, levelProgressBar.rectTransform.sizeDelta.y);
        multiplier = 1;
        numOfDifferentPowerUps = 0;
        addingPowerUp = false;
        removingPowerUp = false;
        pause = false;
        newHighscore = false;
		gameOver = false;
		showLevelUp = false;
		lastPuType1 = -1;
		lastPuSubtype1=-1;
		lastPuType2=-1;
		lastPuSubtype2=-1;
        //scoreY0 = scoreText.transform.position.y;
        //scoreY1 = scoreText.transform.position.y+0.65f;

		tileStartOffset = new Vector3 (0f, 8f, 0f);
        playTiles = new Tile[BoardManager.NUM_OF_BOARDS];
		nextTiles = new Tile[BoardManager.NUM_OF_BOARDS];
        playTilesStartPos = new Vector3[BoardManager.NUM_OF_BOARDS];
		nextTilesStartPos = new Vector3[BoardManager.NUM_OF_BOARDS];

        powerUpsList = new List<GameObject>();

        container = new GameObject();
        container.transform.SetParent(transform);

        for (int i = 0; i < BoardManager.NUM_OF_BOARDS; i++)
        {
            GameObject tile = Instantiate(tilePrefab, new Vector3((i*2-1)*playTilesSpacing,playTilesY,0f),Quaternion.identity) as GameObject;
            tile.transform.SetParent(container.transform);
            BoxCollider2D bc=tile.AddComponent<BoxCollider2D>();
            bc.size=new Vector2(2f,2f);
            if (i==0)
                bc.offset = new Vector2(-playTilesSpacing/2f, 0f);
            else
                bc.offset = new Vector2(playTilesSpacing / 2f, 0f);
			
            tile.AddComponent<DragHandler>();

            playTiles[i] = tile.GetComponent<Tile>();
            tile.transform.Find("Color").gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "PlayTiles";
            tile.transform.Find("Power").gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "PlayTiles";
            tile.transform.Find("Shadow").gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "PlayTiles";
            tile.transform.Find("Highlight").gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "PlayTiles";
            tile.transform.Find("Pattern").gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "PlayTiles";
            playTilesStartPos[i] = tile.transform.position;
            tile.transform.position-=tileStartOffset;
            playTiles[i].UpdateTile(DrawNextTile(i),Tile.P_NONE);

			//next tiles 
			tile = Instantiate(tilePrefab, new Vector3((i*2-1)*nextTilesSpacing,nextTilesY,0f),Quaternion.identity) as GameObject;
			tile.transform.SetParent(container.transform);
			nextTiles[i] = tile.GetComponent<Tile>();
			tile.transform.localScale = Vector3.one * 0.15f;
			nextTiles[i].UpdateStatus (DrawNextTile(i),false,true);
			nextTilesStartPos [i] = tile.transform.position;


        }

        //pauseBtn.transform.position = Camera.main.ViewportToWorldPoint(new Vector3(0.05f,0.97f,10f));


		if (!TitleScreenManager.newGame) 
		{
			SaveAndLoadState.LoadState ();
			if (showLevelUp)
				StartCoroutine(ShowPowerUpPopUp(lastPuType1,lastPuSubtype1,lastPuType2,lastPuSubtype2));

		} 

		if (App.recordGameplay) 
		{
			if (RecordControl.IsPaused ())
				RecordControl.ResumeRecording ();
			else
				RecordControl.StartRecording ();
		}

		MessagePopup.instance.gameObject.SetActive (true);
		if (Tutorial.instance.showTutorial)
			Tutorial.instance.gameObject.SetActive (true);




		//print leveling 

		int l=GameManager.instance.StartMovesToLevelUp;
		int s = 0;
		for (int i = 2; i <= 30; i++) 
		{
			s += l;
			Debug.Log (i +"->" +l + "("+s+")");
			l = Mathf.RoundToInt (l * GameManager.instance.levelUpFactor);

		}

        
	}

    public void DropTiles(Vector3 position)
    {
        Tile[] tiles=new Tile[BoardManager.NUM_OF_BOARDS];
        

        for (int i = 0; i < BoardManager.NUM_OF_BOARDS; i++)
        {
            if (DragHandler.draggedObject.GetComponent<Tile>() == playTiles[i])
            {
                tiles[0] = playTiles[i];
                tiles[1] = playTiles[1 - i];
            }
        }

		if (!Tutorial.instance.showTutorial)
        	SaveAndLoadState.SaveState();
		
        if (!playTiles[0].IsEraser())
        {
            
            if (BoardManager.instance.AddTiles(position, tiles[0], tiles[1]))
            {
                
                StartCoroutine(UpdateLevelProgress());
                SoundManager.instance.PlaySound(SoundManager.instance.dropTileSfx,-1,true);

                for (int i = 0; i < BoardManager.NUM_OF_BOARDS; i++)
                {
					if (!playTiles [i].IsWild ())
						playTiles [i].UpdateStatus (nextTiles[i].status,false,true);

					nextTiles[i].UpdateStatus(DrawNextTile (i),false,true);

					playTiles[i].transform.localScale = nextTiles[i].transform.localScale; //Vector3.one
					playTiles[i].transform.position = nextTiles[i].transform.position;//playTilesStartPos[i] - tileStartOffset;
					StartCoroutine(playTiles[i].toggleOverlay(false));

					nextTiles [i].transform.position -= tileStartOffset;
                }
            }
        }
        else
        {
            int i = BoardManager.GetIFromPosition(position);
            int j = BoardManager.GetJFromPosition(position);
           
            if ((i >= 0) && (i < BoardManager.instance.m) && (j >= 0) && (j < BoardManager.instance.n))
            {
                if (!BoardManager.instance.tileMatrix[0, i, j].IsEmpty())
                {
                    StartCoroutine(UpdateLevelProgress());
                    for (int b = 0; b < BoardManager.NUM_OF_BOARDS; b++)
                    {

                        StartCoroutine(BoardManager.instance.RemoveTile(BoardManager.instance.tileMatrix[b, i, j], 0f));
   

						if (!playTiles [b].IsWild ())
							playTiles [b].UpdateStatus (nextTiles[b].status,false,true);

						nextTiles[b].UpdateStatus(DrawNextTile (b),false,true);

						playTiles[b].transform.localScale = nextTiles[b].transform.localScale; //Vector3.one
						playTiles[b].transform.position = nextTiles[b].transform.position;//playTilesStartPos[i] - tileStartOffset;
						//StartCoroutine(playTiles[i].toggleOverlay(false));


						nextTiles [b].transform.position -= tileStartOffset;

                    }
                }
            }
        }
        
    }

    void InitTileBags()
    {

        tileBags = new int[BoardManager.NUM_OF_BOARDS, Tile.NUM_OF_COLORS];
        drawCount = new int[BoardManager.NUM_OF_BOARDS];
        for (int i = 0; i < BoardManager.NUM_OF_BOARDS; i++)
        {
            drawCount[i] = 0;
            for (int j = 0; j < Tile.NUM_OF_COLORS; j++)
            {
                tileBags[i, j] = j;
            }
            ShuffleTileBag(i);
        }
      
    }

    void InitPoweUpBag()
    {
        powerUpBag = new int[PowerUp.NUM_OF_TYPES+1];
        powDrawCount = 0;
        for (int i = 0; i < PowerUp.NUM_OF_TYPES; i++)
        {
            powerUpBag[i] = i;
        }
        powerUpBag[PowerUp.NUM_OF_TYPES] = PowerUp.X2;
        ShufflePoweUpBag();
    }

    void ShuffleTileBag(int bag)
    {
        int p,rand;

        for (int j = 0; j < Tile.NUM_OF_COLORS; j++)
        {
            rand = Random.Range(0, Tile.NUM_OF_COLORS);
            p = tileBags[bag, j];
            tileBags[bag, j] = tileBags[bag, rand];
            tileBags[bag, rand] = p;
        }
        
    }

    void ShufflePoweUpBag()
    {
        int p, rand;
        for (int i = 0; i < powerUpBag.Length; i++)
        {
            rand = Random.Range(0, powerUpBag.Length);
            p = powerUpBag[rand];
            powerUpBag[rand] = powerUpBag[i];
            powerUpBag[i] = p;
        }

        for (int i = 0; i < powerUpBag.Length; i += 2)
        {
            if (powerUpBag[i] == powerUpBag[i + 1])
            {
                do
                {
                    rand = Random.Range(0, powerUpBag.Length);
                }
                while (powerUpBag[rand] == powerUpBag[i]);
                p = powerUpBag[rand];
                powerUpBag[rand] = powerUpBag[i];
                powerUpBag[i] = p;
                i = 0;
            }
        }
    }

    int DrawNextTile(int bag)
    {
		int t;
		if (!Tutorial.instance.showTutorial) {
			t = tileBags [bag, drawCount [bag]];
			drawCount [bag]++;
			if (drawCount [bag] == Tile.NUM_OF_COLORS) {
				drawCount [bag] = 0;
				ShuffleTileBag (bag);
			}
		} 
		else 
		{
			t=Tutorial.instance.GetPlayTile(bag, drawCount [bag]);
			drawCount [bag]++;
			if (drawCount[bag] == Tutorial.instance.playTiles.Length)
				drawCount [bag] = 0;
		}
        return t;
    }

    int DrawNextPowerUp()
    {
        int t = powerUpBag[powDrawCount];
        powDrawCount++;
        if (powDrawCount == powerUpBag.Length - (powerUpBag.Length % 2))
        {
            powDrawCount = 0;
            ShufflePoweUpBag();
        }
        return t;
    }

    public void AddPowers(int b, int i, int j, int vCount, int hCount)
    {
        if (vCount+hCount<4)
        {
            playTiles[b].UpdatePower(Tile.P_NONE);
            return;
        }
        StartCoroutine(LineTrace.DrawCurve(BoardManager.instance.tileMatrix[b,i,j].transform.position, playTilesStartPos[b], new Color(1f,1f,0.8f,1f),0.2f));
        SoundManager.instance.PlaySound(SoundManager.instance.specialTileLineSfx,0.4f);

		if (!Tutorial.instance.showTutorial)
			App.ach.SetAchievement ("Exploding tile");

		App.analytics.CreateAnalyticEvent ("Exploding tile: " + vCount + "x" + hCount);
		
        if ((vCount == 4) && (hCount < 3))
        {
            playTiles[b].UpdatePower(Tile.P_ROW);
            return;
        }
        if ((vCount < 3) && (hCount == 4))
        {
            playTiles[b].UpdatePower(Tile.P_COL);
            return;
        }
        if ((vCount == 5) && (hCount < 3))
        {
            playTiles[b].UpdatePower(Tile.P_COL_COL);
            return;
        }
        if ((vCount < 3) && (hCount == 5))
        {
            playTiles[b].UpdatePower(Tile.P_ROWCOL);
            return;
        }
        if ((vCount == 3) && (hCount == 3))
        {
            playTiles[b].UpdatePower(Tile.P_BOMB);
            return;
        }
        if (((vCount == 4) && (hCount == 3)) || ((vCount==3) && (hCount==4)))
        {
            playTiles[b].UpdatePower(Tile.P_BIGBOMB);
            return;
        }
        if ((vCount == 4) && (hCount == 4))
        {
            playTiles[b].UpdatePower(Tile.P_ROWCOL_COL);
            return;
        }
        if ((vCount == 5) && (hCount == 3))
        {
            playTiles[b].UpdatePower(Tile.P_ROWROWROW);
            return;
        }
        if ((vCount == 3) && (hCount == 5))
        {
            playTiles[b].UpdatePower(Tile.P_COLCOLCOL);
            return;
        }
        if (((vCount == 5) && (hCount == 4)) || ((vCount == 4) && (hCount == 5)))
        {
            playTiles[b].UpdatePower(Tile.P_ROWROWROWCOLCOLCOL);
            return;
        }
        if ((vCount == 5) && (hCount == 5))
        {
            playTiles[b].UpdatePower(Tile.P_COLCOLCOL_COLCOLCOL);
            return;
        }

    }

    public void EndGame()
    {
		gameOver = true;
		SaveAndLoadState.SaveState (true,false);
        SoundManager.instance.PlaySound(SoundManager.instance.gameOverSfx);
        OutLine.RemoveAllLines();
        //StartCoroutine(SoundManager.FadeOut(SoundManager.instance.musicSource, 0.25f));
        Game.instance.GameOver();
    }

    public void PauseGame()
    {
        if (BoardManager.instance.BoardIsFull())
            return;

        pause = true;
        OutLine.RemoveAllLines();
		if (!Tutorial.instance.showTutorial)
			SaveAndLoadState.SaveState (true);

		StartCoroutine(MessagePopup.instance.HideMessage ());
    
    }

	void OnApplicationPause(bool pauseStatus)
	{
		if ((pauseStatus) && (RecordControl.IsRecording()))
			RecordControl.PauseRecording ();

		if ((!pauseStatus) && (RecordControl.IsPaused()))
			RecordControl.ResumeRecording ();
		
		if ((!Tutorial.instance.showTutorial) && (pauseStatus) && (!LineTrace.lineIsActive))
			SaveAndLoadState.SaveState(true);
	}

    public void UpdateTileCount(Tile tile)
    {
		App.score.ScorePlus(multiplier,"Points");
		score=(int) App.score.GetScore("Points");
		if ((score >= 100) && (score - multiplier < 100))
			App.ach.SetAchievement ("Score 100");
		
		if ((score >= 200) && (score - multiplier < 200))
			App.ach.SetAchievement ("Score 200");

		if ((score >= 500) && (score - multiplier < 500))
			App.ach.SetAchievement ("Score 500");

		if ((score >= 1000) && (score - multiplier < 1000))
			App.ach.SetAchievement ("Score 1000");

		if ((score >= 2000) && (score - multiplier < 2000))
			App.ach.SetAchievement ("Score 2000");
		
        
        if (score > bestScore)
        {
            bestScore = score;
            newHighscore = true;
        }
        lastMoveTileCount++;

		if (lastMoveTileCount==10)
			App.ach.SetAchievement ("Tiles 10");

		if (lastMoveTileCount==20)
			App.ach.SetAchievement ("Tiles 20");

		if (lastMoveTileCount==40)
			App.ach.SetAchievement ("Tiles 40");
			

        if (boardTileCount[tile.b]>0)
            boardTileCount[tile.b]--;

        if (tile.IsColored())
        {
            if (colorTileCount[tile.status] > 0)
            {
                colorTileCount[tile.status]--;
            }
        }
        if (tile.IsWild())
        {
            for (int i = 0; i < Tile.NUM_OF_COLORS; i++)
            {
                if (colorTileCount[i] > 0)
                {
                    colorTileCount[i]--;
                }
            }
        }
        //scoreText.text=""+score;
    }

    public IEnumerator DecreasePoints(int inc)
    {
        //Color oldTextColor = scoreText.color;
        //scoreText.color = new Color(222f/255f,58f/255f,69f/255f);
        for (int i = 1; i <= inc; i++)
        {
            if (score > 0)
                score -= 1;
            else
                break;

            //scoreText.text = "" + score;
            App.score.ScoreMinus(1, "Points");

            yield return null;
            yield return null;
        }
        //.color = oldTextColor;

    }

    public IEnumerator UpdateLevelProgress(int inc = 1)
    {
        lastMoveTileCount = 0;
		showEraserWarning = true;
		bool timeTolevelUp = false;
        if (numOfMoves != multipliedMove)
        {
            multiplier = 1;
            multipliedMove = -1;
        }
        float oldBarXScale = numOfMoves*1f / movesToLevelUp;
        numOfMoves += inc;
		if (numOfMoves >= movesToLevelUp) 
		{
				pause = true;
				showLevelUp = true;
				//pauseBtn.isEnabled = false;
				timeTolevelUp = true;
		}

        int dur = 20;
        float newBarXScale= numOfMoves*1f / movesToLevelUp;
        OutLine.RemoveAllLines();
        Vector3 scale = Vector3.one;
        for (int i = 1; i <= dur; i++)
        {
            scale.x = Easing.EaseOutCubic(oldBarXScale, newBarXScale, i * 1f / dur);
            //levelProgressBar.transform.localScale = scale;
            levelProgressBar.rectTransform.sizeDelta = new Vector2(scale.x * barMaxWidth, levelProgressBar.rectTransform.sizeDelta.y);
            yield return null;
        }
       
		if (timeTolevelUp)
		{
            
			if (BoardManager.instance.BoardIsFull ()) 
			{
				yield return new WaitForSeconds(0.5f);
			}

			if (BoardManager.instance.BoardIsFull ()) 
			{
				yield break;
			}

				dur = 35;
				Invoke ("LevelUp", 0.5f);
				for (int i = 0; i < dur; i++) {
					//scale.x = 1f - (Mathf.FloorToInt(i / 10) % 2);
					//levelProgressBar.transform.localScale = scale;
					if ((Mathf.FloorToInt (i / 5) % 2) == 0) {
						levelProgressBar.color = new Color (1f, 1f, 1f, Easing.EaseOutCubic (1f, 0f, (i % 5) / 5f));
					} else {
						levelProgressBar.color = new Color (1f, 1f, 1f, Easing.EaseOutCubic (0f, 1f, (i % 5) / 5f));
					}
					yield return null;
				}
				levelProgressBar.color = Color.white;
				scale.x = 0f;
				levelProgressBar.rectTransform.sizeDelta = new Vector2 (scale.x * barMaxWidth, levelProgressBar.rectTransform.sizeDelta.y);
				numOfMoves = 0;
			
		}
       
    }

	public string PlayTilesToBombs()
    {
        if ((!playTiles[0].IsEraser()) && ((playTiles[0].power != Tile.P_BOMB) || (playTiles[1].power != Tile.P_BOMB)))
        {
            for (int b = 0; b < BoardManager.NUM_OF_BOARDS; b++)
            {
                playTiles[b].UpdatePower(Tile.P_BOMB);
            }
            return "OK";
        }

		if (playTiles[0].IsEraser())
			return "Erasers are already activated.";

		if ((playTiles[0].power == Tile.P_BOMB) && (playTiles[1].power == Tile.P_BOMB))
			return "Bombs are already activated.";

		return "You can't use this power up."; 
    }

	public string PlayTilesToWilds()
    {
        if ((!playTiles[0].IsEraser()) && ((!playTiles[0].IsWild()) || (!playTiles[1].IsWild())))
        {
            for (int b = 0; b < BoardManager.NUM_OF_BOARDS; b++)
            {
                playTiles[b].UpdateStatus(Tile.WILD);
            }
            return "OK";
        }

		if (playTiles[0].IsEraser()) 
			return "Erasers are already activated.";
		
		if ((playTiles[0].IsWild()) && (playTiles[1].IsWild()))
			return "Wild cards are already activated.";


		return "You can't use this power up."; 
    }

	public string DoubleMultiplier(bool showAnim=true)
    {
        multipliedMove = numOfMoves;
        multiplier *= 2;
        float x = 1.2f*3f + Mathf.Log(multiplier,2) * 0.12f;
        StartCoroutine(OutLine.DrawLine(new Vector3(-x, -6.36f), new Vector3(-x, 6.22f), new Color(48f / 255f, 216f / 255f, 149f / 255f), 0.08f));
        StartCoroutine(OutLine.DrawLine(new Vector3(x, -6.36f), new Vector3(x, 6.22f), new Color(48f / 255f, 216f / 255f, 149f / 255f), 0.08f));

		if (showAnim) 
		{
			SoundManager.instance.PlaySoundWithPitch (SoundManager.instance.multiSfx, 1f + (Mathf.Log (multiplier, 2) - 1) * 0.1f);
			StartCoroutine (ShowMultiplierMsg ());
		}

		if (multiplier == 4)
			App.ach.SetAchievement ("Multiplier 4");
		if (multiplier == 8)
			App.ach.SetAchievement ("Multiplier 8");
		if (multiplier == 16)
			App.ach.SetAchievement ("Multiplier 16");
		
        return "OK";
    }

	private IEnumerator ShowMultiplierMsg()
	{
		GameObject mm = Instantiate (multiplierMsgPrefab, Vector3.zero, Quaternion.identity) as GameObject;
		mm.transform.SetParent (container.transform);
		mm.transform.Find ("Canvas").gameObject.transform.Find ("msg").GetComponent<Text> ().text = "x" + multiplier;
		mm.transform.Find ("bg").gameObject.GetComponent<SpriteRenderer> ().sortingOrder = multiplier;
		mm.transform.Find ("Canvas").gameObject.GetComponent<Canvas> ().sortingOrder = multiplier+1;
		mm.transform.localScale = Vector3.zero;
		float dur = 15;
		for (int i = 1; i <= dur; i++) 
		{
			mm.transform.localScale = Easing.EaseOutBack (0f,1f,i/dur) * Vector3.one;
			yield return null;
		}

		dur = 20;
		for (int i = 1; i <= dur; i++) 
		{
			yield return null;
		}

		dur = 15;
		for (int i = 1; i <= dur; i++) 
		{
			mm.transform.localScale = Easing.EaseInBack (1f,0f,i/dur) * Vector3.one;
			yield return null;
		}

		Destroy (mm);
	}

	public string PlayTilesToErasers()
    {
		if (showEraserWarning)
		{
			if ((playTiles [0].power != Tile.P_NONE) || (playTiles [1].power != Tile.P_NONE)) 
			{
				showEraserWarning = false;
				return "Be careful. You'll lose your exploding tiles.";
			}

			if ((playTiles [0].status == Tile.WILD) || (playTiles [1].power == Tile.WILD)) 
			{
				showEraserWarning = false;
				return "Be careful. You'll lose your wild cards.";
			}
		}

        if ((!playTiles[0].IsEraser()) && (!BoardManager.instance.BoardIsEmpty()))
        {
            for (int b = 0; b < BoardManager.NUM_OF_BOARDS; b++)
            {
                playTiles[b].UpdateTile(Tile.ERASER, Tile.P_NONE, true);
            }
			SoundManager.instance.PlaySound (SoundManager.instance.erasersSfx);
            return "OK";
        }
		if (playTiles[0].IsEraser())
		        return "Erasers are already activated.";
		
		if (BoardManager.instance.BoardIsEmpty ())
			return "The board is empty.";
		
		return "You can't use this power up."; 
    }

    public void SkipTiles()
    {
        for (int i = 0; i < BoardManager.NUM_OF_BOARDS; i++)
        {
            if (!playTiles[i].IsWild())
                playTiles[i].UpdateStatus(DrawNextTile(i));

            playTiles[i].transform.localScale = Vector3.one;
            playTiles[i].transform.position = playTilesStartPos[i] - tileStartOffset;
        }
    }

    private void ResetTileCount()
    {
        for (int i = 0; i < BoardManager.NUM_OF_BOARDS; i++)
            boardTileCount[i] = 0;

        for (int i = 0; i < Tile.NUM_OF_COLORS; i++)
            colorTileCount[i] = 0;

        lastMoveTileCount = 0;
    }


    private void LevelUp()
    {
        movesToLevelUp = Mathf.RoundToInt(movesToLevelUp*levelUpFactor);
        level++;
		if (level == 2)
			App.ach.SetAchievement ("Level 2");
		if (level == 3)
			App.ach.SetAchievement ("Level 3");
		if (level == 5)
			App.ach.SetAchievement ("Level 5");
		if (level == 10)
			App.ach.SetAchievement ("Level 10");


        App.score.ScorePlus(1, "Level");

		lastPuType1 = DrawNextPowerUp();
		lastPuType2 = DrawNextPowerUp();
		lastPuSubtype1 = 0;
		lastPuSubtype2 = 0;

		if (lastPuType1 == PowerUp.DESTROY_COLOR)
			lastPuSubtype1 = Random.Range(0,Tile.NUM_OF_COLORS);
		if (lastPuType1 == PowerUp.CLEAR_BOARD)
			lastPuSubtype1 = Random.Range(0, BoardManager.NUM_OF_BOARDS);

		if (lastPuType2 == PowerUp.DESTROY_COLOR)
			lastPuSubtype2 = Random.Range(0, Tile.NUM_OF_COLORS);
		if (lastPuType2 == PowerUp.CLEAR_BOARD)
			lastPuSubtype2 = Random.Range(0, BoardManager.NUM_OF_BOARDS);

		StartCoroutine(ShowPowerUpPopUp(lastPuType1,lastPuSubtype1,lastPuType2,lastPuSubtype2));
        SoundManager.instance.PlaySound(SoundManager.instance.levelUpSfx);
    }

    // Update is called once per frame


    public IEnumerator AddPowerUp(int type, int subtype = 0, float wait = 0f, bool anim=true)
    {
        yield return new WaitForSeconds(wait);
        while (addingPowerUp || removingPowerUp)
        {
            yield return null;
        }
        addingPowerUp = true;
        bool newPU = true;
        int oldPUIndex = -1;
        int numOfCopies = 0;
        int curNumOfDiff = 0;
        float space = 1.35f;
		float maxScale=1f;
		float currScale;
        float[] oldX = new float[powerUpsList.Count];
        float[] newX = new float[powerUpsList.Count];
		float stackH = 0.15f;

		if (numOfDifferentPowerUps > 4) 
		{
			space = 1f;
			maxScale = 0.9f;
		}

		if (powerUpsList.Count == 0)
			currScale = 1f;
		else
			currScale = powerUpsList[0].transform.localScale.x;
        
        for (int i = 0; i < powerUpsList.Count; i++)
        {
            if (powerUpsList[i].GetComponent<PowerUp>().type == type)
            {
                if (newPU)
                {
                    oldPUIndex = i;
                    newPU = false;
                }
                numOfCopies++;
            }
            if (i > 0)
            {
                if (powerUpsList[i].GetComponent<PowerUp>().type != powerUpsList[i - 1].GetComponent<PowerUp>().type)
                    curNumOfDiff++;
            }
            else
            {
                curNumOfDiff++;
            }
            oldX[i] = powerUpsList[i].transform.position.x;
            newX[i] = (curNumOfDiff - 1) * space - (numOfDifferentPowerUps * space / 2f);
        }




        if (newPU)
        {
            numOfDifferentPowerUps++;
            Vector3 pos = new Vector3(curNumOfDiff * space / 2f, powerUpsY, 0f);
            GameObject pu = Instantiate(powerUpPrefab, pos, Quaternion.identity) as GameObject;
            pu.transform.SetParent(container.transform);
            pu.GetComponent<PowerUp>().UpdatePowerUp(type, subtype);
            powerUpsList.Add(pu);

            int dur=15;
            if (!anim)
                dur = 1; 
            for (int curFr = 1; curFr <= dur; curFr++)
            {
                for (int i = 0; i < powerUpsList.Count - 1; i++)
                {
                    powerUpsList[i].transform.position = new Vector3(Easing.EaseOutCubic(oldX[i], newX[i], curFr * 1f / dur), powerUpsList[i].transform.position.y, 0f);
					powerUpsList[i].transform.localScale = Vector3.one*Easing.EaseOutCubic(currScale,maxScale, curFr * 1f / dur);
                }

				powerUpsList[powerUpsList.Count - 1].transform.localScale = Vector3.one * Easing.EaseInCubic(0f, maxScale, curFr * 1f / dur);
                //if (powerUpsList.Count == 1)
                //{
                    //scoreText.transform.position = new Vector3(scoreText.transform.position.x, Easing.EaseOutCubic(scoreY0, scoreY1, curFr * 1f / dur), 0f);
                //}
                if (anim)
                    yield return null;
            }
        }
        else
        {


            Vector3 pos = powerUpsList[oldPUIndex].transform.position;
            GameObject pu = Instantiate(powerUpPrefab, pos, Quaternion.identity) as GameObject;
            pu.GetComponent<PowerUp>().UpdatePowerUp(type, subtype);
            pu.transform.SetParent(container.transform);
			pu.transform.localScale = Vector3.one * currScale;

            for (int i = oldPUIndex; i < oldPUIndex + numOfCopies; i++)
            {
				float newY =powerUpsY+stackH*(i - oldPUIndex + 1);
                powerUpsList[i].transform.position=new Vector3(powerUpsList[i].transform.position.x, newY, 0f);
                powerUpsList[i].transform.Find("BG").gameObject.GetComponent<SpriteRenderer>().color = powerUpsList[i].GetComponent<PowerUp>().GetBGColor(i- oldPUIndex+1);
                powerUpsList[i].GetComponent<PowerUp>().ChangeSortingLayer(false,-i);
            }

            powerUpsList.Insert(oldPUIndex,pu);
        }
        addingPowerUp = false;
        yield break;
    }
    public IEnumerator RemovePowerUp(GameObject pu, float wait = 0f)
    {
        yield return new WaitForSeconds(wait);
        while (addingPowerUp || removingPowerUp)
        {
            yield return null;
        }
        removingPowerUp = true;

        int type = pu.GetComponent<PowerUp>().type;
        float[] oldX = new float[powerUpsList.Count];
        float[] newX = new float[powerUpsList.Count];
        float space = 1.35f;
		float maxScale = 1f;
        bool hasCopies = false;
        int numOfCopies=0;
		float currScale;
        int firstCopieIndex = -1;
        int curNumOfDiff = 0;
		float stackH = 0.15f;

		if (numOfDifferentPowerUps > 6) 
		{
			space = 1f;
			maxScale = 0.9f;
		}

		currScale = powerUpsList[0].transform.localScale.x;

        powerUpsList.Remove(pu);

        for (int i = 0; i < powerUpsList.Count; i++)
        {
            if (powerUpsList[i].GetComponent<PowerUp>().type == type)
            {
                if (!hasCopies)
                {
                    firstCopieIndex = i;
                    hasCopies = true;
                }
                numOfCopies++;
            }
        }
        if (!hasCopies)
            numOfDifferentPowerUps--;

        for (int i = 0; i < powerUpsList.Count; i++)
        {
            if (i > 0)
            {
                if (powerUpsList[i].GetComponent<PowerUp>().type != powerUpsList[i - 1].GetComponent<PowerUp>().type)
                    curNumOfDiff++;
            }
            else
            {
                curNumOfDiff++;
            }
            oldX[i] = powerUpsList[i].transform.position.x;
            newX[i] = (curNumOfDiff - 1) * space - ((numOfDifferentPowerUps-1) * space / 2f);
        }




		

        if (!hasCopies)
        {
            int dur = 15;
            for (int curFr = 1; curFr <= dur; curFr++)
            {
                pu.transform.localScale = Vector3.one * Easing.EaseInBack(1f, 0f, curFr * 1f / dur);
                yield return null;
            }
            /*if (powerUpsList.Count == 0)
            {
                yield return new WaitForSeconds(0.5f);
                for (int curFr = 1; curFr <= dur; curFr++)
                {
                    //scoreText.transform.position = new Vector3(scoreText.transform.position.x, Easing.EaseOutCubic(scoreY1, scoreY0, curFr * 1f / dur), 0f);
                    yield return null;
                }
            }*/

            for (int curFr = 1; curFr <= dur; curFr++)
            {
                for (int i = 0; i < powerUpsList.Count; i++)
                {
                    powerUpsList[i].transform.position = new Vector3(Easing.EaseOutCubic(oldX[i], newX[i], curFr * 1f / dur), powerUpsList[i].transform.position.y, 0f);
					powerUpsList[i].transform.localScale = Vector3.one*Easing.EaseOutCubic(currScale,maxScale, curFr * 1f / dur);
                }
                yield return null;
            }

        }
        else
        {
            int dur = 15;
            for (int curFr = 1; curFr <= dur; curFr++)
            {
				pu.transform.localScale = Vector3.one * Easing.EaseInBack(maxScale, 0f, curFr * 1f / dur);
                yield return null;
            }
           

            for (int i = firstCopieIndex; i < firstCopieIndex + numOfCopies; i++)
            {
				float newY = powerUpsY + stackH * (i - firstCopieIndex);
                powerUpsList[i].transform.position = new Vector3(powerUpsList[i].transform.position.x, newY, 0f);
                powerUpsList[i].transform.Find("BG").gameObject.GetComponent<SpriteRenderer>().color = powerUpsList[i].GetComponent<PowerUp>().GetBGColor(i - firstCopieIndex);
                powerUpsList[i].GetComponent<PowerUp>().ChangeSortingLayer(false,-i);
            }
            powerUpsList[firstCopieIndex].GetComponent<PowerUp>().ChangeSortingLayer(true);
            Color oldColor = powerUpsList[firstCopieIndex].GetComponent<PowerUp>().GetBGColor(1);
            Color newColor = powerUpsList[firstCopieIndex].GetComponent<PowerUp>().bgColors[0];
            Color midColor;
            dur = 20;
            for (int curFr = 1; curFr <= dur; curFr++)
            {
                midColor = new Color(Easing.Linear(oldColor.r, newColor.r, curFr * 1f / dur), 
                            Easing.Linear(oldColor.g, newColor.g, curFr * 1f / dur), 
                            Easing.Linear(oldColor.b, newColor.b, curFr * 1f / dur));
                powerUpsList[firstCopieIndex].transform.Find("BG").gameObject.GetComponent<SpriteRenderer>().color = midColor;
                yield return null;
            }
            
        }
        Destroy(pu);
        removingPowerUp = false;
        yield break;

    }

    public void RemoveAllPowerUps()
    {
        GameObject pu;
        while(powerUpsList.Count>0)
        {
            pu = powerUpsList[0];
            powerUpsList.RemoveAt(0);
            Destroy(pu);
        }
        numOfDifferentPowerUps = 0;

    }

    public IEnumerator PickPowerUp(GameObject go)
    {
        pause = false;
        //go.transform.parent = null;
        //go.transform.Find("Circle").gameObject.GetComponent<SpriteRenderer>().color = Color.clear;
		SoundManager.instance.PlayClickSound();
        PowerUp p = go.GetComponent<PowerUp>();
		App.analytics.CreateAnalyticEvent ("Power up: " + PowerUp.GetName(p.type));
        GameObject window = powerUpPopUp.transform.Find("Window").gameObject;
        GameObject eye = powerUpPopUp.transform.Find("Eye").gameObject;
        GameObject overlay = powerUpPopUp.transform.Find("BG").gameObject.transform.Find("Overlay").gameObject;
        GameObject[] transLines = new GameObject[4];
        for (int i = 0; i < 4; i++)
        {
            transLines[i] = powerUpPopUp.transform.Find("Transition").gameObject.transform.Find("Line" + (i + 1)).gameObject;
        }

        Color overlayColor = overlay.GetComponent<SpriteRenderer>().color;
        Vector3 eyeStPos = eye.transform.position;
        Vector3 eyeEndPos = eye.transform.position + Vector3.down * 5f;
        float goStScale = go.transform.localScale.x;
        float tLineX = transLines[0].transform.position.x;
		float windowStY = window.transform.position.y;
		//window.transform.localScale = Vector3.zero;
		float windowEndY = window.transform.position.y+15f;
        int dur = 20;
        for (int i = 1; i <= dur; i++)
        {
            for (int l = 0; l < transLines.Length; l++)
            {
                if ((i - l * 5) > 0)
                    transLines[l].transform.position = new Vector3(Easing.EaseOutCubic(tLineX, 0f, (i - l * 5) * 1f / (dur - l * 5)), transLines[l].transform.position.y, 0f);
            }
            go.transform.localScale = Vector3.one * Easing.EaseInBack(goStScale,0f, 1f * i / dur);
            eye.transform.position = new Vector3(eyeEndPos.x, Easing.EaseOutCubic(eyeStPos.y, eyeEndPos.y, 1f * i / dur), eyeEndPos.z);
            overlay.GetComponent<SpriteRenderer>().color = new Color(overlayColor.r, overlayColor.g, overlayColor.b, Easing.EaseOutCubic(overlayColor.a,0f, 1f * i / dur));

            yield return null;
        }
        //window.transform.localScale = Vector3.zero;
        for (int i = 1; i <= dur; i++)
        {
            for (int l = 0; l < transLines.Length; l++)
            {
                if ((i - l * 5) > 0)
                    transLines[l].transform.position = new Vector3(Easing.EaseOutCubic(0f, -tLineX, (i - l * 5) * 1f / (dur - l * 5)), transLines[l].transform.position.y, 0f);
            }
			window.transform.position=new Vector3(window.transform.position.x,Easing.EaseInBack(windowStY,windowEndY,1f*i/dur),0f);
            yield return null;
        }


        for (int i=0;i<p.quantity;i++)
        {
            StartCoroutine(AddPowerUp(p.type, p.subtype,i*0.5f));
        }
        Destroy(powerUpPopUp);
		//pauseBtn.isEnabled = true;
		showLevelUp = false;
		//Game.buttonsEnabled = true;
        yield break;
    }

    public IEnumerator ShowPowerUpPopUp(int type1,int subtype1,int type2,int subtype2)
    {
		//pauseBtn.isEnabled = false;
		//Game.buttonsEnabled = false;
        powerUpPopUp = Instantiate(powUpPopUpPrefab, Vector3.zero, Quaternion.identity) as GameObject;
        powerUpPopUp.transform.SetParent(container.transform);
        GameObject window= powerUpPopUp.transform.Find("Window").gameObject;
        GameObject levelUpAnim= powerUpPopUp.transform.Find("LevelUpAnim").gameObject;
        GameObject eye = powerUpPopUp.transform.Find("Eye").gameObject;
        GameObject overlay = powerUpPopUp.transform.Find("BG").gameObject.transform.Find("Overlay").gameObject;
        GameObject[] transLines = new GameObject[4];
        for (int i = 0; i < 4; i++)
        {
            transLines[i]= powerUpPopUp.transform.Find("Transition").gameObject.transform.Find("Line"+(i+1)).gameObject;
        }
        GameObject p1 = window.transform.Find("PowerUp1").gameObject;
        GameObject p2 = window.transform.Find("PowerUp2").gameObject;
        p1.GetComponent<PowerUp>().UpdatePowerUp(type1, subtype1);
        p2.GetComponent<PowerUp>().UpdatePowerUp(type2, subtype2);
        window.transform.Find("Canvas").gameObject.transform.Find("PUName1").GetComponent<Text>().text=PowerUp.GetName(type1);
        window.transform.Find("Canvas").gameObject.transform.Find("PUName2").GetComponent<Text>().text = PowerUp.GetName(type2);
        window.transform.Find("Canvas").gameObject.transform.Find("PUDesc1").GetComponent<Text>().text = PowerUp.GetDescription(type1,subtype1);
        window.transform.Find("Canvas").gameObject.transform.Find("PUDesc2").GetComponent<Text>().text = PowerUp.GetDescription(type2,subtype2);


		float separation = 0.4f;
		if (type1 == PowerUp.ERASER) 
		{
			p1.transform.position += Vector3.left * separation/2f;
			p1.transform.Find("Other").gameObject.transform.position += Vector3.right * separation;
		}
		if (type2 == PowerUp.ERASER) 
		{
			p2.transform.position += Vector3.left * separation/2f;
			p2.transform.Find("Other").gameObject.transform.position += Vector3.right * separation;
		}

        Color overlayColor = overlay.GetComponent<SpriteRenderer>().color;
        Vector3 eyeEndPos = eye.transform.position;
        Vector3 eyeStPos = eye.transform.position + Vector3.down*5f;
        eye.transform.position = eyeStPos;
        float tLineX = transLines[0].transform.position.x;
		float windowEndY = window.transform.position.y;
        //window.transform.localScale = Vector3.zero;
		window.transform.position+=Vector3.up*15f;
		float windowStY = window.transform.position.y;

        int dur = 60;

        for (int i = 1; i <= dur; i++)
        {
            overlay.GetComponent<SpriteRenderer>().color = new Color(overlayColor.r, overlayColor.g, overlayColor.b, Easing.EaseOutCubic(0f, overlayColor.a, 1f * i / dur));
            yield return null;
        }

        dur = 20;
        for (int i = 1; i <= dur; i++)
        {
            for (int l = 0; l < transLines.Length; l++)
            {
                if ((i - l * 5)>0)
                    transLines[l].transform.position = new Vector3(Easing.EaseOutCubic(tLineX, 0f,(i-l*5)*1f/(dur-l*5)), transLines[l].transform.position.y,0f);
            }
            eye.transform.position=new Vector3(eyeEndPos.x,Easing.EaseOutCubic(eyeStPos.y, eyeEndPos.y, 1f * i / dur),eyeEndPos.z);
			window.transform.position=new Vector3(window.transform.position.x,Easing.EaseOutBack(windowStY,windowEndY,1f*i/dur),0f);
            yield return null;
        }
        window.transform.localScale = Vector3.one;
        levelUpAnim.transform.localScale = Vector3.zero;
        for (int i = 1; i <= dur; i++)
        {
            for (int l = 0; l < transLines.Length; l++)
            {
                if ((i - l * 5) > 0)
                    transLines[l].transform.position = new Vector3(Easing.EaseOutCubic(0f, -tLineX,(i - l * 5) * 1f / (dur - l * 5)), transLines[l].transform.position.y, 0f);
            }
            yield return null;
        }
        yield break;
    }

    void Update ()
    {
		duration += Time.deltaTime;
        if (DragHandler.draggedObject != null)
        {
            for (int i = 0; i < BoardManager.NUM_OF_BOARDS; i++)
            {
                playTiles[i].transform.localScale = Vector3.one*0.9f;
                playTiles[i].toggleShadow(true);

                if (DragHandler.draggedObject.GetComponent<Tile>() == playTiles[i])
                {

                    if (playTiles[i].transform.position.y < BoardManager.startY - BoardManager.TILE_SIZE)
                        tileTargetY = playTiles[i].transform.position.y + BoardManager.boardYOffset;
                    else
                        tileTargetY = playTiles[i].transform.position.y - BoardManager.boardYOffset;

                    tileFollowPos = playTiles[1-i].transform.position;
                    tileFollowPos.y += (tileTargetY - tileFollowPos.y) * TILE_SPEED * Time.deltaTime;
                    tileFollowPos.x += (playTiles[i].transform.position.x - tileFollowPos.x) * TILE_SPEED * Time.deltaTime;
                    playTiles[1-i].transform.position = tileFollowPos;

                    
                }

                int tB = BoardManager.GetBFromPosition(playTiles[i].transform.position);
                int tI = BoardManager.GetIFromPosition(playTiles[i].transform.position);
                int tJ = BoardManager.GetJFromPosition(playTiles[i].transform.position);
                
				if ((tI > -1) && (tJ > -1) && (tI < BoardManager.instance.m) && (tJ < BoardManager.instance.n) && (!Tutorial.instance.showTutorial))
                {
                    if ((highlightedTiles[i] != null) && (highlightedTiles[i] != BoardManager.instance.tileMatrix[tB, tI, tJ]))
                        StartCoroutine(highlightedTiles[i].toggleHighlight(false));

                    if (!playTiles[0].IsEraser())
                    {
                        if ((BoardManager.instance.tileMatrix[tB, tI, tJ].IsEmpty()) && (highlightedTiles[i] != BoardManager.instance.tileMatrix[tB, tI, tJ]))
                        {
                            highlightedTiles[i] = BoardManager.instance.tileMatrix[tB, tI, tJ];
                            StartCoroutine(highlightedTiles[i].toggleHighlight(true));
                        }
                        if (!BoardManager.instance.tileMatrix[tB, tI, tJ].IsEmpty())
                        {
                            highlightedTiles[i] = null;
                        }
                    }
                    else
                    {
                        if ((!BoardManager.instance.tileMatrix[tB, tI, tJ].IsEmpty()) && (highlightedTiles[i] != BoardManager.instance.tileMatrix[tB, tI, tJ]))
                        {
                            highlightedTiles[i] = BoardManager.instance.tileMatrix[tB, tI, tJ];
                            StartCoroutine(highlightedTiles[i].toggleHighlight(true));
                        }
                        if (BoardManager.instance.tileMatrix[tB, tI, tJ].IsEmpty())
                        {
                            highlightedTiles[i] = null;
                        }
                    }
                }
                else
                {
                    if (highlightedTiles[i] != null)
                        StartCoroutine(highlightedTiles[i].toggleHighlight(false));
                    highlightedTiles[i] = null;
                }
            }
        }
        else
        {

            for (int i = 0; i < BoardManager.NUM_OF_BOARDS; i++)
            {
                if (Vector3.Distance(playTiles[i].transform.position,playTilesStartPos[i])>0.001f)
                {
                    playTiles[i].transform.position+=(playTilesStartPos[i]-playTiles[i].transform.position) *TILE_SPEED * Time.deltaTime;
                }

				if (Mathf.Abs (playTiles [i].transform.localScale.x - 1f) > 0.001f) {
					playTiles [i].transform.localScale += (Vector3.one - playTiles [i].transform.localScale) * TILE_SPEED / 1.5f * Time.deltaTime;
				} 

				if (Vector3.Distance(nextTiles[i].transform.position,nextTilesStartPos[i])>0.001f)
				{
					nextTiles[i].transform.position+=(nextTilesStartPos[i]-nextTiles[i].transform.position) *TILE_SPEED * Time.deltaTime;
				}

                playTiles[i].toggleShadow(false);

                if (highlightedTiles[i]!=null)
                    StartCoroutine(highlightedTiles[i].toggleHighlight(false));

                highlightedTiles[i] = null;
            }
           
        }

    }
}
