using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class ItemSpawner : MonoBehaviour
{
    private SpawnPointProvider[] _spawnPointProviders;
    public int maxItemCount = 10;
    public float itemLifeTime = 5f;

    public Item[] itemPrefabs;

    public float timeBetSpawnMin = 1f;
    public float timeBetSpawnMax = 10f;

    public Vector3 spawnOffset;
    
    private float _nextSpawnTime;
    private readonly List<Item> _items = new List<Item>();
    

    private void Awake()
    {
        _spawnPointProviders = FindObjectsOfType<SpawnPointProvider>();
        _nextSpawnTime = Time.time + Random.Range(timeBetSpawnMin, timeBetSpawnMax);
    }

    private void FixedUpdate()
    {
        if (_items.Count < maxItemCount && Time.time >= _nextSpawnTime)
        {
            SpawnAtRandomPosition();
            _nextSpawnTime = Time.time + Random.Range(timeBetSpawnMin, timeBetSpawnMax);
        }
    }

    private void SpawnAtRandomPosition()
    {
        var spawnPointProvider = _spawnPointProviders[Random.Range(0, _spawnPointProviders.Length)];

        var spawnPoint = spawnPointProvider.GetRandomSpawnPoint(1f);
        
        
        var item = Instantiate(itemPrefabs[Random.Range(0,itemPrefabs.Length)],spawnPoint + spawnOffset,  Quaternion.Euler(0f, Random.Range(0f, 360f), 0f));

        _items.Add(item);
        
        Destroy(item.gameObject, itemLifeTime);
        item.onItemDestroyed.AddListener(destroyedItem => _items.Remove(destroyedItem));
    }
}