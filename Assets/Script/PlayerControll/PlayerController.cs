using UnityEngine;
using System.Collections;

public enum firestatus 
{
reday,fireon,fireout
}


public class PlayerController : MonoBehaviour {
    public Person player;
    public MSGcenter msg;
    public GameObject hit_sphere;
    public GameObject bullet, arrows;/*箭的父物体和子物体*/
    private GameObject bowstring;/*弓弦*/
    public GameObject leftrain, rightrain,bow;
    private float timer, bulletlifetimer;
    private GameObject handfirst;

    private Vector3 bowsoldpos;


    /// <summary>
    /// 箭的种类,在箭头碰到火或者冰时改变
    /// </summary>
    [HideInInspector]
    public ArrowsKind kinds;

    /// <summary>
    /// 游戏状态
    /// </summary>
    [HideInInspector]
    public shootstatus status;

    /// <summary>
    /// 射箭状态
    /// </summary>
    public bulletstatus bstatus;

    /// <summary>
    /// 箭头着火的状态
    /// </summary>
    public firestatus fire_status;

    private string name;
    private int hp;
    public string NAME
    {
        set { name = value; }
        get { return name; }
    }

    public int HP 
    {
        set { hp = value; }
        get { return hp; }
    }

    public void Init
        (GameObject _leftrain,GameObject _rightrain,MSGcenter _msg
        ,GameObject _hit_sphere,GameObject _bullet,GameObject _arrow
        ,GameObject target,GameObject _bowstring,Vector3 bowsoldpoint) 
    {
        leftrain = _leftrain;
        rightrain = _rightrain;
        msg = _msg;
        hit_sphere = _hit_sphere;
        bullet = _bullet;
        arrows = _arrow;
        bow = target;
        bowstring = _bowstring;
        bowsoldpos = bowsoldpoint;
    }

    void Awake() 
    {
        kinds = ArrowsKind.normal;
        status = shootstatus.none;
        bstatus = bulletstatus.none;
        fire_status = firestatus.reday;
        mask = LayerMask.GetMask("Floor");
    }

	// Use this for initialization
	void Start () 
    {
        name = player.NAME;
        hp = player.HP;
        msg.endcall += GameEnd;
	}


    void GameEnd() 
    {
        status = shootstatus.lose;
    }

    bool isleftfind = false, isrightfind = false;
    /// <summary>
    /// 手柄消失
    /// </summary>
    /// <param name="g"></param>
    void DestoryMesh(GameObject g)
    {
        if (g != null)
        {
            MeshRenderer[] meshs = g.transform.FindChild("Model").gameObject.GetComponentsInChildren<MeshRenderer>();
            // Debug.Log(">>>>>>>>>>>>>" + meshs.Length + g.transform.FindChild("Model").gameObject);
            for (int i = 0; i < meshs.Length; i++)
            {
                meshs[i].enabled = false;
            }
            if (g.name.Equals("[CameraRig]/Controller (left)") && meshs.Length > 0)
            {
                isleftfind = true;
            }
            if (g.name.Equals("[CameraRig]/Controller (right)") && meshs.Length > 0)
            {
                isrightfind = true;
            }
        }
        else
        {

            Debug.Log("<color=red>请检查" + g.name + "是否打开</color>");
        }
    }


	
	// Update is called once per frame
	void Update () 
    {
        if (!isleftfind)
        {
            DestoryMesh(leftrain);
        }
        if (!isrightfind)
        {
            DestoryMesh(rightrain);
        }
        Chekdic(rightrain.transform, bow.transform, msg.Rightkey);
        Chekdic(leftrain.transform, bow.transform, msg.Leftkey);

        Chekdic(leftrain.transform, rightrain.transform, false);

	}

    float tempdis;
    Rigidbody rb;
    BoxCollider box;
    bool isreset = false;
    Bulletcontroll controll;
    float temp;

    /// <summary>
    /// 检查距离改变状态
    /// </summary>
    /// <param name="me">自身</param>
    /// <param name="target">目标</param>
    /// <param name="key">扳机键</param>

    void Chekdic(Transform me, Transform target, bool key)
    {
        temp = Vector3.Distance(me.position, target.position);
        switch (status)
        {
            case shootstatus.none:
                if (temp < 2f && key)
                {
                    //Debug.Log("nonenonenone");
                    timer += Time.deltaTime;
                    if (timer > 3f)
                    {
                        status = shootstatus.stay;
                        target.SetParent(me);
                        target.transform.localPosition = Vector3.zero;
                        target.transform.localRotation = Quaternion.identity;
                        handfirst = me.gameObject;
                    }
                }
                break;
            case shootstatus.stay:
                CreatArrows();
                break;
            case shootstatus.shoot:
                ShoutOut();
                break;
            case shootstatus.lose:
                //  ShowRanking();
                break;
            default:
                break;
        }
    }

    void ShoutOut() 
    {
        if (hit_sphere.activeSelf)
        {
            hit_sphere.SetActive(false);
            InAttackRange();
         
        }
        if (kinds != ArrowsKind.normal)
        {
            box.enabled = false;
        }
        controll.islunch = true;
        temp = Vector3.Distance(leftrain.transform.position, rightrain.transform.position);
        //Debug.Log(bstatus.ToString() + "<<<<<<<<<<<<");
        if (rb != null)
        {
            rb.mass = 0.2f;
            rb.angularDrag = 0;
            rb.freezeRotation = true;

            switch (kinds)
            {
                case ArrowsKind.normal:
                    if (temp < 0.15f)
                    {
                        rb.AddForce(-bullet.transform.up * 10f);
                    }
                    else
                    {
                        rb.AddForce(-bullet.transform.up * 1000f * (temp - 0.15f));
                    }
                    controll.endpos = arrowstop;
                    break;
                case ArrowsKind.torch:
                case ArrowsKind.ice:

                    rb.AddForce(-bullet.transform.up * 1000f);
                    break;
                default:
                    break;
            }
        }
        else
        {
            Debug.Log("withoutrigidbody");
        }

        status = shootstatus.stay;
        bstatus = bulletstatus.none;
        kinds = ArrowsKind.normal;
        fire_status = firestatus.reday;
        if (fire != null && bullet != null)
        {
            fire.transform.SetParent(bullet.transform);
        }
       // Destroy(fire, 1f);
        isfireon = false; /*重置箭头火焰是否点着*/
    }


    void CreatArrows()
    {
        //调整箭的方向,箭的旋转，发射出去的就不管了
        Vector3 tempforward = Vector3.one;
        if (bullet != null && handfirst == rightrain && !bullet.GetComponent<Bulletcontroll>().islunch)
        {
            bullet.transform.up = Vector3.Normalize(leftrain.transform.position
                    - bow.transform.position);
            tempforward = -bullet.transform.up;
            // Debug.DrawLine(target.transform.position, leftrain.transform.position, Color.red);
            //  arrows.transform.localEulerAngles = new Vector3(0, leftrain.transform.eulerAngles.y, -90);
        }
        else if (bullet != null && handfirst == leftrain && !bullet.GetComponent<Bulletcontroll>().islunch)
        {
            bullet.transform.up = Vector3.Normalize(rightrain.transform.position
                  - bow.transform.position);
            tempforward = -bullet.transform.up;
            // Debug.DrawLine(target.transform.position, rightrain.transform.position, Color.red);
            // arrows.transform.localEulerAngles = new Vector3(0, rightrain.transform.eulerAngles.y, -90);

        }
        //判断哪只手柄拿着弓
        if (handfirst.Equals(leftrain))
        {
            if (bstatus == bulletstatus.none )
            {
                Choiseparent(rightrain,msg.Rightkey);
            }
            if (msg.Rightkey && bstatus == bulletstatus.ready)
            {
                Vector3 tempvec = Vector3.Normalize(leftrain.transform.position - rightrain.transform.position);

                float limitshaking = Vector3.Distance(leftrain.transform.position, rightrain.transform.position);
                if (limitshaking > 0.15f)
                {
                    if (limitshaking * 10 % 2f <= 0.4f)
                    {
                        //Debug.Log("11111111111");
                        RainsShaking(SteamVR_Controller.DeviceRelation.Rightmost, 3000);
                        RainsShaking(SteamVR_Controller.DeviceRelation.Leftmost, 3000);
                    }
                }
                bowstring.transform.position = rightrain.transform.position;
                AttackRange(tempforward, leftrain);
            }
            if (!msg.Rightkey && bstatus == bulletstatus.ready)
            {
                bowstring.transform.localPosition = bowsoldpos;
                bstatus = bulletstatus.lunch;
            }
            if (!msg.Rightkey && bstatus == bulletstatus.lunch)
            {
                status = shootstatus.shoot;
                rb.isKinematic = false;
                bullet.transform.SetParent(null);
            }

        }
        else
        {
            if (bstatus == bulletstatus.none)
            {
                Choiseparent(leftrain,msg.Leftkey);
            }
            if (msg.Leftkey && bstatus == bulletstatus.ready)
            {
                float limitshaking = Vector3.Distance(leftrain.transform.position, rightrain.transform.position);
                // target.transform.LookAt(leftrain.transform.position);
                if (limitshaking > 0.15f)
                {
                    //手柄靠近时震动
                    if (limitshaking < 0.3f)
                    {
                        // RaisShaking(SteamVR_Controller.DeviceRelation.Leftmost, 800);
                        //var deviceIndex2 = SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Rightmost);
                        //SteamVR_Controller.Input(deviceIndex2).TriggerHapticPulse(800);
                    }
                    if (limitshaking * 10 % 2f <= 0.4f)
                    {
                        RainsShaking(SteamVR_Controller.DeviceRelation.Rightmost, 3000);
                        RainsShaking(SteamVR_Controller.DeviceRelation.Leftmost, 3000);
                        //var deviceIndex2 = SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Rightmost);
                        //SteamVR_Controller.Input(deviceIndex2).TriggerHapticPulse(800);
                    }
                }
                bowstring.transform.position = leftrain.transform.position;
                AttackRange(tempforward, rightrain);
            }
            if (!msg.Leftkey && bstatus == bulletstatus.ready)
            {
                bowstring.transform.localPosition = bowsoldpos;
                bstatus = bulletstatus.lunch;
            }
            if (!msg.Leftkey && bstatus == bulletstatus.lunch)
            {
                status = shootstatus.shoot;
                rb.isKinematic = false;
                bullet.transform.SetParent(null);
            }

        }

    }
    void Choiseparent(GameObject parent,bool key)
    {
        if (Vector3.Distance(parent.transform.position, bowstring.transform.position) < 0.2f  && key)
        {

            //引入缓冲池
            if (msg.ai_pool.ContainsKey(5) && msg.ai_pool[5].Count > 0)
            {
                bullet = msg.ai_pool[5][0];
                bullet.SetActive(true);
                msg.ai_pool[5].Remove(bullet);
                box = bullet.GetComponent<BoxCollider>();
                controll = bullet.GetComponent<Bulletcontroll>();
                //Debug.Log("ifififiiffi");
            }
            else
            {
                bullet = GameObject.Instantiate(msg.bulletprefab) as GameObject;
                box = bullet.AddComponent<BoxCollider>();
                controll = bullet.AddComponent<Bulletcontroll>();
                //Debug.Log("ekseelseelse");
            }
            if (bullet.GetComponent<Rigidbody>() != null)
            {
                rb = bullet.GetComponent<Rigidbody>();
            }
            else
            {
                rb = bullet.AddComponent<Rigidbody>();
            }
            bullet.tag = "PlayerBullet";
            box.enabled = true;
            // box.isTrigger = true;
            box.size = new Vector3(0.2f, 1f, 0.1f);
            box.center = new Vector3(0f, -0.5f, 0f);
            // box.isTrigger = true;
            arrows = bullet.transform.FindChild("jian").gameObject;
            controll.msg = msg;
            controll.pcontroll = this;
            controll.islunch = false;
            controll.istrial = false;
            controll.ishit = false;
            controll.isset = false;
            rb.useGravity = true;
            controll.rb = rb;
            controll.timer = 0f;/*重置计时器*/
            controll.stoptimer = 0f;
            controll.isaddtopool = false;
            rb.isKinematic = true;

            bullet.transform.SetParent(parent.transform);
            bullet.transform.localPosition = Vector3.zero;
            bullet.transform.localScale = Vector3.one;
            bullet.transform.localRotation = Quaternion.identity;
            bstatus = bulletstatus.ready;
        }
    }

    /// <summary>
    /// 手柄震动
    /// </summary>
    /// <param name="device">哪只手柄</param>
    /// <param name="deep">震动程度</param>
    void RainsShaking(SteamVR_Controller.DeviceRelation device, ushort deep)
    {
        var deviceIndex2 = SteamVR_Controller.GetDeviceIndex(device);
        SteamVR_Controller.Input(deviceIndex2).TriggerHapticPulse(deep);
    }

    /// <summary>
    /// 射线击中地面产生将要攻击的范围
    /// </summary>
    Ray ray;
    RaycastHit hit;
    RaycastHit[] spherehits;
    LayerMask mask;
    Vector3 arrowstop;
    //bool is_arrow_fire = false;
    // float timer_color;
    void AttackRange(Vector3 direction, GameObject handarrow)
    {
        ray.origin = handarrow.transform.position;
        ray.direction = direction;
        if (kinds != ArrowsKind.normal)
        {
            //fire暂时只有火箭
            Firecreate(bullet.transform.FindChild("fire").transform.position);

            if (!hit_sphere.activeSelf)
            {
                hit_sphere.SetActive(true);
            }
            else
            {
                //timer_color += Time.deltaTime;
                //hit_sphere.GetComponent<SteamVR_PlayArea>().color = Color.Lerp(hit_sphere.GetComponent<SteamVR_PlayArea>().color, Color.red, 0.25f);
                //if (timer_color > 1f)
                //{
                //    hit_sphere.GetComponent<SteamVR_PlayArea>().color = Color.Lerp(hit_sphere.GetComponent<SteamVR_PlayArea>().color, Color.cyan, 0.25f);
                //    timer_color = 0;
                //}
            }
            if (Physics.Raycast(ray, out hit, 1000f, mask))
            {
                spherehits = Physics.SphereCastAll(ray, 5f, 1000f);
                hit_sphere.transform.position = hit.point;
            }
            else
            {
                hit_sphere.SetActive(false);
                spherehits = null;
            }
        }
        else 
        {
            if (Physics.Raycast(ray, out hit, 1000f)) 
            {
                if (hit.collider.CompareTag("OnHurt"))
                {
                    arrowstop = hit.point;
                    //Debug.Log(arrowstop);
                }
            }
        }
    }
    bool isfireon = false;
    GameObject fire ;

    void Firecreate( Vector3 pos) 
    {
        if (!isfireon && fire_status == firestatus.reday)
        {
            //if (msg.ai_pool.ContainsKey(6) && msg.ai_pool[6].Count > 0)
            //{
            //    fire = msg.ai_pool[6][0];

            fire = GameObject.Instantiate(msg.fireprefab, pos, Quaternion.identity) as GameObject;
            fire.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            isfireon = true;
            fire_status = firestatus.fireon;
        }
        else if(fire_status == firestatus.fireon)
        {
            fire.transform.position = pos;
            fire.transform.LookAt(Camera.main.transform.position);
        }
    }


    //int atemp;

    /// <summary>
    /// 使在攻击范围内的AI减血
    /// </summary>
    void InAttackRange()
    {
        //atemp++;
        //Debug.Log(atemp.ToString());
        if (kinds == ArrowsKind.torch)
        {
            for (int i = 0; i < spherehits.Length; i++)
            {
                if (spherehits[i].collider.CompareTag("AI"))
                {
                    //Debug.Log("打印了过少次？");
                    spherehits[i].transform.GetComponent<AIcontroll>().CateDownHp(10);
                    //Debug.Log(spherehits[i].transform.GetComponent<AIcontroll>().HP);
                }
            }
        }
        else if (kinds == ArrowsKind.ice)
        {
            //冰箭的效果
        }
        for (int i = 0; i < spherehits.Length; i++)
        {
            //if (spherehits[i].transform.tag.Equals("AI") && spherehits[i].transform.GetComponent<AIcontroll>() != null) ;
            //Debug.Log("可攻击范围内的AI"+ spherehits[i].collider.name);
        }
        spherehits = null;
    }


}
