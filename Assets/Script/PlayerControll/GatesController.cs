using UnityEngine;
using System.Collections;

public class GatesController : MonoBehaviour {
    public Abstract_ALL allgate;
    public MSGcenter msg;
    //public GameObject gate;
    //public GameObject gate1;
    public GameObject gateAwarrds;
    public GameObject heater, weapon, shield;
    public Animation anim;
    private BoxCollider box;
    public void Init(MSGcenter m,Abstract_ALL gate) 
    {
       
        msg = m;
        hp = gate.HP;
        allgate = gate;
    }
    private int hp;
    public int HP
    {
        get 
        { 
            //Debug.Log(gameObject.name+"::::"+hp);
            return hp;
        }
        set { hp = value; }
    }
	void Start () 
    {
        anim = GetComponent<Animation>();
        box = GetComponent<BoxCollider>();
	}
	
    bool a=false;
    bool isplayedaim = false;
	// Update is called once per frame
	void Update () 
    {
        if (hp!= allgate.HP)
        {
            //Debug.Log("gatecontroller+"+hp);
            hp = allgate.HP;
        }
        if (HP <= 0 && !anim.IsPlaying("OpenDoor"))
        {
           // gameObject.SetActive(false);
            isplayedaim = true;
            PlayeAnim();
            msg.ChangeDestination(out a);
            //Debug.Log("gatecontrollaaa:::"+a);
            if (a)
            {
            //    msg.endcall();
            }
        }

        if (isplayedaim)
        {
            if (!anim.IsPlaying("OpenDoor"))
            {
                gameObject.SetActive(false);
            }
        }
	}

    bool isplay = false;
    void PlayeAnim() 
    {
        if (anim != null && !isplay)
        {
            anim.Play();
            box.enabled = false;
            isplay = true;
        }
    }
}


        //public class Gate 
        //{
        //    protected int hp;
        //    public int HP 
        //    {
        //        get { return hp; }
        //        set { hp = value; }
        //    }
        //    protected virtual void CateDownHp();
        //    public Gate(int maxhp) 
        //    {
        //        hp = maxhp;
        //    }
        //}

        //public class Gatechild0 : Gate 
        //{
        //    public Gatechild0(int maxhp):base(10)
        //    {
        //        hp = maxhp;
        //    }
        //    protected override void CateDownHp()
        //    {
        //        hp -= 10;
        //    }
        //}


        //public class Gatechild1 : Gate 
        //{
        //    protected override void CateDownHp()
        //    {
        //        hp -= 10;
        //    }
        //}
