using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Enum of various possible weapon types and inclueds a "shield" type to allow a shield power up
/// </summary>
public enum WeaponType
{
    none,      
    blaster,   
    spread,
    phaser,
    missle,
    laser,
    shield
}

/// <summary>
/// This class allows you to set the properties of a specific weapon in the Inspector.
/// Main class has an array of WeaponDefinitions that makes this possible.
/// </summary>
[System.Serializable]       // allows it to be editable in the inspector
public class WeaponDefinition
{
    public WeaponType type = WeaponType.none;
    public string letter;       // shown on the power up
    public Color color = Color.white;
    public GameObject projectilePrefab;
    public Color projectileColor = Color.white;
    public float damageOnHit = 0;
    public float continuousDamage = 0;
    public float delayBetweenShots = 0;
    public float velocity = 20;
}

public class Weapon : MonoBehaviour
{
    static public Transform PROJECTILE_ANCHOR;      // created to act as a parent

    [Header("set Dynamically")]
    [SerializeField]
    private WeaponType _type = WeaponType.none;
    public WeaponDefinition def;
    public GameObject collar;
    public float lastShotTime;
    private Renderer collarRend;

    // Use this for initialization
    void Start ()
    {
        collar = transform.Find("Collar").gameObject;
        collarRend = collar.GetComponent<Renderer>();

        SetType(_type);

        // Dynamically screate an anchor for all proj
        if (PROJECTILE_ANCHOR == null)
        {
            GameObject go = new GameObject("_ProjectileAnchor");
            PROJECTILE_ANCHOR = go.transform;
        }

        // find the fireDelegate of the root GO
        GameObject rootGO = transform.root.gameObject;
        if(rootGO.GetComponent<Hero>() != null)
        {
            rootGO.GetComponent<Hero>().fireDelegate += Fire;
        }
	}

    public WeaponType type
    {
        get
        {
            return (_type);
        }
        set
        {
            SetType(value);
        }
    }

    public void SetType(WeaponType wt)
    {
        _type = wt;
        if(type == WeaponType.none)
        {
            this.gameObject.SetActive(false);
            return;
        }
        else
        {
            this.gameObject.SetActive(true);
        }
        def = Main.GetWeaponDefinition(_type);
        collarRend.material.color = def.color;
        lastShotTime = 0;
    }

    public void Fire()
    {
        if(!gameObject.activeInHierarchy)
        {
            return;
        }

        if(Time.time - lastShotTime < def.delayBetweenShots)
        {
            return;
        }

        Projectile p;
        Vector3 vel = Vector3.up * def.velocity;

        if(transform.up.y < 0)
        {
            vel.y = -vel.y;
        }

        switch(type)
        {
            case WeaponType.blaster:
                p = MakeProjectile();
                p.rigid.velocity = vel;
                break;

            case WeaponType.spread:
                p = MakeProjectile();       // middle proj
                p.rigid.velocity = vel;

                p = MakeProjectile();       // right proj
                p.transform.rotation = Quaternion.AngleAxis(10, Vector3.back);
                p.rigid.velocity = p.transform.rotation * vel;

                p = MakeProjectile();       // left proj
                p.transform.rotation = Quaternion.AngleAxis(-10, Vector3.back);
                p.rigid.velocity = p.transform.rotation * vel;
                break;
        }
    }

    public Projectile MakeProjectile()
    {
        GameObject go = Instantiate<GameObject>(def.projectilePrefab);
        if(transform.parent.gameObject.tag == "Hero")
        {
            go.tag = "ProjectileHero";
            go.layer = LayerMask.NameToLayer("ProjectileHero");
        }
        else
        {
            go.tag = "ProjectileEnemy";
            go.layer = LayerMask.NameToLayer("ProjectileEnemy");
        }
        go.transform.position = collar.transform.position;
        go.transform.SetParent(PROJECTILE_ANCHOR, true);
        Projectile p = go.GetComponent<Projectile>();
        p.type = type;
        lastShotTime = Time.time;
        return (p);
    }

}
