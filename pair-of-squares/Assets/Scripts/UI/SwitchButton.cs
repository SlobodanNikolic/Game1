using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using Pokega;

public class SwitchButton : MonoBehaviour
{

    public string type;
    public Sprite onSprite;
    public Sprite offSprite;
    public bool clickable = true;

    private BoxCollider2D bc;
    private SpriteRenderer sr;


    void Awake()
    {
        bc = gameObject.GetComponent<BoxCollider2D>();
        sr = gameObject.GetComponent<SpriteRenderer>();

        type = type.ToLower();

        if (GetValue())
            sr.sprite = onSprite;
        else
            sr.sprite = offSprite;

        if (!clickable)
            sr.color = new Color(1f, 1f, 1f, 0.2f);


        if (bc == null)
        {
            bc = gameObject.AddComponent<BoxCollider2D>();
        }

    }
   

    void OnMouseUp()
    {

        if (!clickable)
            return;

        SwitchValue();

        if (GetValue())
            sr.sprite = onSprite;
        else
            sr.sprite = offSprite;

        SoundManager.instance.PlaySound(SoundManager.instance.clickSfx, -1, true);
        //sr.sprite = upSprite;
    }


    private bool GetValue()
    {
        if (type == "sound")
            return SoundManager.instance.soundOn;

        if (type == "music")
            return SoundManager.instance.musicOn;

        if (type == "colorblindmode")
            return App.colorBlindMode;

        return false;
    }

    private void SwitchValue()
    {
        if (type == "sound")
        {
            SoundManager.instance.soundOn = !SoundManager.instance.soundOn;
        }
        if (type == "music")
        {
            SoundManager.instance.musicOn = !SoundManager.instance.musicOn;
            SoundManager.instance.PauseUnPauseMusic();
        }
        if (type == "colorblindmode")
        {
            App.colorBlindMode = !App.colorBlindMode;
        }
    }

    public void Disable()
    {
        clickable = false;
        sr.color = new Color(1f, 1f, 1f, 0.2f);
    }

    public void Enable()
    {
        clickable = true;
        sr.color = Color.white;
    }

}
