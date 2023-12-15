using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class doorController : MonoBehaviour
{
    Animator _doorAnim;

    bool isLocked = false;


    // Start is called before the first frame update
    void Start()
    {
        _doorAnim = this.transform.parent.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    { if (isLocked == false)
        {
            if (gameManager.instance.enemiesRemaining > 0)
            {
                _doorAnim.SetBool("isOpening", false);
            }

            else
            {
                _doorAnim.SetBool("isOpening", true);
            }
        }

    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isLocked = true;
            _doorAnim.SetBool("isOpening", false);
        }
    }
}
