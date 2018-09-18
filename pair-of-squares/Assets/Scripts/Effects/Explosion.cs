using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour {

    // Use this for initialization
    public static GameObject particlePrefab;
    public static Object[] particleSprites;

	void Start ()
    {
	
	}

	public static IEnumerator Explode(Vector3 position, Color color,int numOfParticles=12, float radius=0.6f, float duration=0.5f, float delay=0f, Transform parent=null)
    {

        if (particlePrefab == null)
        {
            particlePrefab = Resources.Load("Particle") as GameObject;
        }
        if (particleSprites == null)
        {
            particleSprites = Resources.LoadAll("particles");
        }
   		
		if (parent == null)
			parent = GameManager.container.transform;
        
        yield return new WaitForSeconds(delay);
        
        GameObject[] particles = new GameObject[numOfParticles];
        Vector3[] endPoints = new Vector3[numOfParticles];
        float currentTime = 0f;
        for (int i = 0; i < numOfParticles; i++)
        {
            particles[i]=Instantiate(particlePrefab, position, Quaternion.identity) as GameObject;
			particles [i].transform.SetParent (parent);
            particles[i].GetComponent<SpriteRenderer>().sprite = particleSprites[Random.Range(0, particleSprites.Length)] as Sprite;
            if (color!=Color.clear)
                particles[i].GetComponent<SpriteRenderer>().color = color;
            else
                particles[i].GetComponent<SpriteRenderer>().color = GameManager.instance.colors[Random.Range(0,Tile.NUM_OF_COLORS)];

            float angle = Random.Range(0f, 2 * Mathf.PI);
            endPoints[i] = new Vector3(position.x + Mathf.Cos(angle) * radius, position.y + Mathf.Sin(angle) * radius, 0f);
            particles[i].transform.Rotate(0f, 0f, angle*180/Mathf.PI-90);

        }
        while (currentTime < duration+Time.deltaTime)
        {
            for (int i = 0; i < numOfParticles; i++)
            {
                float newX = Easing.EaseOutCubic(position.x,endPoints[i].x,currentTime/duration);
                float newY = Easing.EaseOutCubic(position.y, endPoints[i].y, currentTime / duration);
                particles[i].transform.localScale = Vector3.one * Easing.EaseInCubic(1f, 0f, currentTime / duration);
                particles[i].transform.position = new Vector3(newX, newY, 0f);
                
            }

            yield return null;
            currentTime += Time.deltaTime;
        }

        for (int i = 0; i < numOfParticles; i++)
        {
            Destroy(particles[i]);
        }
        
    }
	
	
}
