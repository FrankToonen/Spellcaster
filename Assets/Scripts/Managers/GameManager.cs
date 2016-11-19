using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

/// <summary>
/// A manager script that handles scene switching and file management.
/// </summary>
public class GameManager : MonoBehaviour
{
    private const string EXTENSION = ".caster";
    
    /// <summary>
    /// Switches to the specified scene.
    /// </summary>
    /// <param name="sceneName">The name of the scene to load.</param>
    public void SwitchToScene(string sceneName)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// Closes the game.
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
    }

    /// <summary>
    /// Creates an empty savefile.
    /// </summary>
    public void CreateNewSave()
    {
        var data = new CharacterData(100, 250, 50, 50, 50);
        SaveFile(Player.FILENAME + "Stats", data);

        var newInventory = new Inventory();
        newInventory.AddItem(new HealthPotion(5));
        newInventory.AddItem(new ManaPotion(3));
        SaveFile(Player.FILENAME + "Save", new PlayerSaveData(data, data.maxHealth, data.maxMana, newInventory));
    }

    /// <summary>
    /// Deletes the savefile.
    /// </summary>
    public void DeleteSave()
    {
        DeleteFile(Player.FILENAME + "Stats");
        DeleteFile(Player.FILENAME + "Save");

        // Disable continue button.
        var continueButton = FindObjectOfType<CheckForSave>();
        if (continueButton != null)
        {
            continueButton.SetInteractable();
        }
    }

    /// <summary>
    /// Save data of type T to a file.
    /// This file can be loaded with the LoadFile function.
    /// </summary>
    /// <typeparam name="T">The type of the data to save.</typeparam>
    /// <param name="fileName">The name of the file to save to.</param>
    /// <param name="data">The data to save to the file.</param>
    public static void SaveFile<T>(string fileName, T data)
    {
        var bf = new BinaryFormatter();
        var file = File.Create(GetPath(fileName));
        bf.Serialize(file, data);
        file.Close();

        DebugHelper.instance.AddMessage(string.Format("The file \"{0}\" has been saved.", fileName));
    }

    /// <summary>
    /// Load data of type T from a file.
    /// This data can be saved with the SaveFile function.
    /// </summary>
    /// <typeparam name="T">The type of the data to load.</typeparam>
    /// <param name="fileName">The name of the file to load.</param>
    /// <returns>The loaded data of type T.</returns>
    public static T LoadFile<T>(string fileName)
    {
        var path = GetPath(fileName);
        if (!File.Exists(path))
        {
            DebugHelper.instance.AddMessage(string.Format("The file \"{0}\" does not exist.", fileName));
            throw new Exception(string.Format("The file \"{0}\" does not exist.", fileName));
        }

        var bf = new BinaryFormatter();
        var file = File.Open(path, FileMode.Open);
        var data = (T) bf.Deserialize(file);
        file.Close();

        DebugHelper.instance.AddMessage(string.Format("The file \"{0}\" has been loaded.", fileName));

        return data;
    }

    /// <summary>
    /// Deletes a file, if it exists.
    /// </summary>
    /// <param name="fileName">The name of the file to delete.</param>
    public static void DeleteFile(string fileName)
    {
        if (!FileExists(fileName))
        {
            return;
        }
        
        var path = GetPath(fileName);
        File.Delete(path);
        DebugHelper.instance.AddMessage(string.Format("The file \"{0}\" has been deleted.", fileName));
    }

    /// <summary>
    /// Finds if the given file exists.
    /// </summary>
    /// <param name="fileName">The name of the file to check its existance of.</param>
    public static bool FileExists(string fileName)
    {
        return File.Exists(GetPath(fileName));
    }

    /// <summary>
    /// Gets the path to the given file.
    /// </summary>
    /// <param name="fileName">The name of the file to get its path of.</param>
    /// <returns>The path to the file.</returns>
    private static string GetPath(string fileName)
    {
        return Application.persistentDataPath + "/" + fileName + EXTENSION;
    }
}
