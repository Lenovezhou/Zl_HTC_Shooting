using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class AIcontroll : MonoBehaviour {

    public Bulletcontroll bulletcontroller;
    public Vector3 endpos, uppos;
    public MSGcenter msg;
    public Vector3 look_gate;
    public bool isattack = false,isalive = false,ischangeclose = false;
    public int gread;
    public GameObject Bip01;
    public GameObject arrowtarget;
    public int index;
    private int hp,attack;
    Rigidbody rb;
    NavMeshAgent nav;
   public aistatus status;
    aistatus tempstatus;
    Animator anim;
    bool isdead = false;
    string[] clipsnames;
    GameObject neck,head, lefthand, righthand;
    GameObject oldheater, oldweapon, oldshield;
  //  Vector3 playerHurtPos;
    public int HP 
    {
        get 
        { 
            //Debug.Log("aiaiaiiaHPHPHPHP"+hp);
            return hp; 
        }
        set { hp = value; }
    }
    public int InDex 
    {
        get { return index; }
        set { index = value; }
    }
    public int ATTACK 
    {
        get { return attack; }
        set { attack = value; }
    }


    public void ChangeClose(GameObject heater,GameObject weapon,GameObject shield) 
    {
        if (!ischangeclose)
        {
            if (heater != null)
            {
                heater.transform.SetParent(head.transform);
                heater.transform.localPosition = Vector3.zero;
                HP += 10;
                gread += 10;
                oldheater = heater;
            }
            if (weapon != null)
            {
                HP += 10;
                gread += 10;
                weapon.transform.SetParent(righthand.transform);
                weapon.transform.localPosition = Vector3.zero;
                oldweapon = weapon;
            }
            if (shield != null)
            {
                HP += 10;
                gread += 10;
                shield.transform.SetParent(lefthand.transform);
                shield.transform.localPosition = Vector3.zero;
                oldshield = shield;
            }
            ischangeclose = true;
        }
       
    }


    /// <summary>
    /// 缓冲池重置所有属性
    /// </summary>
    void OnEnable() 
    {
        rb = gameObject.AddComponent<Rigidbody>();
        //Debug.Log("OnEnable执行"+transform.position);
        status = aistatus.run;
        oldname = string.Empty;
        isdead = false;
        Bip01 = transform.FindChild("xiongdonghua/Bip01").gameObject;
       // arrowtarget = transform.FindChild("xiongdonghua/ArrowTarget").gameObject;
        AiOnHurt ao = Bip01.AddComponent<AiOnHurt>();
        ao.acontroll = this;
        neck = Bip01.transform.FindChild("Bip01 骨盆/Bip01 Spine/Bip01 Spine1/Bip01 Neck").gameObject;
        head = neck.transform.FindChild("Bip01 头部/Bip01 头部Nub").gameObject;
        lefthand = neck.transform.FindChild("Bip01 L Clavicle/Bip01 L UpperArm/Bip01 L Forearm/Bip01 L Hand").gameObject;
        righthand = neck.transform.FindChild("Bip01 R Clavicle/Bip01 R UpperArm/Bip01 R Forearm/Bip01 R Hand").gameObject;
        //Debug.Log("neck::"+neck+head+lefthand+righthand);
   //生成时检查是否身上带有箭并销毁
        for (int i = 0; i < Bip01.transform.childCount; i++)
        {
            if (Bip01.transform.GetChild(i).name.Equals("Arrow(Clone)"))
            {
                Bip01.transform.GetChild(i).GetComponent<Bulletcontroll>().AddToPool();
            }
        }

        Destroy(oldshield);
        Destroy(oldweapon);
        Destroy(oldheater);

    } 


	// Use this for initialization
	void Awake () 
    {
      //  rb.AddForce(Vector3.up * 10f);
        //Debug.Log("Awake执行：："+transform.position);
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
	}

    void OnTriggerEnter( Collider collision) 
    {
        switch (collision.tag)
        {
            case "Player":
                status = aistatus.attack;
                //  InvokeRepeating("Attack", 1f, 2f);
                break;
            case "AI":
                if (nav.enabled)
                {
                 //   nav.SetDestination(uppos);
                }
    
                break;
            case "PlayerBullet":
                //  Destroy(gameObject);
               
                    Bulletcontroll bcontroll =collision.GetComponent<Bulletcontroll>();
                    CateDownHp(bcontroll.damage);
                    //  collision.transform.SetParent(transform);
                
                //Debug.Log("collisionPlayerBullet" + collision.name + HP);
                break;
            case "Walls":
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 减血调用
    /// </summary>
    /// <param name="downHP"></param>
   public void CateDownHp(int downHP) 
    {
        //Debug.Log(downHP+"剩余血量：：：："+hp);
        if (hp > 0)
        {
            HP -= downHP;
            msg.FlyText(false, downHP, transform.position);
        }    
        else
        {
            status = aistatus.die;
            if (!isdead)
            {
                msg.FlyText(true, gread, transform.position, true);
                isdead = true;
            }
        }
    }

    void OnTriggerExit(Collider collision) 
    {
        switch (collision.tag)
        {
            case "AI":
                if (nav.enabled)
                {
                //    nav.SetDestination(endpos);
                }
                //Debug.Log("1111111碰撞结束终点切换为：" + endpos);
                break;
        }
    }



    void OnCollisionEnter(Collision collision)
    {

    }


   void OnCollisionExit( Collision collision) 
    {
       
    }

    void Aideath() 
    {
        //paioz
      //  status = aistatus.die;
        msg.aliveAI--;
        if (transform.FindChild("Arrow(Clone)"))
        {
            //Debug.Log("销毁子物体ARROWQ");
            for (int i = 0; i < transform.childCount; i++)
            {
                /*缓冲池准备*/
                transform.FindChild("Arrow(Clone)").gameObject.SetActive(false);
              //  Destroy(transform.FindChild("Arrow(Clone)").gameObject);
            }
        }
        gameObject.SetActive(false);
        msg.aliveai.Remove(this);
        if (rb != null)
        {
            Destroy(rb);
        }
        switch (InDex)
        {
            case 0:
                if (!msg.ai_pool.ContainsKey(0))
                {
                    msg.ai_pool.Add(0, new List<GameObject>());
                }
                msg.ai_pool[0].Add(gameObject);
                break;
            case 1:
                if (!msg.ai_pool.ContainsKey(1))
                {
                    msg.ai_pool.Add(1, new List<GameObject>());
                }
                msg.ai_pool[1].Add(gameObject);
                break;
            case 2:
                if (!msg.ai_pool.ContainsKey(2))
                {
                    msg.ai_pool.Add(2, new List<GameObject>());
                }
                msg.ai_pool[2].Add(gameObject);
                break;
            default:
                break;
        }
    }



    /// <summary>
    ///aidead动画事件
    /// </summary>
    void NewEvent() 
    {
       // Debug.Log("aideathaideath"+Time.time);
        Aideath();
    }

    /// <summary>
    /// attack动画添加player减血事件
    /// </summary>
    //void ReduceHP() 
    //{
    //    Attack();
    //}


   public bool issetdes = false;
    void Update () 
    {
     //   arrowtarget.transform.localEulerAngles =new Vector3(Bip01.transform.localEulerAngles.x,0,0);
        //Debug.Log(transform.position);
        ChoiseStatus();
        if (gameObject.activeSelf && !issetdes )
        {
            nav.SetDestination(endpos);
            //Debug.Log(endpos);
            issetdes = true;
        }
        if (status == aistatus.attack)
        {
            gameObject.transform.LookAt(look_gate);
        }
	}


    void ChoiseStatus() 
    {
        if (Vector3.Distance(transform.position, endpos) <= 2f && !isattack)
        {
            status = aistatus.attack;
            isattack = true;
        }
        if (!isalive && Vector3.Distance(transform.position, endpos) > 1.5f)
        {
            status = aistatus.run;
           // Debug.Log("rrrrrrrrrr");
            isalive = true;
        }
        //if (Vector3.Distance(transform.position, uppos) <= 0.5f )
        //{
        //    nav.SetDestination(endpos);
        //}
        //Debug.Log(status.ToString());
        if (status != tempstatus)
        {
            //Debug.Log(status.ToString());
            tempstatus = status;
            switch (status)
            {
                case aistatus.ido:
                    AnimPlay("isidle", true);
                    break;
              
                case aistatus.attack:

                      AnimPlay("isattack",true);
                      
                    break;
                case aistatus.die:
                     //Debug.Log("执行多少次？");
                 //   msg.FlyText(true, gread, transform.position,true);
                    AnimPlay("isdead", true);
                   
                    break;
                case aistatus.walk:
                case aistatus.inspeackter:
                case aistatus.damage:
                case aistatus.run:
                 //默认动画为行走,第二次开始需打开navmeshagint
                    AnimPlay("", false);
                    break;
                default:
                    break;
            }
        }
    }

    string oldname;
    void AnimPlay(string clipname,bool isnot) 
    {
       
        if (oldname != clipname)
        {
            anim.SetBool(oldname, false);
            //Debug.Log("播放"+clipname+"动画");
            anim.SetBool(clipname, true);
        }
        oldname = clipname;

        if (rb != null && isnot)
        {
            //rb.Sleep();
            rb.useGravity = false;
            rb.freezeRotation = true;
            rb.isKinematic = true;
           // Destroy(rb);
            nav.enabled = false;
        }
        else if (rb != null && !isnot)
        {
            //rb.WakeUp();
            rb.useGravity = true;
            rb.isKinematic = false;
            rb.freezeRotation = false;
            nav.enabled = true;
        }
    }

   

    /// <summary>
    /// 动画调用事件
    /// </summary>
    void Attack() 
    {
        msg.CateGateHP(attack);
    }

    void Awards() 
    {
        
    }
}
