using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthCollectible : MonoBehaviour
{
    public AudioClip collectedClip;
    private void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log("Object that entered the trigger: " + other);
        RubyController controller = other.GetComponent<RubyController>();
        //This returns a link to the RubyController script associated with the object
        //that collided with the trigger. If the object that collided with the trigger
        //doesn't have a RubyController script component, then it will set the
        //controller variable to null. Hense why we check for null below.
        //In short, if there's no script, it's not the player character so do nothing.

        if(controller != null)
        {
            if(controller.health < controller.maxHealth)
            {
                controller.ChangeHealth(1); //Calls the RubyController function ChangeHealth.
                Destroy(gameObject); //Destroys the gameObject, i.e. The Health pack.
                controller.PlaySound(collectedClip, false); //Calls playSound function from the RubyController script.
            }
        }
    }
}
