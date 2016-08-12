using UnityEngine;
using System.Collections;
using BehaviorTrees;
using TreeSharpPlus;

public class Buy : Affordance {

	public Buy (SmartCharacter afdnt, SmartItem afdee) {

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
		this.UpdateAndPublish()));
	}
}
