using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour {

  public static SaveManager ptr;

  [SerializeField] public SettingsData settings;

  private void Awake() {
    if (ptr != null) {
      Destroy(gameObject);
      return;
    }

    ptr = this;
  }

  // Start is called once before the first execution of Update after the MonoBehaviour is created
  void Start() {
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

    save.playerAbilities = p.GetAbilities();

    save.sceneName = SceneManager.GetActiveScene().name;

    save.savePointIndex = index;

    save.settings = settings;

    return save;
  }

  public void Save(int index, Player p) {

    SaveFile save = UpdateSaveFile(index, p);

    BinaryFormatter bf = new BinaryFormatter();
    FileStream file = File.Create(Application.persistentDataPath + "/gamesave.save");
    bf.Serialize(file, save);
    file.Close();

  }

  public void AutoSave(Scene scene, LoadSceneMode mode) {
    Save(0, GameObject.Find("Player").GetComponent<Player>());
  }

  public void Load(Scene scene, LoadSceneMode mode) {
    if (File.Exists(Application.persistentDataPath + "/gamesave.save")) {
      //Convert the save file to a Save object so we can access the information on it
      BinaryFormatter bf = new BinaryFormatter();
      FileStream file = File.Open(Application.persistentDataPath + "/gamesave.save", FileMode.Open);
      SaveFile save = (SaveFile)bf.Deserialize(file);
      file.Close();

      settings = save.settings;

      Player p = GameObject.Find("Player").GetComponent<Player>();

      p.SetAbilities(save.playerAbilities);

      p.gameObject.transform.position = GameManager.ptr.GetSavePoint(save.savePointIndex).position;

      Debug.Log(save.savePointIndex);
    }
  }

  public void WipeSave() {
    if (File.Exists(Application.persistentDataPath + "/gamesave.save"))
      File.Delete(Application.persistentDataPath + "/gamesave.save");
  }

}
