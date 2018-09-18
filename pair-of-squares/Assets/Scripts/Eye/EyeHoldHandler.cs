using UnityEngine;
using System.Collections;

public class EyeHoldHandler : MonoBehaviour {


    public GameObject[] thingsToHide;
    private BoxCollider2D bc;
    private bool mouseInDown;


    void Start()
    {
        mouseInDown = false;
        bc = gameObject.GetComponent<BoxCollider2D>();
        if (bc == null)
        {
            bc = gameObject.AddComponent<BoxCollider2D>();
        }
		bc.offset = Vector2.zero;
		bc.size = Vector2.one * 1.2f;
    }

    void OnMouseDown()
    {
        mouseInDown = true;
        StartCoroutine(HideThings());
    }

    void OnMouseUp()
    {
        mouseInDown = false;
        StartCoroutine(ShowThings());
    }

    private IEnumerator HideThings()
    {
        for (int i = 0; i < thingsToHide.Length; i++)
        {
            thingsToHide[i].transform.localScale = Vector3.zero;
        }

       yield break;
    }

    private IEnumerator ShowThings()
    {
        for (int i = 0; i < thingsToHide.Length; i++)
        {
            thingsToHide[i].transform.localScale = Vector3.one;
        }

        yield break;
    }

}
