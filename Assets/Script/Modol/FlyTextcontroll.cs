using UnityEngine;
using System.Collections;

public class FlyTextcontroll : MonoBehaviour {


    public MSGcenter msg;
    public bool isplay = false;
    private Animation anim;
	// Use this for initialization
	void Start () {
        anim = GetComponent<Animation>();
	}

    /// <summary>
    /// 动画添加事件
    /// </summary>
    void AnimEvent() 
    {
        //Debug.Log("AnimEvent");
        gameObject.SetActive(false);
        if (!msg.ai_pool.ContainsKey(4))
        {
            msg.ai_pool.Add(4, new System.Collections.Generic.List<GameObject>());
        }
        msg.ai_pool[4].Add(gameObject);
    }

	// Update is called once per frame
	void Update () 
    {
        if (gameObject.activeSelf)
        {
            transform.LookAt(Camera.main.transform.position);
        }
        if (!isplay && anim.IsPlaying("FlyText"))
        {
            //Debug.Log("动画播放完成");
            isplay = true;
        }
        if (isplay)
        {
            if (!anim.IsPlaying("FlyText"))
            {
                AnimEvent();
            }
        }
	}
}
