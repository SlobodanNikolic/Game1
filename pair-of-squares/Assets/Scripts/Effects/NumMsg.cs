using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class NumMsg : MonoBehaviour {

    private static GameObject numMsgPrefab;
    
    public static IEnumerator ShowNumMsg(Vector3 position, int num, Color color)
    {
        if (numMsgPrefab == null)
            numMsgPrefab = Resources.Load("NumMsg") as GameObject;

        GameObject numMsg=Instantiate(numMsgPrefab, position, Quaternion.identity) as GameObject;
		numMsg.transform.SetParent (GameManager.container.transform);
        Text numMsgText = numMsg.transform.Find("Canvas").gameObject.transform.Find("Points").gameObject.GetComponent<Text>();
       // Canvas canvas = numMsg.transform.Find("Canvas").gameObject.GetComponent<Canvas>();
       //canvas.worldCamera = Camera.main;
        numMsgText.text = num.ToString();
        numMsgText.color = color;
        numMsg.transform.localScale = Vector3.zero;
        float oldY = position.y;
        float newY = position.y + 0.2f;

        int dur = 20;
        for (int i = 1; i <= dur; i++)
        {
            numMsg.transform.localScale = Vector3.one * Easing.EaseInCubic(0f, 1f, i * 1f / dur);
            yield return null;
        }
        dur = 5;
        for (int i = 1; i <= dur; i++)
        {
            yield return null;
        }
        dur = 15;
        for (int i = 1; i <= dur; i++)
        {
            numMsg.transform.position = new Vector3(position.x,Easing.EaseInBack(oldY,newY,i*1f/dur), position.z);
            numMsgText.color -= new Color(0f,0f,0f, 1f / dur);
            yield return null;
        }
        Destroy(numMsg);
        yield break; 
    }
}
