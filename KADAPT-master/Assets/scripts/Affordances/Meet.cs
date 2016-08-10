using UnityEngine;
using System.Collections;
using TreeSharpPlus;
using BehaviorTrees;

public class Meet : Affordance {

	Val<Vector3> pos;
	float dist = 2;

	public Meet (SmartCharacter afdnt, SmartCharacter afdee) {

		affordant = afdnt;
		affordee = afdee;
		initialize ();
	}

	void initialize() {

		base.initialize ();
		pos = Val.V (() => affordee.gameObject.transform.position);
		//effects.Add (new Condition (affordant.name, Constants.ConditionType.AT, true));
		effects.Add (new Location (affordant.name, pos.Value));
		//Debug.Log ("Meet pos : " + pos.ToString () + " - " + affordant.name);
		root = this.PBT ();
	}

	public void UpdatePositionAndEffects () {

		//pos = affordee.gameObject.transform.position;
		effects.Add (new Location (affordant.name, pos.Value));
		Debug.Log ("Bleh pos : " + pos.Value.ToString ());
	}

	public Node PopulateDestinationPosition () {

		return new LeafInvoke (
			() => this.UpdatePositionAndEffects ());
	}

	public Node PBT(){

		//TODO : If required, animation code has to be written here
		return (new Sequence(/*this.PopulateDestinationPosition(), */this.affordant.gameObject.GetComponent<BehaviorMecanim> ().Node_GoToUpToRadius (pos, dist), this.UpdateAndPublish()));
	}

}
