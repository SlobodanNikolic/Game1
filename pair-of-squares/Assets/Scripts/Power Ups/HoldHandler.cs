using UnityEngine;
using System.Collections;

public class HoldHandler : MonoBehaviour
{

    public static GameObject heldObject;
    private float lastMouseDownTime;
    private Coroutine startHold;
    private BoxCollider2D bc;


    void Start()
    {
        bc = gameObject.GetComponent<BoxCollider2D>();
        if (bc == null)
        {
            bc=gameObject.AddComponent<BoxCollider2D>();
            bc.size = new Vector2(1.2f,1.2f);
        }
    }

    void OnMouseDown()
    {
        if (GameManager.instance.pause)
            return;

        heldObject = gameObject;
        //startHold = StartCoroutine(gameObject.GetComponent<PowerUp>().StartHoldProgress());
    }

	void OnMouseUpAsButton()
    {
        if (GameManager.instance.pause)
            return;

        //if (gameObject.GetComponent<PowerUp>().ready)
        //{
            gameObject.GetComponent<PowerUp>().Activate();
        //}
        //else
        //{
            //show desc
        //}

        //if (startHold!=null)
          //  gameObject.GetComponent<PowerUp>().StopHoldProgress(startHold);

        //startHold = null;
        heldObject = null;
    }

    public void Remove()
    {
        if (gameObject.GetComponent<BoxCollider2D>() != null)
            Destroy(gameObject.GetComponent<BoxCollider2D>());
        Destroy(this);
    }
}