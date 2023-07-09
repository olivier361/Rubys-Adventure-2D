using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHealthBar : MonoBehaviour
{
    /*Here we are essentially setting the script to be static so we don't need
     * to refer to a specific instance. Just think of how it worked in our Java course.
     * this line below creates the reference we need and makes sure it is accessible
     * while still not showing up in the inspector.
     * See The User Interface module step 12 for more details.
     */
    public static UIHealthBar instance { get; private set; }

    public Image mask;
    float originalSize;

    //This is in relation to setting the script to be static.
    void Awake()
    {
        instance = this;
    }
    void Start()
    {
        originalSize = mask.rectTransform.rect.width;
    }

    public void SetValue(float value)
    {
        mask.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, originalSize * value);
    }
}
