using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExtensionMethods;

/// <summary>
/// Performs search using Dijkstra's algorithm.
/// </summary>
public class Dijkstra : MonoBehaviour
{
    // Colors for the different search categories.
    public static Color openColor = Color.cyan;
    public static Color closedColor = Color.blue;
    public static Color activeColor = Color.yellow;
    public static Color pathColor = Color.yellow;

    // The stopwatch for timing search.
    private static Stopwatch watch = new Stopwatch();

    public static IEnumerator search(GameObject start, GameObject end, float waitTime, bool colorTiles = false, bool displayCosts = false, Stack<NodeRecord> path = null)
    {
        if(path == null)
        {
            path = new Stack<NodeRecord>();
        }
        // Starts the stopwatch.
        watch.Start();

        //Create the initial start node, with a cost of zero
        NodeRecord startRecord = new NodeRecord();
        startRecord.node = start.GetComponentInChildren<Node>();
        startRecord.Tile = start;
        startRecord.costSoFar = 0;
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
            foreach(KeyValuePair<Direction, GameObject> connection in connections)
            {
                GameObject endNode = connection.Value;
                //Since the graph itself isn't weighted, each node adds one to the previous cost
                float endNodeCost = current.costSoFar + 1;
                //If we've already checked it skip the tile
                if (closed.ContainsNode(endNode.GetComponent<Node>()) != null) continue; 

                //If we're going to check it and the other route is faster skip it
                else if (open.ContainsNode(endNode.GetComponent<Node>()) is NodeRecord otherRoute && otherRoute != null)
                {
                    if (otherRoute.costSoFar <= endNodeCost) continue;
                    //Otherwise change that noderecord to store this connection's information and continue 
                    else
                    {
                        otherRoute.costSoFar = endNodeCost;
                        otherRoute.connection = current;
                        continue;
                    }
                }
                //If it's completely new create a record for it and connect it
                else
                {
                    NodeRecord endNodeRecord = new NodeRecord();
                    endNodeRecord.Tile = endNode;
                    endNodeRecord.node = endNode.GetComponent<Node>();
                    endNodeRecord.costSoFar = endNodeCost;
                    endNodeRecord.connection = current;
                    //We've already performed the operations for what to do if it's in open, so just add it
                    open.AddNodeRecord(endNodeRecord);
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

        // Determine whether Dijkstra found a path and print it here.
        if(current.Tile != end)
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
                if(colorTiles)
                {
                    current.ColorTile(pathColor);
                    yield return new WaitForSeconds(waitTime);
                }
            }
            //The other end goes pink too
            current.ColorTile(Color.magenta);
            UnityEngine.Debug.Log("Path Length: "+ path.Count.ToString());
        }

        yield return null;
    }
}

/// <summary>
/// A class for recording search statistics.
/// </summary>
public class NodeRecord
{
    // The tile game object.
    public GameObject Tile { get; set; } = null;

    // Node interface, cost so far and a node's connection
    // Everything else is stored in Node or in the tile
    public Node node;
    public NodeRecord connection;
    public float costSoFar = 0;
    public float estimatedCost = 0;
    // Sets the tile's color.
    public void ColorTile (Color newColor)
    {
        SpriteRenderer renderer = Tile.GetComponentInChildren<SpriteRenderer>();
        renderer.material.color = newColor;
    }

    // Displays a string on the tile.
    public void Display (float value)
    {
        TextMesh text = Tile.GetComponent<TextMesh>();
        text.text = value.ToString();
    }
}