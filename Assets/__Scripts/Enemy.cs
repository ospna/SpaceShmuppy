using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Set In Inspector")]
    public float speed = 10f;       // the speed in m/s
    public float fireRate = 0.3f;   // secd=onds per shot
    public float health = 10;
    public int score = 100;         // points earned for destroying
    public float showDamageDuration = 0.1f;     // # seconds to show damage
    public float powerUpDropChance = 1f;

    [Header("Set Dynamically")]
    public Color[] originalColors;
    public Material[] materials;
    public bool showingDamage = false;
    public float damageDoneTime;
    public bool notifiedOfDestruction = false;

    protected BoundsCheck bndCheck;

    void Awake()
    {
        bndCheck = GetComponent<BoundsCheck>();

        materials = Utils.GetAllMaterials(gameObject);
        originalColors = new Color[materials.Length];
        for(int i = 0; i < materials.Length; i++)
        {
            originalColors[i] = materials[i].color;
        }
    }

    // Property : method that acts like a field
    public Vector3 pos
    {
        get
        {
            return (this.transform.position);
        }
        set
        {
            this.transform.position = value;
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        Move();

        if(showingDamage && Time.time > damageDoneTime)
        {
            UnShowDamage();
        }

        if(bndCheck != null && bndCheck.offDown)
        {
            // check to make sure its gone off screen
            if(pos.y < bndCheck.camHeight - bndCheck.radius)
            {
                Destroy(gameObject);
            }
        }
	}

    // get current position and move it downward in the Y direction
    public virtual void Move()
    {
        Vector3 tempPos = pos;
        tempPos.y -= speed * Time.deltaTime;
        pos = tempPos;
    }

    void OnCollisionEnter(Collision coll)
    {
        GameObject otherGO = coll.gameObject;
        switch (otherGO.tag)
        {
            case "ProjectileHero":
                Projectile p = otherGO.GetComponent<Projectile>();
                // if this enemy is off screen, dont damage it
                if (!bndCheck.isOnScreen)
                {
                    Destroy(otherGO);
                    break;
                }

                // Hurt this enemy by getting the damage amount from the WEAP_DICT
                ShowDamage();
                health -= Main.GetWeaponDefinition(p.type).damageOnHit;
                if(health <= 0)
                {
                    // notify the Main singleton that this ship has benen destroyed
                    if(!notifiedOfDestruction)
                    {
                        Main.S.ShipDestroyed(this);
                    }
                    notifiedOfDestruction = true;

                    Destroy(this.gameObject);
                }
                Destroy(otherGO);
                break;

            default:
                print("Enemy hit by non-ProjectileHero: " + otherGO.name);
                break;
        }
    }

    void ShowDamage()
    {
        foreach(Material m in materials)
        {
            m.color = Color.red;
        }
        showingDamage = true;
        damageDoneTime = Time.time + showDamageDuration;
    }

    void UnShowDamage()
    {
        for(int i = 0; i < materials.Length; i++)
        {
            materials[i].color = originalColors[i];
        }
        showingDamage = false;
    }
}
