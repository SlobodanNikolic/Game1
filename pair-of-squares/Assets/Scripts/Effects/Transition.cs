using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using Pokega;

public class Transition : MonoBehaviour {

    public static Transition instance;
    private static int numOfOpenedLines;
    private const int NUM_OF_PALETTES = 4;
    private const int NUM_OF_COLORS = 3;
    private int paletteIndex;
    private static Color[,] colors = new Color[NUM_OF_PALETTES, NUM_OF_COLORS] {
        {new Color(208f/255f,229f/255f,0f/255f), new Color(249f / 255f, 245f / 255f, 197/255f), new Color(126f / 255f, 50f / 255f, 86f/255f) },
        {new Color(157f/255f,238f/255f,159/255f), new Color(42f / 255f, 180f / 255f, 141/255f), new Color(126f / 255f, 50f / 255f, 86f/255f) },
        {new Color(62f/255f,42f/255f,70f/255f), new Color(42f / 255f, 180f / 255f, 141/255f), new Color(157f / 255f, 238f / 255f, 159f/255f) },
        {new Color(249f/255f,245f/255f,197f/255f), new Color(42f / 255f, 180f / 255f, 141/255f), new Color(62f / 255f, 42f / 255f, 70f/255f) }
        };

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        paletteIndex = Random.Range(0, NUM_OF_PALETTES);
        //DontDestroyOnLoad(gameObject);
    }

    public IEnumerator StartTransition(string scene,string pokegaScreen)
    {
        numOfOpenedLines = 0;
        Game.buttonsEnabled = false;
        paletteIndex = Random.Range(0, NUM_OF_PALETTES);
        LineRenderer[] lr = new LineRenderer[NUM_OF_COLORS];
        for (int i = 0; i < NUM_OF_COLORS; i++)
        {
            GameObject line = new GameObject();
            line.transform.SetParent(transform);
            line.layer = 9;
            line.transform.position = new Vector3(0f, 0f, 0f);
            lr[i] = line.AddComponent<LineRenderer>() as LineRenderer;
            lr[i].material = new Material(Shader.Find("Sprites/Default"));
            lr[i].material.SetColor("_Color", colors[paletteIndex, i]);
            lr[i].SetWidth(0f, 0f);
            lr[i].sortingLayerName = "Effects";
            lr[i].sortingOrder = i+200;
            lr[i].SetVertexCount(2);
            lr[i].SetPosition(0, new Vector3(9f,9f,0));
            lr[i].SetPosition(1, new Vector3(-9f, -9f, 0));

            StartCoroutine(LineOpen(lr[i],i*0.2f));

        }

        while (numOfOpenedLines < NUM_OF_COLORS)
        {
            yield return null;
        }

        //SceneManager.LoadScene(scene);
        ScreenManager.instance.ChangeScreen(scene);
        App.ui.SetScreen(pokegaScreen);

        for (int i = 0; i < NUM_OF_COLORS; i++)
        {
            /*GameObject line = new GameObject();
            line.transform.position = new Vector3(0f, 0f, 0f);
            LineRenderer lr = line.AddComponent<LineRenderer>() as LineRenderer;
            lr.material = new Material(Shader.Find("Sprites/Default"));
            lr.material.SetColor("_Color", colors[paletteIndex, i]);
            lr.SetWidth(25f, 25f);
            lr.sortingLayerName = "Effects";
            lr.sortingOrder = i + 200;
            lr.SetVertexCount(2);
            lr.SetPosition(0, new Vector3(6f, 9f, 0));
            lr.SetPosition(1, new Vector3(-6f, -9f, 0));*/

            StartCoroutine(LineClose(lr[i], (NUM_OF_COLORS - 1 - i) * 0.2f));
        }

        while (numOfOpenedLines > 0)
        {
            yield return null;
        }

        Game.buttonsEnabled = true;

    }

    private IEnumerator LineOpen(LineRenderer lr, float delay)
    {
        yield return new WaitForSeconds(delay);
        int dur = Random.Range(15, 25);
        float width = 0f;
        for (int i = 1; i <= dur; i++)
        {
            width = Easing.EaseInCubic(0f,25f,i*1f/dur);
            lr.SetWidth(width,width);
            yield return null;
        }
        numOfOpenedLines++;
    }

    private IEnumerator LineClose(LineRenderer lr, float delay)
    {
        yield return new WaitForSeconds(delay);
        int dur = Random.Range(15, 25);
        float width = 0f;
        for (int i = 1; i <= dur; i++)
        {
            width = Easing.EaseOutCubic(25f, 0f, i * 1f / dur);
            lr.SetWidth(width, width);
            yield return null;
        }

        Destroy(lr.gameObject);
        numOfOpenedLines--;
    }
}
