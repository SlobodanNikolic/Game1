using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pokega;

public class Tile : MonoBehaviour {

   [HideInInspector] public int status;
   [HideInInspector] public int power;

    public Sprite emptyTile;
    public Sprite maskedTile;
    public Sprite wildTile;
    public Sprite cbWildTile;
    public Sprite eraserTile;
    public Sprite shadow;
    public Sprite eraserShadow;
    public Sprite highlight;
    public Sprite emptyHighlight;
    public Sprite[] colorSprites;
    public Sprite[] powerSprites;
    public Sprite[] cbColorSprites;
    public Sprite[] cbPowerSprites;
    [HideInInspector] public int b, i, j;

    public const int NUM_OF_COLORS=6;
    public const int EMPTY = -1;
    public const int MASKED = -2;
    public const int WILD = -3;
    public const int ERASER = -4;

    public const int P_NONE = -1;
    public const int P_COL = 0;
    public const int P_ROW = 1;
    public const int P_BOMB = 2;
    public const int P_ROWCOL = 3;
    public const int P_COL_COL = 4;
    public const int P_BIGBOMB = 5;
    public const int P_ROWCOL_COL = 6;
    public const int P_COLCOLCOL = 7;
    public const int P_ROWROWROW = 8;
    public const int P_ROWROWROWCOLCOLCOL = 9;
    public const int P_COLCOLCOL_COLCOLCOL = 10;



    private SpriteRenderer colSpRen; //color
    private SpriteRenderer powSpRen; //power
    private SpriteRenderer shadSpRen; //shadow
    private SpriteRenderer hiliSpRen; //highlight 
    private SpriteRenderer patSpRen; //pattern

    private bool highlighting;
    private bool inTransition;

	private float repelPower;

    void Awake()
    {
        colSpRen = transform.Find("Color").gameObject.GetComponent<SpriteRenderer>();
        powSpRen = transform.Find("Power").gameObject.GetComponent<SpriteRenderer>();
        shadSpRen = transform.Find("Shadow").gameObject.GetComponent<SpriteRenderer>();
        hiliSpRen = transform.Find("Highlight").gameObject.GetComponent<SpriteRenderer>();
        patSpRen = transform.Find("Pattern").gameObject.GetComponent<SpriteRenderer>();
        colSpRen.sprite = emptyTile;
        powSpRen.sprite = null;
        shadSpRen.sprite = null;
        hiliSpRen.sprite = null;
        hiliSpRen.color = new Color(1f, 1f, 1f, 0f);
        patSpRen.sprite = null;

        inTransition = false;
        highlighting = false;
		repelPower = 1f;
    }
    

	public void UpdateStatus(int newStatus, bool clear=false, bool keepScale=false)
    {
        int oldStatus = status;
        status = newStatus;
       

		if (!keepScale)
        	StartCoroutine(Transition(oldStatus, newStatus, clear));
		else
			colSpRen.sprite = GetSpriteFromStatus(status);

        
    }

    public void UpdatePower(int newPower,bool slow=true)
    {
        power = newPower;
        if (power == P_NONE)
            powSpRen.sprite = null;
        else
        {

            if (slow)
            {
                StartCoroutine(ShowPower(newPower));
            }
            else
            {
                powSpRen.sprite = GetSpriteFromPower(power);
            }
        }
    }

    private IEnumerator ShowPower(int newPower,float wait=0.4f)
    {
        yield return new WaitForSeconds(wait);
        hiliSpRen.sprite = highlight;
        int dur = 20;
        for (int i = 1; i <= dur; i++)
        {
            gameObject.transform.localScale = new Vector3(Random.Range(0.9f,1.1f), Random.Range(0.9f, 1.1f), Random.Range(0.9f, 1.1f));
            hiliSpRen.color = new Color(1f, 1f, 1f, Easing.EaseOutCubic(0f,1f,i*1f/dur));
			if (i % 5 == 1)
				SoundManager.instance.PlaySound(SoundManager.instance.specialTileShuffleSfx,-1,true);
            yield return null;
        }
        gameObject.transform.localScale = Vector3.one;
        powSpRen.sprite = GetSpriteFromPower(power);
        powSpRen.color -= new Color(0f, 0f, 0f, 1f);
        while (powSpRen.color.a < 1f)
        {
            powSpRen.color += new Color(0f, 0f, 0f, 0.05f);
            hiliSpRen.color -= new Color(0f, 0f, 0f, 0.1f);
            yield return null;
        }
        hiliSpRen.sprite = null;
    }

    public void UpdateTile(int newStatus, int newPower, bool clear=false)
    {
        UpdateStatus(newStatus,clear);
        UpdatePower(newPower,false);
    }

    private IEnumerator Transition(int oldStatus, int newStatus, bool clear=false)
    {

        while (inTransition)
        {
            yield return null;
        }
        int dur;
        float scale;
        gameObject.transform.rotation = Quaternion.identity;
        gameObject.transform.localScale = Vector3.one;
        inTransition = true;

        //clear
        if (clear)
            {
            yield return new WaitForSeconds(Random.Range(0f, 0.2f));
            hiliSpRen.sprite = highlight;
            dur = 30;
            for (int i = 1; i <= dur; i++)
            {
                //gameObject.transform.localScale = new Vector3(Random.Range(0.9f, 1.1f), Random.Range(0.9f, 1.1f), Random.Range(0.9f, 1.1f));
                hiliSpRen.color = new Color(1f, 1f, 1f, Easing.EaseOutCubic(0f, 1f, i * 1f / dur));
                yield return null;
            }
            gameObject.transform.localScale = Vector3.one;
            colSpRen.sprite = GetSpriteFromStatus(status);

            while (hiliSpRen.color.a > 0f)
            {
                hiliSpRen.color -= new Color(0f, 0f, 0f, 0.08f);
                yield return null;
            }
            hiliSpRen.sprite = null;
            inTransition = false;
            yield break;
        }

        //empty->color
        if ((oldStatus == EMPTY) && (newStatus != EMPTY) && (newStatus != MASKED))
        {
            dur = 5;
            colSpRen.sprite = GetSpriteFromStatus(status);
           
            for (int i = 1; i <= dur; i++)
            {
                scale = Easing.Linear(0.9f, 0.85f, (i * 1f / dur));
                gameObject.transform.localScale = new Vector3(scale, scale, scale);
                yield return null;
            }
            for (int i = 1; i <= dur; i++)
            {
                scale = Easing.Linear(0.85f, 1f, (i * 1f / dur));
                gameObject.transform.localScale = new Vector3(scale, scale, scale);
                yield return null;
            }
            inTransition = false;
            yield break;
        }

        //color->wild
        if ((newStatus == WILD) && (oldStatus != EMPTY))
        {
            yield return new WaitForSeconds(0.4f);
            dur = 60;
            for (int i = 1; i <= dur; i++)
            {
                gameObject.transform.localScale = new Vector3(Random.Range(0.9f, 1.1f), Random.Range(0.9f, 1.1f), Random.Range(0.9f, 1.1f));
                if (i % 5 == 1)
                {
                    colSpRen.sprite = GetSpriteFromStatus(Mathf.FloorToInt(i * 1f / 5) % NUM_OF_COLORS);
                    SoundManager.instance.PlaySound(SoundManager.instance.wildCardShuffleSfx,-1,true);
                }
                yield return null;
            }
            hiliSpRen.sprite = highlight;
            colSpRen.sprite = GetSpriteFromStatus(status);
            gameObject.transform.localScale = Vector3.one;
            dur = 10;
            for (int i = 1; i <= dur; i++)
            {
                hiliSpRen.color = new Color(1f, 1f, 1f, Easing.EaseOutCubic(1f, 0f, i * 1f / dur));
                yield return null;
            }

            hiliSpRen.sprite = null;
            inTransition = false;
            yield break;
        }

        //color->color
        if ((oldStatus != EMPTY) && (newStatus != EMPTY) && (newStatus!=MASKED) && (oldStatus!=MASKED) && (newStatus != ERASER))
        {
            colSpRen.sprite = GetSpriteFromStatus(status);
            /*dur = 12;
            gameObject.transform.localScale = Vector3.zero;
            for (int i = 1; i <= dur; i++)
            {
                scale = Easing.EaseOutSine(0f, 1f, (i * 1f / dur));
                Debug.Log(scale);
                gameObject.transform.localScale = new Vector3(scale, scale, scale);
                yield return null;
            }*/
            inTransition = false;
            yield break;
        }


        //color->empty, masked or eraser
        if (((oldStatus >= 0) || (oldStatus == WILD)) && ((newStatus == EMPTY) || (newStatus == MASKED) || (newStatus == ERASER)))
        {
            dur = 12;
            SoundManager.instance.PlayNextExplosion();
            for (int i = 1; i <= dur; i++)
            {
                scale = Easing.EaseOutBack(1f, 0f, i * 1f / dur);
                gameObject.transform.localScale = Vector3.one * scale;
                yield return null;
            }
            if (oldStatus != WILD)
            {
                StartCoroutine(Explosion.Explode(transform.position, GameManager.instance.colors[oldStatus]));
				StarsManager.instance.Repel (transform.position, repelPower);
                StartCoroutine(NumMsg.ShowNumMsg(transform.position, GameManager.instance.multiplier, GameManager.instance.colors[oldStatus]));
            }
            else
            {
                StartCoroutine(Explosion.Explode(transform.position, Color.clear));
				StarsManager.instance.Repel (transform.position, repelPower);
                StartCoroutine(NumMsg.ShowNumMsg(transform.position, GameManager.instance.multiplier, Color.white));
            }


            colSpRen.sprite = GetSpriteFromStatus(status);

            for (int i = 1; i <= dur; i++)
            {
                scale = Easing.EaseOutBack(0f, 1f, i * 1f / dur);
                gameObject.transform.localScale = Vector3.one * scale;
                yield return null;
            }
            gameObject.transform.rotation = Quaternion.identity;
            gameObject.transform.localScale = Vector3.one;
            inTransition = false;
            yield break;

        }

        //masked->empty
        if ((oldStatus == MASKED) && (newStatus == EMPTY))
        {
            dur = 8;
            for (int i = 1; i <= dur; i++)
            {
                scale = Easing.EaseOutBack(1f, 0f, i * 1f / dur);
                gameObject.transform.localScale = Vector3.one * scale;
                yield return null;
            }
            colSpRen.sprite = GetSpriteFromStatus(status);

            for (int i = 1; i <= dur; i++)
            {
                scale = Easing.EaseOutBack(0f, 1f, i * 1f / dur);
                gameObject.transform.localScale = Vector3.one * scale;
                yield return null;
            }
            gameObject.transform.rotation = Quaternion.identity;
            gameObject.transform.localScale = Vector3.one;
            inTransition = false;
            yield break;
        }

        //empty->masked
        if ((oldStatus == EMPTY) && (newStatus == MASKED))
        {
            colSpRen.sprite = GetSpriteFromStatus(status);
            inTransition = false;
            yield break;
        }


    }

    public void toggleShadow(bool enable)
    {
        if ((enable) && (shadSpRen.sprite == null))
        {
            if (!IsEraser())
                shadSpRen.sprite = shadow;
            else
                shadSpRen.sprite = eraserShadow;
        }
        if (!enable)
            shadSpRen.sprite = null;
    }

	public IEnumerator toggleOverlay(bool enable)
	{

		if (enable)
		{
			highlighting = true;
			float maxAlpha=0.6f;

			hiliSpRen.sprite = highlight;
			Color hlColor=new Color(53f/255f,36f/255f,77f/255f,0f);
			if (Mathf.Abs(hiliSpRen.color.r-hlColor.r)>0.01f)
				hiliSpRen.color = hlColor;

			while (hiliSpRen.color.a < maxAlpha)
			{
				hiliSpRen.color += new Color(0f, 0f, 0f, maxAlpha / 10f);
				yield return null;
				if (!highlighting)
					yield break;
			}
		}
		else
		{
			highlighting = false;
			hiliSpRen.sprite = highlight;
			while (hiliSpRen.color.a > 0f)
			{
				hiliSpRen.color -= new Color(0f, 0f, 0f, 0.02f);
				yield return null;
				if (highlighting)
					yield break;
			}
			hiliSpRen.sprite = null;
		}
	}



    public IEnumerator toggleHighlight(bool enable)
    {

        if (enable)
        {
            highlighting = true;
            float maxAlpha;

            hiliSpRen.sprite = emptyHighlight;
			hiliSpRen.color = Color.white;
            maxAlpha = 1f;

            while (hiliSpRen.color.a < maxAlpha)
            {
                hiliSpRen.color += new Color(0f, 0f, 0f, maxAlpha / 10f);
                yield return null;
                if (!highlighting)
                    yield break;
            }
        }
        else
        {
            highlighting = false;
            hiliSpRen.sprite = emptyHighlight;
            while (hiliSpRen.color.a > 0f)
            {
                hiliSpRen.color -= new Color(0f, 0f, 0f, 0.07f);
                yield return null;
                if (highlighting)
                    yield break;
            }
            hiliSpRen.sprite = null;
        }
    }

    public void SetBIJ(int newB,int newI,int newJ)
    {
        b = newB;
        i = newI;
        j = newJ;
    }

    public bool IsEqual(int otherStatus)
    {
        if ((this.status == EMPTY) || (this.status==MASKED) || (otherStatus == EMPTY) || (otherStatus == MASKED))
            return false;

        return ((this.status == otherStatus) || (this.status==WILD) || (otherStatus == WILD));
    }

    private Sprite GetSpriteFromStatus(int st)
    {
        if (st == EMPTY)
            return emptyTile;
        if (st == WILD)
        {
            if (App.colorBlindMode)
                return cbWildTile;
           return wildTile;
        }
        if (st == MASKED)
            return maskedTile;
        if (st == ERASER)
            return eraserTile;

        if (App.colorBlindMode)
            return cbColorSprites[st];

        return colorSprites[st];
    }

    private Sprite GetSpriteFromPower(int pow)
    {
		if (pow == P_NONE)
			return null;
			
        if (App.colorBlindMode)
            return cbPowerSprites[pow];

        return powerSprites[pow];
    }

    public Color GetColor()
    {
        if (IsColored())
            return GameManager.instance.colors[status];
        if (IsWild())
            return GameManager.instance.colors[Random.Range(0, NUM_OF_COLORS)];

        return new Color(0f, 0f, 0f);
    }

    public void AddRandomPower()
    {
        int rand = Random.Range(0,66);
        int pow = 10 - Mathf.FloorToInt((Mathf.Sqrt(8 * rand + 1) - 1) / 2);
        UpdatePower(pow);
    }

    public bool IsEmpty()
    {
        return (this.status == EMPTY);
    }

    public bool IsEraser()
    {
        return (this.status == ERASER);
    }

    public bool IsMasked()
    {
        return (this.status == MASKED);
    }
    public bool IsWild()
    {
        return (this.status == WILD);
    }

    public bool IsColored()
    {
        return ((this.status >= 0) && (this.status < NUM_OF_COLORS));
    }

}
