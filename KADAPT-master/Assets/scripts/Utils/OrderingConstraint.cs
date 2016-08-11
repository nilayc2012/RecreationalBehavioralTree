using UnityEngine;
using System.Collections;

namespace BehaviorTrees {
	public class OrderingConstraint {

		Affordance aff_1, aff_2;

		public OrderingConstraint(Affordance aff_1, Affordance aff_2) {

			this.aff_1 = aff_1;
			this.aff_2 = aff_2;
		}

		public bool Equals (OrderingConstraint cons) {

			if (cons == null)
				return false;
			else if (cons.aff_1 == aff_1 && cons.aff_2 == aff_2)
				return true;
			else
				return false;
		}
	}
}
