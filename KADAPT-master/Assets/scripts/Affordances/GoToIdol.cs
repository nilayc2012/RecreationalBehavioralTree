using UnityEngine;
using System.Collections;
using BehaviorTrees;
using TreeSharpPlus;

public class GoToIdol : Affordance {

	Vector3 pos;

	public GoToIdol (SmartCharacter afdnt, SmartIdol afdee) {

		pos = afdee.transform.position;
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
		return (new Sequence(this.affordant.gameObject.GetComponent<BehaviorMecanim> ().Node_GoToUpToRadius (pos,2),new LeafInvoke(()=>affordee.gameObject.SetActive(false)), this.UpdateAndPublish()));
	}
}
