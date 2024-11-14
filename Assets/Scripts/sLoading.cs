using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sLoading : MonoBehaviour
{

    public float timeLeft;
    // Start is called before the first frame update
    void Start()
    {
        timeLeft = 2.5f;
    }

    // Update is called once per frame
    void Update()
    {
        sMovement.Instance.StopMovement();
        timeLeft -= Time.deltaTime;

        if(timeLeft < 0)
        {
            sMovement.Instance.StartMovement();
            Destroy(gameObject);
        }
    }
}
