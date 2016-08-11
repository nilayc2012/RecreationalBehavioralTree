using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TreeSharpPlus;
using RootMotion.FinalIK;
using System;
using UnityEngine.UI;
using BehaviorTrees;

public class CommonFunction
{

    public Dictionary<GameObject, GameObject> interactionMap;
    private System.Object lockThis = new System.Object();

    public CommonFunction()
    {
        interactionMap = new Dictionary<GameObject, GameObject>();
    }

    public bool AddtoInteractionMap(GameObject person, GameObject agent)
    {
        lock (lockThis)
        {
            if (!this.interactionMap.ContainsKey(person)&& !person.GetComponent<AgentData>().moviestart)
            {
                this.interactionMap.Add(person, agent);
                person.GetComponent<AgentData>().communicationover = false;
                agent.GetComponent<AgentData>().communicationover = false;
                return true;
            }
            return false;
        }
    }

    public void RemoveFromInteractionMap(GameObject agent, GameObject person)
    {
        lock (lockThis)
        {
            this.interactionMap.Remove(person);
            person.GetComponent<AgentData>().communicationover = true;
            agent.GetComponent<AgentData>().communicationover = true;
        }

    }

    public GameObject FindGameObjectCF(Transform agent, string oname)
    {
        if (agent.FindChild(oname) != null)
        {
            return agent.FindChild(oname).gameObject;
        }
        foreach (Transform child in agent)
        {
            GameObject result = FindGameObjectCF(child, oname);
            if (result != null)
            {
                return result;
            }
        }
        return null;
    }

    public void DeactivateAllObject(Transform agent)
    {
        string[] gestures = new string[] { "joke", "laugh", "amazing", "amazed", "bad", "sad", "good", "smile", "cheerful", "cheer", "compliment", "shy", "insult", "angry", "wicked", "wickedsmile", "tellnews", "congratulate", "thanks" };

        foreach (string oname in gestures)
        {
            GameObject panel = FindGameObjectCF(agent, oname);
            if (panel != null)
            {
                panel.SetActive(false);
            }
        }
    }

    public void RemoveFromInteractionmapByVavlue(GameObject agent)
    {
        lock (lockThis)
        {
            foreach (GameObject key in interactionMap.Keys)
            {
                if (interactionMap[key].name == agent.name)
                {
                    this.DeactivateAllObject(key.transform);
                    interactionMap.Remove(key);
                    break;
                }
            }
        }
    }
}

public class Behavior : MonoBehaviour {

    private System.Object thisLock = new System.Object();

    CommonFunction cf;

    public Dictionary<string, string[]> affordanceMap;

    private BehaviorAgent BehaviorAgent;

    public Text time;

    public List<List<GameObject>> availableMovieSeats;

    public GameObject investigatePanel;

    public GameObject exit;

    public bool terminate;

    public int sec,hr,min;
    public string amOrPM;
    // Use this for initialization
    void Awake()
    {
        sec = 0;
        hr = 7;
        min = 0;
        amOrPM = "am";

        cf = new CommonFunction();
        availableMovieSeats = new List<List<GameObject>>();
        List<GameObject> temp = new List<GameObject>();
        foreach (GameObject seat in GameObject.FindGameObjectsWithTag("seat"))
        {
            temp.Add(seat);
        }
        availableMovieSeats.Add(temp);
        availableMovieSeats.Add(temp);
        availableMovieSeats.Add(temp);
        availableMovieSeats.Add(temp);
        availableMovieSeats.Add(temp);
        terminate = false;
        GameObject.FindGameObjectWithTag("sun").GetComponent<Light>().intensity = 0.2f;
        affordanceMap = new Dictionary<string, string[]>();
        LoadAffordances();

    }

    void Start () {
        terminate = false;
    }
	
	// Update is called once per frame
	void Update () {
	}


   public Node BuildTreeRoot()
    {

        List<Condition> initState = new List<Condition>();
        NSM.SetInitialState(initState);


        GameObject[] agents = GameObject.FindGameObjectsWithTag("agent");

        List<Node> agentNodes = new List<Node>() ;
        foreach(GameObject agent in agents)
        {
           agentNodes.Add(GetAgentBehavior(agent));
        }

        foreach(Node shopOwnerBehavior in GetShopOwnerBehaviors())
        {
            agentNodes.Add(shopOwnerBehavior);
        }

        agentNodes.Add(GetGateKeeperBehaviors());

        foreach(Node policeBehavior in GetPoliceBehaviors())
        {
            agentNodes.Add(policeBehavior);
        }
        
        agentNodes.Add(new DecoratorLoop(new LeafInvoke(() => TimeCalculator())));

        return new DecoratorLoop(
            new Sequence(
                new SelectorParallel(
                    new DecoratorForceStatus(RunStatus.Success,
                        new DecoratorLoop(
                            new LeafAssert(()=> !terminate)
                            )),
                new SequenceParallel(
                    agentNodes.ToArray())),
                this.GetComponent<GameController>().TerminateTree(),
                new DecoratorForceStatus(RunStatus.Success,
                    new DecoratorLoop(new LeafAssert(()=> terminate)))
            ));
    }

    protected void TimeCalculator()
    {
        sec++;
        if(sec==60)
        {
            sec = 0;
            min++;

            if (min == 60)
            {
                min = 0;
                hr++;
                if(hr==13 && amOrPM=="pm")
                {
                    hr = 1;
                }
                if (hr == 12)
                {
                    if (amOrPM == "am")
                    {
                        amOrPM = "pm";
                    }
                    else
                    {
                        hr = 0;
                        amOrPM = "am";
                    }
                }
                if (hr == 6 && amOrPM == "am")
                {
                    GameObject.FindGameObjectWithTag("sun").GetComponent<Light>().enabled = true;
                    GameObject.FindGameObjectWithTag("sun").GetComponent<Light>().intensity = 0.0f;

                }
                if ((hr > 6 && amOrPM == "am") && (hr <= 11 && amOrPM == "am"))
                {
                    GameObject.FindGameObjectWithTag("sun").GetComponent<Light>().intensity = GameObject.FindGameObjectWithTag("sun").GetComponent<Light>().intensity + 0.2f;
                }
                if (hr > 3 && amOrPM == "pm" && hr <= 8)
                {
                    GameObject.FindGameObjectWithTag("sun").GetComponent<Light>().intensity = GameObject.FindGameObjectWithTag("sun").GetComponent<Light>().intensity - 0.2f;
                }
                if ((hr > 8 && hr!=12 && amOrPM == "pm") || (hr < 6 && amOrPM == "am"))
                {
                    GameObject.FindGameObjectWithTag("sun").GetComponent<Light>().enabled = false;
                }


                if(hr==9 && amOrPM == "am")
                {
                    List<GameObject> temp = new List<GameObject>();
                    foreach (GameObject seat in GameObject.FindGameObjectsWithTag("seat"))
                    {
                        temp.Add(seat);
                    }

                    availableMovieSeats[0] = temp;

                }
                if (hr == 1 && amOrPM == "pm")
                {
                    List<GameObject> temp = new List<GameObject>();
                    foreach (GameObject seat in GameObject.FindGameObjectsWithTag("seat"))
                    {
                        temp.Add(seat);
                    }

                    availableMovieSeats[1] = temp;

                }
                if (hr == 3 && amOrPM == "pm")
                {
                    List<GameObject> temp = new List<GameObject>();
                    foreach (GameObject seat in GameObject.FindGameObjectsWithTag("seat"))
                    {
                        temp.Add(seat);
                    }

                    availableMovieSeats[2] = temp;

                }
                if (hr == 6 && amOrPM == "pm")
                {
                    List<GameObject> temp = new List<GameObject>();
                    foreach (GameObject seat in GameObject.FindGameObjectsWithTag("seat"))
                    {
                        temp.Add(seat);
                    }

                    availableMovieSeats[3] = temp;

                }
                if (hr == 9 && amOrPM == "pm")
                {
                    List<GameObject> temp = new List<GameObject>();
                    foreach (GameObject seat in GameObject.FindGameObjectsWithTag("seat"))
                    {
                        temp.Add(seat);
                    }

                    availableMovieSeats[4] = temp;

                }

            }
        }
       


        if(min<=9)
        {
            time.text = hr.ToString() + ":0" + min.ToString() + " " + amOrPM;
        }
        else
        time.text = hr.ToString() + ":" + min.ToString()+ " " + amOrPM;

    }

    protected Node[] GetPoliceBehaviors()
    {
        GameObject[] NEPolice = GameObject.FindGameObjectsWithTag("NE");
        GameObject[] NSPolice = GameObject.FindGameObjectsWithTag("NS");
        GameObject[] EWPolice = GameObject.FindGameObjectsWithTag("EW");
        GameObject[] SWPolice = GameObject.FindGameObjectsWithTag("SW");
        GameObject north = GameObject.Find("north");
        GameObject south = GameObject.Find("south");
        GameObject east = GameObject.Find("east");
        GameObject west = GameObject.Find("west");

        List<Node> policeList = new List<Node>();

        foreach (GameObject nep in NEPolice)
        {
            policeList.Add(new DecoratorLoop(new Sequence(new MoveTo(nep.GetComponent<SmartCharacter>(), nep.GetComponent<SmartCharacter>().startloc.GetComponent<SmartCorner>()).PBT(),new GoToCorner(nep.GetComponent<SmartCharacter>(),north.GetComponent<SmartCorner>()).PBT(), new GoToCorner(nep.GetComponent<SmartCharacter>(), east.GetComponent<SmartCorner>()).PBT())));
        }

        foreach (GameObject nsp in NSPolice)
        {
            policeList.Add(new DecoratorLoop(new Sequence(new MoveTo(nsp.GetComponent<SmartCharacter>(), nsp.GetComponent<SmartCharacter>().startloc.GetComponent<SmartCorner>()).PBT(),new GoToCorner(nsp.GetComponent<SmartCharacter>(), north.GetComponent<SmartCorner>()).PBT(), new GoToCorner(nsp.GetComponent<SmartCharacter>(), south.GetComponent<SmartCorner>()).PBT())));
        }

        foreach (GameObject ewp in EWPolice)
        {
            policeList.Add(new DecoratorLoop(new Sequence(new MoveTo(ewp.GetComponent<SmartCharacter>(), ewp.GetComponent<SmartCharacter>().startloc.GetComponent<SmartCorner>()).PBT(),new GoToCorner(ewp.GetComponent<SmartCharacter>(), east.GetComponent<SmartCorner>()).PBT(), new GoToCorner(ewp.GetComponent<SmartCharacter>(), west.GetComponent<SmartCorner>()).PBT())));
        }

        foreach (GameObject swp in SWPolice)
        {
            policeList.Add(new DecoratorLoop(new Sequence(new MoveTo(swp.GetComponent<SmartCharacter>(), swp.GetComponent<SmartCharacter>().startloc.GetComponent<SmartCorner>()).PBT(),new GoToCorner(swp.GetComponent<SmartCharacter>(), south.GetComponent<SmartCorner>()).PBT(), new GoToCorner(swp.GetComponent<SmartCharacter>(), west.GetComponent<SmartCorner>()).PBT())));
        }

        return policeList.ToArray();
    }

    protected Node[] GetShopOwnerBehaviors()
    {
        List<Node> shopOwnBehaviorList = new List<Node>();

        GameObject[] owners = GameObject.FindGameObjectsWithTag("owner");
        GameObject[] owners1 = GameObject.FindGameObjectsWithTag("owner1");
        GameObject[] owners2 = GameObject.FindGameObjectsWithTag("owner2");
        GameObject[] owners3 = GameObject.FindGameObjectsWithTag("owner3");

        foreach(GameObject owner in owners)
        {
            shopOwnBehaviorList.Add(new Sequence(
            new MoveTo(owner.GetComponent<SmartCharacter>(), owner.GetComponent<SmartCharacter>().startloc.GetComponent<SmartCorner>()).PBT(), new LookAt(owner.GetComponent<SmartCharacter>(), owner.GetComponent<OwnerData>().faceloc.GetComponent<SmartCorner>()).PBT()));
        }

        foreach (GameObject owner in owners1)
        {
            shopOwnBehaviorList.Add(new Sequence(
            new MoveTo(owner.GetComponent<SmartCharacter>(), owner.GetComponent<SmartCharacter>().startloc.GetComponent<SmartCorner>()).PBT(), new LookAt(owner.GetComponent<SmartCharacter>(), owner.GetComponent<OwnerData>().faceloc.GetComponent<SmartCorner>()).PBT()));
        }

        foreach (GameObject owner in owners2)
        {
            shopOwnBehaviorList.Add(new Sequence(
            new MoveTo(owner.GetComponent<SmartCharacter>(), owner.GetComponent<SmartCharacter>().startloc.GetComponent<SmartCorner>()).PBT(), new LookAt(owner.GetComponent<SmartCharacter>(), owner.GetComponent<OwnerData>().faceloc.GetComponent<SmartCorner>()).PBT()));
        }

        foreach (GameObject owner in owners3)
        {
            shopOwnBehaviorList.Add(new Sequence(
            new MoveTo(owner.GetComponent<SmartCharacter>(), owner.GetComponent<SmartCharacter>().startloc.GetComponent<SmartCorner>()).PBT(), new LookAt(owner.GetComponent<SmartCharacter>(), owner.GetComponent<OwnerData>().faceloc.GetComponent<SmartCorner>()).PBT()));
        }

        Node[] shopOwnBehaviors = shopOwnBehaviorList.ToArray();
        
        return shopOwnBehaviors; 
    }

    protected Node GetGateKeeperBehaviors()
    {
        GameObject[] gkeepers = GameObject.FindGameObjectsWithTag("gatekeeper");

        return new Sequence(
            new SequenceParallel(
            new MoveTo(gkeepers[0].GetComponent<SmartCharacter>(), gkeepers[0].GetComponent<SmartCharacter>().startloc.GetComponent<SmartCorner>()).PBT(),
            new MoveTo(gkeepers[1].GetComponent<SmartCharacter>(), gkeepers[1].GetComponent<SmartCharacter>().startloc.GetComponent<SmartCorner>()).PBT()),
            new DecoratorLoop(
                new SequenceParallel(
                    new LookAt(gkeepers[0].GetComponent<SmartCharacter>(), gkeepers[0].GetComponent<GateKeeperData>().faceloc.GetComponent<SmartCorner>()).PBT(),
                    new LookAt(gkeepers[1].GetComponent<SmartCharacter>(), gkeepers[1].GetComponent<GateKeeperData>().faceloc.GetComponent<SmartCorner>()).PBT())));
        
    }

    protected Node GetAgentBehavior(GameObject agent)
    {

        return new Sequence( 
            new MoveTo(agent.GetComponent<SmartCharacter>(),agent.GetComponent<SmartCharacter>().startloc.GetComponent<SmartCorner>()).PBT(),
            new DecoratorLoop(
                        GetMainStory(agent)));
    }


    protected Node GetMainStory(GameObject agent)
    {
        return new Selector(
            new SequenceParallel(
                new DecoratorLoop(
                    new LeafAssert(() => !agent.GetComponent<AgentData>().getUserInteractionStatus())),
                new DecoratorLoop(
                    new Selector(
                    new SequenceParallel(
                        new DecoratorLoop(
                                new LeafAssert(() => !(agent.GetComponent<AgentData>().moviestart && agent.GetComponent<AgentData>().communicationover))
                                ),
                                    
                        new DecoratorLoop(
                                        new SequenceShuffle(
                                            GoOrMeet(agent))
                                )
                        ),
                    new Sequence(
                        new LeafInvoke(()=>cf.RemoveFromInteractionmapByVavlue(agent)),
                        GetMovieBehavior(agent))
                    ))
                  
                ),
            new Sequence(
                new LeafInvoke(()=>cf.RemoveFromInteractionmapByVavlue(agent)),
                new LeafInvoke(()=> cf.DeactivateAllObject(agent.transform)),
                new LeafInvoke(() => cf.interactionMap.Remove(agent)),
                new LeafInvoke(()=>agent.GetComponent<AgentData>().communicationover= true),
                GetUserInteractionBehavior(agent)));
    }

    protected Node GetMeetingStory(GameObject agent)
    {
          return  new Sequence(
            new DecoratorForceStatus(RunStatus.Success, new Sequence(
                new LeafAssert(()=> cf.interactionMap.ContainsKey(agent)),
                GetMeetingAction(agent))));
    }



    protected Node GetUserInteractionStory(GameObject agent)
    {
        return new DecoratorForceStatus(RunStatus.Success,
            new Sequence(
            new LeafAssert(() => agent.GetComponent<AgentData>().getUserInteractionStatus()),
            GetUserInteractionBehavior(agent)));
    }

    protected Node GetUserInteractionBehavior(GameObject agent)
    {
        return new Sequence(
            new LeafInvoke(()=>agent.GetComponent<AgentData>().setApproval(true)),
            new DecoratorForceStatus(RunStatus.Success,
                new DecoratorLoop(
                        new LeafAssert(()=>agent.GetComponent<AgentData>().getUserInteractionStatus()))
            ),
            new LeafInvoke(() => agent.GetComponent<AgentData>().setApproval(false)));
    }

    protected Node[] GoOrMeet(GameObject agent)
    {
        Node[] meetList = MeetAllPersons(agent);
        Node[] goList = GoToAllShop(agent);
        Node[] totalList = new Node[meetList.Length + goList.Length+1];
        meetList.CopyTo(totalList, 0);
       goList.CopyTo(totalList, meetList.Length);
        totalList[meetList.Length + goList.Length] = StealIdol(agent);
        return totalList;
    }

    protected Node StealIdol(GameObject agent)
    {
        return new DecoratorForceStatus(RunStatus.Success,
            new Sequence(
                new LeafAssert(()=>GameObject.Find("PricelessIdol")!=null),
                new GoToIdol(agent.GetComponent<SmartCharacter>(),GameObject.Find("PricelessIdol").GetComponent<SmartIdol>()).PBT(),
                new LeafInvoke(()=>investigatePanel.SetActive(true))));
    }

    protected Node GetMovieBehavior(GameObject agent)
    {
        Val<GameObject> position = Val.V(() => agent.GetComponent<AgentData>().movieSeat);
        Func<bool> hourchecker = () => !((hr == 11 && amOrPM.Equals("am")) || ((hr == 3 || hr == 5 || hr == 8 || hr == 11) && (amOrPM.Equals("pm"))));
        return new Sequence(
           GoToShop(agent, GameObject.Find("popcornandsoda")),
            new LeafInvoke(()=>executePBT(new GoToTheatre(agent.GetComponent<SmartCharacter>(),position.Value.GetComponent<SmartTheatreSeat>()).PBT())),
            new DecoratorForceStatus(RunStatus.Success,
                        new DecoratorLoop(new LeafAssert(hourchecker))),
            new LeafTrace("MOVIE OVER "+agent.name),
            new LeafInvoke(()=>agent.GetComponent<AgentData>().movieSeat=null),
            new LeafInvoke(()=>agent.GetComponent<AgentData>().resetMovieTime()),
            new GoToCorner(agent.GetComponent<SmartCharacter>(),exit.GetComponent<SmartCorner>()).PBT());
    }


    protected Node[] GoToAllShop(GameObject agent)
    {
        GameObject[] smallshops = GameObject.FindGameObjectsWithTag("shop");
        GameObject[] bigshops = GameObject.FindGameObjectsWithTag("bigshop");
        GameObject[] shops = new GameObject[smallshops.Length + bigshops.Length];

        smallshops.CopyTo(shops, 0);
        bigshops.CopyTo(shops, smallshops.Length);

        int index = 0;
        Node[] shopVisits = new Node[shops.Length];
        foreach(GameObject shopLocation in shops)
        {
            shopVisits[index++] = new Sequence( GoToShop(agent, shopLocation),
                GetMeetingStory(agent));
        }
        return shopVisits;
    }


    protected Node[] MeetAllPersons(GameObject agent)
    {
        GameObject[] persons = GameObject.FindGameObjectsWithTag("agent");
        GameObject[] personsExceptHim = new GameObject[persons.Length - 1];
        int index = 0;
        foreach (GameObject person in persons)
        {
            if(!person.name.Equals(agent.name))
            {
                personsExceptHim[index++] = person;
            }
        }

        index = 0;
        Node[] agentMeets = new Node[personsExceptHim.Length];
        foreach (GameObject person in personsExceptHim)
        {
            agentMeets[index++] = new Sequence( MeetPerson(agent, person), GetMeetingStory(agent));
        }
        return agentMeets;
    }

    protected Node MeetPerson(GameObject agent,GameObject person)
    {
        
        Val<Vector3> position = person.GetComponent<AgentData>().getPos();
        Func<bool> closeChecker = () => !(Vector3.Distance(agent.transform.position, person.GetComponent<AgentData>().getPos().Value) <= 3.5f);


        return new Sequence(
           new DecoratorForceStatus(RunStatus.Success, new Sequence(
                        new LeafAssert(() => !cf.interactionMap.ContainsKey(person)),
                        new LeafAssert(() => !cf.interactionMap.ContainsValue(person)),
         new DecoratorForceStatus(RunStatus.Success,
                new SequenceParallel(
                        new Sequence(
                         new LeafAssert(() => cf.AddtoInteractionMap(person, agent)),
                         new LeafInvoke(() => person.GetComponent<AgentData>().setCalledBy(agent)),
                            new DecoratorForceStatus(RunStatus.Success,
                            new DecoratorLoop(new LeafAssert(() => !person.GetComponent<AgentData>().getAck()))),
                            new Meet(agent.GetComponent<SmartCharacter>(), person.GetComponent<SmartCharacter>()).PBT(),
                            StartAgentInteraction(agent, person),
                            new LeafTrace("done "+agent.name+" "+person.name),
                            new LeafInvoke(() => cf.RemoveFromInteractionMap(agent,person))
                        ),

                     new Sequence(
                                     new DecoratorForceStatus(RunStatus.Success, new DecoratorLoop(new LeafAssert(closeChecker))),
                                     DoWaving(agent, person))))
                )));
        


        
    }


    protected Node StartAgentInteraction(GameObject agent,GameObject person)
    {
        return new DecoratorForceStatus(RunStatus.Success,
            new SelectorParallel(
                new DecoratorForceStatus(RunStatus.Success, new DecoratorLoop(new LeafAssert(()=> !person.GetComponent<AgentData>().getUserInteractionStatus()))),
                new Sequence(
                    ShakeHands(agent, person),
                    ChooseInteractionLoop(agent, person)
                    )
                )
          );
    }
    
    public GameObject FindGameObject(Transform agent, string oname)
    {
        if (agent.FindChild(oname) != null)
        {
            return agent.FindChild(oname).gameObject;
        }
        foreach(Transform child in agent)
        {
            GameObject result = FindGameObject(child, oname);
            if(result!=null)
            {
                return result;
            }
        }
        return null;
    }

    public void ActivateObject(Transform agent, string oname)
    {
        /*  foreach(GameObject gesture in agent.GetComponent<AgentData>().gesturePanelList)
          {
              if(gesture.name.Equals(oname))
              gesture.SetActive(true);
          }
          */
        GameObject panel = FindGameObject(agent, oname);
        if (panel!=null)
        {
            panel.SetActive(true);
        }
    }


    public void DeactivateObject(Transform agent, string oname)
    {
        /*foreach (GameObject gesture in agent.GetComponent<AgentData>().gesturePanelList)
        {
            if (gesture.name.Equals(oname))
                gesture.SetActive(false);
        }*/
        GameObject panel = FindGameObject(agent, oname);
        if (panel != null)
        {
            panel.SetActive(false);
        }
    }

    protected Node ChooseInteractionLoop(GameObject agent,GameObject person)
    {
        System.Random r = new System.Random();

        return new Sequence(
                    new Greet(agent.GetComponent<SmartCharacter>(),person.GetComponent<SmartCharacter>()).PBT(),
                    new GreetSmile(person.GetComponent<SmartCharacter>(), agent.GetComponent<SmartCharacter>()).PBT(),

          new SequenceShuffle(ChooseInteractionList(agent,person,r)),

            new LeafInvoke(()=>agent.GetComponent<AgentData>().LoadGestures()),

            new Bye(agent.GetComponent<SmartCharacter>(),person.GetComponent<SmartCharacter>()).PBT(),
            new GoodBye(person.GetComponent<SmartCharacter>(),agent.GetComponent<SmartCharacter>()).PBT()
            );
    }

    protected Node[] ChooseInteractionList(GameObject agent, GameObject person, System.Random r)
    {
        int ch;
        Node[] interactionchoiceList = new Node[5];
        for(int i=0;i<interactionchoiceList.Length;i++)
        {
            ch = r.Next(0, agent.GetComponent<AgentData>().gesturelist.Count);
            interactionchoiceList[i] = ChooseInteraction(agent, person, ch);
        }

        return interactionchoiceList;
    }

    protected Node ChooseInteraction(GameObject agent,GameObject person,int ch)
    {
        Val<string[]> gestures = Val.V<string[]>(() => agent.GetComponent<AgentData>().gesturelist[ch]);
        Val<string> gesture0 = Val.V<string>(() => gestures.Value[0]);
        Val<string> gesture1 = Val.V<string>(() => gestures.Value[1]);

        //System.Random r = new System.Random();
        return
                    new Sequence(
                        new DecoratorForceStatus(RunStatus.Success,
                            new Sequence(
                                new LeafAssert(() => gestures.Value[0].Equals("insult")),
                                new LeafInvoke(() => agent.GetComponent<AgentData>().gesturelist.Add(new string[] { "apologize", "acceptapology" })))),

                        GetGestureObject(gesture0.Value, agent.GetComponent<SmartCharacter>(), person.GetComponent<SmartCharacter>()),
                        GetGestureObject(gesture1.Value, person.GetComponent<SmartCharacter>(), agent.GetComponent<SmartCharacter>())
                        );


    }

    protected Node ShakeHands(GameObject actor1, GameObject actor2)
    {
        Val<Vector3> Agent1 = actor1.GetComponent<AgentData>().getPos();
        Val<Vector3> Agent2 = actor2.GetComponent<AgentData>().getPos();

        return new Sequence(
            /*new SequenceParallel(
                actor2.GetComponent<BehaviorMecanim>().Node_OrientTowards(Agent1),
                actor1.GetComponent<BehaviorMecanim>().Node_OrientTowards(Agent2)
                ),*/
                new SequenceParallel(
                new ShakeHands(actor1.GetComponent<SmartCharacter>(),actor2.GetComponent<SmartCharacter>()).PBT(),
                new ShakeHands(actor2.GetComponent<SmartCharacter>(), actor1.GetComponent<SmartCharacter>()).PBT()
                )
            );

    }

    public Node HandAnimation(GameObject agent, string anim, long time)
    {
        Val<string> animation_name = Val.V(() => anim);
        Val<bool> start = Val.V(() => true);
        return new Sequence(
            agent.GetComponent<BehaviorMecanim>().Node_HandAnimation(animation_name, start),
            new LeafWait(time),
            agent.GetComponent<BehaviorMecanim>().Node_HandAnimation(animation_name, false));
    }


    

    protected Node DoWaving(GameObject agent, GameObject person)
    {
        
            return new SequenceParallel( agent.GetComponent<BehaviorMecanim>().ST_PlayHandGesture("WAVE", 1000),
               person.GetComponent<BehaviorMecanim>().ST_PlayHandGesture("WAVE", 1000)
            );
                    
    }

    protected Node GetMeetingAction(GameObject agent)
    {
        return new Sequence(
            new LeafInvoke(()=>agent.GetComponent<AgentData>().setAck(true)),
            new DecoratorForceStatus(RunStatus.Success,
                new DecoratorLoop(
                        new LeafAssert(()=>cf.interactionMap.ContainsKey(agent)))
            ),
            new LeafInvoke(() => agent.GetComponent<AgentData>().setAck(false)));

    }



    protected Node resetVisisted(GameObject agent)
    {
        return new LeafInvoke(()=>agent.GetComponent<AgentData>().resetShopVisited());
    }

    protected Node GoToShop(GameObject agent, GameObject shop)
    {
        Val<Vector3> position = Val.V(() => shop.transform.FindChild("shoplocation").transform.position);

        return
            new Sequence(
                new GoTo(agent.GetComponent<SmartCharacter>(),shop.GetComponent<SmartPlace>()).PBT(),
                new LeafInvoke(()=>agent.GetComponent<AgentData>().setPos(position)),
                //agent.GetComponent<BehaviorMecanim>().Node_OrientTowards(position),
                BuyFromShop(agent,shop),
                new LeafWait(1000));
    }



    protected Node BuyFromShop(GameObject agent, GameObject shop)
    {
        System.Random r = new System.Random();
        int choice = 0;
        int seatNo = -1;
        System.Random randSeat = new System.Random();

        return new Selector(
            new Sequence(
                new LeafAssert(() => shop.name.Equals("cinema")),
                new Selector(
                    new Sequence(
                        new LeafAssert(() => hr >= 1 && hr < 9 && amOrPM == "am"),
                        new LeafInvoke(() => choice = r.Next(0, affordanceMap[shop.name].Length - 1))
                        ),
                    new Sequence(
                        new LeafAssert(() => (hr >= 9 && amOrPM == "am")),
                        new LeafInvoke(() => choice = r.Next(1, affordanceMap[shop.name].Length - 1))
                        ),
                    new Sequence(
                        new LeafAssert(() => hr >= 1 && hr < 3 && amOrPM == "pm"),
                        new LeafInvoke(() => choice = r.Next(2, affordanceMap[shop.name].Length - 1))),
                    new Sequence(
                        new LeafAssert(() => hr >= 3 && hr < 6 && amOrPM == "pm"),
                        new LeafInvoke(() => choice = r.Next(3, affordanceMap[shop.name].Length - 1))),
                    new LeafInvoke(() => choice = 4)),

                    new Selector(
                        new Sequence(
                            new LeafAssert(() => choice == 0),
                            new LeafInvoke(() => agent.GetComponent<AgentData>().movieTime.setSchedule(9, "am", 11, "am")),
                            new LeafInvoke((() => agent.GetComponent<AgentData>().show = "morning")),
                            new LeafInvoke((() => agent.GetComponent<AgentData>().starth = agent.GetComponent<AgentData>().movieTime.starthr)),
                            new LeafInvoke((() => agent.GetComponent<AgentData>().endh = agent.GetComponent<AgentData>().movieTime.endhr)),
                            new LeafInvoke((() => agent.GetComponent<AgentData>().sampm = agent.GetComponent<AgentData>().movieTime.starttime)),
                            new LeafInvoke((() => agent.GetComponent<AgentData>().eampm = agent.GetComponent<AgentData>().movieTime.endtime)),
                            new LeafInvoke(() => seatNo = randSeat.Next(0, availableMovieSeats[0].Count)),
                            new LeafInvoke(() => agent.GetComponent<AgentData>().movieSeat = availableMovieSeats[0][seatNo]),
                            new LeafInvoke(() => availableMovieSeats[0].RemoveAt(seatNo))),
                         new Sequence(
                            new LeafAssert(() => choice == 1),
                            new LeafInvoke(() => agent.GetComponent<AgentData>().movieTime.setSchedule(1, "pm", 3, "pm")),
                            new LeafInvoke((() => agent.GetComponent<AgentData>().show = "noon")),
                            new LeafInvoke((() => agent.GetComponent<AgentData>().starth = agent.GetComponent<AgentData>().movieTime.starthr)),
                            new LeafInvoke((() => agent.GetComponent<AgentData>().endh = agent.GetComponent<AgentData>().movieTime.endhr)),
                            new LeafInvoke((() => agent.GetComponent<AgentData>().sampm = agent.GetComponent<AgentData>().movieTime.starttime)),
                            new LeafInvoke((() => agent.GetComponent<AgentData>().eampm = agent.GetComponent<AgentData>().movieTime.endtime)),
                            new LeafInvoke(() => seatNo = randSeat.Next(0, availableMovieSeats[1].Count)),
                            new LeafInvoke(() => agent.GetComponent<AgentData>().movieSeat = availableMovieSeats[1][seatNo]),
                            new LeafInvoke(() => availableMovieSeats[1].RemoveAt(seatNo))),
                         new Sequence(
                             new LeafAssert(() => choice == 2),
                            new LeafInvoke(() => agent.GetComponent<AgentData>().movieTime.setSchedule(3, "pm", 5, "pm")),
                            new LeafInvoke((() => agent.GetComponent<AgentData>().show = "matinee")),
                            new LeafInvoke((() => agent.GetComponent<AgentData>().starth = agent.GetComponent<AgentData>().movieTime.starthr)),
                            new LeafInvoke((() => agent.GetComponent<AgentData>().endh = agent.GetComponent<AgentData>().movieTime.endhr)),
                            new LeafInvoke((() => agent.GetComponent<AgentData>().sampm = agent.GetComponent<AgentData>().movieTime.starttime)),
                            new LeafInvoke((() => agent.GetComponent<AgentData>().eampm = agent.GetComponent<AgentData>().movieTime.endtime)),
                            new LeafInvoke(() => seatNo = randSeat.Next(0, availableMovieSeats[2].Count)),
                            new LeafInvoke(() => agent.GetComponent<AgentData>().movieSeat = availableMovieSeats[2][seatNo]),
                            new LeafInvoke(() => availableMovieSeats[2].RemoveAt(seatNo))),
                         new Sequence(
                             new LeafAssert(() => choice == 3),
                            new LeafInvoke(() => agent.GetComponent<AgentData>().movieTime.setSchedule(6, "pm", 8, "pm")),
                            new LeafInvoke((() => agent.GetComponent<AgentData>().show = "evening")),
                            new LeafInvoke((() => agent.GetComponent<AgentData>().starth = agent.GetComponent<AgentData>().movieTime.starthr)),
                            new LeafInvoke((() => agent.GetComponent<AgentData>().endh = agent.GetComponent<AgentData>().movieTime.endhr)),
                            new LeafInvoke((() => agent.GetComponent<AgentData>().sampm = agent.GetComponent<AgentData>().movieTime.starttime)),
                            new LeafInvoke((() => agent.GetComponent<AgentData>().eampm = agent.GetComponent<AgentData>().movieTime.endtime)),
                            new LeafInvoke(() => seatNo = randSeat.Next(0, availableMovieSeats[3].Count)),
                            new LeafInvoke(() => agent.GetComponent<AgentData>().movieSeat = availableMovieSeats[3][seatNo]),
                            new LeafInvoke(() => availableMovieSeats[3].RemoveAt(seatNo))),
                         new Sequence(
                            new LeafAssert(() => choice == 4),
                            new LeafInvoke(() => agent.GetComponent<AgentData>().movieTime.setSchedule(9, "pm", 11, "pm")),
                            new LeafInvoke((() => agent.GetComponent<AgentData>().show = "night")),
                            new LeafInvoke((() => agent.GetComponent<AgentData>().starth = agent.GetComponent<AgentData>().movieTime.starthr)),
                            new LeafInvoke((() => agent.GetComponent<AgentData>().endh = agent.GetComponent<AgentData>().movieTime.endhr)),
                            new LeafInvoke((() => agent.GetComponent<AgentData>().sampm = agent.GetComponent<AgentData>().movieTime.starttime)),
                            new LeafInvoke((() => agent.GetComponent<AgentData>().eampm = agent.GetComponent<AgentData>().movieTime.endtime)),
                            new LeafInvoke(() => seatNo = randSeat.Next(0, availableMovieSeats[4].Count)),
                            new LeafInvoke(() => agent.GetComponent<AgentData>().movieSeat = availableMovieSeats[4][seatNo]),
                            new LeafInvoke(() => availableMovieSeats[4].RemoveAt(seatNo)))),


                    new LeafTrace(agent.name + " bought " + affordanceMap[shop.name][choice] + " movie ticket from " + shop.name)

                ),
            new Sequence(
                new LeafInvoke(() => choice = r.Next(1, affordanceMap[shop.name].Length)),
                new LeafTrace(agent.name + " bought " + affordanceMap[shop.name][choice] + " from " + shop.name)
                )
            );
    }

    protected void LoadAffordances()
    {
        affordanceMap.Add("spa",new string[] { "body massage", "face massage", "manicure", "pedicure" });

        affordanceMap.Add("barbershop", new string[] { "regular hair cut", "special haircut", "shave"});

        affordanceMap.Add("juiceshop", new string[] { "mango juice", "apple  juice", "mano milkshake", "grape juice","mixed fruit juice","banana milkshake","orange juice","vanilla milkshake","strawberry milshake" });

        affordanceMap.Add("coffeencakeshop", new string[] { "coffee", "cappuccino", "latte", "cupcake","donut" });

        affordanceMap.Add("clothesshop1", new string[] { "trouser", "jeans", "chinos", "red tshirt","green tshirt","yellow tshirt","blue tshirt" });

        affordanceMap.Add("electronicsshop", new string[] { "phone", "laptop", "television", "ipad" });

        affordanceMap.Add("pawnshop", new string[] { "take a loan", "pay back loan", "buy goods","sell goods" });

        affordanceMap.Add("toyshop", new string[] { "doll", "robot", "car", "props","board game","puzzle" });

        affordanceMap.Add("clothesshop3",new string[] { "trouser", "jeans", "chinos", "red tshirt", "green tshirt", "yellow tshirt", "blue tshirt", "skirt","shorts" });

        affordanceMap.Add("clothesshop2", new string[] { "red tshirt", "green tshirt", "yellow tshirt", "blue tshirt" });

        affordanceMap.Add("delistore", new string[] { "bread", "egg", "jam", "chips","soda" });

        affordanceMap.Add("pizzashop", new string[] { "pizza1", "pizza2", "pizza3", "pizza4", "pizza5", "combo 1" , "combo 2", "combo 3","soda" });

        affordanceMap.Add("kitchenappliancesshop", new string[] { "oven", "microwave", "toaster", "utensils" });

        affordanceMap.Add("medicalshop1", new string[] { "medicine1", "medicine2", "medicine3", "medicine4" });

        affordanceMap.Add("laundry1", new string[] { "wash cloth", "dry cloth", "special wash", "special dry", "washing soap" });

        affordanceMap.Add("sportsshop", new string[] { "soccer goods", "football goods", "golf goods", "basketball goods" });

        affordanceMap.Add("bookshop1", new string[] { "book 1", "book 2", "book 3", "book 4", "book 5" });

        affordanceMap.Add("petshop", new string[] { "dog", "cat", "rabbit", "hamster","dog food","cat food","rabbit food","hamster food" });

        affordanceMap.Add("bakeryshop", new string[] { "bagel", "donut", "cupcake", "cake","mousse" });

        affordanceMap.Add("shoeshop", new string[] { "shoe 1", "shoe 2", "shoe 3", "shoe 4", "shoe 5"});

        affordanceMap.Add("burgershop", new string[] { "burger 1", "burger 2", "burger 3", "burger 4","fries","soda","burger combo 1","burger combo 2","burger combo 3" });

        affordanceMap.Add("hotdogshop", new string[] { "hotdog 1", "hotdog 2", "hotdog 3", "hotdog combo" });

        affordanceMap.Add("cinema", new string[] { "morning show", "noon show", "matinee show", "evening show","night show" });

        affordanceMap.Add("popcornandsoda", new string[] { "popcorn 1", "popcorn 2", "soda 1", "soda 2","combo 1","combo 2","burger","sandwich" });

        affordanceMap.Add("departmentstore", new string[] { "food", "baby food", "home appliance", "bathroom appliance" });

        affordanceMap.Add("postoffice", new string[] { "send mail", "receive mail" });

        affordanceMap.Add("PoliceStation", new string[] { "file a complaint", "Ask about previous complaint" });

        affordanceMap.Add("bank", new string[] { "loan", "withdraw", "deposit", "pay loan" });

        affordanceMap.Add("medicalshp2", new string[] { "medicine1", "medicine2", "medicine3", "medicine4", "medicine5", "medicine6", "medicine7", "medicine8", "medicine9" });

        affordanceMap.Add("homeappliancesshop", new string[] { "cutains", "couch", "table", "chair","bed" });

        affordanceMap.Add("laundry2", new string[] { "wash cloth", "dry cloth", "special wash", "special dry", "washing soap" });

        affordanceMap.Add("hardwareshop", new string[] { "tools", "screws", "adhesive","ladder" });

        affordanceMap.Add("icecreamshop", new string[] { "ice cream 1", "ice cream 2", "ice cream 3", "ice cream 4", "ice cream 5" });

        affordanceMap.Add("groceryshop", new string[] { "rice", "flour", "ketchup", "cooking oil", "butter" });


    }

    public Node GetGestureObject(string gesture, SmartCharacter agent1, SmartCharacter agent2)
    {
        return new Selector(
            new Sequence(new LeafAssert(()=> gesture.Equals("acceptapology")),new AcceptApology(agent1, agent2).PBT()), 
            new Sequence(new LeafAssert(()=> gesture.Equals("amazed")), new Amazed(agent1, agent2).PBT()), 
            new Sequence(new LeafAssert(()=> gesture.Equals("amazing")), new Amazing(agent1, agent2).PBT()), 
            new Sequence(new LeafAssert(()=> gesture.Equals("angry")), new Angry(agent1, agent2).PBT()), 
            new Sequence(new LeafAssert(()=> gesture.Equals("apologize")), new Apologize(agent1, agent2).PBT()), 
            new Sequence(new LeafAssert(()=> gesture.Equals("bad")), new Bad(agent1, agent2).PBT()), 
            new Sequence(new LeafAssert(()=> gesture.Equals("bye")), new Bye(agent1, agent2).PBT()), 
            new Sequence(new LeafAssert(()=> gesture.Equals("cheer")), new Cheer(agent1, agent2).PBT()), 
            new Sequence(new LeafAssert(()=> gesture.Equals("cheerful")), new Cheerful(agent1, agent2).PBT()), 
            new Sequence(new LeafAssert(()=> gesture.Equals("compliment")), new Compliment(agent1, agent2).PBT()), 
            new Sequence(new LeafAssert(()=> gesture.Equals("congratulate")), new Congratulate(agent1, agent2).PBT()), 
            new Sequence(new LeafAssert(()=> gesture.Equals("good")), new Good(agent1, agent2).PBT()), 
            new Sequence(new LeafAssert(()=> gesture.Equals("goodbye")), new GoodBye(agent1, agent2).PBT()), 
            new Sequence(new LeafAssert(()=> gesture.Equals("greet")), new Greet(agent1, agent2).PBT()), 
            new Sequence(new LeafAssert(()=> gesture.Equals("greetsmile")), new GreetSmile(agent1, agent2).PBT()), 
            new Sequence(new LeafAssert(()=> gesture.Equals("insult")), new Insult(agent1, agent2).PBT()), 
            new Sequence(new LeafAssert(()=> gesture.Equals("joke")), new Joke(agent1, agent2).PBT()), 
            new Sequence(new LeafAssert(()=> gesture.Equals("laugh")), new Laugh(agent1, agent2).PBT()), 
            new Sequence(new LeafAssert(()=> gesture.Equals("sad")), new Sad(agent1, agent2).PBT()), 
            new Sequence(new LeafAssert(()=> gesture.Equals("shy")), new Shy(agent1, agent2).PBT()), 
            new Sequence(new LeafAssert(()=> gesture.Equals("smile")), new Smile(agent1, agent2).PBT()), 
            new Sequence(new LeafAssert(()=> gesture.Equals("tellnews")), new TellNews(agent1, agent2).PBT()), 
            new Sequence(new LeafAssert(()=> gesture.Equals("thanks")), new Thanks(agent1, agent2).PBT()), 
            new Sequence(new LeafAssert(()=> gesture.Equals("wicked")), new Wicked(agent1, agent2).PBT()), 
            new Sequence(new LeafAssert(()=> gesture.Equals("wickedsmile")), new WickedSmile(agent1, agent2).PBT())

        );
    }

    protected void executePBT(Node pbt)
    {
        BehaviorAgent = new BehaviorAgent(pbt);
        BehaviorManager.Instance.Register(BehaviorAgent);
        BehaviorAgent.StartBehavior();
    }

}
