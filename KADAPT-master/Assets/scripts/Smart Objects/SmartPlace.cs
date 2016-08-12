using UnityEngine;
using System.Collections;
using BehaviorTrees;

public class SmartPlace :  SmartObject{

	void Awake () {

		name = this.gameObject.name;
	}

	void Start() {
		
		name = this.gameObject.name;
	}

	public SmartPlace(string name) {
		this.name = "\"" + name + "\"";
		Constants.PBT_Trace = Constants.PBT_Trace + "A place " + this.name + " / Exists\n";
	}

	public Vector3 GetPosition(){

		return this.gameObject.transform.FindChild("shoplocation").transform.position;
	}
}
