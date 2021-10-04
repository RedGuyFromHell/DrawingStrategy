using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallHandler : MonoBehaviour
{
    [SerializeField] int ballDamage;

    [HideInInspector] public MeshRenderer mesh;
    [HideInInspector] public int throwerIndex = 0;

    Rigidbody rb;
    bool canMove = true;

    private void Awake()
    {
        mesh = GetComponent<MeshRenderer>();
        StartCoroutine(Lifecycle(8));

        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (transform.position.y < -5)
            Destroy(gameObject);

        if (canMove)
            transform.rotation = Quaternion.LookRotation(rb.velocity, Vector3.up);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Unit")
            if (other.transform.GetComponent<Unit>().playerIndex != throwerIndex)
                other.transform.GetComponent<Unit>().TakeDamage(ballDamage);

        if (other.gameObject.tag == "Ground")
        {
            this.rb.constraints = RigidbodyConstraints.FreezeAll;
            canMove = false;
        }
    }

    IEnumerator Lifecycle (float lifeSpan)
    {
        yield return new WaitForSeconds(lifeSpan);
        Destroy(this.gameObject);
    }
}
