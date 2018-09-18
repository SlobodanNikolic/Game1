using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenManager : MonoBehaviour {

    // Use this for initialization
    public GameObject[] screens;
    public static ScreenManager instance;


    void Start()
    {   
        if (instance == null)
            instance = this;
        else if (instance!=this)
            Destroy(this);

        screens[0].SetActive(true);
        for (int i=1;i<screens.Length;i++)
        {
            screens[i].SetActive(false);
        }
    }

    
    public void ChangeScreen(string screenName)
    {
        foreach (GameObject s in screens)
        {
           s.SetActive(s.name==screenName);     
        }
    }
	
	
}
