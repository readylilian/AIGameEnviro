using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Import extension methods
using ExtensionMethods;

/// <summary>
/// Performs search using A*.
/// </summary>
public class AStar : MonoBehaviour
{
    // Colors for the different search categories.
    public static Color openColor = Color.cyan;
    public static Color closedColor = Color.blue;
    public static Color activeColor = Color.yellow;
    public static Color pathColor = Color.yellow;

    // The stopwatch for timing search.
    private static Stopwatch watch = new Stopwatch();

    public static IEnumerator search(GameObject start, GameObject end, Heuristic heuristic, float waitTime, bool colorTiles = false, bool displayCosts = false, Stack<NodeRecord> path = null)
    {
        if (path == null)
        {
            path = new Stack<NodeRecord>();
        }
        // Starts the stopwatch.
        watch.Start();

        // Add your A* code here.
        //Create the initial start node, with a cost of zero
        NodeRecord startRecord = new NodeRecord();
        startRecord.node = start.GetComponentInChildren<Node>();
        startRecord.Tile = start;
        startRecord.costSoFar = 0;
        startRecord.estimatedCost = heuristic.Invoke(start, start, end);

        //Retrieves the number used to scale the game world tiles.
        float scale = start.transform.localScale.x;

        //Create our initial open and closed list, and declare the current noderecord so we can use it later
        List<NodeRecord> open = new List<NodeRecord>();
        open.AddNodeRecord(startRecord);
        List<NodeRecord> closed = new List<NodeRecord>();
        NodeRecord current = null;

        //While there are items in the open list, keep going through them
        while (open.Count > 0)
        {
            //Set current to the smallest value, which is the top of the list
            current = open[0];
            if (colorTiles)
            {
                current.ColorTile(activeColor);
                yield return new WaitForSeconds(waitTime);
            }
            //If we're at the goal, break
            if (current.Tile == end) break;
            //Otherwise add the valid connections to our list to check
            Dictionary<Direction, GameObject> connections = current.node.Connections;
            foreach (KeyValuePair<Direction, GameObject> connection in connections)
            {
                GameObject endNode = connection.Value;
                NodeRecord endNodeRecord = new NodeRecord();
                endNodeRecord.Tile = endNode;
                endNodeRecord.node = endNode.GetComponent<Node>();
                //This time we use scale and heuristics to change the cost and estimates
                float endNodeCost = current.costSoFar + scale;
                float endNodeHeuristic = current.estimatedCost;
                //If we've already checked it
                if (closed.ContainsNode(endNode.GetComponent<Node>()) is NodeRecord closedRoute && closedRoute != null)
                {
                    if (closedRoute.costSoFar <= endNodeCost) continue;
                    closed.Remove(closedRoute);
                    endNodeHeuristic = closedRoute.estimatedCost - closedRoute.costSoFar;
                }

                //If we're going to check it and the other route is faster skip it
                else if (open.ContainsNode(endNode.GetComponent<Node>()) is NodeRecord openRoute && openRoute != null)
                {
                    if (openRoute.costSoFar <= endNodeCost) continue;
                    endNodeHeuristic = openRoute.estimatedCost - openRoute.costSoFar;
                }
                //If it's completely new create a record for it and connect it
                else
                {
                    endNodeHeuristic = heuristic.Invoke(start, endNode, end);
                }
                endNodeRecord.costSoFar = endNodeCost;
                endNodeRecord.connection = current;
                endNodeRecord.estimatedCost = endNodeHeuristic + endNodeCost;
                //This time we're not comparing the nodes, so we can use regular contain
                if(!open.Contains(endNodeRecord))
                {
                    open.AddNodeRecord(endNodeRecord);
                }
                //Then update the display
                if (displayCosts)
                {
                    endNodeRecord.Display(endNodeCost);
                }
                if (colorTiles)
                {
                    endNodeRecord.ColorTile(openColor);
                    yield return new WaitForSeconds(waitTime);
                }
            }
            //When done add the current record to closed and remove it from the list, and update the color
            closed.AddNodeRecord(current);
            open.Remove(current);
            if (colorTiles)
            {
                current.ColorTile(closedColor);
            }
        }

        // Stops the stopwatch.
        watch.Stop();

        UnityEngine.Debug.Log("Seconds Elapsed: " + (watch.ElapsedMilliseconds / 1000f).ToString());
        UnityEngine.Debug.Log("Nodes Expanded: " + closed.Count);

        // Reset the stopwatch.
        watch.Reset();

        // Determine whether A* found a path and print it here.
        if (current.Tile != end)
        {
            UnityEngine.Debug.Log("Search Failed");
        }
        else
        {
            //Make the ends pink again
            current.ColorTile(Color.magenta);
            //Color the path
            while (current.Tile != start)
            {
                path.Push(current);

                current = current.connection;
                if (colorTiles)
                {
                    current.ColorTile(pathColor);
                    yield return new WaitForSeconds(waitTime);
                }
            }
            //The other end goes pink too
            current.ColorTile(Color.magenta);
            UnityEngine.Debug.Log("Path Length: " + path.Count.ToString());
        }
        yield return null;
    }

    
    public delegate float Heuristic(GameObject start, GameObject tile, GameObject goal);

    public static float Uniform(GameObject start, GameObject tile, GameObject goal)
    {
        return 0f;
    }

    public static float Manhattan(GameObject start, GameObject tile, GameObject goal)
    {
        float dx = Mathf.Abs(tile.transform.position.x - goal.transform.position.x );
        float dy = Mathf.Abs(tile.transform.position.y - goal.transform.position.y );
        return dx + dy;
    }

    public static float CrossProduct(GameObject start, GameObject tile, GameObject goal)
    {
        float dx1 = (tile.transform.position.x - goal.transform.position.x);
        float dy1 = (tile.transform.position.y - goal.transform.position.y);
        float dx2 = start.transform.position.x - goal.transform.position.x;
        float dy2 = start.transform.position.y - goal.transform.position.y;
        float cross = Mathf.Abs((dx1 * dy2) - (dx2 * dy1));
        return Manhattan(start, tile, goal) + cross * 0.001f;
    }

}
