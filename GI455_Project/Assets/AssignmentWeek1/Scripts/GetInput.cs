using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetInput : MonoBehaviour
{
    public InputField CheckWords;
    string words;
    public Text WordText;

    void Update()
    {
        words = CheckWords.text;
    }

    public void GetInputOnClick()
    {
        if (words == "Seasons")
        {
            WordText.text = $"<Color=green>{words}</color>" + " is found.";
            Debug.Log(CheckWords.text + " is found.");
        }
        else if (words == "Autumn")
        {
            WordText.text = $"<Color=green>{words}</color>" + " is found.";
            Debug.Log(CheckWords.text + " is found.");
        }
        else if (words == "Spring")
        {
            WordText.text = $"<Color=green>{words}</color>" + " is found.";
            Debug.Log(CheckWords.text + " is found.");
        }
        else if (words == "Winter")
        {
            WordText.text = $"<Color=green>{words}</color>" + " is found.";
            Debug.Log(CheckWords.text + " is found.");
        }
        else if (words == "Summer")
        {
            WordText.text = $"<Color=green>{words}</color>" + " is found.";
            Debug.Log(CheckWords.text + " is found.");
        }
        else 
        {
            WordText.text = $"<Color=red>{words}</color>" + " is not found.";
            Debug.Log(CheckWords.text + " is not found.");
        }
    }
}
