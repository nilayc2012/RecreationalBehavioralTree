using UnityEngine;
using System.Collections;
using BehaviorTrees;

public class SmartCorner :  SmartObject{

	void Awake () {

		name = this.gameObject.name;
	}

	void Start() {
		
		name = this.gameObject.name;
	}

	public SmartCorner(string name) {
		this.name = "\"" + name + "\"";
		Constants.PBT_Trace = Constants.PBT_Trace + "A place " + this.name + " / Exists\n";
	}

	public Quaternion GetRotation(){

		return this.transform.rotation;
	}
	public Vector3 GetPosition()
	{
		return this.transform.position;
	}
}
