using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System;
using System.Globalization;

public class DialogueManager : MonoBehaviour
{
    private string[] fileLines;
    private List<string> spilittedText = new List<string>();

    public List<float> subtitleTiming = new List<float>();
    public List<string> subtitleText = new List<string>();

    public int nextSub = 0;
    private string displaySub;

    private DateTime start_time;

    public string breakTag = "<break/>";

    private Regex digitsOnly = new Regex(@"[^\d+(\,\d+)*S]");

    private GUIStyle subs = new GUIStyle();
    public int font_size = 19;

    public static DialogueManager Instance { get; private set;  }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }   
    
    public float string_to_float(string input)
    {
        string temp = digitsOnly.Replace(input, "");
        return float.Parse(temp);
    }

    public void DialogueStart(string Filename)
    {

        spilittedText = new List<string>();
        subtitleTiming = new List<float>();
        subtitleText = new List<string>();

        nextSub = 1;

        TextAsset temp = Resources.Load("Text_Script/" + Filename) as TextAsset;
        fileLines = temp.text.Split('\n');
        
        foreach(string line in fileLines)
        {
            string[] temp2 = line.Split('|');
            subtitleTiming.Add(string_to_float(temp2[0]));
            subtitleText.Add(temp2[1]);
        }

        displaySub = subtitleText[0];
        start_time = DateTime.Now;
    }

    void OnGUI()
    {
        TimeSpan elapsed = DateTime.Now - start_time;
        
        if (nextSub > 0 && !subtitleText[nextSub - 1].Contains(breakTag))
        {
            GUI.depth = -1001;
            subs.fixedWidth = Screen.width / 1.5f;
            subs.wordWrap = true;
            subs.alignment = TextAnchor.MiddleCenter;
            subs.normal.textColor = Color.white;
            subs.fontSize = font_size;

            Vector2 size = subs.CalcSize(new GUIContent());
            //Cień
            GUI.contentColor = Color.black;
            GUI.Label(new Rect(Screen.width / 2 - size.x / 2 + 2, Screen.height / 1.25f - size.y / 2 + 2, size.x, size.y), displaySub, subs);
            //Tekst właściwy
            GUI.contentColor = Color.white;
            GUI.Label(new Rect(Screen.width / 2 - size.x / 2, Screen.height / 1.25f - size.y / 2 , size.x, size.y), displaySub, subs);
        }
        
        if (nextSub < subtitleText.Count)
        {
            //Debug.Log(Convert.ToSingle(elapsed.TotalSeconds));
            //Debug.Log(subtitleTiming[nextSub]);
            if (Convert.ToSingle(elapsed.TotalSeconds) > subtitleTiming[nextSub])
            {
                displaySub = subtitleText[nextSub];
                nextSub++;
                Debug.Log(displaySub);
            }
        }
    }
    
}
