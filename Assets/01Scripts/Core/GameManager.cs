using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    [SerializeField] private Player _player;
    public Transform PlayerTrm => _player.transform;
    public Player Player => _player;

    private Camera _mainCam;

    public Camera MainCam
    {
        get {
            if (_mainCam == null)
            {
                _mainCam = Camera.main;
            }

            return _mainCam;
        }
    }
}
