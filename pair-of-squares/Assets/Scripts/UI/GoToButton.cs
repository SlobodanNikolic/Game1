using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class GoToButton : MonoBehaviour {

    // Use this for initialization
    public string goToScene="";
    public UnityEvent callMethod;
    public Sprite upSprite;
    public Sprite downSprite;
    public bool clickable=true;

    private BoxCollider2D bc;
    private SpriteRenderer sr;
    

    void Awake()
    {
        bc = gameObject.GetComponent<BoxCollider2D>();
        sr = gameObject.GetComponent<SpriteRenderer>();

        if (upSprite!=null)
            sr.sprite = upSprite;

        if (!clickable)
            Disable();


        if (bc == null)
        {
            bc = gameObject.AddComponent<BoxCollider2D>();
        }
    }

    // Update is called once per frame
    void OnMouseDown ()
    {
        if (!clickable)
            return;

        SoundManager.instance.PlaySound(SoundManager.instance.clickSfx, -1, true);
        if (downSprite!=null)
            sr.sprite = downSprite;
	}

    void OnMouseUp()
    {
        if (!clickable)
            return;

        if (upSprite!=null)
            sr.sprite = upSprite;
        if (callMethod!=null)
        {
            callMethod.Invoke();
        }
        if (goToScene!="")
        {
            //StartCoroutine(Transition.instance.StartTransition(goToScene));
            //ScreenManager.instance.ChangeScreen("GameScreen");
            clickable = false;
        }


    }

    public void Disable()
    {
        clickable = false;
        sr.color = new Color(1f, 1f, 1f, 0.3f);
    }

    public void Enable()
    {
        clickable = true;
        sr.color = Color.white;
    }

}
