using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LineTrace : MonoBehaviour {

	// Use this for initialization
    private static GameObject circlePrefab;

	public static bool lineIsActive = false;

	void Start () {
	    
	}

    public static IEnumerator DrawCurve(Vector3 start, Vector3 end, Color color, float lineWidth=0.3f,float duration = 0.4f, float delay=0f)
    {
        if (circlePrefab==null)
            circlePrefab = Resources.Load("LineEnd") as GameObject;

        if (duration < 0.01f)
            yield break;

		lineIsActive = true;
        yield return new WaitForSeconds(delay);

        GameObject startCircle = Instantiate(circlePrefab);
        GameObject endCircle = Instantiate(circlePrefab);
        GameObject line = new GameObject();
        List<Vector3> points=new List<Vector3>();
        int maxNumOfPoints = 9;
        float t;
        float cF = Mathf.Sign(start.x)*(1.2f+lineWidth*2f);
        int sortingOrder = Random.Range(0,9999);
        Vector3 currPoint = Vector3.zero;
        float currDuration = 0f;
        line.transform.position = start;
        line.AddComponent<LineRenderer>();
        LineRenderer lr = line.GetComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.material.SetColor("_Color", color);
        lr.SetWidth(lineWidth, lineWidth);
        lr.sortingLayerName = "Effects";
        lr.sortingOrder = sortingOrder;

        startCircle.transform.position = start;
        startCircle.GetComponent<SpriteRenderer>().color = color;
        startCircle.transform.localScale = Vector3.one * lineWidth;
        startCircle.GetComponent<SpriteRenderer>().sortingLayerName = "Effects";
        startCircle.GetComponent<SpriteRenderer>().sortingOrder = sortingOrder;

        endCircle.transform.position = start;
        endCircle.GetComponent<SpriteRenderer>().color = color;
        endCircle.transform.localScale = Vector3.one * lineWidth;
        endCircle.GetComponent<SpriteRenderer>().sortingLayerName = "Effects";
        endCircle.GetComponent<SpriteRenderer>().sortingOrder = sortingOrder;

        while (currDuration < duration + Time.deltaTime)
        {
            t = Mathf.Min(duration, currDuration) / duration;
            currPoint.x = Easing.Linear(start.x, end.x, t) + cF * Mathf.Sin(Mathf.PI * t);
            currPoint.y = Easing.EaseInCubic(start.y, end.y, t);
            points.Add(currPoint);
            
            if (points.Count > maxNumOfPoints)
                points.RemoveAt(0);

            lr.SetVertexCount(points.Count);
            for (int i = 0;i<points.Count; i++)
            {
                lr.SetPosition(i, points[i]);
            }
            startCircle.transform.position = points[points.Count - 1];
            endCircle.transform.position = points[0];
            yield return null;
            currDuration += Time.deltaTime;
			lineIsActive = true;
        }

        while (points.Count > 0)
        {
            lr.SetVertexCount(points.Count);
            for (int i = 0; i < points.Count; i++)
            {
                lr.SetPosition(i, points[i]);
            }
            startCircle.transform.position = points[points.Count - 1];
            endCircle.transform.position = points[0];
            startCircle.transform.localScale = Vector3.one * lineWidth * 0.1f * points.Count;
            endCircle.transform.localScale = Vector3.one * lineWidth * 0.1f * points.Count;
            lr.SetWidth(lineWidth * 0.1f * points.Count, lineWidth * 0.1f * points.Count);
            yield return null;
            points.RemoveAt(0);
			lineIsActive = true;
        }
        GameObject.Destroy(line);
        GameObject.Destroy(startCircle);
        GameObject.Destroy(endCircle);
		lineIsActive = false;
    }

    public static IEnumerator DrawLine(Vector3 start, Vector3 end, Color color, float lineWidth = 0.3f, float duration = 0.4f, float delay = 0f)
    {

        if (circlePrefab == null)
            circlePrefab = Resources.Load("LineEnd") as GameObject;

        if (duration < 0.01f)
            yield break;

		lineIsActive = true;
        yield return new WaitForSeconds(delay);

        GameObject startCircle = Instantiate(circlePrefab);
        GameObject endCircle = Instantiate(circlePrefab);
        GameObject line = new GameObject();
        List<Vector3> points = new List<Vector3>();
        int maxNumOfPoints = 9;
        float t;
        int sortingOrder = Random.Range(0, 9999);
        Vector3 currPoint = Vector3.zero;
        float currDuration = 0f;
        line.transform.position = start;
        line.AddComponent<LineRenderer>();
        LineRenderer lr = line.GetComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.material.SetColor("_Color", color);
        lr.SetWidth(lineWidth, lineWidth);
        lr.sortingLayerName = "Effects";
        lr.sortingOrder = sortingOrder;

        startCircle.transform.position = start;
        startCircle.GetComponent<SpriteRenderer>().color = color;
        startCircle.transform.localScale = Vector3.one * lineWidth;
        startCircle.GetComponent<SpriteRenderer>().sortingLayerName = "Effects";
        startCircle.GetComponent<SpriteRenderer>().sortingOrder = sortingOrder;

        endCircle.transform.position = start;
        endCircle.GetComponent<SpriteRenderer>().color = color;
        endCircle.transform.localScale = Vector3.one * lineWidth;
        endCircle.GetComponent<SpriteRenderer>().sortingLayerName = "Effects";
        endCircle.GetComponent<SpriteRenderer>().sortingOrder = sortingOrder;

        while (currDuration < duration + Time.deltaTime)
        {
            t = Mathf.Min(duration, currDuration) / duration;
            currPoint.x = Easing.EaseOutExpo(start.x, end.x, t);
            currPoint.y = Easing.EaseOutExpo(start.y, end.y, t);
            points.Add(currPoint);

            if (points.Count > maxNumOfPoints)
                points.RemoveAt(0);

            lr.SetVertexCount(points.Count);
            for (int i = 0; i < points.Count; i++)
            {
                lr.SetPosition(i, points[i]);
            }
            startCircle.transform.position = points[points.Count - 1];
            endCircle.transform.position = points[0];
            yield return null;
            currDuration += Time.deltaTime;
			lineIsActive = true;
        }

        while (points.Count > 0)
        {
            lr.SetVertexCount(points.Count);
            for (int i = 0; i < points.Count; i++)
            {
                lr.SetPosition(i, points[i]);
            }
            startCircle.transform.position = points[points.Count - 1];
            endCircle.transform.position = points[0];
            startCircle.transform.localScale = Vector3.one * lineWidth * 0.1f * points.Count;
            endCircle.transform.localScale = Vector3.one * lineWidth * 0.1f * points.Count;
            lr.SetWidth(lineWidth * 0.1f * points.Count, lineWidth * 0.1f * points.Count);
            yield return null;
            points.RemoveAt(0);
			lineIsActive = true;
            
        }
        GameObject.Destroy(line);
        GameObject.Destroy(startCircle);
        GameObject.Destroy(endCircle);
		lineIsActive = false;
    }

}
