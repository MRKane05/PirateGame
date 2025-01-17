using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A quick little script to make a combat engagement every time we come close to a target boat but to increment the difficulities
public class TutorialEngagement : MonoBehaviour
{
    public int Wave = 0;
    public float EngagementRadius = 50f;
    public GameObject EnemyBoatPrefab;
    public GameObject CombatControllerPrefab;
    public GameObject CombatCenter;
    public GameObject PlayerFinishPosition;

    void Update()
    {
        if (SailingGameController.Instance.GameplayMode == SailingGameController.enGameMode.SAILING)
        {
            //Keep an eye on our player distance
            if (Vector3.Distance(gameObject.transform.position, SailingGameController.Instance.PlayerBoat.transform.position) < EngagementRadius)
            {
                Wave++;
                List<GameObject> EnemyBoatPrefabs = new List<GameObject>();
                List<EnemyBoatsDetails> EnemyBoatDetails = new List<EnemyBoatsDetails>();

                //So now something to graduate our details, which I guess can start with one enemy and go up in difficulities
                int numEnemies = Mathf.Clamp(Wave / 2, 1, 3);
                for (int i = 0; i < numEnemies; i++)
                {
                    EnemyBoatPrefabs.Add(EnemyBoatPrefab);
                    //Come up with some clever function for our details, or just say "fuckit" and add random details
                    EnemyBoatsDetails BoatDetails = new EnemyBoatsDetails();
                    BoatDetails.BoatHealth = Mathf.Lerp(100f, 600f, Mathf.Clamp01(Wave / 15f) * Random.Range(0.75f, 1.5f));
                    //Virtual cannons need to be multiples of 2
                    BoatDetails.VirutalCannons = 2 * Mathf.FloorToInt(Mathf.Lerp(2f, 8f, Mathf.Clamp01(Wave / 15f) * Random.Range(0.75f, 1.5f)));
                    EnemyBoatDetails.Add(BoatDetails);
                }

                //Notify our player of the Wave they're undertaking
                //SailingCanvasController.Instance.SetAndDisplayMessage("Combat Engagement Wave: " + Wave.ToString(), 4f);
                SailingGameController.Instance.SetupCombatEngagement(CombatControllerPrefab, EnemyBoatPrefabs, EnemyBoatDetails, CombatCenter.transform.position, PlayerFinishPosition.transform.position);
            }
        }
    }
}
