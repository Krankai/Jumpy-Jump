using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler SharedInstance;

    public GameObject dropEnemyObject;
    public int dropEnemyPoolAmount;
    private List<GameObject> dropEnemyPooledObjects;
    
    public GameObject moveEnemyObject;
    public int moveEnemyPoolAmount;
    private List<GameObject> moveEnemyPooledObjects;

    public GameObject throwEnemyObject;
    public int throwEnemyPoolAmount;
    private List<GameObject> throwEnemyPooledObject;

    private void Awake()
    {
        SharedInstance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        GeneratePooledObjects();
    }

    // Generate list of pooled objects for each type
    void GeneratePooledObjects()
    {
        // Drop enemy
        dropEnemyPooledObjects = new List<GameObject>();
        for (int i = 0; i < dropEnemyPoolAmount; ++i)
        {
            GameObject pooledObject = (GameObject)Instantiate(dropEnemyObject);
            pooledObject.SetActive(false);
            pooledObject.transform.SetParent(this.transform);       // set as children of Spawn Manager

            dropEnemyPooledObjects.Add(pooledObject);
        }

        // Move enemy
        moveEnemyPooledObjects = new List<GameObject>();
        for (int i = 0; i < moveEnemyPoolAmount; ++i)
        {
            GameObject pooledObject = (GameObject)Instantiate(moveEnemyObject);
            pooledObject.SetActive(false);
            pooledObject.transform.SetParent(this.transform);       // set as children of Spawn Manager

            moveEnemyPooledObjects.Add(pooledObject);
        }

        // Throw enemy
        throwEnemyPooledObject = new List<GameObject>();
        for (int i = 0; i < throwEnemyPoolAmount; ++i)
        {
            GameObject pooledObject = (GameObject)Instantiate(throwEnemyObject);
            pooledObject.SetActive(false);
            pooledObject.transform.SetParent(this.transform);       // set as children of Spawn Manager

            throwEnemyPooledObject.Add(pooledObject);
        }
    }

    public GameObject GetDropEnemyPooledObject()
    {
        for (int i = 0; i < dropEnemyPoolAmount; ++i)
        {
            if (!dropEnemyPooledObjects[i].activeInHierarchy)
            {
                return dropEnemyPooledObjects[i];
            }
        }
        return null;
    }

    public GameObject GetMoveEnemyPooledObject()
    {
        for (int i = 0; i < moveEnemyPoolAmount; ++i)
        {
            if (!moveEnemyPooledObjects[i].activeInHierarchy)
            {
                return moveEnemyPooledObjects[i];
            }
        }
        return null;
    }

    public GameObject GetThrowEnemyPooledObject()
    {
        for (int i = 0; i < throwEnemyPoolAmount; ++i)
        {
            if (!throwEnemyPooledObject[i].activeInHierarchy)
            {
                return throwEnemyPooledObject[i];
            }
        }
        return null;
    }
}
