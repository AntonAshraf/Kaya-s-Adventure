using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMount : MonoBehaviour
{

    public Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.name.Contains("kaya"))
        {
            PlayerMove playerMove = other.gameObject.GetComponent<PlayerMove>();
            if (playerMove.hasItem)
            {
                if (playerMove.isGrounded)
                {
                    other.transform.SetParent(this.transform);
                    anim.SetBool("isMounted", true);
                }
            }

        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "staff")
        {
            return;
        }
        other.transform.SetParent(null);
        anim.SetBool("isMounted", false);
    }
}
