using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    [SerializeField] private Player _player;
    public Transform PlayerTrm => _player.transform;

    private void Awake()
    {
        Instance = this;
    }
}
