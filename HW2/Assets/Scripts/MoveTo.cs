using System;
using UnityEngine;

namespace BehaviorTree
{
    [Serializable]
    public class MoveTo : Task
    {
        // The character that is moving to a different location.
        private Character mover;
        public Character Mover
        {
            get { return mover; }
            set { mover = value; }
        }

        // The location the character is moving to.
        private Location where;
        public Location Where
        {
            get { return where; }
            set { where = value; }
        }

        // This constructor defaults the character and location to None.
        public MoveTo() : this(Character.None, Location.None) { }

        // This constructor takes parameters for the character and location.
        public MoveTo (Character mover, Location where)
        {
            Mover = mover;
            Where = where;
        }

        // This method runs the MoveTo action on the given WorldState object.
        public override bool run (WorldState state)
        {
            if(state.CharacterPosition[Mover] != Where && state.ConnectedLocations[Where].Contains(state.CharacterPosition[Mover]))
            {
                //Debug.Log(state.BetweenLocations[Where].Item1);
                //Debug.Log(state.BetweenLocations[Where].Item2);
                if(state.BetweenLocations.ContainsKey(state.CharacterPosition[Mover])
                    && state.BetweenLocations[state.CharacterPosition[Mover]].Item1 == Thing.Gate
                    && new IsOpen(state.BetweenLocations[Where].Item1).run(state) == false)
                {
                    if (state.Debug) Debug.Log(this + " Fail");
                    return false;
                }
                if (state.Debug) Debug.Log(this + " Success");
                state.CharacterPosition[Mover] = Where;
                return true;
            }
            if (state.Debug) Debug.Log(this + " Fail");
            return false;
        }

        // Creates and returns a string describing the MoveTo action.
        public override string ToString()
        {
            return mover + " moves to " + where;
        }
    }
}
