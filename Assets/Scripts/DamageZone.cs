using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageZone : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D other) //This is continuously triggered every frame where there's a trigger rather than once with OnTriggerEnter2D.
    {
        //Debug.Log("Object that entered the Damage trigger: " + other);
        RubyController controller = other.GetComponent<RubyController>();
        //See HealthCollectible script if you want info on what this line does.

        if (controller != null)
        {
            controller.ChangeHealth(-1); //Calls the RubyController function ChangeHealth.
        }
    }

}
