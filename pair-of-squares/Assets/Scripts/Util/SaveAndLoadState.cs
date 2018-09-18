using UnityEngine;
using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Pokega;

public class SaveAndLoadState : MonoBehaviour, ILocal {

    // Use this for initialization
    [Serializable]
    public struct TileData
    {
        public int power;
        public int status;
    }
    [Serializable]
    public struct PowerUpData
    {
        public int type;
        public int subtype;
    }
    public static bool loadable;
    private static PlayerData data = new PlayerData();
    private static float lastSaveTime=-100f;
    private const float saveFreq = 20f;

    public static void SaveState(bool force=false, bool newLoadable=true)
    {
        if ((!force) && (Time.time - lastSaveTime < saveFreq))
            return;

		loadable = newLoadable;
        data.tileMatrix = new TileData[BoardManager.NUM_OF_BOARDS,BoardManager.instance.m, BoardManager.instance.n];
        data.playTiles = new TileData[BoardManager.NUM_OF_BOARDS];
		data.nextTiles = new TileData[BoardManager.NUM_OF_BOARDS];
        for (int b = 0; b < BoardManager.NUM_OF_BOARDS; b++)
        {
            data.playTiles[b].status = GameManager.instance.playTiles[b].status;
            data.playTiles[b].power = GameManager.instance.playTiles[b].power;

			data.nextTiles[b].status = GameManager.instance.nextTiles[b].status;
			data.nextTiles[b].power = GameManager.instance.nextTiles[b].power;

            for (int i = 0; i < BoardManager.instance.m; i++)
            {
                for (int j = 0; j < BoardManager.instance.n; j++)
                {
                    data.tileMatrix[b, i, j].status = BoardManager.instance.tileMatrix[b, i, j].status;
                    data.tileMatrix[b,i,j].power= BoardManager.instance.tileMatrix[b, i, j].power;
                }
            }
        }

        data.powerUps = new PowerUpData[GameManager.instance.powerUpsList.Count];
        for (int i = 0; i < GameManager.instance.powerUpsList.Count; i++)
        {
            data.powerUps[i].type = GameManager.instance.powerUpsList[i].GetComponent<PowerUp>().type;
            data.powerUps[i].subtype = GameManager.instance.powerUpsList[i].GetComponent<PowerUp>().subtype;
        }

        data.numOfMoves = GameManager.instance.numOfMoves;
        data.movesToLevelUp = GameManager.instance.movesToLevelUp;
        data.level = GameManager.level;
		if (GameManager.instance.multipliedMove != GameManager.instance.numOfMoves) 
		{
			GameManager.instance.multiplier = 1;
			GameManager.instance.multipliedMove = -1;
		}
        data.multiplier = GameManager.instance.multiplier;
		data.score = (int) App.score.GetScore("Points");
        data.bestScore = GameManager.bestScore;
		data.gameOver = GameManager.gameOver;
		data.duration = GameManager.duration;
		data.powerUpBag = GameManager.instance.powerUpBag;
		data.powDrawCount = GameManager.instance.powDrawCount;
		data.showLevelUp = GameManager.showLevelUp;
		data.lastPuType1 = GameManager.lastPuType1;
		data.lastPuType2 = GameManager.lastPuType2;
		data.lastPuSubtype1 = GameManager.lastPuSubtype1;
		data.lastPuSubtype2 = GameManager.lastPuSubtype2;

        SaveToFile();
        lastSaveTime = Time.time;
    }

    public static void LoadState()
    {
        if (!loadable)
            return;

        LoadFromFile();

        for (int b = 0; b < BoardManager.NUM_OF_BOARDS; b++)
        {
            GameManager.instance.playTiles[b].UpdateTile(data.playTiles[b].status, data.playTiles[b].power, true);
			GameManager.instance.nextTiles[b].UpdateStatus(data.nextTiles[b].status,false,true);
            for (int i = 0; i < BoardManager.instance.m; i++)
            {
                for (int j = 0; j < BoardManager.instance.n; j++)
                {
                    BoardManager.instance.tileMatrix[b, i, j].UpdateTile(data.tileMatrix[b, i, j].status, data.tileMatrix[b, i, j].power, true);
                }
            }
        }


        GameManager.instance.RemoveAllPowerUps();
        for (int i = 0; i < data.powerUps.Length; i++)
        {
            GameManager.instance.StartCoroutine(GameManager.instance.AddPowerUp(data.powerUps[i].type, data.powerUps[i].subtype, 0f, false));
        }


        GameManager.instance.numOfMoves = data.numOfMoves;
        GameManager.instance.movesToLevelUp = data.movesToLevelUp;
        float barWidth = (data.numOfMoves * 1f / data.movesToLevelUp)*GameManager.instance.barMaxWidth;
        GameManager.instance. levelProgressBar.rectTransform.sizeDelta = new Vector2(barWidth, GameManager.instance.levelProgressBar.rectTransform.sizeDelta.y); 
        GameManager.level = data.level;
        GameManager.score = data.score;
        GameManager.bestScore = data.bestScore;
        //GameManager.instance.scoreText.text = "" + data.score;
        App.score.SetScore(data.score, "Points");

		if (data.gameOver!=null)
			GameManager.gameOver = data.gameOver;
		if (data.duration!=null)
			GameManager.duration = data.duration;
		
		if (data.powerUpBag!=null)
			GameManager.instance.powerUpBag = data.powerUpBag;
		if (data.powDrawCount!=null)
			GameManager.instance.powDrawCount = data.powDrawCount;
		
		if (data.showLevelUp!=null)
			GameManager.showLevelUp = data.showLevelUp;
		if (data.lastPuType1!=null)
			GameManager.lastPuType1 = data.lastPuType1;
		if (data.lastPuType2!=null)
			GameManager.lastPuType2 = data.lastPuType2;
		if (data.lastPuSubtype1!=null)
			GameManager.lastPuSubtype1 = data.lastPuSubtype1;
		if (data.lastPuSubtype2!=null)
			GameManager.lastPuSubtype2 = data.lastPuSubtype2;

        for (int i=0;i<Mathf.Log(data.multiplier,2);i++)
            GameManager.instance.DoubleMultiplier(false);
        
    }

    private static void SaveToFile()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/playerSave.dat");
        bf.Serialize(file, data);
        file.Close();
    }

    private static bool LoadFromFile()
    {
        if (File.Exists(Application.persistentDataPath + "/playerSave.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/playerSave.dat", FileMode.Open);
            data = (PlayerData)bf.Deserialize(file);
            file.Close();
            return true;
        }
        return false;
    }

	public static bool SaveFileExists()
	{
		return File.Exists (Application.persistentDataPath + "/playerSave.dat");
	}

    public static void SetLoadable()
    {
		if (LoadFromFile ()) 
		{
			loadable = !data.gameOver;
		} 
		else
			loadable = false;
    }

    public static int GetBestScore()
    {
        if (LoadFromFile())
            return data.bestScore;
        else
            return 0;
    }

    public void Save()
    {
        //SaveState();
    }

    public void Load()
    {
        //LoadState();
    }

    public void Reset()
    {
        Debug.Log("RESET STATE");
    }
}
[Serializable]
class PlayerData
{
    public SaveAndLoadState.TileData[,,] tileMatrix;
    public SaveAndLoadState.TileData[] playTiles;
	public SaveAndLoadState.TileData[] nextTiles;
    public SaveAndLoadState.PowerUpData[] powerUps;

    public int numOfMoves;
    public int movesToLevelUp;
    public int level;
    public int score;
	public float duration;
    public int multiplier;
    public int bestScore;
	public bool gameOver;
	public int[] powerUpBag;
	public int powDrawCount;
	public bool showLevelUp;
	public int lastPuType1, lastPuSubtype1, lastPuType2, lastPuSubtype2;
}
         
