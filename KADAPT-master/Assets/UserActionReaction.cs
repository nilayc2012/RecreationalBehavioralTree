using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Collections.Generic;
using TreeSharpPlus;
using RootMotion.FinalIK;

public class UserActionReaction : MonoBehaviour {

    public GameObject gotobutton;
    public GameObject meetbutton;
    public GameObject gotopanel;
    public GameObject mainmeetpanel;
    public GameObject gesturePanel;
    public GameObject shopPanel;
    public GameObject mainPanel;
    public GameObject moviemessagepanel;
    public GameObject movieTimePanel;
    public GameObject exitPanel;

    public GameObject gotoDropDown;
    public GameObject meetDropDown;
    public GameObject shopDropDown;
    public GameObject gestureDropDown;

    public GameObject newcampos;

    public GameObject exit;

    bool goingToMovie;

    public Camera maincamera;

    public Text movieMessage;
    public Text movieTimeMessage;

    private BehaviorAgent BehaviorAgent;

    public List<string[]> gesturelist;

    private int timecount;

    GameObject activePanel;

    private string currentShop;

    private string currentPerson;

    public Camera newcamera;

    void Awake()
    {
        currentPerson = "";
        currentShop = "";
        timecount = 0;
        goingToMovie = false;
        LoadGestures();
    }
    // Use this for initialization
    void Start()
    {
        Node action = new MoveTo(this.GetComponent<SmartCharacter>(), this.GetComponent<SmartCharacter>().startloc.GetComponent<SmartCorner>()).PBT();
        BehaviorAgent = new BehaviorAgent(action);
        BehaviorManager.Instance.Register(BehaviorAgent);
        BehaviorAgent.StartBehavior();

    }
	
	// Update is called once per frame
	void Update () {


        if((60 - GameObject.Find("GameController").GetComponent<Behavior>().min)>0 && GameObject.Find("GameController").GetComponent<Behavior>().min>=30)
            movieTimeMessage.text = "The Movie is starting in " + (60- GameObject.Find("GameController").GetComponent<Behavior>().min) + " mins";
        
        if(this.GetComponent<AgentData>().movieSeat!=null && (GameObject.Find("GameController").GetComponent<Behavior>().hr==this.GetComponent<AgentData>().movieTime.starthr-1)&& (GameObject.Find("GameController").GetComponent<Behavior>().amOrPM == this.GetComponent<AgentData>().movieTime.starttime)&&(GameObject.Find("GameController").GetComponent<Behavior>().min==30))
        {
          
            if (gotopanel.activeSelf)
            {
                activePanel = gotopanel;
                gotopanel.SetActive(false);
            }
            if (mainmeetpanel.activeSelf)
            {
                activePanel = mainmeetpanel;
                mainmeetpanel.SetActive(false);
            }
            if(gesturePanel.activeSelf)
            {
                activePanel = gesturePanel;
                gesturePanel.SetActive(false);
            }
            if(shopPanel.activeSelf)
            {
                activePanel = shopPanel;
                shopPanel.SetActive(false);
            }
            if(mainPanel.activeSelf)
            {
                activePanel = mainPanel;
                mainPanel.SetActive(false);
            }
            if(moviemessagepanel.activeSelf)
            {
                activePanel = moviemessagepanel;
                moviemessagepanel.SetActive(false);
            }

            movieTimePanel.SetActive(true);
        }

        if(GameObject.Find("GameController").GetComponent<Behavior>().hr == this.GetComponent<AgentData>().movieTime.endhr && GameObject.Find("GameController").GetComponent<Behavior>().amOrPM.Equals(this.GetComponent<AgentData>().movieTime.endtime))
        {

                exitPanel.SetActive(true);
            
        }
        
	
	}
    

   /* void OnTriggerEnter(Collider other)
    {
        button.SetActive(true);
    }
    void OnTriggerExit(Collider other)
    {
        button.SetActive(false);
    }*/

    public void startActionGoto()
    {
        mainPanel.SetActive(false);
        LoadGotoDropDown();
        gotopanel.SetActive(true);
    }

    public void startActionMeet()
    {
        mainPanel.SetActive(false);
        LoadMeetDropDown();
        mainmeetpanel.SetActive(true);
    }

    public void IntercationListPopUp()
    {
        gesturePanel.SetActive(true);
    }

    public void OnChangeMeet()
    {

        GameObject person= GameObject.Find(meetDropDown.GetComponent<Dropdown>().options[meetDropDown.GetComponent<Dropdown>().value].text);
        
        currentPerson = person.name;
        
        mainmeetpanel.SetActive(false);
        Node action = new Sequence(
            new LeafInvoke(() => person.GetComponent<AgentData>().setUserInteractionStatus(true)),
            new LeafTrace(this.gameObject.name + "user interacting " + person.name+" is "+!person.GetComponent<AgentData>().getApproval()),
            new DecoratorForceStatus(RunStatus.Success, new DecoratorLoop(new LeafAssert(() => !person.GetComponent<AgentData>().getApproval()))),
            new LeafTrace(this.gameObject.name + "user interacting "+person.name),
            this.GetComponent<BehaviorMecanim>().Node_GoToUpToRadius(person.transform.position,2),
            new LeafInvoke(()=>person.GetComponent<BehaviorMecanim>().Node_OrientTowards(this.transform.position)),
            new LeafInvoke(() => gesturePanel.SetActive(true))

            );
        BehaviorAgent = new BehaviorAgent(action);
        BehaviorManager.Instance.Register(BehaviorAgent);
        BehaviorAgent.StartBehavior();
       
    }

    public void loadIteamsForShop(GameObject shop)
    {
        string[] affordances= GameObject.Find("GameController").GetComponent<Behavior>().affordanceMap[shop.name];

        List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();

        foreach(string affordance in affordances)
        {
            options.Add(new Dropdown.OptionData(affordance));
        }
        shopDropDown.GetComponent<Dropdown>().options.Clear();
        shopDropDown.GetComponent<Dropdown>().AddOptions(options);

        
    }

    public void loadItemsForMovie(GameObject shop)
    {
        Behavior gameController = GameObject.Find("GameController").GetComponent<Behavior>();
        string[] affordances = null;

        if (gameController.hr >= 1 && gameController.hr < 9 && gameController.amOrPM == "am")
        {
            affordances= GameObject.Find("GameController").GetComponent<Behavior>().affordanceMap[shop.name];

        }
        else if (gameController.hr >= 9 && gameController.amOrPM == "am")
        {
            affordances = new string[gameController.affordanceMap[shop.name].Length - 1];
            Array.Copy(gameController.affordanceMap[shop.name], 1, affordances, 0, gameController.affordanceMap[shop.name].Length - 1);
        }
        else if (gameController.hr >= 1 && gameController.hr < 3 && gameController.amOrPM == "pm")
        {
            affordances = new string[gameController.affordanceMap[shop.name].Length - 2];
            Array.Copy(gameController.affordanceMap[shop.name], 2, affordances, 0, gameController.affordanceMap[shop.name].Length - 2);
        }
        else if (gameController.hr >= 3 && gameController.hr < 6 && gameController.amOrPM == "pm")
        {
            affordances = new string[gameController.affordanceMap[shop.name].Length - 3];
            Array.Copy(gameController.affordanceMap[shop.name], 3, affordances, 0, gameController.affordanceMap[shop.name].Length - 3);
        }
        else if (gameController.hr >= 6 && gameController.amOrPM == "pm")
        {
            affordances = new string[1];
            Array.Copy(gameController.affordanceMap[shop.name], 4, affordances, 0, 1);
        }
        List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();


        foreach (string affordance in affordances)
        {
            options.Add(new Dropdown.OptionData(affordance));
        }
        shopDropDown.GetComponent<Dropdown>().options.Clear();
        shopDropDown.GetComponent<Dropdown>().AddOptions(options);
    }

    public void OnChangeGoTo()
    {
        //Debug.Log(GameObject.Find("Dropdown List")!= null);
        GameObject shop = GameObject.Find(gotoDropDown.GetComponent<Dropdown>().options[gotoDropDown.GetComponent<Dropdown>().value].text);
        //Debug.Log(GameObject.Find("Dropdown List") != null);


        gotopanel.SetActive(false);
        if (gotoDropDown.GetComponent<Dropdown>().options[gotoDropDown.GetComponent<Dropdown>().value].text.Equals("cinema"))
        {
            Transform location = shop.transform.FindChild("shoplocation");
            currentShop = shop.name;
            Node action = new Sequence(
                new GoTo(this.GetComponent<SmartCharacter>(),shop.GetComponent<SmartPlace>()).PBT(),
                new LeafInvoke(()=> BuyMovieTicket(shop)));

            BehaviorAgent = new BehaviorAgent(action);
            BehaviorManager.Instance.Register(BehaviorAgent);
            BehaviorAgent.StartBehavior();

        }
        else if(gotoDropDown.GetComponent<Dropdown>().options[gotoDropDown.GetComponent<Dropdown>().value].text.Equals("Go To Movie"))
        {
            currentShop = "theatre";
            GoToMovie();
        }
        else
        {
            Transform location = shop.transform.FindChild("shoplocation");
            currentShop = shop.name;
            Node action = new Sequence(
                new GoTo(this.GetComponent<SmartCharacter>(), shop.GetComponent<SmartPlace>()).PBT(),
                new LeafInvoke(() => shopPanel.SetActive(true)))
                ;

            BehaviorAgent = new BehaviorAgent(action);
            BehaviorManager.Instance.Register(BehaviorAgent);
            BehaviorAgent.StartBehavior();
            loadIteamsForShop(shop);
        }
    }

    public void OnChangeBuy()
    {
        string choice = null;

        if (currentShop.Equals("cinema"))
        {
            choice=shopDropDown.GetComponent<Dropdown>().options[shopDropDown.GetComponent<Dropdown>().value].text;
            System.Random randSeat = new System.Random();
            int seatNo;

            switch (choice)
            {
                case "morning show":
                    this.gameObject.GetComponent<AgentData>().movieTime.setSchedule(9, "am", 11, "am");
                    seatNo = randSeat.Next(0, GameObject.Find("GameController").GetComponent<Behavior>().availableMovieSeats[0].Count);

                    this.gameObject.GetComponent<AgentData>().movieSeat = GameObject.Find("GameController").GetComponent<Behavior>().availableMovieSeats[0][seatNo];
                    GameObject.Find("GameController").GetComponent<Behavior>().availableMovieSeats[0].RemoveAt(seatNo);
                    Debug.Log(this.gameObject.name + " bought " + choice + " movie ticket from " + currentShop);
                    break;
                case "noon show":
                    this.gameObject.GetComponent<AgentData>().movieTime.setSchedule(1, "pm", 3, "pm");
                    seatNo = randSeat.Next(0, GameObject.Find("GameController").GetComponent<Behavior>().availableMovieSeats[1].Count);

                    this.gameObject.GetComponent<AgentData>().movieSeat = GameObject.Find("GameController").GetComponent<Behavior>().availableMovieSeats[1][seatNo];
                    GameObject.Find("GameController").GetComponent<Behavior>().availableMovieSeats[1].RemoveAt(seatNo);
                    Debug.Log(this.gameObject.name + " bought " + choice + " movie ticket from " + currentShop);
                    break;
                case "matinee show":
                    this.gameObject.GetComponent<AgentData>().movieTime.setSchedule(3, "pm", 5, "pm");
                    seatNo = randSeat.Next(0, GameObject.Find("GameController").GetComponent<Behavior>().availableMovieSeats[2].Count);

                    this.gameObject.GetComponent<AgentData>().movieSeat = GameObject.Find("GameController").GetComponent<Behavior>().availableMovieSeats[2][seatNo];
                    GameObject.Find("GameController").GetComponent<Behavior>().availableMovieSeats[2].RemoveAt(seatNo);
                    Debug.Log(this.gameObject.name + " bought " + choice + " movie ticket from " + currentShop);
                    break;
                case "evening show":
                    this.gameObject.GetComponent<AgentData>().movieTime.setSchedule(6, "pm", 8, "pm");
                    seatNo = randSeat.Next(0, GameObject.Find("GameController").GetComponent<Behavior>().availableMovieSeats[3].Count);

                    this.gameObject.GetComponent<AgentData>().movieSeat = GameObject.Find("GameController").GetComponent<Behavior>().availableMovieSeats[3][seatNo];
                    GameObject.Find("GameController").GetComponent<Behavior>().availableMovieSeats[3].RemoveAt(seatNo);
                    Debug.Log(this.gameObject.name + " bought " + choice + " movie ticket from " + currentShop);
                    break;
                case "night show":
                    this.gameObject.GetComponent<AgentData>().movieTime.setSchedule(9, "pm", 11, "pm");
                    seatNo = randSeat.Next(0, GameObject.Find("GameController").GetComponent<Behavior>().availableMovieSeats[4].Count);

                    this.gameObject.GetComponent<AgentData>().movieSeat = GameObject.Find("GameController").GetComponent<Behavior>().availableMovieSeats[4][seatNo];
                    GameObject.Find("GameController").GetComponent<Behavior>().availableMovieSeats[4].RemoveAt(seatNo);
                    Debug.Log(this.gameObject.name + " bought " + choice + " movie ticket from " + currentShop);
                    break;

            }



            List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();

            options.Add(new Dropdown.OptionData("Go To Movie"));

            gotoDropDown.GetComponent<Dropdown>().AddOptions(options);

            shopPanel.SetActive(false);
            movieMessage.text = choice + " Booked";
            moviemessagepanel.SetActive(true);
        }
        else
        {
            Debug.Log("Robert bought " + shopDropDown.GetComponent<Dropdown>().options[shopDropDown.GetComponent<Dropdown>().value].text + " from " + currentShop);
        }

    }

    public void OnBookedTicket()
    {
        moviemessagepanel.SetActive(false);
        mainPanel.SetActive(true);
        
    }

    public void BuyMovieTicket(GameObject shop)
    {
        loadItemsForMovie(shop);
        shopPanel.SetActive(true);
    
    }

    public void onDoneShopping()
    {
        shopPanel.SetActive(false);
        mainPanel.SetActive(true);
    }

    public void OnChangeGesture()
    {
        GameObject person = GameObject.Find(currentPerson);
        string choice = gestureDropDown.GetComponent<Dropdown>().options[gestureDropDown.GetComponent<Dropdown>().value].text;
        Debug.Log(choice);
        Node action;

        if(choice.ToLower().Equals("greet"))
        {
            
            action = new Sequence(new LeafInvoke(()=> gesturePanel.SetActive(false)),
                    new Greet(this.GetComponent<SmartCharacter>(), person.GetComponent<SmartCharacter>()).PBT(),
                    new GreetSmile(person.GetComponent<SmartCharacter>(), this.GetComponent<SmartCharacter>()).PBT(),
            new LeafInvoke(() => gesturePanel.SetActive(true))
            );
        }
        else if(choice.ToLower().Equals("apologize"))
        {
            action = new Sequence(new LeafInvoke(()=> gesturePanel.SetActive(false)),
            new Apologize(this.GetComponent<SmartCharacter>(),person.GetComponent<SmartCharacter>()).PBT(),
            new AcceptApology(person.GetComponent<SmartCharacter>(),this.GetComponent<SmartCharacter>()).PBT(),
            new LeafInvoke(() => gesturePanel.SetActive(true)));
        }

        else
        {
            Val< string[] > gestures = Val.V<string[]>(() => gesturelist[gestureDropDown.GetComponent<Dropdown>().value - 1]);
            Val<string> gesture0 = Val.V<string>(() => gestures.Value[0]);
            Val<string> gesture1 = Val.V<string>(() => gestures.Value[1]);
            Debug.Log("number " + gesturelist.Count);

                action = new Sequence(new LeafInvoke(()=> gesturePanel.SetActive(false)),

                       GameObject.Find("GameController").GetComponent<Behavior>(). GetGestureObject(gesture0.Value, this.GetComponent<SmartCharacter>(), person.GetComponent<SmartCharacter>()),
                       GameObject.Find("GameController").GetComponent<Behavior>(). GetGestureObject(gesture1.Value, person.GetComponent<SmartCharacter>(), this.GetComponent<SmartCharacter>()),
                new LeafInvoke(()=> gesturePanel.SetActive(true))
                    
                );
            
        }

        
        BehaviorAgent = new BehaviorAgent(action);
        BehaviorManager.Instance.Register(BehaviorAgent);
        BehaviorAgent.StartBehavior();
        

    }

    public void OnDoneMeeting()
    {
        gesturePanel.SetActive(false);
        Node action = new Sequence(
            new Bye(this.GetComponent<SmartCharacter>(), GameObject.Find(currentPerson).GetComponent<SmartCharacter>()).PBT(),
            new GoodBye(GameObject.Find(currentPerson).GetComponent<SmartCharacter>(), this.GetComponent<SmartCharacter>()).PBT());
        BehaviorAgent = new BehaviorAgent(action);
        BehaviorManager.Instance.Register(BehaviorAgent);
        BehaviorAgent.StartBehavior();
        mainPanel.SetActive(true);
        GameObject.Find(currentPerson).GetComponent<AgentData>().setUserInteractionStatus(false);
    }

    public void LoadGotoDropDown()
    {
        List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();


        foreach (string key in GameObject.Find("GameController").GetComponent<Behavior>().affordanceMap.Keys)
        {
            options.Add(new Dropdown.OptionData(key));
        }
        gotoDropDown.GetComponent<Dropdown>().AddOptions(options);
    }

    public void LoadMeetDropDown()
    {
        List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();


        foreach (GameObject agent in GameObject.FindGameObjectsWithTag("agent"))
        {
            options.Add(new Dropdown.OptionData(agent.name));
        }
        meetDropDown.GetComponent<Dropdown>().AddOptions(options);
    }

    public void closeAllpanel()
    {
        gotopanel.SetActive(false);
        mainmeetpanel.SetActive(false);
        gesturePanel.SetActive(false);
        shopPanel.SetActive(false);
        mainPanel.SetActive(false);
        moviemessagepanel.SetActive(false);
        movieTimePanel.SetActive(false);
    }

    public void GoToMovie()
    {
        Val<GameObject> position = Val.V(() => this.GetComponent<AgentData>().movieSeat);
        movieTimePanel.SetActive(false);
        Node action = new Sequence(new GoToTheatre(this.GetComponent<SmartCharacter>(), position.Value.GetComponent<SmartTheatreSeat>()).PBT(),
            new LeafInvoke(()=> this.GetComponent<AgentData>().movieSeat=null),
            new LeafInvoke(() => closeAllpanel()));
        BehaviorAgent = new BehaviorAgent(action);
        BehaviorManager.Instance.Register(BehaviorAgent);
        BehaviorAgent.StartBehavior();
       
    }

    public void Ignore()
    {
        movieTimePanel.SetActive(false);
        if(activePanel!=null)
        activePanel.SetActive(true);
    }

    public void GotoExit()
    {
        Debug.Log("User exit");
        exitPanel.SetActive(false);
        this.GetComponent<AgentData>().resetMovieTime();
        Node action = new Sequence(new GoToCorner(this.GetComponent<SmartCharacter>(), exit.GetComponent<SmartCorner>()).PBT(),
            new LeafInvoke(() => mainPanel.SetActive(true)));
        BehaviorAgent = new BehaviorAgent(action);
        BehaviorManager.Instance.Register(BehaviorAgent);
        BehaviorAgent.StartBehavior();
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
