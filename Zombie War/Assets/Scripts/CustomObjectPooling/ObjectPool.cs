using UnityEngine;
using System.Collections.Generic;

public class ObjectPool : MonoBehaviour {
    [Header("Pool Settings")]
    [SerializeField] private GameObject objectToPool;
    [SerializeField] private int amountToPool = 10;
    [SerializeField] private PoolExhaustedBehavior exhaustBehavior = PoolExhaustedBehavior.CreateNew;
    [SerializeField] private int maxPoolSize = 50;

    private readonly List<GameObject> pooledObjects = new List<GameObject>();
    private readonly SpecialQueue<GameObject> spawnedObjects = new SpecialQueue<GameObject>();

    private void Awake() {
        for (int i = 0; i < amountToPool; i++) {
            CreateNewObject();
        }
    }

    private GameObject CreateNewObject() 
    {
        var obj = Instantiate(objectToPool, transform);
        var poolable = obj.GetComponent<Poolable>();
        if (poolable == null) poolable = obj.AddComponent<Poolable>();
        poolable.Init(this);
        pooledObjects.Add(obj);
        obj.SetActive(false);
        return obj;
    }

    public GameObject Pop() 
    {
        // Get object from pool
		for (int i = 0; i < pooledObjects.Count; i++)
		{
			if (!pooledObjects[i].activeSelf)
			{
				// If object is in spawned queue
				// Remove the object from queue then add it to the last position in the queue
				if (spawnedObjects.Contains(pooledObjects[i]))
				{
					spawnedObjects.Remove(pooledObjects[i]);
				}
				spawnedObjects.Enqueue(pooledObjects[i]);
				return pooledObjects[i];
			}
		}

        // Pool exhausted
        switch (exhaustBehavior) {
            case PoolExhaustedBehavior.CreateNew:
                var newObj = CreateNewObject();
                spawnedObjects.Enqueue(newObj);
                return newObj;

            case PoolExhaustedBehavior.Recycle:
                if (spawnedObjects.Count > 0) {
                    var oldest = spawnedObjects.Dequeue();
                    spawnedObjects.Enqueue(oldest);
                    return oldest;
                }
                break;

            case PoolExhaustedBehavior.ExpandWithLimit:
                if (spawnedObjects.Count < maxPoolSize) {
                    var expandedObj = CreateNewObject();
                    spawnedObjects.Enqueue(expandedObj);
                    return expandedObj;
                }
                break;

            case PoolExhaustedBehavior.FailSilently:
                Debug.LogWarning("Pool is empty");
                break;
        }

        return null;
    }

    public void RecallAllObjects()
    {
        while (spawnedObjects.Count > 0)
        {
            var spawnedObject = spawnedObjects.Dequeue();
            spawnedObject.SetActive(false);
        }
    }
}
