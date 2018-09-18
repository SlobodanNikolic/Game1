using UnityEngine;
using System.Collections;
using Pokega;

public class PowerUp : MonoBehaviour {

    public int type=0;
    public int subtype=0;

    public Color[] bgColors;
    public Color[] readyColors;
    public Sprite[] patternSprites;
    public Sprite eraserSprite;
    public Sprite x2Sprite;
    public Sprite bombsSprite;
    public Sprite destroyColorSprite;
    public Sprite clearBoardSprite;
    public Sprite activateSpecialSprite;
    public Sprite wildsSprite;
    [HideInInspector] public bool ready;
    [HideInInspector] public int quantity;

    private SpriteRenderer patSpRen;
    private SpriteRenderer bgSpRen; 
    private SpriteRenderer typeSpRen;
    private SpriteRenderer readySpRen;


    public const int NUM_OF_TYPES = 7;
    public const int ERASER = 0;
    public const int X2 = 1;
    public const int BOMBS = 2;
    public const int DESTROY_COLOR = 3;
    public const int CLEAR_BOARD = 4;
    public const int ACTIVATE_SPECIAL = 5;
    public const int WILDS = 6;


    void Awake()
    {
        bgSpRen = transform.Find("BG").gameObject.GetComponent<SpriteRenderer>();
        typeSpRen = transform.Find("Type").gameObject.GetComponent<SpriteRenderer>();
        readySpRen = transform.Find("Ready").gameObject.GetComponent<SpriteRenderer>();
        patSpRen = transform.Find("Pattern").gameObject.GetComponent<SpriteRenderer>();

        bgSpRen.color = bgColors[0];
        readySpRen.color = Color.clear;
        readySpRen.transform.localScale = Vector3.zero;
        patSpRen.color = Color.clear;

        ready = false;
        quantity = 1;
    }

    public void UpdatePowerUp(int newType, int newSubtype=0)
    {
        type = newType;
        subtype = newSubtype;
        typeSpRen.sprite = GetSpriteFromType(newType);
        patSpRen.sprite = null;
        patSpRen.color = Color.clear;
        quantity = 1;

        if (newType == DESTROY_COLOR)
        {
            typeSpRen.color = GameManager.instance.colors[subtype];
            patSpRen.sprite = patternSprites[newSubtype];
            if (App.colorBlindMode)
                patSpRen.color = Color.white;
        }
        if (newType == CLEAR_BOARD)
        {
            typeSpRen.transform.rotation = Quaternion.Euler(new Vector3(0f,0f,subtype*180f));
        }

        if (newType == ERASER)
        {
            quantity = 2;
        }
    }

    public void Activate()
    {

		if ((LineTrace.lineIsActive) || (BoardManager.instance.BoardIsFull()))
			return;
		
        string validMove = "OK";

        if (type == ERASER)
        {
            validMove = GameManager.instance.PlayTilesToErasers();
        }
        if (type == X2)
        {
            validMove = GameManager.instance.DoubleMultiplier();
        }

        if (type == BOMBS)
        {
            validMove = GameManager.instance.PlayTilesToBombs();
        }
        if (type == DESTROY_COLOR)
        {
            validMove = BoardManager.instance.DestroyColor(subtype);
        }
        if (type == CLEAR_BOARD)
        {
            validMove = BoardManager.instance.ClearBoard(subtype);
        }
        if (type == ACTIVATE_SPECIAL)
        {
            validMove = BoardManager.instance.ActivateSpecialTiles();
        }
        if (type == WILDS)
        {
            validMove = GameManager.instance.PlayTilesToWilds();
        }

        if (validMove=="OK")
        {
            StartCoroutine(GameManager.instance.RemovePowerUp(gameObject));
        }
        else
        {
            StartCoroutine(InvalidMove());
			SoundManager.instance.PlaySound (SoundManager.instance.unavailableSfx);
			Vector3 msgPos = new Vector3 (0f,GameManager.instance.powerUpsY-1.8f,0f);
			StartCoroutine(MessagePopup.instance.ShowMessage (validMove,msgPos,3f,transform.position.x));
        }
    }

    public void ChangeSortingLayer(bool active, int index=0)
    {
        HoldHandler hh = gameObject.GetComponent<HoldHandler>();
        if (active)
        {
            readySpRen.sortingLayerName = "ActivePowerUps";
            readySpRen.sortingOrder = 0;
            bgSpRen.sortingLayerName = "ActivePowerUps";
            bgSpRen.sortingOrder = 1;
            typeSpRen.sortingLayerName = "ActivePowerUps";
            typeSpRen.color = new Color(typeSpRen.color.r, typeSpRen.color.g, typeSpRen.color.b, 1f);
            typeSpRen.sortingOrder = 2;
            patSpRen.sortingLayerName = "ActivePowerUps";
            if (App.colorBlindMode)
                patSpRen.color = Color.white;
            patSpRen.sortingOrder = 3;

            if (hh == null)
                gameObject.AddComponent<HoldHandler>();
        }
        else
        {
            readySpRen.sortingLayerName = "InactivePowerUps";
            readySpRen.sortingOrder = index*4;
            bgSpRen.sortingLayerName = "InactivePowerUps";
            bgSpRen.sortingOrder = index * 4 + 1;
            typeSpRen.sortingLayerName = "InactivePowerUps";
            typeSpRen.color = new Color(typeSpRen.color.r, typeSpRen.color.g, typeSpRen.color.b, 0f);
            typeSpRen.sortingOrder = index * 4 + 2;
            patSpRen.sortingLayerName= "InactivePowerUps";
            patSpRen.color = Color.clear;
            patSpRen.sortingOrder = index * 4 + 3;
            if (hh != null)
                hh.Remove();
        }
    }

    public Color GetBGColor(int index)
    {
        return bgColors[(index % (bgColors.Length-1))+1];
    }


    private Sprite GetSpriteFromType(int t)
    {

        if (t == ERASER)
            return eraserSprite;

        if (t == BOMBS)
            return bombsSprite;

        if (t == DESTROY_COLOR)
            return destroyColorSprite;

        if (t == CLEAR_BOARD)
            return clearBoardSprite;

        if (t == X2)
            return x2Sprite;

        if (t == ACTIVATE_SPECIAL)
            return activateSpecialSprite;

        if (t == WILDS)
            return wildsSprite;

        return null;
    }

    public IEnumerator StartHoldProgress()
    {
        ready = false;
        int dur = 30;
        readySpRen.color = readyColors[0];
        readySpRen.transform.localScale = Vector3.zero;
        for (int i = 1; i <= dur; i++)
        {
            readySpRen.transform.localScale = Vector3.one * Easing.EaseOutCubic(0f, 1f, i * 1f / dur);
            bgSpRen.transform.localScale = Vector3.one * Easing.EaseOutCubic(1f, 0.95f, i * 1f / dur);
            typeSpRen.transform.localScale= Vector3.one * Easing.EaseOutCubic(1f, 0.8f, i * 1f / dur);
            yield return null;
        }
        readySpRen.sortingOrder = 3;
        ready = true;
    }

    public void StopHoldProgress(Coroutine start)
    {
        StopCoroutine(start);
        ready = false;
        readySpRen.color = Color.clear;
        readySpRen.sortingOrder = 0;
        readySpRen.transform.localScale = Vector3.zero;
        bgSpRen.transform.localScale = Vector3.one;
        typeSpRen.transform.localScale = Vector3.one;
    }

    public IEnumerator InvalidMove()
    {
        int dur = 5;
        Color redColor = readyColors[1];
        readySpRen.color = redColor;
        readySpRen.transform.localScale = Vector3.one;
        for (int i = 1; i <= dur; i++)
        {
            readySpRen.transform.localScale = Vector3.one * Easing.EaseOutCubic(0f, 1f, i * 1f / dur);
            yield return null;
        }
        dur = 70;
        readySpRen.sortingOrder = 3;
        for (int i = 1; i <= dur; i++)
        {
            if ((Mathf.FloorToInt(i / 10) % 2) == 0)
            {
                redColor.a= Easing.EaseOutCubic(1f, 0f, (i % 10) / 10f);
            }
            else
            {
                redColor.a= Easing.EaseOutCubic(0f, 1f, (i % 10) / 10f);
            }
            readySpRen.color = redColor;
            yield return null;
        }
        readySpRen.color = Color.clear;
        readySpRen.transform.localScale = Vector3.zero;
        readySpRen.sortingOrder = 0;

    }

    public static string GetDescription(int type, int subtype)
    {
        if (type == DESTROY_COLOR)
        {
            string color="";
            if (subtype == 0)
                color = "purple";
            if (subtype == 1)
                color = "peach";
            if (subtype == 2)
                color = "red";
            if (subtype == 3)
                color = "dark blue";
            if (subtype == 4)
                color = "green";
            if (subtype == 5)
                color = "light blue";

            return "Destroys all\n" + color + " tiles\non both boards";

        }
        if (type==ERASER)
        {
			return "Destroys two\ntiles of\nyour choice";
        }
        if (type == BOMBS)
        {
            return "Turns both\nplayable tiles\ninto bombs";
        }
        if (type == WILDS)
        {
            return "Turns both\nplayable tiles\ninto wild cards";
        }
        if (type == X2)
        {
            return "Doubles the\npoints on\none turn\n(can be stacked)";
        }
        if (type == ACTIVATE_SPECIAL)
        {
            return "Activates all\nexploding tiles\non both boards";
        }
        if (type == CLEAR_BOARD)
        {
            string board = "";
            if (subtype == 0)
                board = "upper";
            if (subtype == 1)
                board = "lower";
            return "Clears all\ntiles from the\n" + board + " board\n(-10 pts)";
        }

        return "";
    }
    public static string GetName(int type)
    {
        if (type == ERASER)
            return "2 Erasers";
        if (type == BOMBS)
            return "Bombs";
        if (type == X2)
            return "Multiplier";
        if (type == WILDS)
            return "Wild cards";
        if (type == DESTROY_COLOR)
            return "Thinner";
        if (type == ACTIVATE_SPECIAL)
            return "Blaster";
        if (type == CLEAR_BOARD)
            return "Wiper";

        return "";
    }


    /*
	
	
	/*void Update () {
	
	}*/
}
