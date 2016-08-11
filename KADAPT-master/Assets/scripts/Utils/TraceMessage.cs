using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BehaviorTrees;

/* Messages that are present in the  Message bus are of TraceMessage
 * type. It has the names of actor1 and actor2, and the message.
 */

public class TraceMessage {

	string msg, actorOne, actorTwo;
	float time;

	// Optional Fields
	List<Condition> actorOneState, actorTwoState;
	bool hasStateInformation = false;

	public string GetMessage() {

		return msg;
	}

	public float GetTime() {

		return time;
	}

	public string GetActorOneName () {

		return actorOne;
	}

	public string GetActorTwoName () {

		return actorTwo;
	}

	public List<Condition> GetActorOneState () {

		return actorOneState;
	}

	public List<Condition> GetActorTwoState () {

		return actorTwoState;
	}

	public bool HasActorStates () {

		return hasStateInformation;
	}

	public void SetActorStates (List<Condition> actorOneState, List<Condition> actorTwoState) {

		this.actorOneState = actorOneState;
		this.actorTwoState = actorTwoState;
		hasStateInformation = true;
	}

	public bool HappensDuring (TraceMessage tMsg) {

		return (this.time == tMsg.GetTime ()) ? true : false;
	}

	public bool HappensBefore (TraceMessage tMsg) {

		return (this.time < tMsg.GetTime ()) ? true : false;
	}

	public void ConvertToThridPerson (string actorName) {

		if (msg.IndexOf ("I ") == 0) {
			msg = actorName + msg.Substring (1);
			actorOne = actorName;
		}

		if (msg.Substring (msg.Length - 2,2).Equals("me")) {
			msg = msg.Substring (0, msg.Length - 2) + actorName;
			actorTwo = actorName;
		}
	}

	public bool ConvertToFirstPerson (string actorName) {

		int indx = msg.IndexOf (actorName);
		if (indx != -1) {
			string newMsg = msg;
			if (indx != (msg.Length - actorName.Length)) {
				int strtPoint = indx + actorName.Length;
				newMsg = msg.Substring (0, indx) + "I" + msg.Substring(strtPoint);
			} else {
				newMsg = msg.Substring (0, indx) + "me";
			}
			msg = newMsg;
			return true;
		} else {
			return false;
		}
	}

	public string asString() {

		return time.ToString("n2") + " " + msg;
	}

	void SetUpValues (float msgTime, string msg, string actOne, string actTwo) {

		this.msg = msg;
		this.time = msgTime;
		this.actorOne = actOne;
		this.actorTwo = actTwo;
	}

	public TraceMessage (float msgTime, string msg) {

		string[] words = msg.Split(new char[] {' '});
		SetUpValues (msgTime, msg, words[0], words[2]);
	}

	public TraceMessage (float msgTime, string msg, string actOne, string actTwo) {

		SetUpValues (msgTime, msg, actOne, actTwo);
	}

	public TraceMessage (TraceMessage tMsg) {

		SetUpValues (tMsg.GetTime (), tMsg.GetMessage (), tMsg.GetActorOneName (), tMsg.GetActorTwoName ());
	}

	public bool Equals(TraceMessage tMsg) {
		
		if (tMsg == null)
			return false;
		if ((this.GetTime() == tMsg.GetTime()) && (this.GetMessage().Equals(tMsg.GetMessage()))) {
			return true;
		} else {
			return false;
		}
	}
}
