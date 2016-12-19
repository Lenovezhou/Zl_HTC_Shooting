using UnityEngine;
using System.Collections;

public class AiOnHurt : MonoBehaviour {

    public AIcontroll acontroll;

	void Start () {
	
	}
	
	void Update () {
	
	}
    Vector3 temppos;
    void OnTriggerEnter(Collider collision)
    {
        //Debug.Log(collision.name);
        switch (collision.tag)
        {
            case "Player":
                acontroll.status = aistatus.attack;
                //  InvokeRepeating("Attack", 1f, 2f);
                break;
            case "AI":
                break;
            case "PlayerBullet":
               //temppos = collision.transform.position;
               Bulletcontroll bcontroll = collision.GetComponent<Bulletcontroll>();
               acontroll.CateDownHp(bcontroll.damage);
               // Debug.Log("Onhurt"+collision.transform.gameObject);
               //     Debug.Log("Onhurtif" + collision.transform.gameObject);
               //     ArrowStop(collision.transform, collision.GetComponent<TrailRenderer>()
               //         , collision.GetComponent<Rigidbody>()
               //         ,collision.GetComponent<BoxCollider>(), true);
                break;
            case "Walls":
                break;
            default:
                break;
        }
    }

    void ArrowStop(Transform t,TrailRenderer trail,Rigidbody rb,BoxCollider box, bool canset = false)
    {
          Debug.Log(t.tag);
        if (canset)
        {
            t.SetParent(transform);
        }
        t.transform.position = temppos;
        trail.enabled = false;
        rb.isKinematic = true;
        rb.freezeRotation = true;
        rb.useGravity = false;
        box.enabled = false;
    }


}
