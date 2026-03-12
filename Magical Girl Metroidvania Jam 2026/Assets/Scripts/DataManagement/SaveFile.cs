using UnityEngine;

[System.Serializable]

public class SaveFile {

  public int playerHealth;

  public bool[] playerAbilities = new bool[5];

  public bool[] roomsSeen;

  public string sceneName;

  public int savePointIndex;

  public float[] settings = new float[3];

}
