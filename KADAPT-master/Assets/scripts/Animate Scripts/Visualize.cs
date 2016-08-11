using UnityEngine;
using System.Collections;
using BehaviorTrees;

public static class Visualize {

	public static void UpdateSceneWithState (Condition state) {

		Constants.ConditionType condType = (Constants.ConditionType)System.Enum.Parse (typeof(Constants.ConditionType), state.getConditionName ());
		switch (condType) {
		case Constants.ConditionType.AT:
			if (state.isPosition ()) {
				MoveObjectToLocation (state.getActor (), state.GetPosition ());
			} else {
				MoveObjectToLocation (state.getActor (), state.getRealtedActor ());
			}
			break;
		default :
			break;
		}
	}

	static void MoveObjectToLocation (string agentName, Vector3 location) {

		GameObject agentObj = GameObject.Find (Constants.smartObjToGameObjMap [agentName]);
		agentObj.transform.position = location;
	}

	static void MoveObjectToLocation (string agentName, string locName) {

		GameObject agentObj = GameObject.Find (Constants.smartObjToGameObjMap [agentName]);
		GameObject locationObj = GameObject.Find (Constants.smartObjToGameObjMap [locName]);
		Vector3 loc = locationObj.transform.position;
		agentObj.transform.position = loc;
	}
}