using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// 拉弓步骤
/// </summary>
public enum shootstatus
{
none,stay,shoot,lose
}
/// <summary>
/// 箭的状态
/// </summary>
public enum bulletstatus 
{
    none,ready,lunch
}

/// <summary>
/// 箭的种类
/// </summary>
public enum ArrowsKind 
{
    normal,torch,ice
}

public delegate void GameEnd();



/// <summary>
/// ai状态
/// </summary>
public enum aistatus 
{
    ido,walk,run,damage,attack,inspeackter,die
}

public class MSGcenter : MonoBehaviour {

    public const string ai_tag = "AI";
    public const string player_bullet_tag = "PlayerBullet";
    public const string player_tag = "Player";

    /// <summary>
    /// 1.2.3为AI缓冲池，4为FlyText缓冲池，5为Arrows缓冲池,6为箭上的火缓存池
    /// </summary>
    public Dictionary<int, List<GameObject>> ai_pool = new Dictionary<int, List<GameObject>>();/*ai对象池*/

    public GameObject heater, weapon, shield;

    static public MSGcenter instance;
    public List<GameObject> bulletpool = new List<GameObject>();
    public GameObject playerprefab,targetprefab,bulletprefab,fireprefab;

    public Transform playerbornpos, bowbornpos;

    [HideInInspector]
    public GameObject bullet,arrows;/*箭的父物体和子物体*/

    /// <summary>
    /// 地面攻击范围
    /// </summary>
    public GameObject hit_sphere;

    /// <summary>
    /// 所有环境
    /// </summary>
    public GameObject enviroment;

    public GameObject[] ai_prefab;
    public Transform[] destinations;

    public GameObject[] gate_inScene;

    public GameObject[] aworrdsgate;

    public List<Abstract_ALL> gates = new List<Abstract_ALL>();/*终点门管理*/

    public Vector3 ai_end_pos;

    public List<AIcontroll> aliveai = new List<AIcontroll>();

    public GameEnd endcall;

    /// <summary>
    /// 每？个小AI出一个Boss
    /// </summary>
    public int AIcount_BOSS = 10;
    [HideInInspector]
    public int invokeCreatAI;


    /// <summary>
    /// 场景内所有AI总数
    /// </summary>
    public int aliveaicount = 6;

    /// <summary>
    /// 场景内初始化门的个数
    /// </summary>
    public int gates_num=3;


    /// <summary>
    /// 城门左，右边点x轴坐标
    /// </summary>
    public Transform gateleft, gateright;
    [HideInInspector]
    public float tempX ;

    [HideInInspector]
    //场景中现存的Ai数量
    public int aliveAI;

    public Vector3 currentgate;

    [HideInInspector]
    public GameObject flytext;

    [HideInInspector]
    public GameObject player;
    private GameObject leftrain, rightrain;
    private GameObject target;
    private bool leftkey, rightkey;
    private bool canshake = false;

 
 
    private GameObject bowstring;/*弓弦*/
    private Vector3 bowsoldpos,arrowforwordpos;

    private string[] nameprefab;

    private TextMesh score_show;
    private Animation scoranim;

    private string playername;
    private int score;
    public int Score 
    {
        set { score = value; }
        get { return score; }
    }

   /// <summary>
   /// AI生成位置
   /// </summary>
    private int bornpos = 2;
   /// <summary>
   /// 生成AI种类
   /// </summary>
    private int borAIindex = 3;


    public bool Leftkey 
    {  
        set {
            if (leftrain != null)
            {
                leftkey = value;
            }
            else {
                Debug.LogError("cant find leftrain");
            }
        }
        get {
            if (leftrain != null)
            {
                return leftkey;
            }
            else {
                return false;
            }
        }
    }

    public bool Rightkey
    {
        set
        {
            if (rightrain != null)
            {
                rightkey = value;
            }
            else
            {
                Debug.LogError("cant find rihtrain");
            }
        }
        get
        {
            if (rightrain != null)
            {
                return rightkey;
            }
            else {
                return false;
            }
        }
    }

    void Awake()
    {
        InitPlayer();
        instance = this;
        tempX = gateleft.position.x;
        CreateGateDate();
        ai_end_pos = gates[0].Destination;
        currentgate = gates[0].Destination;
        endcall = GameEnd;
    
      //  Debug.Log(LayerMask.GetMask("Floor"));
    }

    void GameEnd() 
    {
        Debug.Log("msgcenter+++gameend");
      //  Application.LoadLevel(1);
        for (int i = 0; i < aliveai.Count; i++)
        {
                Debug.Log("执行状态切换");
           // aliveai[i].endpos = ai_end_pos;
            aliveai[i].status = aistatus.ido;
           // aliveai[i].isattack = false;
           // aliveai[i].isalive = false;
           // aliveai[i].gate = currentgate;
        }
    }

    public void ChangeDestination(out bool islose) 
    {
        bool isallbroken = true;
        for (int i = 0; i < gates.Count; i++)
        {
            if (!gates[i].isbroken)
            {
                Debug.Log("iiiiiii::::::"+i);
                currentgate = gates[i].Destination;
                ai_end_pos = gates[i].Destination;
                isallbroken = false;
                break;
            }
        }
        islose = isallbroken;
        if (!islose)
        {
            for (int i = 0; i < aliveai.Count; i++)
            {
                     Debug.Log("执行状态切换"+ai_end_pos);
                aliveai[i].endpos = ai_end_pos;
                aliveai[i].status = aistatus.run;
                aliveai[i].isattack = false;
                aliveai[i].isalive = false;
                aliveai[i].issetdes = false;
                aliveai[i].look_gate = currentgate;
            }
        }
    }


    public void CateGateHP(int cate) 
    {
       // Debug.Log("catehp"+cate);
        for (int i = 0; i < gates.Count; i++)
        {
            if (!gates[i].isbroken)
            {
                gates[i].BeHited(cate);
                FlyText(false,cate,gates[i].Destination);
                
                //Debug.Log(gates[i].HP);
                break;
            }
            
        }
    
    }


    void Start () 
    {
        nameprefab = new string[10] {"0","1","2","3","4","5","6","7","8","9" };
	}

    void InitPlayer() 
    {
       //GameObject e = GameObject.Instantiate(enviroment,enviroment.transform.position,Quaternion.identity)as GameObject;
       //e.SetActive(true);
        player = GameObject.Instantiate(playerprefab) as GameObject;
        target = GameObject.Instantiate(targetprefab) as GameObject;
        player.tag = player_tag;
        PlayerController controll = player.AddComponent<PlayerController>();
        controll.player = new Pioneer(180,"none");
        
        player.transform.SetParent(transform);
        target.transform.SetParent(transform);
        player.transform.position = playerbornpos.position;
        target.transform.localPosition = bowbornpos.position;
        target.transform.rotation = bowbornpos.rotation;
       // Debug.Log(target+"transform+"+target.transform.localPosition);
       // target.transform.Rotate(-90, 90, 0);

        //target.transform.localPosition = new Vector3(0,0,1);
        leftrain = player.transform.FindChild("[CameraRig]/Controller (left)").gameObject;
        rightrain = player.transform.FindChild("[CameraRig]/Controller (right)").gameObject;
        bowstring = target.transform.FindChild("gong/Point001/Point002").gameObject;
        score_show = player.transform.FindChild("[CameraRig]/Camera (eye)/New Text").gameObject.GetComponent<TextMesh>();
        bowsoldpos = bowstring.transform.localPosition;
        controll.Init(leftrain, rightrain,this,hit_sphere,bullet,arrows,target,bowstring,bowsoldpos);
        scoranim = score_show.GetComponent<Animation>();
        leftrain.AddComponent<HandControl>();
        rightrain.AddComponent<HandControl>();
    }


    bool isinvoke = false;
	// Update is called once per frame
	void Update () 
    {

        //根据场景中现存的Ai数量创建AI
        if (aliveAI < aliveaicount && !isinvoke)
        {
           // StartCoroutine(CreatAi());
            InvokeRepeating("CreatAi", 3f, 2f);
            isinvoke = true;
            //Debug.Log("ifififififi"+aliveAI);
        }
        else if(aliveAI >=aliveaicount)
        {
            //Debug.Log("elseelseelse"+aliveAI);
            CancelInvoke("CreatAi");
            isinvoke = false;
        }
        if (invokeCreatAI % AIcount_BOSS == 0)
        {
           
        }
	}

    void CreatBoss() 
    {
    
    }

    /// <summary>
    /// invokeRepeating调用
    /// </summary>
   void CreatAi() 
    {
        GameObject g = null;
        AIcontroll controll=null;
        int temp_aiIndex = Random.Range(0, borAIindex);
        int temp_bornpos = Random.Range(0, bornpos);
        if (ai_pool.ContainsKey(temp_aiIndex) && ai_pool[temp_aiIndex].Count > 0)
        {
            g = ai_pool[temp_aiIndex][0];
            ai_pool[temp_aiIndex].Remove(g);
            controll = g.GetComponent<AIcontroll>();
            g.SetActive(true);
            
           // ai_pool.Remove(ai_pool[temp][0]);
        }
        else
        {
            g = GameObject.Instantiate(ai_prefab[temp_aiIndex], destinations[temp_bornpos].position, Quaternion.identity) as GameObject;
            controll = g.AddComponent<AIcontroll>();
        }
      //  controll.Init();
            aliveai.Add(controll);
            controll.issetdes = false;
            controll.isattack = false;
            controll.isalive = false;
            controll.msg = this;
            controll.ischangeclose = false;
           // controll.Bip01 = g.transform.FindChild("xiongdonghua/Bip01").gameObject;
           // controll.endpos = CreateAIendPos();
            controll.endpos = ai_end_pos;
            controll.uppos = gateright.position;

            controll.look_gate = currentgate;
          //  Debug.Log(controll.endpos);
            switch (temp_aiIndex)
            {
                    case 0:
                    controll.HP = 10;
                    controll.InDex = 0;
                    controll.ATTACK = 10;
                    controll.gread = 10;
                    break;
                    case 1:
                    controll.HP = 20;
                    controll.InDex = 1;
                    controll.ATTACK = 20;
                    controll.gread = 20;
                    break;
                    case 2:
                    controll.HP = 30;
                    controll.InDex = 2;
                    controll.ATTACK = 30;
                    controll.gread = 30;
                    break;
                default:
                    break;
            }
            aliveAI++;      /*实时监测AI数量*/
            invokeCreatAI++;/*Boss生成数量依据*/
            g.tag = ai_tag;
           // Debug.Log(temp_bornpos);
            g.transform.position = destinations[temp_bornpos].position;
            //Debug.Log("<color=yellow>AI生成位置序号" + temp_bornpos + destinations[temp_bornpos].position + "AI种类随机" + temp_aiIndex + "</color>");
    }

    //Vector3 CreateAIendPos()    
    //{
    //    float tempoffset = (gateright.position.x - gateleft.position.x) / aliveaicount - 1;
    //    tempX += tempoffset;
    //    //Debug.Log(tempX);
    //    if (tempX >= gateright.position.x)
    //    {
    //        tempX = gateleft.position.x;
    //    }

    //    Vector3 endpos = new Vector3(tempX, gateright.position.y, gateright.position.z);
    //    //Debug.Log("endpos==="+endpos+"tempx==="+tempX);
    //    return endpos;
    //}

    ScenceGate firstgate, secondgate;

    void CreateGateDate() 
    {
        aworrdsgate = new GameObject[gates_num];
        for (int i = 0; i < gates_num; i++)
        {
            GameObject gt = Instantiate(gate_inScene[0]) as GameObject;
            gt.SetActive(true);
            //gt.transform.position = new Vector3(gate_inScene[0].transform.position.x, gate_inScene[0].transform.position.y
            //,i*(24/(gates_num-1)));
            gt.transform.position = gate_inScene[0].transform.position;
            firstgate = new ScenceGate();
            firstgate.HP = 300;
            firstgate.Destination = gt.transform.FindChild("distination").transform.position;
            gates.Add(firstgate);
            GatesController controll = gt.AddComponent<GatesController>();
            controll.Init(this, gates[i]);
            aworrdsgate[i] = gt.transform.FindChild("GateAwarrds").gameObject;
            Awarrds awarrds = aworrdsgate[i].AddComponent<Awarrds>();
            awarrds.Init(heater, weapon, shield);
        }
    }

   public void FlyText(bool isadd ,int num,Vector3 pos,bool isranking = false) 
    {
        //Debug.Log("谁在调用？？？");
        GameObject go = null;
        FlyTextcontroll fcontroll = null;
        if (ai_pool.ContainsKey(4) && ai_pool[4].Count > 0)
        {
            go = ai_pool[4][0];
            go.SetActive(true);
            ai_pool[4].Remove(go);
            fcontroll = go.GetComponent<FlyTextcontroll>();
            //Debug.Log("ififififiifififi");
        }
        else 
        {
            go = Instantiate(flytext) as GameObject;
            //ai_pool.Add(4,new List<GameObject>());
            //ai_pool[4].Add(flytext);
            fcontroll = go.GetComponent<FlyTextcontroll>();
            //Debug.Log("ekseksejlsslekfslkejfk");
        }
        fcontroll.isplay = false;
        TextMesh textmesh = go.transform.FindChild("New Text").GetComponent<TextMesh>();
        if (isadd)
        {
            if (isranking)
            {
                textmesh.text = "<color=yellow>+" + num + "</color>";
                ScorShow(num);
            }
            else {
                textmesh.text = "<color=green>+" + num + "</color>";
            }
        }
        else {
            textmesh.text = "<color=red>-" + num + "</color>";
        }
        fcontroll.msg = this;
        go.transform.position = pos;
    }


   void ScorShow(int temp) 
   {
       //g.transform.SetParent(score_show.transform);
       //g.transform.localScale = Vector3.one;
       score += temp;
       score_show.text = "Score:"+score.ToString();
       score_show.transform.FindChild("New Text").GetComponent<TextMesh>().text = "<color=yellow>+" + temp.ToString()+"</color>";
       scoranim.Play();
   }



   void ShowRanking() 
   {
       PlayerPrefs.SetInt(playername, score);
       for (int i = 0; i < nameprefab.Length; i++)
       {
           if (PlayerPrefs.HasKey(i.ToString()))
           {
              Debug.Log( PlayerPrefs.GetInt(i.ToString()));
           }
           if (i > nameprefab.Length - 1)
           {
               PlayerPrefs.DeleteKey(i.ToString());
           }
       }
       //PlayerPrefs.GetInt(
   }

}
