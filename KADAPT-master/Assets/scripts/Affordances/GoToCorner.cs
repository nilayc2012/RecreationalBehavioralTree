﻿using UnityEngine;
using System.Collections;
using BehaviorTrees;
using TreeSharpPlus;

public class GoToCorner : Affordance {

	Vector3 pos;

	public GoToCorner (SmartCharacter afdnt, SmartCorner afdee) {

		pos = afdee.GetPosition ();
		affordant = afdnt;
		affordee = afdee;
		initialize ();
	}

	void initialize() {

		base.initialize ();

		//effects.Add (new Condition (affordant.name, Constants.ConditionType.AT, true));
		effects.Add (new Location (affordant.name, affordee.name));
		Debug.Log ("Goto pos : " + pos.ToString () + " - " + affordant.name);
		root = this.PBT ();
	}

	//PBT for GoTo affordace goes here
	public Node PBT(){

		//TODO : If required, animation code has to be written here
		return (new Sequence(this.affordant.gameObject.GetComponent<BehaviorMecanim> ().Node_GoToUpToRadius (Val.V(() =>pos),3), this.UpdateAndPublish(),new LeafWait(1000)));
	}
}
