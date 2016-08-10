using UnityEngine;
using System.Collections;
using BehaviorTrees;

public class SmartItem :  SmartObject{

	void Awake () {

	name = this.gameObject.name;
	}

	public SmartItem(string name) {
		this.name = "\"" + name + "\"";
		Constants.PBT_Trace = Constants.PBT_Trace + "An item " + this.name + " / Exists\n";
	}

}
