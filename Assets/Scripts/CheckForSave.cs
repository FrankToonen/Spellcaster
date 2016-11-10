using UnityEngine;
using UnityEngine.UI;

public class CheckForSave : MonoBehaviour
{
    private void Start()
    {
        SetInteractable();
    }

    public void SetInteractable()
    {
        GetComponent<Button>().interactable = GameManager.FileExists(Player.FILENAME + "Stats") && GameManager.FileExists(Player.FILENAME + "Save");
    }
}
