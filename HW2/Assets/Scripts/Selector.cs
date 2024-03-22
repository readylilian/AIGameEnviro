using System;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree
{
    [Serializable]
    public class Selector : Task
    {
        // An ordered list of this Selector's children tasks.
        public List<Task> children { get; set; } = new List<Task>();

        // This method implements the selector behavior.
        public override bool run (WorldState state)
        {
            if (state.Debug) Debug.Log("Selector Start");
            foreach(Task c in children)
            {
                if(c.run(state))
                {
                    if (state.Debug) Debug.Log("Selector Success");
                    return true;
                }
            }
            // Fill in your selector logic here:
            if (state.Debug) Debug.Log("Selector Fail");
            return false;
        }
    }
}
