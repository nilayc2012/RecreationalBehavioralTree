using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* This script is attached to objects with an auto-biograpical
 * memory. The observer will only recored events which it does
 * or sees 
 */

public class Observer : MonoBehaviour {

	List<string> observerTrace = new List<string>();
	string obsName;
	bool msgRead = false;
	int visionFieldLength = 30;

	Quaternion startingAngle = Quaternion.AngleAxis(-60, Vector3.up);
	Quaternion stepAngle = Quaternion.AngleAxis(4, Vector3.up);

	// Get the name of the character that the observer is attached to.
	void Start () {

		obsName = this.gameObject.GetComponent<SmartCharacter> ().name;
	}
	
	// Update is called once per frame
	void Update () {

		// If the msgs in the msg bus are read, set it to false
		if (msgRead) {
			MessageBus.ResetMsgBus ();
			msgRead = false;
		}

		// If msg bus has any unread msgs, get them
		if (MessageBus.HasUnreadMsgs ()) {
			
			List<TraceMessage> msgs = MessageBus.GetMsgsInMsgBus ();
			msgs = GetMsgsForObserver (msgs);
			foreach (TraceMessage msg in msgs) {
				
				string actOneState = "+" + NSM.GetStateForActorAsString (msg.GetActorOneName ());
				string actTwoState = "+" + NSM.GetStateForActorAsString (msg.GetActorTwoName ());
				observerTrace.Add (msg.asString ());
				observerTrace.Add (actOneState);
				observerTrace.Add (actTwoState);
			}
			msgRead = true;
		}


		// When program ends, save trace
		if (MessageBus.killSignal) {
			SaveObserverTrace ();
			msgRead = true;
		}
		GetObjectsInFieldOfView ();
		//DetectObjectsInFOV ();
	}


	/* Return the msgs that are related-to (in field-of-view)
	 * of the observer.
	 * Input  : List of msgs
	 * Return : List of msgs in first person.
	 */
	List<TraceMessage> GetMsgsForObserver (List<TraceMessage> tMsgs) {
		
		List<string> objs = GetObjectsInFieldOfView ();
		List<TraceMessage> observedMsgs = new List<TraceMessage>();
		foreach (TraceMessage tMsg in tMsgs) {

			TraceMessage msg = new TraceMessage(tMsg);
			if (msg.ConvertToFirstPerson (obsName)) {
				observedMsgs.Add (msg);
			} else {
				//go through all observers
				List<string> objsInFOV = GetObjectsInFieldOfView ();
				foreach (string obj in objsInFOV) {
					if ((msg.GetActorOneName ().Equals (obj) || msg.GetActorTwoName ().Equals (obj)) && (!observedMsgs.Contains (msg))) {
						observedMsgs.Add (msg);
					}
				}
			}
		}
		return observedMsgs;
	}

	/* Returns the names of all SmartObjects in the Field Of View 
	 * of the observer.
	 * SIDE NOTES : All the Smart Objects in the scene will have a
	 * 'SmartObject' tag attaced to them, and variable of SmartObject
	 * type has a 'name' field. This function should return the names
	 * of all the objects with tag 'SmartObject' in its field of view.
	*/
	List<string> GetObjectsInFieldOfView () {
		
		RaycastHit hit;
		List<string> objs = new List<string> ();
		var angle = transform.rotation * startingAngle;
		var direction = angle * Vector3.forward;
		var pos =  transform.position;
		pos.y = 1.5f;
		for(var i = 0; i < 30; i++)
		{
			if (Physics.Raycast (pos, direction, out hit, visionFieldLength)) {
				//Debug.LogWarning ("Rayhit - " + hit.collider.name + " -for- " + obsName);
				Debug.DrawLine (pos, hit.collider.transform.position, Color.red);
                if (hit.collider.GetComponentInParent<SmartCharacter>() != null)
                    {
                        string objName = hit.collider.GetComponentInParent<SmartCharacter>().name;
                        if (!objs.Contains(objName))
                            objs.Add(objName);
                    }
			} else {
				Debug.DrawLine (pos, pos + direction * visionFieldLength, Color.green);
			}
			direction = stepAngle * direction;
		}

		return objs;
	}

	// Saves the observer trace to a text file
	public void SaveObserverTrace () {

		Debug.Log ("Observer " + obsName + " : ");
		string fileName = Constants.traceFilesPath + obsName + ".txt";
		string[] msgList = observerTrace.ToArray ();
		System.IO.File.WriteAllLines (fileName, msgList);
		foreach (string msg in msgList)
			Debug.Log (msg);
	}
}
