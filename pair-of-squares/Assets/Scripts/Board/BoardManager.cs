using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using Pokega;


public class BoardManager : MonoBehaviour {

    // Use this for initialization
   
    public int m = 5;
    public int n = 6;
    public GameObject tilePrefab;

    public static BoardManager instance;
    public static float boardYOffset;
    public static float startX;
    public static float startY;
    public const int NUM_OF_BOARDS = 2;
    public const float TILE_SIZE = 1.20f;

    [HideInInspector] public Tile[,,] tileMatrix;

    private GameObject container;
    private float checkGameOverTime;

    void OnDisable()
    {
        Destroy(container);
    }

    void OnEnable()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(this);

        container = new GameObject();
        container.transform.SetParent(transform);

        boardYOffset = m * TILE_SIZE + 0.7f;
        startX = (-n*TILE_SIZE / 2f + TILE_SIZE/2);
        startY = 0.9f;

        CreateBoard();
        InitBoard(1);

    }

    void CreateBoard()
    {
        tileMatrix = new Tile[NUM_OF_BOARDS, m,n];
        
       

        for (int b = 0; b < NUM_OF_BOARDS; b++)
        {
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    Vector3 tilePos = new Vector3(startX + j*TILE_SIZE, startY + i*TILE_SIZE - b * boardYOffset, 0);
                    GameObject tile = Instantiate(tilePrefab, tilePos, Quaternion.identity) as GameObject;
                    tile.transform.SetParent(container.transform);
                    tileMatrix[b, i, j] = tile.GetComponent<Tile>();
                    tileMatrix[b, i, j].SetBIJ(b, i, j);
                }
            }
        }
    }

    void InitBoard(int level)
    {
        int numOfStartingTiles = Tile.NUM_OF_COLORS/2;
        int randI;
        int randJ;
        int colorIndex;
        int numOfColors = Tile.NUM_OF_COLORS;
		if (!Tutorial.instance.showTutorial) 
		{
			for (int b = 0; b < NUM_OF_BOARDS; b++) {
				for (int i = 0; i < numOfStartingTiles; i++) {
					do {
						randI = Random.Range (0, m);
						randJ = Random.Range (0, n);
					} while (!tileMatrix [b, randI, randJ].IsEmpty ());

					colorIndex = (i + b * numOfStartingTiles) % numOfColors;
					UpdateBoards (b, randI, randJ, colorIndex, Tile.P_NONE);
				}
			}
		} 
		else 
		{
			for (var b = 0; b < NUM_OF_BOARDS; b++) 
			{
				for (int i = 0; i < m; i++) 
				{
					for (int j = 0; j < n; j++) 
					{
						colorIndex = Tutorial.instance.GetBoardTile(b,i,j);
						if (colorIndex!=Tile.EMPTY)
							UpdateBoards (b, i, j, colorIndex, Tile.P_NONE);
					}
				}
			}

			StartCoroutine(Tutorial.instance.StartTutorial());

		}
         
    }

    void UpdateBoards(int b, int i, int j, int newStatus, int newPower, bool clear = false)
    {
        

        if (newStatus == Tile.EMPTY)
        {
            if (tileMatrix[1 - b, i, j].IsMasked())
                tileMatrix[1 - b, i, j].UpdateStatus(Tile.EMPTY);
            else if (!tileMatrix[1 - b, i, j].IsEmpty())
            {
                tileMatrix[b, i, j].UpdateTile(Tile.MASKED,newPower,clear);
                return;
            }
        }
        else
        {
            if (tileMatrix[1 - b, i, j].IsEmpty())
                tileMatrix[1 - b, i, j].UpdateStatus(Tile.MASKED);
        }

        tileMatrix[b, i, j].UpdateTile(newStatus, newPower, clear);
    }

    public bool AddTiles(Vector3 position,Tile tile1,Tile tile2)
    {

		if (LineTrace.lineIsActive)
			return false;
		
        int b = GetBFromPosition(position);
        int i = GetIFromPosition(position);
        int j = GetJFromPosition(position);

		if (Tutorial.instance.showTutorial)  
		{
			if (!Tutorial.instance.ValidMove (b, i, j, tile1.status)) {
				return false;
			} 
			else 
			{
				StartCoroutine (tileMatrix [0, i, j].toggleOverlay (false));
				StartCoroutine (tileMatrix [1, i, j].toggleOverlay (false));
				StartCoroutine(Tutorial.instance.NextStep());
			}
				
		}

        if ((i >= 0) && (i < m) && (j >= 0) && (j < n) && (tileMatrix[b, i, j].IsEmpty()))
        {
            tileMatrix[b, i, j].UpdateTile(tile1.status,tile1.power);
            tileMatrix[1 - b, i, j].UpdateTile(tile2.status,tile2.power);
            tile1.UpdateTile(0,Tile.P_NONE);
            tile2.UpdateTile(0, Tile.P_NONE);
            bool matchMade0=CheckForMatches(0, i, j);
            bool matchMade1=CheckForMatches(1, i, j);
            if ((matchMade0) && (matchMade1)) 
             {
                Color animColor;
                float animDuration;
                int randTileInd = Random.Range(0, 2);
                int randColInd = Random.Range(0, Tile.NUM_OF_COLORS);

                for (int c = 0; c < 1+Tile.NUM_OF_COLORS/2; c++)
                {
                    animColor = GameManager.instance.colors[(randColInd+c)%Tile.NUM_OF_COLORS];
                    animDuration = Random.Range(0.2f, 0.4f);
                    StartCoroutine(LineTrace.DrawCurve(tileMatrix[b, i, j].transform.position, GameManager.instance.playTilesStartPos[randTileInd], animColor, 0.2f, animDuration, 0.1f * (c+3)));
                    animColor = GameManager.instance.colors[(randColInd + c + 3) % Tile.NUM_OF_COLORS];
                    animDuration = Random.Range(0.2f, 0.4f);
                    StartCoroutine(LineTrace.DrawCurve(tileMatrix[1 - b, i, j].transform.position, GameManager.instance.playTilesStartPos[randTileInd], animColor, 0.2f, animDuration, 0.1f * (c + 5)));
                }
                SoundManager.instance.PlaySound(SoundManager.instance.wildCardLineSfx, 0.8f);
                GameManager.instance.playTiles[randTileInd].UpdateStatus(Tile.WILD);
				if (!Tutorial.instance.showTutorial)
					App.ach.SetAchievement ("Wild card");

				App.analytics.CreateAnalyticEvent ("Wild card");
            }

            if ((matchMade0) || (matchMade1))
                checkGameOverTime = 2f;
            else
                checkGameOverTime = 0.5f;

            CancelInvoke("CheckGameOver");
            Invoke("CheckGameOver",checkGameOverTime);

            return true;
        }
        return false;
    }

    public IEnumerator RemoveTile(Tile tile,float waitTime)
    {
		
        yield return new WaitForSeconds(waitTime);
        if ((tile.IsEmpty()) || (tile.IsMasked()))
            yield break;

        int power = tile.power;
        GameManager.instance.UpdateTileCount(tile);
        UpdateBoards(tile.b, tile.i, tile.j, Tile.EMPTY, Tile.P_NONE);
        Tile other;
        float delay;
        float lineW = 0.35f;
        if (power == Tile.P_NONE)
        {
            StartCoroutine(Camera.main.GetComponent<CameraControl>().Shake(0.09f, 0.07f));
            yield break;
        }
        else
        {
            StartCoroutine(Camera.main.GetComponent<CameraControl>().Shake((power + 5) * 0.022f, 0.15f));
        }

        if (power == Tile.P_ROW)
        {
            for (int j = 0; j < n; j++)
            {
                other = tileMatrix[tile.b, tile.i, j];
                delay = GetExplosionDelay(tile, other);
                StartCoroutine(RemoveTile(other, delay));
                StartCoroutine(LineTrace.DrawLine(tile.transform.position, other.transform.position, new Color(1f, 1f, 0.8f, 1f), lineW, delay));
            }
            yield break;
        }

        if (power == Tile.P_COL)
        {
            for (int i = 0; i < m; i++)
            {
                other = tileMatrix[tile.b, i, tile.j];
                delay = GetExplosionDelay(tile, other);
                StartCoroutine(RemoveTile(other, delay));
                StartCoroutine(LineTrace.DrawLine(tile.transform.position, other.transform.position, new Color(1f, 1f, 0.8f, 1f), lineW, delay));
            }
            yield break;
        }

        if (power == Tile.P_ROWCOL)
        {
            for (int i = 0; i < m; i++)
            {
                other = tileMatrix[tile.b, i, tile.j];
                delay = GetExplosionDelay(tile, other);
                StartCoroutine(RemoveTile(other, delay));
                StartCoroutine(LineTrace.DrawLine(tile.transform.position, other.transform.position, new Color(1f, 1f, 0.8f, 1f), lineW, delay));
            }
            for (int j = 0; j < n; j++)
            {
                other = tileMatrix[tile.b, tile.i, j];
                delay = GetExplosionDelay(tile, other);
                StartCoroutine(RemoveTile(other, delay));
                StartCoroutine(LineTrace.DrawLine(tile.transform.position, other.transform.position, new Color(1f, 1f, 0.8f, 1f), lineW, delay));
            }
            yield break;
        }
    
        if (power == Tile.P_COL_COL)
        {
            for (int i = 0; i < m; i++)
            {
                other = tileMatrix[tile.b, i, tile.j];
                delay = GetExplosionDelay(tile, other);
                StartCoroutine(RemoveTile(other, delay));
                StartCoroutine(LineTrace.DrawLine(tile.transform.position, other.transform.position, new Color(1f, 1f, 0.8f, 1f), lineW, delay));

                other = tileMatrix[1 - tile.b, i, tile.j];
                delay = GetExplosionDelay(tile, other);
                StartCoroutine(RemoveTile(other, delay));
                StartCoroutine(LineTrace.DrawLine(tile.transform.position, other.transform.position, new Color(1f, 1f, 0.8f, 1f), lineW, delay));
            }
            yield break;
        }
        if (power == Tile.P_BOMB)
        {
            for (int i=tile.i-1; i<=tile.i+1; i++)
            {
                for (int j = tile.j - 1; j <= tile.j + 1; j++)
                {
                    if ((i >= 0) && (i < m) && (j >= 0) && (j < n))
                    {
                        other = tileMatrix[tile.b, i, j];
                        delay = GetExplosionDelay(tile, other);
                        StartCoroutine(RemoveTile(other, delay));
                        StartCoroutine(LineTrace.DrawLine(tile.transform.position, other.transform.position, new Color(1f, 1f, 0.8f, 1f), lineW, delay));
                    }
                }
            }
            yield break;
        }
        if (power == Tile.P_BIGBOMB)
        {
            for (int i = tile.i - 1; i <= tile.i + 1; i++)
            {
                for (int j = tile.j - 1; j <= tile.j + 1; j++)
                {
                    if ((i >= 0) && (i < m) && (j >= 0) && (j < n))
                    {
                        other = tileMatrix[tile.b, i, j];
                        delay = GetExplosionDelay(tile, other);
                        StartCoroutine(RemoveTile(other, delay));
                        StartCoroutine(LineTrace.DrawLine(tile.transform.position, other.transform.position, new Color(1f, 1f, 0.8f, 1f), lineW, delay));
                    }
                }
            }
            if (tile.i - 2 >= 0)
            {
                other = tileMatrix[tile.b, tile.i - 2, tile.j];
                delay = GetExplosionDelay(tile, other);
                StartCoroutine(RemoveTile(other, delay));
                StartCoroutine(LineTrace.DrawLine(tile.transform.position, other.transform.position, new Color(1f, 1f, 0.8f, 1f), lineW, delay));
            }
            if (tile.i + 2 < m)
            {
                other = tileMatrix[tile.b, tile.i + 2, tile.j];
                delay = GetExplosionDelay(tile, other);
                StartCoroutine(RemoveTile(other, delay));
                StartCoroutine(LineTrace.DrawLine(tile.transform.position, other.transform.position, new Color(1f, 1f, 0.8f, 1f), lineW, delay));
            }
            if (tile.j - 2 >= 0)
            {
                other = tileMatrix[tile.b, tile.i, tile.j-2];
                delay = GetExplosionDelay(tile, other);
                StartCoroutine(RemoveTile(other, delay));
                StartCoroutine(LineTrace.DrawLine(tile.transform.position, other.transform.position, new Color(1f, 1f, 0.8f, 1f), lineW, delay));
            }
            if (tile.j + 2 < n)
            {
                other = tileMatrix[tile.b, tile.i, tile.j+2];
                delay = GetExplosionDelay(tile, other);
                StartCoroutine(RemoveTile(other, delay));
                StartCoroutine(LineTrace.DrawLine(tile.transform.position, other.transform.position, new Color(1f, 1f, 0.8f, 1f), lineW, delay));
            }
            yield break;
        }

        if (power == Tile.P_ROWCOL_COL)
        {

            for (int i = 0; i < m; i++)
            {
                other = tileMatrix[tile.b, i, tile.j];
                delay = GetExplosionDelay(tile, other);
                StartCoroutine(RemoveTile(other, delay));
                StartCoroutine(LineTrace.DrawLine(tile.transform.position, other.transform.position, new Color(1f, 1f, 0.8f, 1f), lineW, delay));

                other = tileMatrix[1-tile.b, i, tile.j];
                delay = GetExplosionDelay(tile, other);
                StartCoroutine(RemoveTile(other, delay));
                StartCoroutine(LineTrace.DrawLine(tile.transform.position, other.transform.position, new Color(1f, 1f, 0.8f, 1f), lineW, delay));
            }
            for (int j = 0; j < n; j++)
            {
                other = tileMatrix[tile.b, tile.i, j];
                delay = GetExplosionDelay(tile, other);
                StartCoroutine(RemoveTile(other, delay));
                StartCoroutine(LineTrace.DrawLine(tile.transform.position, other.transform.position, new Color(1f, 1f, 0.8f, 1f), lineW, delay));
            }
            yield break;
        }

        if (power == Tile.P_COLCOLCOL)
        {

            for (int i = 0; i < m ; i++)
            {
                for (int j = tile.j - 1; j <= tile.j + 1; j++)
                {
                    if ((j >= 0) && (j < n))
                    {
                        other = tileMatrix[tile.b, i, j];
                        delay = GetExplosionDelay(tile, other);
                        StartCoroutine(RemoveTile(other, delay));
                        StartCoroutine(LineTrace.DrawLine(tileMatrix[tile.b, tile.i, j].transform.position, other.transform.position, new Color(1f, 1f, 0.8f, 1f), lineW, delay));
                    }
                }
            }
            yield break;
        }

        if (power == Tile.P_ROWROWROW)
        {
            for (int i = tile.i - 1; i <= tile.i + 1; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if ((i >= 0) && (i < m))
                    {
                        other = tileMatrix[tile.b, i, j];
                        delay = GetExplosionDelay(tile, other);
                        StartCoroutine(RemoveTile(other, delay));
                        StartCoroutine(LineTrace.DrawLine(tileMatrix[tile.b, i, tile.j].transform.position, other.transform.position, new Color(1f, 1f, 0.8f, 1f), lineW, delay));
                    }
                }
            }
            yield break;
        }

        if (power == Tile.P_ROWROWROWCOLCOLCOL)
        {
            for (int i = tile.i - 1; i <= tile.i + 1; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if ((i >= 0) && (i < m))
                    {
                        other = tileMatrix[tile.b, i, j];
                        delay = GetExplosionDelay(tile, other);
                        StartCoroutine(RemoveTile(other, delay));
                        StartCoroutine(LineTrace.DrawLine(tileMatrix[tile.b, i, tile.j].transform.position, other.transform.position, new Color(1f, 1f, 0.8f, 1f), lineW, delay));
                    }
                }
            }
            for (int i = 0; i < m; i++)
            {
                for (int j = tile.j - 1; j <= tile.j + 1; j++)
                {
                    if ((j >= 0) && (j < n))
                    {
                        other = tileMatrix[tile.b, i, j];
                        delay = GetExplosionDelay(tile, other);
                        StartCoroutine(RemoveTile(other, delay));
                        StartCoroutine(LineTrace.DrawLine(tileMatrix[tile.b, tile.i, j].transform.position, other.transform.position, new Color(1f, 1f, 0.8f, 1f), lineW, delay));
                    }
                }
            }
            yield break;
        }

        if (power == Tile.P_COLCOLCOL_COLCOLCOL)
        {
            for (int b = 0; b < NUM_OF_BOARDS; b++)
            {
                for (int i = 0; i < m; i++)
                {
                    for (int j = tile.j - 1; j <= tile.j + 1; j++)
                    {
                        if ((j >= 0) && (j < n))
                        {
                            other = tileMatrix[b, i, j];
                            delay = GetExplosionDelay(tile, other);
                            StartCoroutine(RemoveTile(other, delay));
                            StartCoroutine(LineTrace.DrawLine(tileMatrix[tile.b, tile.i, j].transform.position, other.transform.position, new Color(1f, 1f, 0.8f, 1f), lineW, delay));
                        }
                    }
                }
            }
            yield break;
        }


    }

    public bool CheckForMatches(int mB, int mI,int mJ)
    {

        bool matchMade = false;
        int vCount=0;
        int hCount=0;
        int[] compStatuses = new int[1] {tileMatrix[mB, mI, mJ].status};
        if (tileMatrix[mB, mI, mJ].IsWild())
        {
            compStatuses = new int[Tile.NUM_OF_COLORS];
            for (int c = 0; c < Tile.NUM_OF_COLORS; c++)
                compStatuses[c] = c;
        }
        Tile[] vTiles;
        Tile[] hTiles;
        HashSet<Tile> allVTiles=new HashSet<Tile>();
        HashSet<Tile> allHTiles=new HashSet<Tile>();
		int numOfWilds = 0;

        for (int c = 0; c < compStatuses.Length; c++)
        {
            vCount = 0;
            hCount = 0;
            vTiles = new Tile[m];
            hTiles = new Tile[n];

            for (int i = 0; i < m; i++)
            {
                if (tileMatrix[mB, i, mJ].IsEqual(compStatuses[c]))
                {
                    vTiles[vCount] = tileMatrix[mB, i, mJ];
                    vCount++;
                }
                else
                {
                    if (vCount >= 3)
                        break;
                    vTiles = new Tile[m];
                    vCount = 0;
                }
            }

            for (int j = 0; j < n; j++)
            {
                if (tileMatrix[mB, mI, j].IsEqual(compStatuses[c]))
                {
                    hTiles[hCount] = tileMatrix[mB, mI, j];
                    hCount++;
                }
                else
                {
                    if (hCount >= 3)
                        break;
                    hTiles = new Tile[n];
                    hCount = 0;
                }
            }

            if (vCount >= 3)
            {
                for (int i = 0; i < vCount; i++)
                {
                    StartCoroutine(RemoveTile(vTiles[i], 0.2f+GetExplosionDelay(vTiles[i], tileMatrix[mB, mI, mJ])));
					if (allVTiles.Add (vTiles [i])) 
					{
						if (vTiles [i].IsWild())
							numOfWilds++;
					}
                }
                matchMade = true;
            }
            else
            {
                vCount = 0;
            }

            if (hCount >= 3)
            {
                for (int j = 0; j < hCount; j++)
                {
                    StartCoroutine(RemoveTile(hTiles[j], 0.2f+GetExplosionDelay(hTiles[j], tileMatrix[mB, mI, mJ])));
					if (allHTiles.Add(hTiles[j]))
					{
						if (hTiles [j].IsWild())
							numOfWilds++;
					}
                }
                matchMade = true;
            }
            else
            {
                hCount = 0;
            }

            
          
        }
		if (numOfWilds >= 3)
			App.ach.SetAchievement ("3 Wild cards");

        GameManager.instance.AddPowers(mB, mI, mJ, allVTiles.Count, allHTiles.Count);
        return matchMade;

    }

    public void CheckGameOver()
    {
        if (BoardIsFull())
            GameManager.instance.EndGame();

    }

    private float Distance(Tile a, Tile b)
    {
        int iDist = (a.i + (1-a.b) * m) - (b.i + (1-b.b) * m);
        int jDist = a.j - b.j;
        return Mathf.Sqrt(iDist*iDist + jDist*jDist);
        
    }

    private float GetExplosionDelay(Tile a, Tile b)
    {
        return (Distance(a, b) * 0.13f);
    }

    public static int GetBFromPosition(Vector3 position)
    {
        if (position.y > startY - TILE_SIZE)
            return 0;
        else
            return 1;
    }

    public static int GetIFromPosition(Vector3 position)
    {
        int b = -1;
        if (position.y > startY - TILE_SIZE)
            b = 0;
        else
            b = 1;

        return Mathf.RoundToInt((position.y - startY + b * boardYOffset) / TILE_SIZE);
    }

    public static int GetJFromPosition(Vector3 position)
    {
        return Mathf.RoundToInt((position.x - startX) / TILE_SIZE);
    }

	public static Vector3 GetPositionFromBIJ(int b, int i, int j)
	{
		float x = startX + j * TILE_SIZE;
		float y = startY + i * TILE_SIZE - b * boardYOffset;

		return new Vector3 (x, y, 0f);
	}

    public string DestroyColor(int status)
    {
        if (GameManager.instance.playTiles[0].IsEraser())
			return "Erasers are already activated.";
		
        int count = 0;
        for (int b = 0; b < NUM_OF_BOARDS; b++)
        {
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (tileMatrix[b, i, j].status==status)
                    {
                        StartCoroutine(RemoveTile(tileMatrix[b, i, j], Random.Range(0f, 0.3f)));
                        count++;
                    }
                    //UpdateBoards(b, i, j, Tile.EMPTY, Tile.P_NONE);
                }
            }
        }

        if (count > 0)
        {
            StartCoroutine(GameManager.instance.UpdateLevelProgress());
            return "OK";
        }
        return "There are no tiles of that color on the board.";
    }

    public string ActivateSpecialTiles()
    {
        if (GameManager.instance.playTiles[0].IsEraser())
			return "Erasers are already activated.";
		
        int count = 0;
        for (int b = 0; b < NUM_OF_BOARDS; b++)
        {
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (tileMatrix[b, i, j].power != Tile.P_NONE)
                    {
                        StartCoroutine(RemoveTile(tileMatrix[b, i, j], Random.Range(0f, 0.3f)));
                        count++;
                    }
                    //UpdateBoards(b, i, j, Tile.EMPTY, Tile.P_NONE);
                }
            }
        }
        if (count > 0)
        {
            StartCoroutine(GameManager.instance.UpdateLevelProgress());
            return "OK";
        }
        return "There are no exploding tiles on the board.";
    }

	public void AddOverlay(int exceptB=-1, int exceptI=-1, int exceptJ=-1) //adds overlay to all empty tiles except (b,i,j)
	{
		for (int b = 0; b < NUM_OF_BOARDS; b++)
		{
			for (int i = 0; i < m; i++)
			{
				for (int j = 0; j < n; j++)
				{
					if (((b != exceptB) || (i != exceptI) || (j != exceptJ)) && (!tileMatrix[b,i,j].IsColored())) 
					{
						StartCoroutine(tileMatrix [b, i, j].toggleOverlay (true));
					} 
					else 
					{
						StartCoroutine(tileMatrix [b, i, j].toggleOverlay (false));
					}
				}
			}
		}
	}

	public void RemoveOverlay()
	{
		for (int b = 0; b < NUM_OF_BOARDS; b++)
		{
			for (int i = 0; i < m; i++)
			{
				for (int j = 0; j < n; j++)
				{
					StartCoroutine (tileMatrix [b, i, j].toggleOverlay (false));
				}
			}
		}
	}


	public string ClearBoard(int b)
    {
        if (GameManager.instance.playTiles[0].IsEraser())
			return "Erasers are already activated.";
		
        int count = 0;
        for (int i = 0; i < m; i++)
        {
            for (int j = 0; j < n; j++)
            {
                if ((!tileMatrix[b, i, j].IsEmpty()) && (!tileMatrix[b, i, j].IsMasked()))
                {
                    UpdateBoards(b, i, j, Tile.EMPTY, Tile.P_NONE, true);
                    count++;
                }
            }
        }

        if (count > 0)
        {
            //StartCoroutine(GameManager.instance.UpdateLevelProgress());
            StartCoroutine(GameManager.instance.DecreasePoints(10));
			SoundManager.instance.PlaySound (SoundManager.instance.clearBoardSfx);
            return "OK";
        }
		if (b==0)
        	return "The upper board is empty.";
		else
			return "The lower board is empty.";
    }

    public bool BoardIsEmpty()
    {
        for (int i = 0; i < m; i++)
        {
            for (int j = 0; j < n; j++)
            {
                if (!tileMatrix[0, i, j].IsEmpty())
                    return false;
            }
        }
        return true;
    }

    public bool BoardIsFull()
    {
        for (int i = 0; i < m; i++)
        {
            for (int j = 0; j < n; j++)
            {
                if (tileMatrix[0, i, j].IsEmpty())
                    return false;
            }
        }
        return true;
    }

}
