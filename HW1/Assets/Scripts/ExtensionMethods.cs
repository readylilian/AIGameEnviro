using System.Collections;
using System.Collections.Generic;

//I used extension methods to add to the list methods, making it easier to 
namespace ExtensionMethods
{
    public static class ListExtension
    {
        //This adds the node record in the link in order from smallest value to greatest
        //This way we don't need a smallestElement method, we just grab the top
        public static void AddNodeRecord(this List<NodeRecord> list, NodeRecord record)
        {
            for (int i = 0; i < list.Count; i++)
            {
                //Since djikstra doesn't use estimate cost, astar is the only one this happens for
                if (list[i].estimatedCost > record.estimatedCost)
                {
                    list.Insert(i, record);
                    return;
                }
                if (list[i].costSoFar >= record.costSoFar)
                {
                    list.Insert(i, record);
                    return;
                }
                
                
            }
            list.Add(record);
        }
        //This allows us to search our noderecords more effectively, by comparing the nodes within
        //instead of comparing the actual objects, which could cause issues with the connection and cost differences 
        public static NodeRecord ContainsNode(this List<NodeRecord> list, Node node)
        {
            if (list.Count > 0)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    //If the node is the same, return that record
                    if (list[i].node == node)
                    {
                        return list[i];
                    }
                }
            }
            //Otherwise return null
            return null;
        }
    }
}