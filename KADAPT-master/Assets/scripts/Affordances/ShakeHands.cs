using UnityEngine;
using System.Collections;
using BehaviorTrees;
using TreeSharpPlus;

public class ShakeHands : Affordance {

	public ShakeHands (SmartCharacter afdnt, SmartCharacter afdee) {

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
		return (
		new Sequence( 
			new Sequence(
				this.affordant.gameObject.GetComponent<BehaviorMecanim>().Node_HandAnimation("reachright", true),
				new LeafWait(2000),
				this.affordant.gameObject.GetComponent<BehaviorMecanim>().Node_HandAnimation("reachright", false)),
		this.UpdateAndPublish()));
	}
}
