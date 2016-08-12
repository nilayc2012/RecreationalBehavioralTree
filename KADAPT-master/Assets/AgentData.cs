using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TreeSharpPlus;

public class TimeClass
{
    public int starthr;
    public string starttime;
    public int endhr;
    public string endtime;


    public TimeClass()
    {
        starthr = 0;
        endhr = 0;
        starttime = "";
        endtime = "";
    }
    public void setSchedule(int starthr,string starttime,int endhr,string endtime)
    {
        this.starthr = starthr;
        this.starttime = starttime;
        this.endhr = endhr;
        this.endtime = endtime;
    }
}

public class AgentData : MonoBehaviour {

    public bool approval;

    public string show;
    public int starth;
    public int endh;
    public string sampm;
    public string eampm;
    public bool moviestart;
    public bool communicationover;

    public List<Transform> shopsVisited;
    public Val<Vector3> pos;

    public bool ack;
    public TimeClass movieTime;
    public GameObject movieSeat;
    public GameObject[] gesturePanelList;

    public List<string[]> gesturelist;

    public GameObject calledby;

    public bool userInteractionStatus;


    // Use this for initialization
    void Awake()
    {
        communicationover = true;
        moviestart = false;
        pos = Val.V(() => this.transform.position);
        userInteractionStatus = false;
        ack = false;
        approval = false;
        shopsVisited = new List<Transform>();
        movieTime = new TimeClass();
        LoadGestures();
        movieSeat = null;
    }
	void Start () {

    }

	// Update is called once per frame
	void Update () {
        Behavior gc = GameObject.Find("GameController").GetComponent<Behavior>();
        if ((gc.hr == 8 && gc.min >= 30 && gc.amOrPM == "am") || (gc.hr == 2 && gc.min >= 30 && gc.amOrPM == "pm") || (gc.hr == 5 && gc.min >= 30 && gc.amOrPM == "pm") || (gc.hr == 8 && gc.min >= 30 && gc.amOrPM == "pm"))
        {

                if (this.GetComponent<AgentData>().starth == gc.hr + 1 && this.GetComponent<AgentData>().sampm == gc.amOrPM)
                {
                    this.GetComponent<AgentData>().moviestart = true;
                }
            
        }

        if (gc.hr == 12 && gc.min >= 30 && gc.amOrPM == "pm")
        {

                if (this.GetComponent<AgentData>().starth == 1 && this.GetComponent<AgentData>().sampm == gc.amOrPM)
                {
                    this.GetComponent<AgentData>().moviestart = true;
                }
            
        }

        if ((gc.hr == 11 && gc.amOrPM == "am") || (gc.hr == 3 && gc.amOrPM == "pm") || (gc.hr == 5 && gc.amOrPM == "pm") || (gc.hr == 8 && gc.amOrPM == "pm") || (gc.hr == 11 && gc.amOrPM == "pm"))
        {

            if (this.GetComponent<AgentData>().endh == gc.hr && this.GetComponent<AgentData>().eampm == gc.amOrPM)
            {
                this.GetComponent<AgentData>().moviestart = false;
            }

        }

    }

    public GameObject getCalledbY()
    {
        return calledby;
    }
    public void setCalledBy(GameObject calledby)
    {
        this.calledby = calledby;
    }

    public bool getUserInteractionStatus()
    {

            return userInteractionStatus;

    }


    public void setUserInteractionStatus(bool flag)
    {

            userInteractionStatus = flag;

    }

    public void setApproval(bool status)
    {
        approval = status;
    }

    public bool getApproval()
    {
        return approval;
    }

    public bool getAck()
    {

            return ack;

    }

    public void resetMovieTime()
    {
        movieTime = new TimeClass();
    }

    public void setAck(bool ack)
    {

            this.ack = ack;

    }

    public Val<Vector3> getPos()
    {

            return pos;
    }

    public void setPos(Val<Vector3> pos)
    {

            this.pos = pos;

    }

    public void resetShopVisited()
    {

            shopsVisited.Clear();
    }

    public bool checkShopVisted(Transform shop)
    {

            foreach(Transform sh in this.shopsVisited)
            {
                if(sh.position.Equals(shop.position))
                {
                    return true;
                }
            }
            return false;

    }


    public void includeShopVisited(GameObject shop)
    {

            if(shop.name!="cinema" && shop.name!= "popcornandsoda")
            shopsVisited.Add(shop.transform.FindChild("shoplocation"));

    }


    public void LoadGestures()
    {
        gesturelist = new List<string[]>();
        gesturelist.Add(new string[] { "joke", "laugh" });
        gesturelist.Add(new string[] { "amazing", "amazed" });
        gesturelist.Add(new string[] { "bad", "sad" });
        gesturelist.Add(new string[] { "good", "smile" });
        gesturelist.Add(new string[] { "cheerful", "cheer" });
        gesturelist.Add(new string[] { "compliment", "shy" });
        gesturelist.Add(new string[] { "insult", "angry" });
        gesturelist.Add(new string[] { "wicked", "wickedsmile" });
        gesturelist.Add(new string[] { "tellnews", "congratulate", "thanks" });
    }



}
