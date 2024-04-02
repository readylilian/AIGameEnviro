using UnityEngine;
using RPS;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;

/// <summary>
/// This class receives input from the player, queries the AI for predictions, and updates the total wins/losses.
/// </summary>
public class PlayerInterface : MonoBehaviour
{
    [SerializeField]
    TMP_Text resultText;
    // Records the input from the user as a char: 'r', 'p', or 's'
    private char input = '0';

    // Records the number of times the player has won.
    private int playerWins = 0;

    // Records the number of times the AI has won.
    private int aiWins = 0;

    List<RPSMove> sequence = new List<RPSMove>();

    class KeyDataRecord
    {
        public Hashtable counts = new Hashtable();
        public int total = 0;

        public KeyDataRecord() 
        {
            counts.Add(RPSMove.Rock, 0);
            counts.Add(RPSMove.Paper, 0);
            counts.Add(RPSMove.Scissors, 0);
        }
    }
    static class NGramPredictor
    {
        private static Hashtable data = new Hashtable();
        //nValue = window + 1
        public static int nValue = 4;

        public static void registerSequence(List<RPSMove> actions)
        {
            //We don't want an inclusive range, because the value is the last action
            //We want this to be the size of the window
            RPSMove[] key = actions.GetRange(0,nValue - 1).ToArray();
            //I changed it to a string since array comparison is by reference, so wasn't working
            string keyString = "";
            for (int i = 0; i < key.Length; i++)
            {
                keyString += key[i].ToString();
            }
            //Then the last action is the value
            RPSMove value = actions[nValue - 1];
            KeyDataRecord keyData;
            if (!data.Contains(keyString))
            {
                data.Add(keyString, new KeyDataRecord());
            }
            keyData = (KeyDataRecord)data[keyString];
            keyData.counts[value] = (int)keyData.counts[value] + 1;
            keyData.total += 1;

        }

        public static RPSMove getMostLikely(RPSMove[] actions)
        {
            //Same reason as above, it's easier to compare strings than arrays
            string key = "";
            for (int i = 0; i < actions.Length; i++)
            {
                key += actions[i].ToString();
            }
            KeyDataRecord keyData = data[key] as KeyDataRecord;
            int highestValue = 0;
            RPSMove bestAction = 0;
            
            //if there's no data choose randomly
            if (keyData == null)
            {
                return RockPaperScissors.CharToMove(RockPaperScissors.RandomMove());
            }

            //Otherwise get the best action
            RPSMove[] actionArr = new RPSMove[keyData.counts.Keys.Count];
            keyData.counts.Keys.CopyTo(actionArr, 0);
            foreach (RPSMove action in actionArr)
            {
                if ((int)keyData.counts[action] > highestValue)
                {
                    highestValue = (int)keyData.counts[action];
                    bestAction = action;
                }
            }
            return bestAction;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the mouse button was pressed.
        if (Input.GetMouseButtonDown(0))
        {
            // Grab the position that was clicked by the mouse.
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

            // Use a raycast to determine whether a tile was clicked.
            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);

            // If a tile with a collider was clicked...
            if (hit.collider != null)
            {
                // Begin an output string we will print to the log.
                string output = "You selected: " + hit.collider.gameObject.name;

                // Convert the collider clicked by the user to the r/p/s character in the input variable.
                if (hit.collider.gameObject.name == "Rock")
                {
                    input = 'r';
                    sequence.Add(RPSMove.Rock);
                }
                else if (hit.collider.gameObject.name == "Paper")
                {
                    input = 'p';
                    sequence.Add(RPSMove.Paper);
                }
                else if (hit.collider.gameObject.name == "Scissors")
                {
                    input = 's';
                    sequence.Add(RPSMove.Scissors);
                }
                else return;
                //if our sequence is larger than our window, remove the first in the sequence
                if(sequence.Count > NGramPredictor.nValue)
                {
                    sequence.RemoveAt(0);
                }
                //Default to a random move
                RPSMove predMove = RockPaperScissors.CharToMove(RockPaperScissors.RandomMove());
                //Once we have enough moves for our sequence start registering them and predicting moves
                if (sequence.Count == NGramPredictor.nValue)
                {
                    NGramPredictor.registerSequence(sequence);
                    //Then use the previous window of moves to predict the next
                    predMove = NGramPredictor.getMostLikely(sequence.GetRange(0, NGramPredictor.nValue - 1).ToArray());
                }
                // Ask the ngram AI to predict what the player will choose..
                // You will need to implement this code and any history tracking it requires.
                // For now, we will predict a move at random.
                output += "\nThe NGram AI predicts you will play: " + predMove;

                // Given the predicted user move, choose the move that will win against it.
                RPSMove aiMove = RockPaperScissors.GetWinner(predMove);
                output += "\nThe NGram AI plays: " + aiMove;

                // Get the result of playing the user and AI moves.
                int result = RockPaperScissors.Play(RockPaperScissors.CharToMove(input), aiMove);

                // If the result is 1, the player wins.
                if (result > 0)
                {
                    output += "\nYou win!";
                    playerWins++;
                }
                // If the result is -1, the AI wins.
                else if (result < 0)
                {
                    output += "\nYou lose...";
                    aiWins++;
                }
                // If the result is 0, there is a tie.
                else output += "\nTie";
                
                // Print the total wins to the log.
                output += "\nPlayer Wins: " + playerWins;
                output += "\nAI Wins: " + aiWins;

                // Output the combined output string to the log.
                Debug.Log(output);
                resultText.text = output;
            }
        }
    }
}
