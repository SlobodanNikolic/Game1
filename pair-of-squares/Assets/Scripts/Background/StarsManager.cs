using UnityEngine;
using System.Collections;

public class StarsManager : MonoBehaviour {

    // Use this for initialization
    public GameObject starPrefab;
    public Sprite[] starSprites;
    public int numOfStars = 20;
    public float maxSpeed = 0.2f;
	public float acceleration=0.999f;
	public static StarsManager instance;

    private GameObject[] stars;
    private Vector3[] starSpeeds;
    private int[] starDurations;

    private Vector3 rotInc = new Vector3(0f,0f,0.3f);

    void Start () {
        stars = new GameObject[numOfStars];
        starSpeeds = new Vector3[numOfStars];
        starDurations = new int[numOfStars];
       
        for (int i = 0; i < numOfStars; i++)
        {
            Vector3 starStPos = new Vector3(Random.Range(-5f, 5f), Random.Range(-7f, 7f), 0f);
            stars[i] = Instantiate(starPrefab, starStPos, Quaternion.identity) as GameObject;
            stars[i].transform.SetParent(transform);
            starSpeeds[i] = new Vector3(Random.Range(-maxSpeed, maxSpeed), Random.Range(-maxSpeed, maxSpeed), 0f);
            starDurations[i] = 10;
            stars[i].GetComponent<SpriteRenderer>().color -= new Color(0f,0f,0f,1f);
            stars[i].GetComponent<SpriteRenderer>().sprite = starSprites[Random.Range(0,starSprites.Length)];
            float scale = Random.Range(0.3f, 0.6f);
            stars[i].transform.localScale = new Vector3(scale, scale, scale);
            stars[i].transform.Rotate(new Vector3(0f,0f,Random.Range(0f,360)));
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
		instance = this;
        for (int i = 0; i < numOfStars; i++)
        {
            stars[i].transform.position += starSpeeds[i]*Time.deltaTime;
            stars[i].transform.Rotate(rotInc);
            starDurations[i]--;
            
            if (starDurations[i] <= 20)
            {
                stars[i].GetComponent<SpriteRenderer>().color -= new Color(0, 0, 0, 0.05f);
            }
            else if (stars[i].GetComponent<SpriteRenderer>().color.a < 0.96f)
            {
                stars[i].GetComponent<SpriteRenderer>().color += new Color(0, 0, 0, 0.05f);
            }

            if (starDurations[i]<=0)
            {
                stars[i].transform.position= new Vector3(Random.Range(-5f, 5f), Random.Range(-7f, 7f), 0f);
                starDurations[i] = Random.Range(120, 300);
                starSpeeds[i] = new Vector3(Random.Range(-maxSpeed, maxSpeed), Random.Range(-maxSpeed, maxSpeed), 0f);
                float scale = Random.Range(0.3f, 0.6f);
                stars[i].transform.localScale = new Vector3(scale, scale, scale);
                stars[i].GetComponent<SpriteRenderer>().sprite = starSprites[Random.Range(0, starSprites.Length)];
                //stars[i].GetComponent<SpriteRenderer>().color = GameManager.instance.colors[Random.Range(0, Tile.NUM_OF_COLORS)] - new Color(0, 0, 0, 1f);
            }

			if (Mathf.Abs(starSpeeds [i].x)>maxSpeed || Mathf.Abs(starSpeeds [i].y)>maxSpeed)
				starSpeeds [i] *= acceleration;

        }



    }

	public void Repel(Vector3 position, float power)
	{
		for (int i = 0; i < numOfStars; i++) 
		{
			//power /= Vector3.Distance (stars [i].transform.position, position);
			starSpeeds[i]+=Vector3.Normalize(stars [i].transform.position - position)*power;
		}
	}
}
