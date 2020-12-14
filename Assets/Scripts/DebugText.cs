using UnityEngine;

public class DebugText
{
    private static DebugText log;
    public static DebugText Log
    {
        get
        {
            if (log == null)
            {
                log = new DebugText();
            }

            return log;
        }
    }

    public void Green(string text)
    {
        string color = ColorUtility.ToHtmlStringRGB(Color.green);
        Debug.Log($"<color=#{color}> {text} </color>");
    }

    public void Red(string text)
    {
        string color = ColorUtility.ToHtmlStringRGB(Color.red);
        Debug.Log($"<color=#{color}> {text} </color>");
    }

    public void Blue(string text)
    {
        string color = ColorUtility.ToHtmlStringRGB(Color.blue);
        Debug.Log($"<color=#{color}> {text} </color>");
    }

    public void White(string text)
    {
        string color = ColorUtility.ToHtmlStringRGB(Color.white);
        Debug.Log($"<color=#{color}> {text} </color>");
    }

    public void Error(string text)
    {
        string color = ColorUtility.ToHtmlStringRGB(Color.red);
        Debug.Log($"<color=#{color}> {text} </color>");
    }

    public void Warnning(string text)
    {
        string color = ColorUtility.ToHtmlStringRGB(Color.yellow);
        Debug.Log($"<color=#{color}> {text} </color>");
    }

    public void Black(string text)
    {
        string color = ColorUtility.ToHtmlStringRGB(Color.black);
        Debug.Log($"<color=#{color}> {text} </color>");
    }

    public void Grey(string text)
    {
        string color = ColorUtility.ToHtmlStringRGB(Color.grey);
        Debug.Log($"<color=#{color}> {text} </color>");
    }

    public void Magenta(string text)
    {
        string color = ColorUtility.ToHtmlStringRGB(Color.magenta);
        Debug.Log($"<color=#{color}> {text} </color>");
    }
}
