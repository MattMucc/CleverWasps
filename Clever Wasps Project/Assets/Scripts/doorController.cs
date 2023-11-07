using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class doorController : MonoBehaviour
{
    Animator _doorAnim;
    bool isLocked = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!isLocked)
        {
            _doorAnim.SetBool("isOpening", true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!isLocked)
        {
            _doorAnim.SetBool("isOpening", false);
        }
    }

    public void lockDoor()
    {
        isLocked = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        _doorAnim = this.transform.parent.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
