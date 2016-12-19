using UnityEngine;
using System.Collections;

public class Bulletcontroll : MonoBehaviour {
    public bool islunch = false, isaddtopool = false,istrial = false,ishit = false,isset = false;
    public int damage=10;
    public MSGcenter msg;
    public PlayerController pcontroll;
    public Rigidbody rb;
    public float timer,stoptimer;
    public Vector3 endpos;
    GameObject child;
    TrailRenderer trail;
    BoxCollider box;
    GameObject Bip01;

    Vector3 hitpoint;
	// Use this for initialization
	void Start () 
    {
      //  rb = GetComponent<Rigidbody>();
        trail = GetComponent<TrailRenderer>();
        box = GetComponent<BoxCollider>();
    //    child = transform.FindChild("Bullet").gameObject;
	}
    void OnTriggerEnter ( Collider other  ) 
    {
        //Debug.Log("collisionEnter" + other.name);
        switch (other.tag) 
        {
            case "Player":
            case "Bullet":
                break;
            case "Torch":/*火炬*/
                // Debug.Log("huojiu111111111");
                if (pcontroll.bstatus == bulletstatus.ready)
                {
                 //   Debug.Log("torchifiififi");
                    pcontroll.kinds = ArrowsKind.torch;
                }
                break;
            case "AI":
              //  Debug.Log("OntriggerEnterAIIAI:::::::"+other.name);
                break;
            case "Walls":
                //Debug.Log("OnWalls   hit ");
                //trail.enabled = false;
                //if (rb != null)
                //{
                //    Debug.Log("stop");
                //    rb.isKinematic = true;
                //    rb.freezeRotation = true;
                //    rb.useGravity = false;
                //}
                //else
                //{
                //    Debug.LogError("没有Rigidbody！！！！");
                //}
                break;
            case "OnHurt":
                ArrowStop(other.transform,endpos,true);
                
              //  Debug.Log("OnHurtOnHurt00000"+other.name);
                //Bip01 = other.gameObject;
                //Transform t = null;
                //if (other.transform.tag.Equals("AI"))
                //{
                //    Debug.Log("Onhurtif111111" + other.transform.gameObject);
                //    t = other.transform.FindChild("xiongdonghua/Bip01");
                //    ArrowStop(t, true);
                //}
                //else if (other.transform.tag.Equals("OnHurt"))
                //{
                //    Debug.Log("Onhurtif222222" + other.transform.gameObject);

                //    t = other.transform;
                //    ArrowStop(t, true);
                //}
                //else
                //{
                //    //Debug.Log("不能改变父物体！！！！");
                //}

                //if (rb != null)
                //{
                //    rb.isKinematic = true;
                //    rb.freezeRotation = true;
                //    rb.useGravity = false;
                //    //防止未销毁的箭，不停伤害
                   
                //}
                //else
                //{
                //    Debug.LogError("没有rigidbody！！！！");
                //} 

                break;
            default:
                break;
        }
     //   hitpoint = transform.position;
     //   ishit = true;

	    //Destroy(other.gameObject);
    }

    /// <summary>
    /// 保证碰撞到Walls时，和Ai时都有停留效果
    /// </summary>
    /// <param name="collision"></param>
    void OnCollisionEnter( Collision collision) 
    {
        switch (collision.collider.tag)
        {
            case "Player":
            case "Bullet":
                break;
            case "AI":
                break;
            case "Torch":/*火炬*/
                break;
            case "Walls":
               // Debug.Log("collisionenterwall");
                ArrowStop(collision.transform,collision.collider.transform.position);
                break;
            case "OnHurt":
                //Transform t = null;
                Debug.Log("Onhurt"+collision.transform.gameObject);
                //if (collision.transform.tag.Equals("AI"))
                //{
                //    Debug.Log("Onhurtif" + collision.transform.gameObject);
                //    t = collision.transform.FindChild("xiongdonghua/Bip01");
                //    ArrowStop(t, true);
                //}
                //else if (collision.transform.tag.Equals("Onhurt"))
                //{
                //    Debug.Log("Onhurtif" + collision.transform.gameObject);
                //    t = collision.transform;
                //    ArrowStop(t, true);
                //}
               // else {
                    //Debug.Log("不能改变父物体！！！！");
               // }
                
                break;
            default:
                break;
        }
        trail.enabled = false;
    }



   public void ArrowStop(Transform t,Vector3 stoppos ,bool canset=false) 
    {
        Debug.Log(stoppos);
        trail.enabled = false;
        rb.isKinematic = true;
        rb.freezeRotation = true;
        rb.useGravity = false;
        box.enabled = false;
        if (transform.FindChild("Fire_0") )
        {
            transform.FindChild("Fire_0").gameObject.SetActive(false);
        }
        if (canset)
        {
            Debug.Log("sh世界坐标" + stoppos);
            Debug.Log("本地坐标" + transform.position);
            transform.SetParent(t);
            transform.localPosition= new Vector3(transform.localPosition.x,transform.localPosition.y,0.5f);
            Debug.Log("sh" + transform.position);
        }
    }


    //Ray ray;
    //RaycastHit hit;
	// Update is called once per frame
	void Update () {
        //ray.origin = transform.position;
        //ray.direction = (transform.up * 3 - transform.position) ;
        //Debug.DrawLine(ray.origin, ray.direction, Color.red);
        Debug.DrawLine(transform.position, -(transform.up * 3 - transform.position), Color.red);
        //Debug.DrawLine(transform.position, transform.forward * 3, Color.red);
        //  child.transform.rotation = Quaternion.identity;
        if (islunch)
        {
            if (!istrial)
            {
                trail.enabled = true;
                istrial = true;
            }
            timer += Time.deltaTime;
            if (timer > 10f)
            {
                AddToPool();
            }
        }
        LaseTranslate();
	}

    void LaseTranslate() 
    {
        if (ishit)
        {
            Debug.DrawLine(transform.position, -transform.forward * 3, Color.red);
            stoptimer += Time.deltaTime;
            if (stoptimer > 0.05f)
            {
                if (Bip01 != null)
                {
                    if (!isset)
                    {
                        transform.SetParent(Bip01.transform);
                        isset = true;
                    }
                    if (Vector3.Distance(hitpoint, transform.position) < 0.1f)
                    {
                        transform.Translate(transform.forward * Time.deltaTime, Space.Self);
                    }

                }
                else
                {
                    Debug.LogError("没有Bip01！！！！");
                    if (Vector3.Distance(hitpoint, transform.position) < 1f)
                    {
                        Debug.Log("translate");
                        transform.Translate(transform.forward * Time.deltaTime, Space.Self);
                    }

                }
            }
        }
    }



   public void AddToPool() 
    {
        //Debug.Log("addadddadddadddaddd"+isaddtopool);
        if (!isaddtopool)
        {
           // Debug.Log("addtopool" + rb == null);
            box.enabled = true;
            islunch = false;
            if (!msg.ai_pool.ContainsKey(5))
            {
                msg.ai_pool.Add(5, new System.Collections.Generic.List<GameObject>());
            }
            msg.ai_pool[5].Add(gameObject);
            isaddtopool = true;
            gameObject.SetActive(false);
            //Debug.Log("complete add to pool !!!!!!!");
        }
    }

}
