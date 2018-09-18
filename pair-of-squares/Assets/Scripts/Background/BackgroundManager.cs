using UnityEngine;
using System.Collections;

public class BackgroundManager : MonoBehaviour {

    // Use this for initialization
    public GameObject[] backgrounds;
    public float xSpeed;

    private Vector3 speed;
    private Vector3 offset;
	void Start () {
        speed = new Vector3(xSpeed,0f,0f);
        offset = backgrounds[1].transform.position - backgrounds[0].transform.position;

    }
	
	// Update is called once per frame
	void Update () {
        for (int i = 0; i < backgrounds.Length; i++)
        {
            backgrounds[i].transform.position += speed * Time.deltaTime;
        }
        for (int i = 0; i < backgrounds.Length; i++)
        {
            if (xSpeed < 0)
            {
                if (backgrounds[i].transform.position.x < -20f)
                {
                    backgrounds[i].transform.position = backgrounds[1 - i].transform.position + offset;
                }
            }
            else
            {
                if (backgrounds[i].transform.position.x > 20f)
                {
                    backgrounds[i].transform.position = backgrounds[1 - i].transform.position - offset;
                }
            }
        }
    }
}
