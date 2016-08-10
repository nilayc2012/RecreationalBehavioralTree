using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BehaviorTrees;

public static class NSM {

	static List<Condition> narrativeState = new List<Condition> ();


	public static void SetInitialState (List<Condition> initState) {

		if (AreConditionsConsistent (initState))
			narrativeState = initState;
		else
			throw new System.Exception ("Invalid initial state");
	}

	static bool AreConditionsConsistent (List<Condition> conds) {

		foreach (Condition cond1 in conds) {
			foreach (Condition cond2 in conds) {
				//Debug.Log (cond1.asString () + " ; " + cond2.asString ());
				if (cond1.IsContradicts(cond2)) {
					return false;
				}
			}
		}
		return true;
	}

	public static void UpdateNarrativeState (List<Condition> updateConds) {


		Debug.Log (HelperFunctions.GetConditionsAsString (updateConds));

		bool isCondAdded = false;
		foreach (Condition updateCond in updateConds) {
			for (int i = 0; i < narrativeState.Count; i++) {
				if (updateCond.IsContradicts (narrativeState [i])) {
					narrativeState [i] = updateCond;
					isCondAdded = true;
				}
			}

			if (!isCondAdded)
				narrativeState.Add (updateCond);
			else
				isCondAdded = false;
		}
	}

	public static List<Condition> GetStateForActor (string actorName) {

		List<Condition> agentState = new List<Condition> ();

		foreach (Condition cond in narrativeState) {
			if (cond.getActor ().Equals (actorName))
				agentState.Add (cond);
		}

		return agentState;
	}

	public static string GetConditiosAsString (List<Condition> state) {

		string stateStr = "";
		foreach(Condition cond in state)
			stateStr = stateStr + cond.asString() + ";";

		return stateStr;
	}

	public static string GetStateForActorAsString(string actorName) {

		return GetConditiosAsString (GetStateForActor (actorName));
	}
}
