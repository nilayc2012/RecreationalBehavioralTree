using UnityEngine;
using System.Collections;
using BehaviorTrees;

public static class Visualize {

	public static void UpdateSceneWithState (Condition state) {

		Constants.ConditionType condType = (Constants.ConditionType)System.Enum.Parse (typeof(Constants.ConditionType), state.getConditionName ());
		switch (condType) {
		case Constants.ConditionType.AT:
			MoveObjectToLocation (state.getActor (), state.getRealtedActor ());
			break;
		default :
			break;
		}
	}

	static void MoveObjectToLocation (string agentName, string locName) {

		GameObject agentObj = GameObject.Find (Constants.smartObjToGameObjMap [agentName]);
		GameObject locationObj = GameObject.Find (Constants.smartObjToGameObjMap [locName]);
		Vector3 loc = locationObj.transform.position;
		agentObj.transform.position = loc;
	}
}
