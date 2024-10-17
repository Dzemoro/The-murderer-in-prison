using System.Collections.Generic;
using UnityEngine;

public class sPortal : MonoBehaviour
{
    private HashSet<GameObject> portalObjects = new HashSet<GameObject>();
    [SerializeField] private Transform destination;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (portalObjects.Contains(collision.gameObject))
            return;

        if (destination.TryGetComponent(out sPortal destinationPortal))
            destinationPortal.portalObjects.Add(collision.gameObject);

        collision.transform.position = destination.position;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        portalObjects.Remove(collision.gameObject);
    }
}