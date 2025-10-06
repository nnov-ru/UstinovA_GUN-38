using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public class Gates : MonoBehaviour
    {
        [SerializeField] private int _score = 0;

        private void OnTriggerEnter(Collider other)
        {
            Ball ball = other.GetComponent<Ball>();
            if (ball != null)
            {
                Destroy(other.gameObject);
                _score++;
                Debug.Log("Score: " + _score);
            }
        }
    }