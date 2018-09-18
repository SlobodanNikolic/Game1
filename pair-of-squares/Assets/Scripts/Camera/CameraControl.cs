using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {

    // Use this for initialization
    //public float width=16;
    //public float height=9;
    private Vector3 startPosition;

    void Start()
    {

        startPosition = transform.position;
        float targetaspect = (float)Screen.width / (float)Screen.height;

        // determine the game window's current aspect ratio
        float windowaspect = (float)Screen.width / (float)Screen.height;

        // current viewport height should be scaled by this amount
        float scaleheight = windowaspect / targetaspect;

        // obtain camera component so we can modify its viewport
        Camera camera = GetComponent<Camera>();

        // if scaled height is less than current height, add letterbox
        if (scaleheight < 1.0f)
        {
            Rect rect = camera.rect;

            rect.width = 1.0f;
            rect.height = scaleheight;
            rect.x = 0;
            rect.y = (1.0f - scaleheight) / 2.0f;

            camera.rect = rect;
        }
        else // add pillarbox
        {
            float scalewidth = 1.0f / scaleheight;

            Rect rect = camera.rect;

            rect.width = scalewidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scalewidth) / 2.0f;
            rect.y = 0;

            camera.rect = rect;
        }
    }

    public IEnumerator Shake(float power=0.1f, float duration=0.1f)
    {
        float newX;
        float newY;
        while (duration > 0)
        {
            newX = startPosition.x + Random.Range(-power, power);
            newY = startPosition.y + Random.Range(-power, power);
            transform.position = new Vector3(newX, newY, transform.position.z);
            duration -= Time.deltaTime;
            yield return null;
        }

        transform.position = startPosition;
    }




    
}
