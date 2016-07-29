using UnityEngine;
using System.Collections;

public class RotTest : MonoBehaviour {

    public GameObject other;

	// Use this for initialization
	void Start ()
    {
        print(other.transform.localRotation.eulerAngles);
        other.transform.parent = transform;
        print(other.transform.localRotation.eulerAngles);
        transform.rotation = Quaternion.Euler(0, 60, 0);
        print(other.transform.localRotation.eulerAngles);
        other.transform.parent = null;
        print(other.transform.localRotation.eulerAngles);


    }

    // Update is called once per frame
    void Update () {
	
	}
}
