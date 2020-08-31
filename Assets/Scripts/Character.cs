using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{

    protected SpriteRenderer sR;
    protected Animator anim;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        sR = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        
    }
}
