using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour {

  public static SaveManager ptr;

  [SerializeField] public SettingsData settings;

  private int spawnPoint;
  private bool spawnedAtSavePoint;

  private void Awake() {
    if (ptr != null) {
      Destroy(gameObject);
      return;
    }

    ptr = this;
  }

  // Start is called once before the first execution of Update after the MonoBehaviour is created
  void Start() {
    StartMenuLoad();

    DontDestroyOnLoad(gameObject);
    SceneManager.sceneLoaded += Load;
    SceneManager.sceneLoaded += AutoSave;
  }

  // Update is called once per frame
  void Update() {

  }

  public void UpdateSounds() {

  }

  private SaveFile UpdateSaveFile(int index, Player p) {
    SaveFile save = new SaveFile();

    save.playerHealth = p.GetHealth();
    save.playerAbilities = p.GetAbilities();

    save.roomsSeen = GameManager.ptr.GetSeenRooms();

    save.sceneName = SceneManager.GetActiveScene().name;

    save.savePointIndex = index;

    Debug.Log("Save Point Index at Save: " + save.savePointIndex);

    save.settings[0] = settings.GetMasterVolume();
    save.settings[1] = settings.GetMusicVolume();
    save.settings[2] = settings.GetSFXVolume();

    return save;
  }

  public void Save(int index, Player p) {

    SaveFile save = UpdateSaveFile(index, p);

    BinaryFormatter bf = new BinaryFormatter();
    FileStream file = File.Create(Application.persistentDataPath + "/gamesave.save");
    bf.Serialize(file, save);
    file.Close();

  }

  public void BeginningSceneSaveFile() {
    SaveFile save = new SaveFile();
    BinaryFormatter bf = new BinaryFormatter();

    //Convert the save file to a Save object so we can access the information on it
    FileStream file = File.Open(Application.persistentDataPath + "/gamesave.save", FileMode.Open);
    save = (SaveFile) bf.Deserialize(file);

    save.settings[0] = settings.GetMasterVolume();
    save.settings[1] = settings.GetMusicVolume();
    save.settings[2] = settings.GetSFXVolume();

    file.Close();

    FileStream fileSave = File.Create(Application.persistentDataPath + "/gamesave.save");
    bf.Serialize(fileSave, save);
    fileSave.Close();
  }

  public void AutoSave(Scene scene, LoadSceneMode mode) {
    Save(spawnPoint, GameManager.ptr.p);
  }

  public void Load(Scene scene, LoadSceneMode mode) {
    if (File.Exists(Application.persistentDataPath + "/gamesave.save")) {
      //Convert the save file to a Save object so we can access the information on it
      BinaryFormatter bf = new BinaryFormatter();
      FileStream file = File.Open(Application.persistentDataPath + "/gamesave.save", FileMode.Open);
      SaveFile save = (SaveFile)bf.Deserialize(file);
      file.Close();

      spawnPoint = save.savePointIndex;

      settings.SetMasterVolume(save.settings[0]);
      settings.SetMusicVolume(save.settings[1]);
      settings.SetSFXVolume(save.settings[2]);

      Player p = GameObject.Find("Player").GetComponent<Player>();

      p.SetAbilities(save.playerAbilities);

      if (!SceneManager.GetActiveScene().name.Equals("BeginningArea")) {
        p.gameObject.transform.position = GameManager.ptr.GetSavePoint(save.savePointIndex).position;
        Debug.Log("Save Point Index: " + save.savePointIndex);
        Debug.Log(GameManager.ptr.GetSavePoint(save.savePointIndex).position);
      } else {
        LevelHUD.ptr.ToggleHealth(false);
        GameManager.ptr.nextScene = save.sceneName;
      }

      GameManager.ptr.SetSeenRooms(-1, save.roomsSeen);
    }
  }

  public void WipeSave() {
    if (File.Exists(Application.persistentDataPath + "/gamesave.save"))
      File.Delete(Application.persistentDataPath + "/gamesave.save");
  }

  private void StartMenuLoad() {
    if (File.Exists(Application.persistentDataPath + "/gamesave.save")) {
      Debug.Log(Application.persistentDataPath + "/gamesave.save");
      //Convert the save file to a Save object so we can access the information on it
      BinaryFormatter bf = new BinaryFormatter();
      FileStream file = File.Open(Application.persistentDataPath + "/gamesave.save", FileMode.Open);
      SaveFile save = (SaveFile)bf.Deserialize(file);
      file.Close();

      settings.SetMasterVolume(save.settings[0]);
      settings.SetMusicVolume(save.settings[1]);
      settings.SetSFXVolume(save.settings[2]);

      GameManager.ptr.nextScene = save.sceneName;
    } else
      GameObject.Find("StartMenu").GetComponent<StartMenu>().NoLoadedGame();

    LevelHUD.ptr.ToggleHealth(false);
  }

  public int GetSpawnPoint() {
    return spawnPoint;
  }

}
