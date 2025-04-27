using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                return null;
            }
            return instance;
        }
    }

    private SettingManager settingManager;
    private DataManager dataManager;
    private InventoryManager inventoryManager;
    private StoreManager storeManager;
    private DialogueManager dialogueManager;
    private UIManager uiManager;

    public static SettingManager Setting => Instance.settingManager;
    public static DataManager Data => Instance.dataManager;
    public static InventoryManager Inventory => Instance.inventoryManager;
    public static StoreManager Store => Instance.storeManager;
    public static DialogueManager Dialogue => Instance.dialogueManager;
    public static UIManager UI => Instance.uiManager;

    private GameManager() { }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);

        settingManager = SettingManager.Instance;
        dataManager = DataManager.Instance;
        inventoryManager = InventoryManager.Instance;
        storeManager = StoreManager.Instance;
        dialogueManager = DialogueManager.Instance;

        uiManager = UIManager.Instance;
    }

    void Start()
    {

    }

    public void ChangeScene(string sceneString)
    {
        UI.UIList.Clear();

        if (int.TryParse(sceneString, out int sceneIndex))
        {
            SceneManager.LoadScene(sceneIndex);
            return;
        }

        SceneManager.LoadScene(sceneString);
    }

    public void ExitGame()
    {

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
