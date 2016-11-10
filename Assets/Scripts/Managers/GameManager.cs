using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private void Start()
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
    }

    public void SwitchToScene(string sceneName)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
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
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "\\" + fileName);
        bf.Serialize(file, data);
        file.Close();

        Debug.Log(string.Format("The file \"{0}\" has been saved.", fileName));
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
        if (File.Exists(Application.persistentDataPath + "\\" + fileName))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "\\" + fileName, FileMode.Open);
            T data = (T) bf.Deserialize(file);
            file.Close();

            Debug.Log(string.Format("The file \"{0}\" has been loaded.", fileName));

            return data;
        }

        return default(T);
    }

    /// <summary>
    /// Deletes a file, if it exists.
    /// </summary>
    /// <param name="fileName">The name of the file to delete.</param>
    public static void DeleteFile(string fileName)
    {
        if (File.Exists(Application.persistentDataPath + "\\" + fileName))
        {
            File.Delete(Application.persistentDataPath + "\\" + fileName);
            Debug.Log(string.Format("The file \"{0}\" has been deleted.", fileName));
        }
    }
}
