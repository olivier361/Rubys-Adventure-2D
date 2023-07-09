using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float speed = 4.0f;
    public bool vertical;
    public float changeTime = 3.0f;
    public ParticleSystem smokeEffect;
    bool broken = true;

    Rigidbody2D rigidbody2D;
    float timer;
    int direction = 1;

    Animator animator;
    AudioSource audioSource;
    public AudioClip fixClip;
    public AudioClip walkClip;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        timer = changeTime;
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        PlaySound(walkClip, true);

    }

    void Update()
    {
        if (!broken)
        {
            return;
        }

        timer -= Time.deltaTime;

        if(timer <= 0)
        {
            direction = -direction;
            timer = changeTime;
        }
    }

    void FixedUpdate()
    {
        if (!broken)
        {
            return;
        }

        Vector2 position = rigidbody2D.position;
        if (vertical)
        {
            position.y = position.y + Time.deltaTime * speed * direction;
            animator.SetFloat("Move X", 0);
            animator.SetFloat("Move Y", direction);
        }
        else
        {
            position.x = position.x + Time.deltaTime * speed * direction;
            animator.SetFloat("Move X", direction);
            animator.SetFloat("Move Y", 0);
        }
        

        rigidbody2D.MovePosition(position);
    }

    void OnCollisionEnter2D(Collision2D other) //This works like OnTriggerEnter2D but allows us to keep the "solid" physics properties of the box collider.
    {
        RubyController player = other.gameObject.GetComponent<RubyController>(); //sets a player variable to reference the RubyController of the hit object is indeed the player.

        if (player != null)
        {
            player.ChangeHealth(-1);
        }
    }

    public void Fix()
    {
        broken = false;
        animator.SetTrigger("Fixed");
        rigidbody2D.simulated = false;
        //Turns off the rigidbody essentially. So projectiles and players can go through
        //and it won't do damage to the player anymore too.
        //This is an odd game mechanic choice but let's stick with the tutorial for now.
        smokeEffect.Stop();
        //This is better than using Destoy() as destroy removes the particles already generated
        //while stop will just stop generating and let the existing ones fade as usual.
        audioSource.Stop();
        PlaySound(fixClip, false);

    }

    public void PlaySound(AudioClip clip, bool repeat)
    {
        if (!repeat)
        {
            audioSource.loop = false;
            audioSource.PlayOneShot(clip);
        }
        else
        {
            audioSource.loop = true;
            audioSource.Play();
        }
    }
}
