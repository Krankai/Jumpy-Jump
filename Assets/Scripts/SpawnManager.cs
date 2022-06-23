using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    GameObject enemyFromTopPrefab;

    [SerializeField]
    GameObject sideEnemyMovingPrefab;

    [SerializeField]
    GameObject sideEnemyThrowedPrefab;

    public bool IsSpawning { get; private set; }

    const float spawnZ = 6.0f;
    const float spawnYFromTop = 22.0f;
    float spawnRangeXFromTop;

    [SerializeField]
    float offsetSpawnRangeXFromTop = 1;

    const float spawnMinRangeYFromSide = 1.0f;
    const float spawnMaxRangeYFromSide = 9.0f;
    float spawnXFromSide;

    [SerializeField]
    float offsetSpawnXFromSide = 5;

    [SerializeField]
    float spawnFromTopDelay = 1f;

    [SerializeField]
    float spawnFromTopInterval = 1.5f;

    [SerializeField]
    float spawnFromSideDelay = 2f;

    [SerializeField]
    float spawnFromSideInterval = 2.5f;

    ObjectPooler pooler;
    Vector3 vector3Zero = Vector3.zero;

    private void Awake()
    {
        SetupBoundaries();
    }

    // Start is called before the first frame update
    void Start()
    {
        pooler = GetComponent<ObjectPooler>();
    }

    // Repeatedly spawn corresponding enemies from the top of the viewport
    void SpawnEnemyFromTop()
    {
        var spawnPosition = new Vector3(RandomizeXPos(), spawnYFromTop, spawnZ);

        var enemyObject = pooler.GetDropEnemyPooledObject();
        if (enemyObject != null)
        {
            ReactivatePooledObject(enemyObject, spawnPosition, RandomizeRotation());
        }
    }

    // Repeatedly spawn enemies moving from either left or right side of the viewport
    void SpawnEnemyMovingFromSide()
    {
        bool isFromLeft = Random.value < 0.5f;
        var spawnPosition = new Vector3(isFromLeft ? -spawnXFromSide : spawnXFromSide, RandomizeYPos(), spawnZ);

        var rotation = (sideEnemyMovingPrefab.GetComponent<MoveFromSide>().isFixedRotation) ? sideEnemyMovingPrefab.transform.rotation : RandomizeRotation();
        var enemyObject = pooler.GetMoveEnemyPooledObject();
        if (enemyObject != null)
        {
            ReactivatePooledObject(enemyObject, spawnPosition, rotation);

            // Setup direction and start moving motion
            var movingScript = enemyObject.GetComponent<MoveFromSide>();
            movingScript.movingDirection = isFromLeft ? MoveDirection.Right : MoveDirection.Left;
            movingScript.StartMoving();
        }
    }

    // Repeatedly spawn enemies being throwed from either left or right side of the viewport
    void SpawnEnemyThrowedFromSide()
    {
        bool isFromLeft = Random.value < 0.5f;
        var spawnPosition = new Vector3(isFromLeft ? -spawnXFromSide : spawnXFromSide, RandomizeYPos(), spawnZ);

        var enemyObject = pooler.GetThrowEnemyPooledObject();
        if (enemyObject != null)
        {
            ReactivatePooledObject(enemyObject, spawnPosition, RandomizeRotation());

            // Setup direction and start motion
            var throwingScript = enemyObject.GetComponent<ThrowFromSide>();
            throwingScript.throwingDirection = isFromLeft ? MoveDirection.Right : MoveDirection.Left;
            throwingScript.StartThrowing();
        }
    }

    // Randomize rotation angle for enemy object
    Quaternion RandomizeRotation()
    {
        var rotation = new Vector3(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f));
        return Quaternion.Euler(rotation);
    }

    // Randomize X position (for spawning top enemies)
    float RandomizeXPos()
    {
        return Random.Range(-spawnRangeXFromTop, spawnRangeXFromTop);
    }

    // Randomize Y position (for spawing side enemies)
    float RandomizeYPos()
    {
        return Random.Range(spawnMinRangeYFromSide, spawnMaxRangeYFromSide);
    }

    // Stop spawning
    public void StopSpawning()
    {
        IsSpawning = false;
        CancelInvoke();
    }

    // Start spawning
    public void StartSpawning()
    {
        IsSpawning = true;

        InvokeRepeating("SpawnEnemyFromTop", spawnFromTopDelay, spawnFromTopInterval);
        InvokeRepeating("SpawnEnemyMovingFromSide", spawnFromSideDelay, spawnFromSideInterval);
        InvokeRepeating("SpawnEnemyThrowedFromSide", spawnFromSideDelay + spawnFromSideInterval / 2, spawnFromSideInterval);
    }

    // Determine spawning boundaries based on player's
    private void SetupBoundaries()
    {
        var playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        var playerRangeX = playerController.RangeX;

        spawnRangeXFromTop = playerRangeX - offsetSpawnRangeXFromTop;
        spawnXFromSide = playerRangeX + offsetSpawnXFromSide;
    }

    // Reactivate object get from the pooler
    private void ReactivatePooledObject(GameObject retrievedObject, Vector3 position, Quaternion rotation)
    {
        var enemyRb = retrievedObject.GetComponent<Rigidbody>();
        enemyRb.velocity = vector3Zero;
        enemyRb.angularVelocity = vector3Zero;

        retrievedObject.SetActive(true);
        retrievedObject.transform.position = position;
        retrievedObject.transform.rotation = rotation;
    }
}

public enum MoveDirection
{
    Right = 1,
    Left = -1,
}
