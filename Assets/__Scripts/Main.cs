using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour
{
    static public Main S;
    static Dictionary<WeaponType, WeaponDefinition> WEAP_DICT;

    [Header("Set In Inspector")]
    public GameObject[] prefabEnemies;
    public float enemySpawnPerSecond = 0.5f;
    public float enemyDefaultPadding = 1.5f;
    public WeaponDefinition[] weaponDefinitions;

    private BoundsCheck bndCheck;

    void Awake()
    {
        S = this;
        bndCheck = GetComponent<BoundsCheck>();

        Invoke("SpawnEnemy", 1f / enemySpawnPerSecond);

        // A generic Dictionary with WeaponType as the key
        WEAP_DICT = new Dictionary<WeaponType, WeaponDefinition>();
        foreach(WeaponDefinition def in weaponDefinitions)
        {
            WEAP_DICT[def.type] = def;
        }
    }

    public void SpawnEnemy()
    {
        // picking a random enemy
        int ndx = Random.Range(0, prefabEnemies.Length);
        GameObject go = Instantiate<GameObject>(prefabEnemies[ndx]);

        // positioning the enemy
        float enemyPadding = enemyDefaultPadding;
        if(go.GetComponent<BoundsCheck>() != null)
        {
            enemyPadding = Mathf.Abs(go.GetComponent<BoundsCheck>().radius);
        }

        Vector3 pos = Vector3.zero;

        float xMin = -bndCheck.camWidth + enemyPadding;
        float xMax = bndCheck.camWidth - enemyPadding;

        pos.x = Random.Range(xMin, xMax);
        pos.y = bndCheck.camHeight + enemyPadding;
        go.transform.position = pos;

        Invoke("SpawnEnemy", 1f / enemySpawnPerSecond);
    }

    public void DelayedRestart(float delay)
    {
        Invoke("Restart", delay);
    }

    public void Restart()
    {
        SceneManager.LoadScene("_Scene_0");
    }

    /// <summary>
    /// Static function that gets a WeaponDefinition from the WEAP_DICT
    /// </summary>
    /// <returns> The WeaponDefinition of, if there is no WeaponDefinition with the WeaponType passed in
    /// returns a new WeaponDefinition with a WeaponType of non
    /// </returns>
    /// <param name = "wt"> The WeaponType of the desired WeaponDefinition </param>
    static public WeaponDefinition GetWeaponDefinition(WeaponType wt)
    {
        // checks to see if the key exists in the Dictionary
        // Would throw an error if the key does not the exist
        if (WEAP_DICT.ContainsKey(wt))
        {
            return (WEAP_DICT[wt]);
        }
        // this returns a new WeaponDefinition with a type of WeaponType.none
        // which means that it has failed to find the right WeaponDefinition
        return (new WeaponDefinition());
    }
}
