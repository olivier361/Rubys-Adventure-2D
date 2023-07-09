using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RubyController : MonoBehaviour
{
    public float speed = 3.0f;
    public int maxHealth = 5;
    public float timeInvincible = 2.0f;
    public GameObject projectilePrefab;
    public float projectileForce = 300f;
    public ParticleSystem hitEffect;

    public int health { get { return currentHealth; } } //This is known as a Property definition.
    //Properties can be used like variables in other scripts. Here we use it as a dummy read only variable for currentHealth.
    int currentHealth;

    bool isInvincible;
    float invincibleTimer;

    Rigidbody2D rigidbody2d;
    float horizontal;
    float vertical;

    Animator animator;
    Vector2 lookDirection = new Vector2(1, 0);

    AudioSource audioSource;
    public AudioClip hitClip;
    public AudioClip cogClip;
    public AudioClip walkingClip;
    bool walkingSound = false;

    // Start is called before the first frame update
    void Start()
    {
        /*This seems to disable vSync & drop to 10fps for the whole game.
         *It's not explained what it really does but it's only used
         *to demonstrate a point so we will see.
         */
        //QualitySettings.vSyncCount = 0;
        //Application.targetFrameRate = 10;

        rigidbody2d = GetComponent<Rigidbody2D>(); //Variable is set with the proper reference to Ruby's RigidBody2D component.
        animator = GetComponent<Animator>();

        currentHealth = maxHealth;
        //currentHealth = 1; //for testing only.

        audioSource = GetComponent<AudioSource>();
        walkingSound = false;
    }

    // Update is called once per frame
    void Update()
    {
        //MOVEMENT Inputs:
        horizontal = Input.GetAxis("Horizontal"); //Sets a variable to track input.
        vertical = Input.GetAxis("Vertical");
        //Debug.Log("H: " + horizontal + " V: " + vertical);

        //This section as all about sending data to the animators parameters so it can handle all the player animations.
        //The idea behing the lookDirection variable is so that the animator knows which way we are facing while not moving for idle animations.
        Vector2 move = new Vector2(horizontal, vertical);


        if (!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            lookDirection.Set(move.x, move.y);
            lookDirection.Normalize(); //There is more info on why we normalize the vector in the Sprite Animation Module - Step 11.
            
            if (!walkingSound)
            {
                walkingSound = true;
                PlaySound(walkingClip, true);
                //Debug.Log("Footsteps intensify!");
            }
        }
        else
        {
            /* This is a makeshift script not part of the course that stops the Ruby walkking sound.
             * Since I don't yet know how to stop just one audio clip this works pretty well
             * except for the somewhat rare case where you were walking as a one-time sound plays and you
             * stop walking before it fully ends, then it will brutally stop it. Otherwise it works pretty
             * great even if it's maybe not best practice.
             */
            if (walkingSound)
            {
                audioSource.Stop();
            }
            walkingSound = false;
        }

        
        

        animator.SetFloat("Look X", lookDirection.x);
        animator.SetFloat("Look Y", lookDirection.y);
        animator.SetFloat("Speed", move.magnitude);

        /* This code was the simple movement code we used before adding the physics.
         * It simply changed the Transform coordinates of our GameObject.
         * This is not recommended as it clashes with the physics calculation trying to
         * push our character back out of what it collided with.
         * 
         * It is instead recommended to take the player input in the Update() method
         * and apply the movement in the FixedUpdate() method which is a method that executes
         * at a fixed rate independant from frame rate and is part of the physics calculations
         * rather than taking place before.
         * 
         * The following is the commented out code that was used before the use of Physics:
         */

        /*
        Vector2 position = transform.position; //declare a variable of type Vector2 called position with position values of referenced GAmeObject. //Use Vector3 to also store z-axis.
        position.x = position.x + 3.0f * horizontal * Time.deltaTime; //just adding 0.1f to the variable every frame.
        position.y = position.y + 3.0f * vertical * Time.deltaTime; //we are using the same vector2 as the one initiated in Horizontal movement.

        transform.position = position; //set the GameObjects Transform position to the position variable value.
        */

        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer <= 0)
            {
                isInvincible = false;
            }
        }

        if (Input.GetButtonDown("Fire1")) 
        {
            Launch();
        }

        //This sends a ray to check if there is a collission if front of the main character. Mainly used to initiate dialog.
        if (Input.GetButtonDown("Fire2"))
        {
            RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, lookDirection, 1.5f, LayerMask.GetMask("NPC"));
            if (hit.collider != null)
            {
                //Debug.Log("Raycast has hit the object " + hit.collider.gameObject);
                NonPlayerCharacter character = hit.collider.GetComponent<NonPlayerCharacter>();
                if(character != null)
                {
                    character.DisplayDialog();
                }
            }
        }
    }

    void FixedUpdate()
    {
        Vector2 position = rigidbody2d.position; //declare a variable of type Vector2 called position with position values of referenced GameObject. //Use Vector3 to also store z-axis.
        position.x = position.x + speed * horizontal * Time.deltaTime; //just adding 3.0f to the variable every frame.
        position.y = position.y + speed * vertical * Time.deltaTime; //we are using the same vector2 as the one initiated in Horizontal movement.

        rigidbody2d.MovePosition(position); //set the GameObject's RigidBody2D position to the position variable value.
    }

    public void ChangeHealth(int amount)
    {
        if (amount < 0)
        {
            if (isInvincible)
                return;

            isInvincible = true;
            invincibleTimer = timeInvincible;

            animator.SetTrigger("Hit");
            ParticleSystem hitEffectInstance = Instantiate(hitEffect, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);
            PlaySound(hitClip, false);
        }
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth); //Mathf.Clamp is a built-in function that sets a minimum and maximum range. So the health will be between 0 & maxHealth.
        //Debug.Log(currentHealth + "/" + maxHealth);
        UIHealthBar.instance.SetValue(currentHealth / (float)maxHealth); //Here we have a divistion so we can get a ratio/percentage of the bar to display.
        //We also cast maxHealth as a float to avoid an integer division.
    }

    void Launch()
    {
        GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);
        //Generate an instance of the projectile at correct position. Quaternion.identity means no rotation.
        Projectile projectile = projectileObject.GetComponent<Projectile>(); //Get a reference to that projectiles Projectile component/script.
        projectile.Launch(lookDirection, projectileForce); //Calls the function made in the Projectile.cs script.
        PlaySound(cogClip, false);
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
            audioSource.clip = clip;
            audioSource.Play();
        }
    }

}
