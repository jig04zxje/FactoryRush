using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace FactoryRush.Scripts.Map
{
    public class FakeMachineSpawner : MonoBehaviour
    {
        [SerializeField]
        private GameObject itemPrefab;

        [SerializeField]
        private float spawnInterval = 3f;

        [SerializeField]
        private Transform spawnPoint;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            StartCoroutine(SpawnRoutine());
        }

        IEnumerator SpawnRoutine()
        {
            // Random delay
            yield return new WaitForSeconds(Random.Range(0f, spawnInterval));

            while (true)
            {
                // spawnInterval
                yield return new WaitForSeconds(spawnInterval);

                // Pop up item
                Instantiate(itemPrefab, spawnPoint.position, Quaternion.identity);

                // drop item machine animation
                StartCoroutine(PunchScale());
            }
        }

        IEnumerator PunchScale()
        {
            Vector3 originalScale = transform.localScale;
            transform.localScale = originalScale * 1.1f; 
            yield return new WaitForSeconds(0.1f);
            transform.localScale = originalScale;       
        }
    }

}
