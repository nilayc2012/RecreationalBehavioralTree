using UnityEngine;
using System.Collections;
using BehaviorTrees;
using TreeSharpPlus;

public class LookAt : Affordance {

	Vector3 pos;
	Quaternion rot;

	
	public LookAt (SmartCharacter afdnt, SmartCorner afdee) {

		rot = afdee.GetRotation ();
		affordant = afdnt;
		affordee = afdee;
		initialize ();
	}
	

	void initialize() {

		base.initialize ();
		root = this.PBT ();
	}

	//PBT for GoTo affordace goes here
	public Node PBT(){

		//TODO : If required, animation code has to be written here
		return (new Sequence(this.affordant.gameObject.GetComponent<BehaviorMecanim> ().Node_Orient(rot), this.UpdateAndPublish()));
	}
}
