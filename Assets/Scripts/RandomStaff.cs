using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomStaff : MonoBehaviour
{
    public GameObject arrow;
    public int random;
    void Start()
    {
        GameObject staff1 = GameObject.Find("staff1");
        GameObject staff2 = GameObject.Find("staff2");
        GameObject staff3 = GameObject.Find("staff3");

        random = Random.Range(1, 4);

        if (random == 1)
        {
            staff1.SetActive(true);
            staff2.SetActive(false);
            staff3.SetActive(false);
        }
        else if (random == 2)
        {
            staff1.SetActive(false);
            staff2.SetActive(true);
            staff3.SetActive(false);
        }
        else if (random == 3)
        {
            staff1.SetActive(false);
            staff2.SetActive(false);
            staff3.SetActive(true);
        }

    }

    void Update()
    {
        if (GameObject.Find("kaya").GetComponent<PlayerMove>().hasItem)
            arrow.transform.LookAt(GameObject.Find("Sacred Tree").transform);
        else
            arrow.transform.LookAt(GameObject.Find("staff" + random).transform);

            arrow.transform.Rotate(-90, 90, 0);
    }
}
