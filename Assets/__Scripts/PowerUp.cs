using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [Header("Se in Inspector")]
    // this is an unusual but handy us of Vectors. x holds a min value and y a max value for Random.Range()
    public Vector2 rotMinMax = new Vector2(15, 90);
    public Vector2 driftMinMax = new Vector2(0.25f, 2);
    public float lifeTime = 6f;
    public float fadeTime = 4f;

    [Header("Set Dynamically")]
    public WeaponType type;
    public GameObject cube;
    public TextMesh letter;
    public Vector3 rotPerSecond;
    public float birthTime;

    private Rigidbody rigid;
    private BoundsCheck bndCheck;
    private Renderer cubeRend;

    private void Awake()
    {
        cube = transform.Find("Cube").gameObject;
        letter = GetComponent<TextMesh>();
        rigid = GetComponent<Rigidbody>();
        bndCheck = GetComponent<BoundsCheck>();
        cubeRend = GetComponent<Renderer>();

        Vector3 vel = Random.onUnitSphere;  // get random XYZ velocity
        vel.z = 0;  // flatten the vel to the XY plane
        vel.Normalize();    // Normalizing a Vector3 makes it length 1m
        vel *= Random.Range(driftMinMax.x, driftMinMax.y);
        rigid.velocity = vel;

        transform.rotation = Quaternion.identity;    // set the rotation to [0,0,0] or no rotation

        //set up the rotPerSecond for the Cube child using rotMinMax x & y
        rotPerSecond = new Vector3(Random.Range(rotMinMax.x, rotMinMax.y),
            Random.Range(rotMinMax.x, rotMinMax.y),
            Random.Range(rotMinMax.x, rotMinMax.y));

        birthTime = Time.time;
    }
   
	// Update is called once per frame
	void Update ()
    {
        cube.transform.rotation = Quaternion.Euler(rotPerSecond * Time.time);

        // fade out the PowerUp over time
        // will exist for 10 seconds and fade out over 4 seconds
        float u = (Time.time - (birthTime + lifeTime)) / fadeTime;
        // for lifeTime seconds, u will be <= 0. will transition to 1 over the course of fadeTime.

        if(u >= 1)
        {
            Destroy(this.gameObject);
            return;
        }

        // use u to determine the alpha value of the Cube & Letter
        if( u > 0 )
        {
            Color c = cubeRend.material.color;
            c.a = 1f - u;
            cubeRend.material.color = c;
            c = letter.color;
            c.a = 1f - (u * 0.5f);
            letter.color = c;
        }

        if (!bndCheck.isOnScreen)
        {
            Destroy(gameObject);
        }
	}

    public void SetType(WeaponType wt)
    {
        WeaponDefinition def = Main.GetWeaponDefinition(wt);
        cubeRend.material.color = def.color;
        letter.color = def.color;
        letter.text = def.letter;
        type = wt;
    }

    // When a PowerUp is collected, call and destroy
    public void AbsorbedBy(GameObject target)
    {
        Destroy(this.gameObject);
    }
}
