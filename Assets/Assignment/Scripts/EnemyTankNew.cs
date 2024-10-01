using UnityEngine;

public class EnemyTankNew : MonoBehaviour
{
    public float speed;
    public GameObject AI;

    private Rigidbody rbody;
    // private GameObject enemy;

    private const float posThreshold = -37f;

    void Start()
    {
        rbody = GetComponent<Rigidbody>();
        // enemy = GetComponent<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 moveVect = transform.forward * speed * Time.deltaTime;
        rbody.MovePosition(rbody.position + moveVect);

        if (transform.localPosition.z < posThreshold)
        {
            Destroy(gameObject);
        }
    }

    public void Hit()
    {
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Tank")
        {
            Destroy(gameObject);

        }
    }
}
