using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class GlobalMessage : MonoBehaviour
{
    public static GlobalMessage instance;

    private Text textComponent;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        textComponent = GetComponent<Text>();
    }

    public void BroadCastMessage(string messsage, float time)
    {
        StopAllCoroutines();
        StartCoroutine(ShowMessage(messsage, time));
    }

    private IEnumerator ShowMessage(string message, float time)
    {
        textComponent.text = message;

        if (time == -1)
        {
            yield return null;
        }
        else
        {
            yield return new WaitForSeconds(time);
            textComponent.text = "";
        }
    }
}
