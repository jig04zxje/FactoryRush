using UnityEngine;

namespace FactoryRush.Scripts.Map
{
    public class FloatingItem : MonoBehaviour
    {
        [SerializeField]
        private float floatSpeed = 1f;
        [SerializeField]
        private float lifeTime = 1.5f;
        private SpriteRenderer sr;
        private Color color;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            sr = GetComponent<SpriteRenderer>();
            color = sr.color;
            Destroy(gameObject, lifeTime);
        }

        // Update is called once per frame
        void Update()
        {
            // 1. Pop up
            transform.Translate(Vector3.up * floatSpeed * Time.deltaTime);

            // 2. Fade out
            color.a -= Time.deltaTime / lifeTime;
            sr.color = color;
        }
    }
}

