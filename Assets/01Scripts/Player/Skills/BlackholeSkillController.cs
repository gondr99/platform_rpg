using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class BlackholeSkillController : MonoBehaviour
{
    [Header("Hotkey info")] 
    [SerializeField] private HotKeyController _hotKeyPrefab;
    [SerializeField] private HoyKeyIconSO _hoyKeyIcon;
    
    [SerializeField] private float _maxSize;
    [SerializeField] private float _growSpeed;
    [SerializeField] private bool _canGrow;

    [SerializeField] private float _maxRiffleCount = 2;
    [SerializeField] private float _maxRiffleSpeed = 1f;
    
    private readonly int _HashRiffleSpeed = Shader.PropertyToID("_RiffleSpeed");
    private readonly int _HashRiffleCount = Shader.PropertyToID("_RiffleCount");

    private List<Enemy> _targets = new List<Enemy>();
    private Material _riffleMat;
    private SpriteRenderer _spriteRenderer;

    private List<Key> _keyCodeList;
    
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _riffleMat = _spriteRenderer.material;
        _riffleMat.SetFloat(_HashRiffleSpeed, 0);
        _riffleMat.SetFloat(_HashRiffleCount, 0);

        _keyCodeList = _hoyKeyIcon.GetAllKeyFromList();
    }

    private void Update()
    {
        if (_canGrow)
        {
            transform.localScale =
                Vector3.Lerp(transform.localScale, Vector3.one * _maxSize, _growSpeed * Time.deltaTime);

            float currentRiffleSpeed = _riffleMat.GetFloat(_HashRiffleSpeed);
            currentRiffleSpeed = Mathf.Lerp(currentRiffleSpeed, _maxRiffleSpeed, _growSpeed * Time.deltaTime);
            _riffleMat.SetFloat(_HashRiffleSpeed, currentRiffleSpeed);
            
            
            float currentRiffleCount = _riffleMat.GetFloat(_HashRiffleCount);
            currentRiffleCount = Mathf.Lerp(currentRiffleCount, _maxRiffleCount, _growSpeed * Time.deltaTime);
            _riffleMat.SetFloat(_HashRiffleCount, currentRiffleCount);
            
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<Enemy>(out Enemy enemy))
        {
            enemy.FreezeTime(true); //정지

            CreateHoyKeyOnEnemyHead(enemy);
        }
    }

    private void CreateHoyKeyOnEnemyHead(Enemy enemy)
    {
        if (_keyCodeList.Count == 0) return;
        
        Vector3 spawnPostion = enemy.transform.position + new Vector3(0, 1.5f);
        
        //Key key = _hoyKeyIcon.GetRandomKeyFromList(); 중복 허용할꺼면 이렇게
        Key key = _keyCodeList[Random.Range(0, _keyCodeList.Count)];
        _keyCodeList.Remove(key); //뽑은건 없애버려.
        
        HotKeyController hotKeyInstance = Instantiate(_hotKeyPrefab, spawnPostion, Quaternion.identity);
        hotKeyInstance.SetupHotKey(key, enemy, this);
    }

    public void AddEnemyToTargetList(Enemy enemy)
    {
        _targets.Add(enemy);
    }
}
