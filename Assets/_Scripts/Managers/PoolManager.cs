using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Core.Pooling
{
    public class PoolManager : MonoBehaviour
    {
        [System.Serializable]
        public class Pool
        {
            public GameObject poolItem;
            public Transform container;
            public int prespawnAmount;

            public Pool(GameObject poolItem, int prespawnAmount, Transform container)
            {
                this.poolItem = poolItem;
                this.container = container;
                this.prespawnAmount = prespawnAmount;
            }
        }

        public static PoolManager instance;
        private void Awake()
        {
            instance = this;
        }

        public Transform poolContainer;
        public List<Pool> prePools;
        public Dictionary<string, Queue<GameObject>> pools { get; private set; }

        private void Start()
        {
            prePools = new List<Pool>();
            pools = new Dictionary<string, Queue<GameObject>>();
            for (int i = 0; i < prePools.Count; i++)
            {
                for (int j = 0; j < prePools[i].prespawnAmount; j++)
                {
                    GameObject poolItem = CreatePoolItem(prePools[i].poolItem);
                    PushToPool(poolItem);
                }
            }
        }

        public GameObject PullFromPool(GameObject poolRep)
        {
            string poolName = poolRep.name;
            GameObject poolItem;
            if (pools.ContainsKey(poolName))
            {
                //Greater than 1 because we never want a queue to be completely empty
                //because we wont know what type of gameobject belongs to that queue
                if (pools[poolName].Count > 1)
                {
                    poolItem = pools[poolName].Dequeue();
                }
                else
                {
                    poolItem = CreatePoolItem(poolRep);
                }

            }
            else
            {
                CreatePool(poolName, poolRep);
                poolItem = CreatePoolItem(poolRep);
            }

            poolItem.SetActive(true);
            return poolItem;
        }

        public void PushToPool(GameObject poolItem)
        {
            string poolName = poolItem.name;
            if (!pools.ContainsKey(poolName))
            {
                CreatePool(poolName, poolItem);
            }

            poolItem.SetActive(false);
            for (int i = 0; i < prePools.Count; i++)
            {
                if (prePools[i].poolItem.name == poolItem.name)
                {
                    poolItem.transform.SetParent(prePools[i].container);
                }
            }

            pools[poolName].Enqueue(poolItem);
        }

        public void CreatePool(string poolName, GameObject item)
        {
            Queue<GameObject> newPoolQueue = new Queue<GameObject>();
            pools.Add(poolName, newPoolQueue);
            prePools.Add(new Pool(item, 0, CreatePoolContainer(item.name)));
        }

        private GameObject CreatePoolItem(GameObject item)
        {
            GameObject newItem = Instantiate(item);
            newItem.name = item.name;
            return newItem;
        }

        private Transform CreatePoolContainer(string itemName = "GameObject")
        {
            Transform container = new GameObject(itemName + "_Pool").transform;
            container.SetParent(poolContainer);
            return container;
        }
    }

}