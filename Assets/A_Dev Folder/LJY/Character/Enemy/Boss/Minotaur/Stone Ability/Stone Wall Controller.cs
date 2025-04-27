using System.Collections;
using UnityEngine;

public class StoneWallController : MonoBehaviour
{
    [SerializeField] StoneWall[] StoneWalls;
    [SerializeField] float SpawnDelay = .3f;
    [SerializeField] float AttackSpeed = 1f;

    private void Start()
    {
        for(int i = 0; i < StoneWalls.Length; i++)
        {
            //StoneWalls[i].DisableActivate();
            //StoneWalls[i].DisableHitBox();
        }
    }

    public void StartSpawnWall()
    {
        StartCoroutine(SpawnWall());
    }

    IEnumerator SpawnWall()
    {
        int SpawnWallNum = StoneWalls.Length;
        int CurIdx = 0;
        while (SpawnWallNum > 0)
        {
            StoneWalls[CurIdx].EnableActivate();
            StoneWalls[CurIdx].Anim.speed = AttackSpeed;
            yield return new WaitForSecondsRealtime(SpawnDelay);
            CurIdx++;
            SpawnWallNum--;
        }
    }
}
