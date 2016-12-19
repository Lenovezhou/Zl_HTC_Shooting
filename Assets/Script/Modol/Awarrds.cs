using UnityEngine;
using System.Collections;

public class Awarrds : MonoBehaviour {

    public GameObject heater, weapon, shield;
    private GameObject A_heater, A_weapon, A_shield;
	void Start () 
    {
	    
	}

   public void Init(GameObject h,GameObject w,GameObject s) 
    {
        heater = h;
        weapon = w;
        shield = s;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("AI"))
        {
            //Debug.Log("aiaiiaiai");
           AIcontroll tempa = other.GetComponent<AIcontroll>();
           if (!tempa.ischangeclose)
           {
               CreateNewAwarrds();
               tempa.ChangeClose(A_heater, A_weapon, A_shield);
           }
        }
    
    }


    void CreateNewAwarrds() 
    {
        A_heater = GameObject.Instantiate(heater) as GameObject;
        A_shield = GameObject.Instantiate(shield) as GameObject;
        A_weapon = GameObject.Instantiate(weapon) as GameObject;
    }
	// Update is called once per frame
	void Update () {
	
	}
}
