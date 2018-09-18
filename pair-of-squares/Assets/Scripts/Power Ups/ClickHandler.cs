using UnityEngine;
using System.Collections;

public class ClickHandler : MonoBehaviour
{
	private BoxCollider2D bc;

    void Start()
    {
		bc = gameObject.GetComponent<BoxCollider2D> ();
        if (bc == null)
        {
            bc=gameObject.AddComponent<BoxCollider2D>();
        }
		bc.offset = new Vector2 (0f, 0f);
		bc.size = new Vector2 (3f,3f);
    }

    void OnMouseDown()
    {
        StartCoroutine(GameManager.instance.PickPowerUp(gameObject));
    }

}