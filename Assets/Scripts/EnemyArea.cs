using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnemyArea : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float respawnWaitTime = 5;
    //public Vector3 respawnAreaSize = Vector3.one;

    public Vector2 respawnAreaSize;

    private float currentWaitTime;

    private List<GameObject> enemies;
    private Dictionary<GameObject, Vector3> enemyPositions;
    private Dictionary<string, Vector2> enemyMovement;
    private string resEnemy;

    void Start()
    {
        currentWaitTime = respawnWaitTime;
        enemies = new List<GameObject>();
        enemyPositions = new Dictionary<GameObject, Vector3>();
        enemyMovement = new Dictionary<string, Vector2>();

        GetEnemiesInRange();
    }
    private void GetAllEnemies()
    {
        enemies = new List<GameObject>();
        var enems = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject g in enems)
        {
            print(g);
            enemies.Add(g);
            enemyPositions.Add(g, g.transform.position);
            enemyMovement.Add(g.name, new Vector2(g.GetComponent<Enemy>().speed, g.GetComponent<Enemy>().range));
        }
    }
    private void GetEnemiesInRange()
    {
        //enemies = new List<GameObject>();
        var enems = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject g in enems)
        {
            if (IsInRange(g.transform.position))
            {   
                print("funciona");
                var enemy = g.GetComponent<Enemy>();
                StartCoroutine(TrackPosition(enemy));
                enemy.onDeath = ()=>RespawnByDeath(enemy);
                // enemies.Add(g);
                // enemyPositions.Add(g.name, g.transform.position);
                // enemyMovement.Add(g.name, new Vector2(g.GetComponent<Enemy>().speed, g.GetComponent<Enemy>().range));
            }
        }
    }
    private bool IsInRange(Vector3 pos)
    {
        float x = transform.position.x;
        float y = transform.position.y;
        if (pos.x < x + respawnAreaSize.x/2 && pos.x > x - respawnAreaSize.x/2 && pos.y < y + respawnAreaSize.y/2 && pos.y > y - respawnAreaSize.y/2)
            return true;
        return false;
    }

    IEnumerator TrackPosition(Enemy target){
        yield return new WaitWhile(()=> IsInRange(target.transform.position) );
        print("saiu da área");        
        RespawnEnemy(target);
    }
    public void RespawnEnemy(Enemy enemy, float respawnTime = -1f) //-1 usa tempo de espera atual
    {
        // if(!enemies.Contains(g))
        // {
        //     print("don't respawn");
        //     return;
        // }
        if (respawnWaitTime != -1f && respawnWaitTime > 0)
            currentWaitTime = respawnWaitTime;
        //resEnemy = g.name;
        //enemies.Remove(g);
        StartCoroutine(RespawnRoutine(enemy));
    }

    void RespawnByDeath(Enemy enemy){
        RespawnEnemy(enemy);
    }
    private IEnumerator RespawnRoutine(Enemy enemy)
    {
        yield return new WaitForSeconds(currentWaitTime);
        enemy.Respawn();
        //GameObject g = Instantiate(enemy, enemyPositions[enemy], Quaternion.identity);
        //g.name = resEnemy;
        // g.transform.position = enemyPositions[resEnemy];
        // g.GetComponent<Enemy>().speed = enemyMovement[resEnemy][0];
        // g.GetComponent<Enemy>().range = enemyMovement[resEnemy][1];
        // g.GetComponent<SpriteRenderer>().DOColor(Color.clear, 0);
        // g.GetComponent<SpriteRenderer>().DOColor(Color.white, 1).OnStart(() => g.GetComponent<Enemy>().enabled = false).OnComplete(() => g.GetComponent<Enemy>().enabled = true);
        //enemies.Add(g);

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, respawnAreaSize);
    }
}