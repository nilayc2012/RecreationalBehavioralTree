using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TreeSharpPlus;

public class MenuScript : MonoBehaviour {

	public GameObject mergePane;
	bool memoriesMerged = false;
    public Camera mergecamera;
    public Camera mainCamera;
    public GameObject mainpanel;
    public bool animateflag;

    public void Awake()
    {
        animateflag = false;
    }

    public void Update()
    {
        if(animateflag)
        {
            CameraController cameraController = mergecamera.GetComponent<CameraController>();

            cameraController.EnableCameraMoveControls();
            GameObject holder = cameraController.cameraHolder;
            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");
            Vector3 movement = new Vector3(-moveHorizontal, 0f, -moveVertical);
            holder.transform.position = holder.transform.position + movement;
        }


    }
    public void RunSimulation()
    {

        Constants.StartSimulation = true;
    }

    public void Animate (Node treeRoot) {

        animateflag = true;

        GameObject[] gestures = GameObject.FindGameObjectsWithTag("gesture");
        foreach(GameObject gesture in gestures)
        {
            gesture.SetActive(false);
        }

        Debug.LogError("not null " + (treeRoot != null));
		BehaviorAgent behaviorAgent = new BehaviorAgent (treeRoot);
		BehaviorManager.Instance.Register (behaviorAgent);
		behaviorAgent.StartBehavior ();

		mergePane.SetActive (false);
	}

	public void ShowStartState () {

		Vector3 oc = new Vector3 (0, 0, 0);
		Debug.Log ("Bleh" + oc.ToString());
		Node treeRoot = MergeTraceHelper.SetupSimulationEmvironment ();

		GameObject mergeButton = GameObject.Find ("MergeContinue");
		Button button = mergeButton.GetComponent<Button> ();
		button.onClick.RemoveAllListeners ();
		button.onClick.AddListener (delegate {
			this.Animate (treeRoot);
		});

		//mergePane.SetActive (false);
	}

	public void MergeMemories () {
        mainCamera.gameObject.SetActive(false);
        mainpanel.SetActive(false);
        GameObject.Find("GameController").GetComponent<Behavior>().terminate = true;
       
        mergecamera.gameObject.SetActive(true);
		Constants.MergeMemories = true;

	}

	public void DisplayMergeMemories () {

		if (!memoriesMerged) {
			Debug.Log ("Old bleh");
			MergeTraceHelper.MergeMemories ();
			memoriesMerged = true;
		}
		GameObject mergeButton = GameObject.Find ("MergeContinue");
		Button button = mergeButton.GetComponent<Button> ();
		button.onClick.RemoveAllListeners ();
		button.onClick.AddListener (delegate {this.ShowStartState ();});
	}

	public void ResetSelect () {

		Constants.MergeMemories = true;
		memoriesMerged = false;
		mergePane.SetActive (false);
	}
}
