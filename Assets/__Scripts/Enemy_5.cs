using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_5 : Enemy
{
    [Header("Set in Inspetor: Enemy_5")]
    // # seconds for a full sine wave
    public float waveFrequency = 8;

    // sine wave width in meters
    public float waveWidth = 2;
    public float waveRotY = 45;

    private float x0;       // initial x position of Enemy_5
    private float birthTime;

    // Use this for initialization
    void Start()
    {
        x0 = pos.x;

        birthTime = Time.time;
    }

    public override void Move()
    {
        // cant directly set pos.x so we need to get the pos as an editable Vector3
        Vector3 tempPos = pos;
        float age = Time.time - birthTime;
        float theta = Mathf.PI * 2 * age / waveFrequency;       // theta adjusts based on time
        float sin = Mathf.Sin(theta);
        tempPos.x = x0 + waveWidth * sin;
        pos = tempPos;

        // rotate a bit about y
        Vector3 rot = new Vector3(0, sin * waveRotY, 0);
        this.transform.rotation = Quaternion.Euler(rot);

        // still handles the movement down in y
        base.Move();

        // print(bndCheck.isOnScreen);
    }

}
