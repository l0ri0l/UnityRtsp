using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class LampController : MonoBehaviour
{
    private float timeLeft;
    public Image image;
    
    
    public void ChangeColor(Color newColor)
    {
        image.DOColor(newColor, 3f);
    }
        
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.C))
        {
            ChangeColor(Random.ColorHSV());
        }
    }
}
