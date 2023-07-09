using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour  
{
    Rigidbody2D rigidbody2d;

    /* Here we used Awake() instead of Start() because start is done at the first frame after the onject is instantiated,
     * But in this case to avoid a NullPointerException, we want this to be done WHILE the onject is instanciated
     * and so that is exactly what Awake() does. It's a variant on Start() essentially.
     */
    void Awake() 
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        /* For performance reasons, it's important to think of destroying a projectile
         * that didn't collide with anything and is now ways off screen.
         */
        if(transform.position.magnitude > 100.0f)
        //Since position treated by unity as a vector and magnitude is a keywords to check for vector lenght,
        //this is essentially checking that our projectile is less than 1000 units away from the center of
        // the scene. If so, it destoys it.
        {
            Destroy(gameObject);
        }

        /* Other alternatives to this are to have a timer or the bullets, or check the distance to player.
         * 
         * Extract from tutorial:
         * "For example, you could get the distance between the character and the cog
         * (with the function Vector3.Distance (a,b) to compute the distance
         * between the position a and position b)."
         */
    }

    public void Launch(Vector2 direction, float force)
    {
        rigidbody2d.AddForce(direction * force);
    }

    /* To avoid collision problems with the player itself (which would aslo stop the projectile's movement),
     * we want to use different layers for different types of objects.
     */
        void OnCollisionEnter2D(Collision2D other)
    {
        //Debug.Log("Projectile collission with " + other.gameObject); //this one refers to the object hit.

        EnemyController e = other.collider.GetComponent<EnemyController>();
        if(e != null)
        {
            e.Fix();
        }
        Destroy(gameObject); //this one refers to that instance of the projectile.
    }
}
