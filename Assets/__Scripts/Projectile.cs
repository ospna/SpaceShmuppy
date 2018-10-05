using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private BoundsCheck bndCheck;


	void Awake ()
    {
        bndCheck = GetComponent<BoundsCheck>();
	}
	
	// Update is called once per frame
	void Update ()
    {
		if(bndCheck.offUp)
        {
            Destroy(gameObject);
        }
	}
}
