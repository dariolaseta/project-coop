using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private Button createGameButton;
    [SerializeField] private Button joinGameButton;
    
    [SerializeField] private string sceneToLoad;

    private void Awake()
    {
        createGameButton.onClick.AddListener(CreateGame);
        joinGameButton.onClick.AddListener(JoinGame);
    }

    private void CreateGame()
    {
        MultiplayerManager.Instance.StartHost();
        NetworkManager.Singleton.SceneManager.LoadScene(sceneToLoad, LoadSceneMode.Single);
    }

    private void JoinGame()
    {
        MultiplayerManager.Instance.StartClient();
    }
}
