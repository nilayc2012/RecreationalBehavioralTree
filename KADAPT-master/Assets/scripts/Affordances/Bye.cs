using UnityEngine;
using System.Collections;
using BehaviorTrees;
using TreeSharpPlus;

public class Bye : Affordance {

	public Bye (SmartCharacter afdnt, SmartCharacter afdee) {

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
                    new SequenceParallel(
                        new LeafInvoke(() => GameObject.Find("GameController").GetComponent<Behavior>().ActivateObject(affordant.gameObject.transform, "bye")),
                        affordant.gameObject.GetComponent<BehaviorMecanim>().Node_HandAnimation("reachright",true)),
                    new LeafWait(2000),
					new SequenceParallel(
                        new LeafInvoke(() => GameObject.Find("GameController").GetComponent<Behavior>().DeactivateObject(affordant.gameObject.transform, "bye")),
                        affordant.gameObject.GetComponent<BehaviorMecanim>().Node_HandAnimation("reachright", false)),
		this.UpdateAndPublish()));
	}
}
