using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using BehaviorTrees;
using TreeSharpPlus;

public static class MergeTraceHelper {

	static List<TraceMessage> chronoNarrative = new List<TraceMessage>();

	/*public void GetAgentsWithTrace () {

		string[] traceFiles = System.IO.Directory.GetFiles (Constants.traceFilesPath, "*.txt");
		foreach (string file in traceFiles)
			Debug.Log (file);
	}*/

	public static void MergeMemories () {

		ConvertAllMemoriesTo3P ();
		chronoNarrative = ChronologicalMerge ();


		Text memoriesTxt = GameObject.Find ("Memories").GetComponent<Text>();
		memoriesTxt.text = HelperFunctions.GetTraceMsgsAsString (chronoNarrative);
	}

	public static void ConvertAllMemoriesTo3P () {

		foreach (string key in Constants.agentMemories.Keys) {
			for (int i = 0; i < Constants.agentMemories [key].Count; i++) {
				Constants.agentMemories [key] [i].ConvertToThridPerson (key);
			}
		}
	}

	public static List<TraceMessage> ChronologicalMerge () {

		List<TraceMessage> orderedMsgs = new List<TraceMessage> ();
		foreach (string key in Constants.agentMemories.Keys) {
			foreach (TraceMessage msg in Constants.agentMemories[key]) {
				if (!orderedMsgs.Any(tMsg => tMsg.asString() == msg.asString())) {
					orderedMsgs.Add (msg);
				}
			}
		}
		return orderedMsgs.OrderBy (tMsg => tMsg.GetTime ()).ToList ();
	}

	public static Node SetupSimulationEmvironment () {

		List<string> objsInKnowledge = new List<string> ();
		List<Affordance> affs = new List<Affordance> ();

		//Create Affordances & populate agents in knowledge
		foreach (TraceMessage msg in chronoNarrative) {
			affs.Add (HelperFunctions.GetAffordanceFromString (msg.GetMessage ()));
			if (!objsInKnowledge.Contains (msg.GetActorOneName ()))
				objsInKnowledge.Add (msg.GetActorOneName ());
			if (!objsInKnowledge.Contains (msg.GetActorTwoName ()))
				objsInKnowledge.Add (msg.GetActorTwoName ());
		}

		//Get the startState
		List<Condition> startState = new List<Condition>();
		bool isValid = ValidateNarrativeAndReturnStartState (affs, out startState);

		//Logs
		Debug.LogError (isValid.ToString ());
		foreach (Condition con in startState)
			Debug.Log (con.asString ());

		//Visualize Start State
		foreach (Condition cond in startState) {
			if (!objsInKnowledge.Contains (cond.getActor ()))
				objsInKnowledge.Add (cond.getActor ());
			if (!string.IsNullOrEmpty (cond.getRealtedActor ()) && !objsInKnowledge.Contains (cond.getRealtedActor ()))
				objsInKnowledge.Add (cond.getRealtedActor ());
		}

		Node treeRoot = null;
		if (isValid) {

			Text memoriesTxt = GameObject.Find ("Memories").GetComponent<Text>();
			memoriesTxt.text = memoriesTxt.text + "\nStart State : \n" + HelperFunctions.GetConditionsAsString (startState);

			VisualizeStartState (startState, objsInKnowledge);
			treeRoot = BuildNarrativeTree (affs);

			//BehaviorAgent behaviorAgent = new BehaviorAgent (treeRoot);
			//BehaviorManager.Instance.Register (behaviorAgent);
			//behaviorAgent.StartBehavior ();
		}

		return treeRoot;
	}

	static Node BuildNarrativeTree (List<Affordance> affs) {
		
		Node[] pbt = affs.Select (x => x.GetPBTRoot ()).ToArray ();
		return new Sequence (pbt);

	}

	static void VisualizeStartState (List<Condition> startState, List<string> objs) {

		//foreach (string obj in objs)
		//	Debug.LogWarning (obj);

		foreach (string objName in Constants.smartObjToGameObjMap.Keys) {
			if (!objs.Contains (objName))
				GameObject.Find (Constants.smartObjToGameObjMap [objName]).SetActive (false);
		}

		foreach (Condition state in startState) {
			Visualize.UpdateSceneWithState (state);
		}
	}

	static bool ValidateNarrativeAndReturnStartState(List<Affordance> affs, out List<Condition> systemStartState) {

		systemStartState = new List<Condition> ();
		bool valid = true;

		for (int i = chronoNarrative.Count - 1; i >= 0; i--) {
			// Adding trace to start state
			foreach (Condition cond in chronoNarrative[i].GetActorOneState()) {
				//Debug.Log ("1" + cond.asString ());
				foreach (Condition state in systemStartState) {
					if (cond.IsContradicts (state))
						valid = false;
				}

				if (!systemStartState.Contains (cond)) {
					//Debug.LogError ("+" + cond.asString ());
					systemStartState.Add (cond);
				}
			}

			foreach (Condition cond in chronoNarrative[i].GetActorTwoState()) {
				//Debug.Log ("2" + cond.asString ());
				foreach (Condition state in systemStartState) {
					if (cond.IsContradicts (state))
						valid = false;
				}

				if (!systemStartState.Contains (cond)) {
				//	Debug.LogError ("+" + cond.asString ());
					systemStartState.Add (cond);
				}
			}


			//Verifying with affordance
			foreach (Condition effect in affs[i].GetEffects()) {
				//Debug.Log ("3" + effect.asString ());
				foreach (Condition cond in systemStartState) {
					if (effect.IsContradicts (cond)) {
						valid = false;
					}
				}

				if (systemStartState.Contains (effect)) {
				//	Debug.LogError ("+" + effect.asString ());
				//	systemStartState.Add (effect);
				//} else {
				//	Debug.LogError ("-" + effect.asString ());
					systemStartState.Remove (effect);
				}
			}

			foreach (Condition preCond in affs[i].GetPreConditions()) {
				//Debug.Log ("4" + preCond.asString ());
				for (int j=0; j<systemStartState.Count;j++) {
					if (preCond.IsContradicts (systemStartState[j])) {
					//	Debug.LogError ("*" + preCond.asString ());
						systemStartState [j] = preCond;
					}
				}

				if (!systemStartState.Contains (preCond)) {
					//Debug.LogError ("+" + preCond.asString ());
					systemStartState.Add (preCond);
				}
			}
		}
		return valid;
	}
}
