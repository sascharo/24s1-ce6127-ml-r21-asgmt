using UnityEngine;

public class TankNewSpawn : MonoBehaviour
{
    public GameObject enemyTank;
    public float delay1 = 4f;
    public GameObject friendlyTank;
    public float delay2 = 8f;
    public float spawnRange = 30f;

    private float timeStart1;
    private float timeStart2;

    void Start()
    {
        timeStart1 = 0f;
        timeStart2 = 0f;
    }

    void Update()
    {
        timeStart1 += Time.deltaTime;
        timeStart2 += Time.deltaTime;

        if (timeStart1 > delay1)
        {
            SpawnTank(true);
            timeStart1 -= delay1;
        }

        if (timeStart2 > delay2)
        {
            SpawnTank(false);
            timeStart2 -= delay2;
        }
    }

    public void SpawnTank(bool isEnemy)
    {
        Vector3 spawnPosition = new Vector3(
            Random.Range(-spawnRange, spawnRange) + transform.position.x,
            transform.position.y,
            30f + transform.position.z
        );

        Quaternion spawnRotation = Quaternion.AngleAxis(180f, Vector3.up);
        GameObject tankToSpawn = isEnemy ? enemyTank : friendlyTank;

        Instantiate(tankToSpawn, spawnPosition, spawnRotation, transform);
    }

    public void DestroyAllTanks()
    {
        foreach (Transform child in transform)
        {
            if (child.CompareTag("EnemyAI") || child.CompareTag("Friendly"))
            {
                Destroy(child.gameObject);
            }
        }

        timeStart1 = 0f;
        timeStart2 = 0f;
    }
}
