
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [SerializeField] private float parallaxEffect;
    private Transform _mainCamTrm;
    private float _xPosition;
    private float _length;
    
    void Awake()
    {
        _mainCamTrm = Camera.main.transform;
        _xPosition = transform.position.x; //시작 X위치

        _length = GetComponent<SpriteRenderer>().bounds.size.x;
    }
    
    void LateUpdate()
    {
        float distanceToMoved = _mainCamTrm.position.x * (1 - parallaxEffect);
        float distanceToMove = _mainCamTrm.position.x * parallaxEffect;

        transform.position = new Vector3(_xPosition + distanceToMove, transform.position.y);

        //이동한 거리가 시작위치에서 length만큼 더한거를 이동했다면 타일링 이동.
        if (distanceToMoved > _xPosition + _length) //오른쪽이동.
        {
            _xPosition = _xPosition + _length;
        }
        else if( distanceToMoved < _xPosition - _length)  //왼쪽이동
        {
            _xPosition = _xPosition - _length;
        }
    }
}
