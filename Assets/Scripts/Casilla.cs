using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class Casilla : MonoBehaviour
{
    public int value = 0;
    public Color color;
    public bool error;
    public Text valueText;
    public bool isWritable = true;

    private void Awake()
    {
        valueText = transform.GetChild(0).GetChild(0).GetComponent<Text>();
    }

    public void SetValue(int v)
    {
        if (!isWritable) return;

        value = v;
        valueText.text = v.ToString();
        if (v == 0) valueText.text = "";
    }

    public void SetFeatures(Font font)
    {
        valueText.font = font;
    }
}


[Serializable]
public class ValuesToSaveCasilla
{
    public int value = 0;
    public bool isWritable = true;
}
