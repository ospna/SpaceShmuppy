using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_2 : Enemy {

    [Header("Set in Inspector: Enemy_2")]
    // determines how much the Sine wave will affect movement
    public float sinEccentricity = 0.6f;
    public float lifeTime = 10;

    [Header("Set Dynamically: Enemy_2")]
    // Enemy_2 uses a Sin wave to modify a 2-point linear interpolation
    public Vector3 p0;
    public Vector3 p1;
    public float birthTime;

    // Use this for initialization
    void Start ()
    {
        // pick any point on the left side of the screen
        p0 = Vector3.zero;
        p0.x = -bndCheck.camWidth - bndCheck.radius;
        p0.y = Random.Range(-bndCheck.camHeight, bndCheck.camHeight);

        // pick any point on the right side of the screen
        p1 = Vector3.zero;
        p1.x = bndCheck.camWidth + bndCheck.radius;
        p1.y = Random.Range(-bndCheck.camHeight, bndCheck.camHeight);

        // swap side (possibly)
        if (Random.value > 0.5f)
        {
            // setting the .x of each point to its negative will move it to the other side
            p0.x *= -1;
            p1.x *= -1;
        }

        birthTime = Time.time;
    }

    public override void Move()
    {
        // Bezier curve works based on a u value between 0 & 1
        float u = (Time.time - birthTime) / lifeTime;

        // If u > 1, has been longer than lifeTime since birth
        if (u > 1)
        {
            Destroy(this.gameObject);
            return;
        }

        // adjust U by adding a U curve based on a Sine wave
        u = u + sinEccentricity * (Mathf.Sin(u * Mathf.PI * 2));

        // inerpolat the two linear iner points
        pos = (1 - u) * p0 + u * p1;


        base.Move();
    }
}
