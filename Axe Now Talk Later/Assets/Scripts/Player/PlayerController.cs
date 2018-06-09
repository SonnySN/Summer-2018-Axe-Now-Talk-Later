using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * CREATED by MINH
 * SUMMARY: button inputs mapping and listener
 */
public class PlayerController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	



	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKey(KeyCode.W))
        {
            print("Move Up");
        }
        else if (Input.GetKey(KeyCode.S))
        {
            print("Move Down");
        }
        else if (Input.GetKey(KeyCode.A))
        {
            print("Move Left");
        }
        else if (Input.GetKey(KeyCode.D))
        {
            print("Move Right");
        }

        //TODO: 
    }
}
