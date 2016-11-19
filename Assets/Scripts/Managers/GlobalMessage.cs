using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class GlobalMessage : MonoBehaviour
{
    public static GlobalMessage instance;

    private Text textComponent;

    /// <summary>
    /// Creates a singleton of this class.
    /// </summary>
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

    /// <summary>
    /// Shows a message on the screen on the Text-component attached to this object.
    /// </summary>
    /// <param name="message">The message to display.</param>
    /// <param name="time">The duration to display the message for.</param>
    public void BroadCastMessage(string message, float time)
    {
        StopAllCoroutines();
        StartCoroutine(ShowMessage(message, time));
    }

    /// <summary>
    /// Shows a message on the screen for the given amount of time. 
    /// -1 is a special case in which the text remains until the next BroadCastMessage call.
    /// </summary>
    /// <param name="message">The message to display.</param>
    /// <param name="time">The duration to display the message for.</param>
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
