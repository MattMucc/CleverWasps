using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveTexture : MonoBehaviour
{

    [SerializeField] float speed = 0.1f;
    public bool isMoving = true;

    // Update is called once per frame
    void Update()
    {
        float offset = Time.time * speed;

        // Apply the offset to the material's main texture
        //if (isMoving)
            GetComponent<Renderer>().material.mainTextureOffset = new Vector2(0, offset);
    }
}
