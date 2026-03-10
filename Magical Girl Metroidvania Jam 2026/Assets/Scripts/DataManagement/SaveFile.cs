using UnityEngine;

[System.Serializable]

public class SaveFile {

  public bool[] playerAbilities = new bool[5];

  public bool[] roomsSeen;

  public string sceneName;

  public int savePointIndex;

  public SettingsData settings;

}
