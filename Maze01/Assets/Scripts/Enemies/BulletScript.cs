using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public float bulletSpeed = 0.5f;
    public Vector3 direction;
    public GameObject shooter; // todo: remove this 
    
    private bool startShooting;
    private Rigidbody2D rb2d;
    
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        startShooting = false;
    }

    void FixedUpdate()
    {
//        if (!startShooting)
//            return;
        
//        rb2d.AddForce(direction);
//        var velocity = rb2d.velocity;
//        if (velocity.magnitude > bulletSpeed)
//        {
//            rb2d.velocity = velocity.normalized * bulletSpeed;
//        }
//        rb2d.velocity = direction * bulletSpeed;
        transform.position += direction * bulletSpeed * Time.deltaTime;
    
    }

    public void Shoot(Vector2 direction)
    {
        this.direction = direction.normalized;
        float angle = Mathf.Atan2(this.direction.y, this.direction.x) * Mathf.Rad2Deg;
        Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * bulletSpeed);

        startShooting = true;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (shooter != null && other.gameObject == shooter)
            return;
        
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Bullet: hit player");
            var playerScript = other.gameObject.GetComponentInParent<PlayerScript>();            
        }
        else
        {
//            Debug.Log("Bullet: destroyed by " + other.gameObject.name);
            Destroy(gameObject);
        }
    }
}
