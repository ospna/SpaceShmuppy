using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Part is another serialiazble data storage class like WeaponDefintion
/// This holds information about each part for Enemy_4 (Cokpit, Fuselage and WingL & R)
/// </summary>
[System.Serializable]
public class Part
{
    // set in inspector
    public string name;
    public float health;
    public string[] protectedBy;

    // set automatically in Start()
    [HideInInspector]
    public GameObject go;

    [HideInInspector]
    public Material mat;
}

/// <summary>
/// This enemy will start offscreen and then pick a random point on screen to move to.
/// </summary>
public class Enemy_4 : Enemy
{
    [Header("Set in Inspector: Enemy_4")]
    public Part[] parts;        // array of the ship parts

    private Vector3 p0, p1;
    private float timeStart;
    private float duration = 4;

	// Use this for initialization
	void Start ()
    {
        p0 = p1 = pos;

        InitMovement();

        // Cache GO & Mat of each Part in parts
        Transform t;
        foreach(Part prt in parts)
        {
            t = transform.Find(prt.name);
            if( t != null)
            {
                prt.go = t.gameObject;
                prt.mat = prt.go.GetComponent<Renderer>().material;

            }
        }
	}
	
    void InitMovement()
    {
        p0 = p1;

        float widMinRad = bndCheck.camWidth - bndCheck.radius;
        float hgtMinRad = bndCheck.camHeight - bndCheck.radius;
        p1.x = Random.Range(-widMinRad, widMinRad);
        p1.y = Random.Range(-hgtMinRad, hgtMinRad);

        timeStart = Time.time;

    }

    public override void Move()
    {
        // overrides enemy with a linear interpolation
        float u = (Time.time - timeStart) / duration;

        if (u >= 1)
        {
            InitMovement();
            u = 0;
        }

        u = 1 - Mathf.Pow(1 - u, 2);    // apply Ease Out easing to u
        pos = (1 - u) * p0 + u * p1;    // linear interpolation

        // base.Move();
    }

    Part FindPart(string n)
    {
        foreach (Part prt in parts)
        {
            if (prt.name == n)
            {
                return (prt);
            }
        }
        return(null);
    }

    Part FindPart(GameObject go)
    {
        foreach(Part prt in parts)
        {
            if(prt.go == go)
            {
                return(prt);
            }
        }
        return(null);
    }

    // these functions return true if the Part has been destroyed
    bool Destroyed(GameObject go)
    {
        return(Destroyed (FindPart (go) ));
    }
    bool Destroyed(string n)
    {
        return(Destroyed (FindPart (n) ));
    }
    bool Destroyed(Part prt)
    {
        if(prt == null)
        {
            return (true);
        }
        return (prt.health <= 0);
    }

    // changes the color of only one part of the ship
    void ShowLocalizedDamage(Material m)
    {
        m.color = Color.red;
        damageDoneTime = Time.time + showDamageDuration;
        showingDamage = true;
    }

    private void OnCollisionEnter(Collision coll)
    {
        GameObject other = coll.gameObject;
        switch(other.tag)
        {
            case "ProjectileHero":
                Projectile p = other.GetComponent<Projectile>();
                if(!bndCheck.isOnScreen)
                {
                    Destroy(other);
                    break;
                }

                // Hurt this enemy
                GameObject goHit = coll.contacts[0].thisCollider.gameObject;
                Part prtHit = FindPart(goHit);
                if(prtHit == null)
                {
                    goHit = coll.contacts[0].otherCollider.gameObject;
                    prtHit = FindPart(goHit);
                }
                // check whether this part is still protected
                if(prtHit.protectedBy != null)          // if the protecting part hasn't been destroyed
                {
                    foreach(string s in prtHit.protectedBy)
                    {
                        if(!Destroyed(s))               // dont damage this
                        {
                            Destroy(other);    
                            return;
                        }
                    }
                }
                // its not protected, so make it take damage
                prtHit.health -= Main.GetWeaponDefinition(p.type).damageOnHit;
                ShowLocalizedDamage(prtHit.mat);

                if(prtHit.health <= 0)
                {
                    prtHit.go.SetActive(false);     // disable the damaged part
                }

                // check to see if the whole ship is destroyed
                bool allDestroyed = true;
                foreach (Part prt in parts)
                {
                    if(!Destroyed(prt))     // if a part still exists
                    {
                        allDestroyed = false;
                        break;
                    }
                }
                if(allDestroyed)
                {
                    Main.S.ShipDestroyed(this);
                    Destroy(this.gameObject);
                }
                Destroy(other);
                break;
        }
    }
}
