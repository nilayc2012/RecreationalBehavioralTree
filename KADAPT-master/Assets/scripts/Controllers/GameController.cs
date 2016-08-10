using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TreeSharpPlus;

public class GameController : MonoBehaviour {

	private BehaviorAgent behaviorAgent;
	private CameraController cameraController;
	private GameObject holder;
	private string displayMessage;
    public Camera camerad;

	public GameObject gameMenu;
	public GameObject mergePane;

	// Use this for initialization
	void Start () {

		HelperFunctions.GetAgentsWithTrace ();
       
        HelperFunctions.CreateSmartObjectToGameObjectMap ();
        Debug.Log("done");

        behaviorAgent = null;
		Time.timeScale = 0f;
		cameraController = camerad.GetComponent<CameraController> ();
	}
	
	// Update is called once per frame
	void Update () {


		if (Constants.MergeMemories) {

			cameraController.EnableCameraMoveControls ();
			holder = cameraController.cameraHolder;
			Time.timeScale = 1f;
			gameMenu.SetActive (false);

			// Camera movement
			float moveHorizontal = Input.GetAxis ("Horizontal");
			float moveVertical = Input.GetAxis ("Vertical");
			Vector3 movement = new Vector3 (moveVertical, 0f, -moveHorizontal);
			holder.transform.position = holder.transform.position + movement;

			// Get Objects mouse points to
			RaycastHit hit = new RaycastHit ();
			string hitCharacter = null;
			Ray ray = camerad.ScreenPointToRay (Input.mousePosition);
			if (Physics.Raycast (ray, out hit, 1000)) {
				if (hit.collider.GetComponentInParent<SmartCharacter> ())
					hitCharacter = hit.collider.GetComponentInParent<SmartCharacter> ().name;
			}

			if (hitCharacter != null) {
				if (!Constants.objsWithMemories.Contains (hitCharacter)) {
					displayMessage = hitCharacter + " doesn't have a memory";
				} else {
					if (Constants.objsMergeMemories.Contains (hitCharacter)) {
						displayMessage = "Memory added for " + hitCharacter + " - 'Click' to Remove!!";
					} else {
						displayMessage = "Memory exists for " + hitCharacter + " - 'Click' to Add!!";
					}

					if (Input.GetMouseButtonDown (0)) {
						if (Constants.objsMergeMemories.Contains(hitCharacter)) {
							Constants.objsMergeMemories.Remove (hitCharacter);
						} else {
							Constants.objsMergeMemories.Add (hitCharacter);
						}
					}
				}
			} else {
				displayMessage = "";
			}
		}
	
	}


	public Node SaveTraces () {

        return new LeafInvoke (
			() => MessageBus.SaveTraces ());
	}

	public Node ResetApplication () {

		return new LeafInvoke (
			() => SceneManager.LoadScene (0));
	}

	public Node TerminateTree () {
        
		return new Sequence (this.SaveTraces (), new LeafWait (1000), this.ResetApplication ());
	}

	public void DisplayMergePane () {

		Constants.MergeMemories = false;
		HelperFunctions.PopulateAgentStories ();
		string txt = HelperFunctions.GetAgentStoriesAsString ();
		mergePane.SetActive (true);
		Text memoriesTxt = GameObject.Find ("Memories").GetComponent<Text>();
		memoriesTxt.text = txt;
	}

	void OnGUI () {

		if (Constants.MergeMemories) {

			GUI.color = Color.white;
			if (displayMessage != "")
				GUI.Box (new Rect (40, Screen.height - 30, 250, 25), displayMessage);

			if (Constants.objsMergeMemories.Count != 0) {

				GUI.Box (new Rect (Screen.width - 75, 5, 65, Screen.height-10), HelperFunctions.GetAsString (Constants.objsMergeMemories));
				if (GUI.Button (new Rect (Screen.width / 2 - 27, Screen.height - 25, 54, 25), "Merge"))
					DisplayMergePane ();
			}
		}
	}
}
