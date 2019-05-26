using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public float bulletSpeed = 0.5f;
    public Vector3 direction;
    public GameObject shooter; // todo: remove this 
    public Sprite UpLeftSprite, DownRightSprite;
    public float bulletSpawnOffset = 0.5f;
    public int distanceToLive = 1000;
    
    private bool startShooting;
    private Rigidbody2D rb2d;
    private SpriteRenderer spriteRenderer;
    private Vector3 startPos;
    
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        startShooting = false;

        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
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

        UpdateSprite();
        
        transform.position += direction * bulletSpeed * Time.deltaTime;

        var pos = transform.position;
        if ((pos - startPos).magnitude > distanceToLive)
        {
            Destroy(gameObject);
        }
    }

    public void Shoot(Vector2 direction)
    {
        var pos = transform.position;
        pos += new Vector3(direction.x, direction.y) * bulletSpawnOffset;
        transform.position = pos;
        
        this.direction = direction.normalized;
        float angle = Mathf.Atan2(this.direction.y, this.direction.x) * Mathf.Rad2Deg;
        Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * bulletSpeed);

        startPos = pos;
        startShooting = true;
    }
    
    private void UpdateSprite()
    {
        if (direction == IsoVectors.UP)
        {
            spriteRenderer.sprite = UpLeftSprite;
            spriteRenderer.flipX = true;
            return;
        }
        
        if (direction == IsoVectors.LEFT)
        {
            spriteRenderer.sprite = UpLeftSprite;
            spriteRenderer.flipX = false;
            return;
        }

        if (direction == IsoVectors.DOWN)
        {
            spriteRenderer.sprite = DownRightSprite;
            spriteRenderer.flipX = false;
            return;
        }

        if (direction == IsoVectors.RIGHT)
        {
            spriteRenderer.sprite = DownRightSprite;
            spriteRenderer.flipX = true;
            return;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
//    private void OnCollisionEnter2D(Collision2D other)
    {
        if (shooter != null && other.gameObject == shooter)
            return;
        
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Bullet: hit player");
            var playerScript = other.gameObject.GetComponentInParent<PlayerScript>(); 
            playerScript.ReduceLives();
            Destroy(gameObject);
        }
        else
        {
//            Debug.Log("Bullet: destroyed by " + other.gameObject.transform.parent.name);
            Destroy(gameObject);
        }
    }
}
