using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OutLine : MonoBehaviour {

    static List<GameObject> lines = new List<GameObject>();

    // Use this for initialization
    public static void DrawRect(Rect rect, Color color, float lineWidth)
    {

        List<Vector3> points = new List<Vector3>();
        List<Vector3> inc = new List<Vector3>();
        LineRenderer lr;
        GameObject lgo;
        points.Add(new Vector3(rect.xMin, rect.yMin, 0f));
        points.Add(new Vector3(rect.xMax, rect.yMin, 0f));
        points.Add(new Vector3(rect.xMax, rect.yMax, 0f));
        points.Add(new Vector3(rect.xMin, rect.yMax, 0f));
        points.Add(new Vector3(rect.xMin, rect.yMin, 0f));

        inc.Add(new Vector3(lineWidth/2f,0f,0f));
        inc.Add(new Vector3(0f, lineWidth / 2f, 0f));
        inc.Add(new Vector3(-lineWidth / 2f, 0f, 0f));
        inc.Add(new Vector3(0f, -lineWidth / 2f, 0f));

        for (int i = 0; i < 4; i++)
        {
            lgo = new GameObject();
            lines.Add(lgo);
            lgo.transform.position = new Vector3(rect.xMin, rect.yMin, 0f);
            lr = lgo.AddComponent<LineRenderer>() as LineRenderer;
            lr.material = new Material(Shader.Find("Sprites/Default"));
            lr.material.SetColor("_Color", color);
            lr.SetWidth(lineWidth, lineWidth);
            lr.sortingLayerName = "UI";
            lr.sortingOrder = i;
            lr.SetVertexCount(2);
            lr.SetPosition(0, points[i] + inc[i]);
            lr.SetPosition(1, points[i + 1] + inc[i]);   
        }

    }

    public static IEnumerator DrawLine(Vector3 startPoint, Vector3 endPoint, Color color, float lineWidth, float wait=0f)
    {

        yield return new WaitForSeconds(wait);

        LineRenderer lr;
        GameObject lgo = new GameObject();
        Vector3 avgPoint = (startPoint + endPoint) / 2f;
        Vector3 curStPoint = avgPoint;
        Vector3 curEndPoint = avgPoint;
        lines.Add(lgo);
        lgo.transform.position = startPoint;
        lr = lgo.AddComponent<LineRenderer>() as LineRenderer;
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.material.SetColor("_Color", color);
        lr.SetWidth(lineWidth, lineWidth);
        lr.sortingLayerName = "Effectss";
        lr.sortingOrder = 0;
        lr.SetVertexCount(2);
        

        int dur = 15;
        float newX, newY;
        for (int i = 1; i <= dur; i++)
        {
            newX = Easing.EaseOutCubic(avgPoint.x, startPoint.x, i * 1f / dur);
            newY= Easing.EaseOutCubic(avgPoint.y, startPoint.y, i * 1f / dur);
            curStPoint = new Vector3(newX, newY, 0f);
            newX = Easing.EaseOutCubic(avgPoint.x, endPoint.x, i * 1f / dur);
            newY = Easing.EaseOutCubic(avgPoint.y, endPoint.y, i * 1f / dur);
            curEndPoint = new Vector3(newX, newY, 0f);
            lr.SetPosition(0, curStPoint);
            lr.SetPosition(1, curEndPoint);
            yield return null;
        }

        yield break;

    }

    public static IEnumerator RemoveLine(GameObject lgo, float wait=0f)
    {
        yield return new WaitForSeconds(wait);
        LineRenderer lr = lgo.GetComponent<LineRenderer>() as LineRenderer;
        
        int dur = 15;

        Color color = lr.material.GetColor("_Color");
        for (int i = 1; i <= dur; i++)
        {
            color-= new Color(0f, 0f, 0f, 1f*i/dur);
            lr.material.SetColor("_Color", color);
            yield return null;
        }
        Destroy(lgo);
    }

    public static void RemoveAllLines()
    {
        if (lines.Count == 0)
            return;

        while (lines.Count>0)
        {
            GameObject ll = lines[0];
            GameManager.instance.StartCoroutine(OutLine.RemoveLine(ll,Mathf.FloorToInt(lines.Count / 2)*0.07f));
            lines.RemoveAt(0);
        }

        lines = new List<GameObject>();
    }


}
