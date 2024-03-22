using System;
using UnityEngine;

namespace BehaviorTree
{
    [Serializable]
    public class Open : Task
    {
        // The character that is opening the thing.
        private Character opener;
        public Character Opener
        {
            get { return opener; }
            set { opener = value; }
        }

        // The thing that is being opened.
        private Thing openThis;
        public Thing OpenThis
        {
            get { return openThis; }
            set { openThis = value; }
        }

        // This constructor defaults the character and thing to None.
        public Open() : this(Character.None, Thing.None) { }

        // This constructor takes parameters for the character and thing.
        public Open (Character opener, Thing openThis)
        {
            Opener = opener;
            OpenThis = openThis;
        }

        // This method runs the Open action on the given WorldState object.
        public override bool run(WorldState state)
        {
            // Fill in your conditional logic here:
            bool opCheck = new IsOpen(OpenThis).run(state);
            if (state.CharacterPosition[Opener] == state.ThingPosition[OpenThis] && !opCheck)
            {
                if (state.Debug) Debug.Log(this + " Success");
                state.Open[OpenThis] = true;
                return true;
            }
            if (state.Debug) Debug.Log(this + " Fail");
            return false;
        }

        // Creates and returns a string describing the Open action.
        public override string ToString()
        {
            return opener + " opens " + openThis;
        }
    }
}
