using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pokega;

public class Tutorial : MonoBehaviour {

	public TutorialBoards[] boards;
	public TutorialPlayTiles[] playTiles;
	public TutorialPositions[] positions; 
	public string[] texts;
	public int step=0;
	public float[] waitBetweenSteps;
	public bool showTutorial;
	public bool closeAtTheEnd;
	public GameObject handPrefab;
	public GameObject tilePrefab;
	public Sprite handUp;
	public Sprite handDown;
	public Sprite handTU;

	public static Tutorial instance;

	private GameObject tutHand;
	private GameObject[] tutTiles;

	private Coroutine moveHandCoroutine;


	void Awake()
	{
		instance = this;
	}

	public int GetBoardTile(int b,int i,int j)
	{
		return boards [b].boards [i].tiles [j];
	}

	public int GetPlayTile (int i, int ord=-1)
	{
		if (ord == -1)
			ord = step;
		
		return playTiles [ord].tiles [i];
	}

	public int GetPositionB ()
	{
		return positions[step].b;
	}

	public int GetPositionI ()
	{
		return positions[step].i;
	}

	public int GetPositionJ ()
	{
		return positions[step].j;
	}

	public int GetPositionIndex ()
	{
		return positions[step].playTileIndex;
	}

	public bool ValidMove(int b,int i, int j, int dragTileStatus)
	{
		if (step >= positions.Length)
			return false;
		
		if (dragTileStatus == GetPlayTile (GetPositionIndex ())) {
			return ((b == GetPositionB()) && (i == GetPositionI ()) && (j == GetPositionJ ()));
		} 
		else 
		{
			return (((1-b) == GetPositionB()) && (i == GetPositionI ()) && (j == GetPositionJ ()));
		}
			
	}

	public IEnumerator StartTutorial(float wait=0.2f)
	{
		step = 0;
		yield return new WaitForSeconds (wait);
		BoardManager.instance.AddOverlay (GetPositionB(),GetPositionI(),GetPositionJ());
		//GameManager.instance.pauseBtn.isEnabled = false;
		Vector3 msgPos = new Vector3 (0f, GameManager.instance.powerUpsY, 0f);
		StartCoroutine (MessagePopup.instance.ShowMessage (texts[step],msgPos));
		if (tutHand != null) 
		{
			tutHand.transform.position=Vector3.down*10f;
			tutHand.transform.rotation = Quaternion.identity;
			tutTiles [0].SetActive (false);
			tutTiles [1].SetActive (false);
		}
		moveHandCoroutine=StartCoroutine(MoveHand(1f));

	}

	public IEnumerator NextStep()
	{
		step++;
		if (step == positions.Length) 
		{
			StopCoroutine (moveHandCoroutine);
			StartCoroutine (RemoveHand (1.5f));
			Vector3 msgPos = new Vector3 (0f, GameManager.instance.powerUpsY, 0f);
			StartCoroutine (MessagePopup.instance.ShowMessage (texts[step],msgPos));
			yield return new WaitForSeconds (2.5f);
			BoardManager.instance.RemoveOverlay ();
			yield return new WaitForSeconds (0.5f);
			StartCoroutine (MessagePopup.instance.HideMessage());
			//GameManager.instance.pauseBtn.isEnabled = true;
			if (closeAtTheEnd) 
			{
				Game.instance.PauseGame ();
				yield return new WaitForSeconds (0.3f);
				//Game.instance.OpenSettings ();
			} 

			showTutorial = false;
		} 
		else 
		{
			//GameManager.instance.pause = true;
			yield return new WaitForSeconds (waitBetweenSteps[step]);
			//GameManager.instance.pause = false;
			if (step == positions.Length)
				yield break;
			Vector3 msgPos = new Vector3 (0f, GameManager.instance.powerUpsY, 0f);
			StartCoroutine (MessagePopup.instance.ShowMessage (texts[step],msgPos));
			BoardManager.instance.AddOverlay (GetPositionB(),GetPositionI(),GetPositionJ());
		}
			

	}

	public IEnumerator MoveHand(float wait=0.2f)
	{

		int currentStep = step;
		
		yield return new WaitForSeconds (wait);
		if (step == positions.Length)
			yield break;
		
		int playTileIndex=GetPositionIndex();
		int endB=GetPositionB();
		int endI=GetPositionI();
		int endJ=GetPositionJ();
		Vector3 startPos = GameManager.instance.playTilesStartPos [playTileIndex];
		Vector3 endPos = BoardManager.GetPositionFromBIJ(endB,endI,endJ);

		if (tutHand == null) 
		{
			tutHand = Instantiate (handPrefab, Vector3.down*10f, Quaternion.identity) as GameObject;
			tutHand.transform.SetParent (gameObject.transform);
			tutTiles = new GameObject[2];
			tutTiles[0] = Instantiate (tilePrefab, GameManager.instance.playTilesStartPos[0], Quaternion.identity) as GameObject;
			tutTiles[1] = Instantiate (tilePrefab, GameManager.instance.playTilesStartPos[1], Quaternion.identity) as GameObject;
			tutTiles[0].transform.SetParent (gameObject.transform);
			tutTiles[1].transform.SetParent (gameObject.transform);
		}

		tutHand.SetActive (true);
		tutHand.GetComponent<SpriteRenderer> ().color = Color.white;
		SpriteRenderer handSpRen=tutHand.GetComponent<SpriteRenderer>();
		handSpRen.sprite = handUp;
		Vector3 currentPos = tutHand.transform.position;
		tutTiles [0].SetActive (false);
		tutTiles [1].SetActive (false);
		float dur = 15f;
		for (int i = 0; i < dur; i++) 
		{
			float newX = Easing.EaseOutBack (currentPos.x,startPos.x,i/dur);
			float newY = Easing.EaseOutCubic (currentPos.y,startPos.y,i/dur);
			tutHand.transform.position = new Vector3 (newX,newY,0f);
			yield return null;
			if (currentStep != step) 
			{
				tutTiles [0].SetActive (false);
				tutTiles [1].SetActive (false);
				//tutHand.GetComponent<SpriteRenderer> ().color -= new Color (0f,0f,0f,0.5f);
				moveHandCoroutine=StartCoroutine (MoveHand(waitBetweenSteps[step]));
				yield break;
			}
		}

		yield return new WaitForSeconds (0.2f);
		handSpRen.sprite = handDown;
		tutTiles [0].transform.position = GameManager.instance.playTilesStartPos [0];
		tutTiles [1].transform.position = GameManager.instance.playTilesStartPos [1];
		tutTiles [0].SetActive (true);
		tutTiles [1].SetActive (true);
		if (!GameManager.instance.playTiles [0].IsWild())
			tutTiles [0].GetComponent<SpriteRenderer> ().color = GameManager.instance.colors [GameManager.instance.playTiles [0].status];
		if (!GameManager.instance.playTiles [1].IsWild())
			tutTiles [1].GetComponent<SpriteRenderer> ().color = GameManager.instance.colors [GameManager.instance.playTiles [1].status];
		yield return new WaitForSeconds (0.1f);
		dur = 60f;
		Vector3 targetPos=Vector3.zero;
		for (int i = 0; i < dur; i++) 
		{
			float newX = Easing.EaseOutBack (startPos.x,endPos.x,i/dur);
			float newY = Easing.EaseOutCubic (startPos.y,endPos.y,i/dur);
			tutHand.transform.position = new Vector3 (newX,newY,0f);
			tutTiles[playTileIndex].transform.position = new Vector3 (newX, newY, 0f);


			if (newY<BoardManager.startY)
				targetPos = new Vector3 (newX, newY+BoardManager.boardYOffset, 0f);
			else
				targetPos = new Vector3 (newX, newY-BoardManager.boardYOffset, 0f);


			tutTiles [1 - playTileIndex].transform.position += (targetPos - tutTiles [1 - playTileIndex].transform.position) * GameManager.TILE_SPEED * Time.deltaTime;
			yield return null;

			if (currentStep != step) 
			{
				tutTiles [0].SetActive (false);
				tutTiles [1].SetActive (false);
				//tutHand.GetComponent<SpriteRenderer> ().color -= new Color (0f,0f,0f,0.5f);
				moveHandCoroutine=StartCoroutine (MoveHand(waitBetweenSteps[step]));
				yield break;
			}
		}
		tutTiles [1 - playTileIndex].transform.position = targetPos;
		handSpRen.sprite = handUp;
		yield return new WaitForSeconds (0.3f);
		moveHandCoroutine=StartCoroutine (MoveHand());

	}

	public IEnumerator RemoveHand(float wait=0f)
	{
		tutTiles [0].SetActive (false);
		tutTiles [1].SetActive (false);
		yield return new WaitForSeconds (wait);
		tutHand.GetComponent<SpriteRenderer> ().sprite = handUp;
		tutHand.GetComponent<SpriteRenderer> ().color = Color.white;
		Vector3 currentPos = tutHand.transform.position;
		Vector3 midPos = Vector3.right*2f;
		Vector3 endPos = midPos + Vector3.down * 10f;
		tutTiles [0].SetActive (false);
		tutTiles [1].SetActive (false);
		float dur = 30f;

		for (int i = 1; i <= dur; i++) 
		{
			float newX = Easing.EaseInOutCubic (currentPos.x,midPos.x,i/dur);
			float newY = Easing.EaseInOutCubic (currentPos.y,midPos.y,i/dur);
			tutHand.transform.position = new Vector3 (newX,newY,0f);

			if (i<=Mathf.RoundToInt (dur/2f))
				tutHand.transform.eulerAngles = new Vector3 (0f, 0f, Easing.EaseOutCubic (0f, -90f, i / (dur/2f)));
			
			if (i == Mathf.RoundToInt (dur/10f)) 
			{
				tutHand.GetComponent<SpriteRenderer> ().sprite = handTU;
				SoundManager.instance.PlaySound (SoundManager.instance.levelUpSfx);
			}
			yield return null;
		}

		dur = 60f;
		for (int i = 0; i < dur; i++) 
		{
			float newX = Easing.EaseOutCubic (midPos.x,endPos.x,i/dur);
			float newY = Easing.EaseInBack (midPos.y,endPos.y,i/dur);
			tutHand.transform.position = new Vector3 (newX,newY,0f);
			yield return null;
		}
		Destroy (tutTiles [0]);
		Destroy (tutTiles [1]);
		Destroy (tutHand);
	}


}

[System.Serializable]
public class TutorialBoards
{
	public TutorialBoard[] boards;
}

[System.Serializable]
public class TutorialBoard
{
	public int[] tiles;
}

[System.Serializable]
public class TutorialPlayTiles
{
	public int[] tiles;
}

[System.Serializable]
public class TutorialPositions
{
	public int b;
	public int i;
	public int j;
	public int playTileIndex;
}