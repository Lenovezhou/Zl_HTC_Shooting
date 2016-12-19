using UnityEngine;
using System.Collections;

public class LookATplayer : MonoBehaviour {
   // public GameObject target;
	// Use this for initialization
	void Start () {
        transform.SetParent(null);
        Debug.Log(Camera.main.transform.position);
	}
	
	// Update is called once per frame
	void Update () {
        transform.LookAt(new Vector3(Camera.main.transform.position.x,5f,Camera.main.transform.position.z));
       // target.transform.position = Camera.main.transform.position;
       // Debug.DrawLine(transform.position, transform.forward * 10,Color.red);
	}
}
