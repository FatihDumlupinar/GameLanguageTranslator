namespace GameLanguageTranslator.Utilities;

/// <summary>
/// son kullanım zamanı en eski olan verilerin önbellekten atılmasını sağlayan bir cache yönetim algoritması
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TValue"></typeparam>
public class LRUCache<TKey, TValue>
{
    private readonly int _capacity;
    private readonly Dictionary<TKey, LinkedListNode<KeyValuePair<TKey, TValue>>> _cacheMap;
    private readonly LinkedList<KeyValuePair<TKey, TValue>> _cacheList;

    public LRUCache(int capacity)
    {
        _capacity = capacity;
        _cacheMap = new Dictionary<TKey, LinkedListNode<KeyValuePair<TKey, TValue>>>();
        _cacheList = new LinkedList<KeyValuePair<TKey, TValue>>();
    }

    public bool TryGet(TKey key, out TValue value)
    {
        if (_cacheMap.TryGetValue(key, out LinkedListNode<KeyValuePair<TKey, TValue>> node))
        {
            value = node.Value.Value;
            _cacheList.Remove(node);
            _cacheList.AddFirst(node);
            return true;
        }

        value = default;
        return false;
    }

    public void Add(TKey key, TValue value)
    {
        if (_cacheMap.TryGetValue(key, out LinkedListNode<KeyValuePair<TKey, TValue>> node))
        {
            _cacheList.Remove(node);
            _cacheMap.Remove(key);
        }
        else if (_cacheList.Count >= _capacity)
        {
            LinkedListNode<KeyValuePair<TKey, TValue>> last = _cacheList.Last;
            _cacheList.Remove(last);
            _cacheMap.Remove(last.Value.Key);
        }

        LinkedListNode<KeyValuePair<TKey, TValue>> newNode = new LinkedListNode<KeyValuePair<TKey, TValue>>(new KeyValuePair<TKey, TValue>(key, value));
        _cacheList.AddFirst(newNode);
        _cacheMap[key] = newNode;
    }
}

