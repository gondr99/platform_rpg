using System;
using System.Collections.Generic;
using UnityEngine;

//직렬화 가능한 딕셔너리
[Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
    [SerializeField] private List<TKey> keys = new List<TKey>();
    [SerializeField] private List<TValue> values = new List<TValue>();
    
    
    //딕셔너리에 있는 데이터를 직렬화데이터로
    public void OnBeforeSerialize()
    {
        keys.Clear();
        values.Clear();

        foreach (var pair in this)
        {
            keys.Add(pair.Key);
            values.Add(pair.Value);
        }
    }

    //직렬화된 데이터를 가져온뒤 다시 딕셔너리로
    public void OnAfterDeserialize()
    {
        this.Clear();
        if (keys.Count != values.Count)
        {
            Debug.LogError("key count not equal to value count");
        }
        for (int i = 0; i < keys.Count; ++i)
        {
            this.Add(keys[i], values[i]);
        }
    }
}
