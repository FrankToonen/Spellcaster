using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A helper class for debugging on mobile. 
/// This class allows messages to be logged to the screen.
/// This requires an UI Text element with the tag "DebugText" to function.
/// </summary>
public class DebugHelper : MonoBehaviour
{
    public static DebugHelper instance;

    [SerializeField] private int MAXMESSAGES = 5;
    [SerializeField] private bool isVisible = false;

    private LinkedList<Message> messages;

    /// <summary>
    /// Creates a singleton of this class.
    /// </summary>
    private void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        messages = new LinkedList<Message>();
    }
    
    /// <summary>
    /// Adds a message to the debugText to display on the screen.
    /// </summary>
    /// <param name="message">The message to display.</param>
    /// <param name="time">How long the message should be displayed for.</param>
    public void AddMessage(string message, float time = 5)
    {
        Debug.Log(message);

        // Create a new message type.
        var newMessage = new Message(message, time);
        newMessage.coroutine = StartCoroutine(RemoveMessageAfterTime(newMessage));
        messages.AddLast(newMessage);

        // Removes all the messages that would exceed MAXMESSAGES.
        while (messages.Count > MAXMESSAGES)
        {
            // First stop the coroutine, then remove it from the LinkedList.
            StopCoroutine(messages.First.Value.coroutine);
            messages.RemoveFirst();
        }

        // Update the debugText to include this message.
        UpdateDebugText();
    }
    
    /// <summary>
    /// Updates the displayed debugText to include every message in the LinkedList.
    /// </summary>
    private void UpdateDebugText()
    {
        var newDebugText = "";
        foreach (var message in messages)
        {
            newDebugText += message.message + "\n";
        }

        Text textComponent = GameObject.Find("DebugText").GetComponent<Text>();
        textComponent.text = newDebugText;
        textComponent.enabled = isVisible;
    }

    private IEnumerator RemoveMessageAfterTime(Message message)
    {
        // Remove the message and update the debugText after the time has passed.
        yield return new WaitForSeconds(message.time);

        messages.Remove(message);
        UpdateDebugText();
    }

    /// <summary>
    /// A struct describing a message and its properties.
    /// </summary>
    private class Message
    {
        public readonly string message;
        public readonly float time;
        public Coroutine coroutine;

        public Message(string message, float time)
        {
            this.message = message;
            this.time = time;
        }
    }
}
