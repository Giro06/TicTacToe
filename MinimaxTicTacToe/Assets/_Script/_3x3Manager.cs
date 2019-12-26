using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
 
public class _3x3Manager : MonoBehaviour
{
     int[,] board= new int[3,3];
    _3x3Slot[,] slots = new _3x3Slot[3, 3];
    int count=0;
    public GameObject slot;
    public int maxDepth;


    private int checkedTableCount;
    private int lookingDepth;
    public TMP_Text statisticText;
    private float timer = 0;
    private void Start()
    {
        
        Vector2 startPosition = new Vector2(-3, -3);
        for(int i = 0; i < 3; i++)
        {
            for(int y = 0; y < 3; y++)
            {
                GameObject temp= Instantiate(slot, startPosition + new Vector2(y * 3,i * 3), Quaternion.identity);
                temp.GetComponent<_3x3Slot>().SetIndex(i, y);
                slots[i,y]=temp.GetComponent<_3x3Slot>();
            }
        }
        for (int i = 0; i < 3; i++)
        {
            for (int y = 0; y < 3; y++)
            {
                board[i, y] = 0;
            }
        }

    }
    private void Update()
    {
        timer += Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Space))
        {
             ResetGame(); 
        }
    }
   public void ResetGame()
    {
        for (int i = 0; i < 3; i++)
        {
            for (int y = 0; y < 3; y++)
            {
                board[i, y] = 0;
                slots[i, y].SetShape(0);
            }
        }

        timer = 0;
        statisticText.text = "";
    }
    public void ActivateAi()
    {
        if (FindAvailablePositions(board).Count > 0)
        {
           StartCoroutine(PlayAi(board, maxDepth));
        }
      
    }
    IEnumerator PlayAi(int[,] board, int maxDepth)
    {
        yield return new WaitForSeconds(0);
        checkedTableCount = 0;
        lookingDepth = 0;
        Move playHere = GetMeBestMove(board, maxDepth, true); 
        UpdateStatisticText(checkedTableCount,lookingDepth);
        Debug.Log("Place to play :"+playHere.place + " Score of this move: " + playHere.score);
        SetBoardPiece((int)playHere.place.x, (int)playHere.place.y, 2);
        slots[(int)playHere.place.x, (int)playHere.place.y].SetShape(2);
    }
   
    Move GetMeBestMove(int[,] currentBoard,int depth,bool maximizingPlayer)
    {
        checkedTableCount++;
        Move temp = new Move();
        List<Vector2> playablePlace = FindAvailablePositions(currentBoard);
        int scoreOfCurrentBoard = CheckScore(currentBoard);
        
        if (depth == 0)
        {
            temp.score=scoreOfCurrentBoard;
            lookingDepth = depth;
            return temp;
        }
        if (playablePlace.Count == 0)
        {
            temp.score = scoreOfCurrentBoard;
            lookingDepth = depth;
            return temp;
        }
        if (scoreOfCurrentBoard == -1)
        {
            temp.score = scoreOfCurrentBoard;
            lookingDepth = depth;
            return temp;
        }
        if (scoreOfCurrentBoard == 1)
        {
            temp.score = scoreOfCurrentBoard;
            lookingDepth = depth;
            return temp;
        }
        if (maximizingPlayer)
        {
            
            Move value = new Move();
            value.score = -999;

           
          
            foreach(Vector2 x in playablePlace)
            {
                int[,] tempBoard = new int[3, 3];
                for (int i = 0; i < 3; i++)
                {
                    for (int y = 0; y < 3; y++)
                    {
                        tempBoard[i, y] = currentBoard[i, y];
                    }

                }
                tempBoard[(int)x.x, (int)x.y] = 2;
                Move aTemp=GetMeBestMove(tempBoard, depth - 1, false);
                if (value.score <= aTemp.score)
                {
                    aTemp.place = x;
                    value = aTemp;
                }
             
            }
            return value;
          
        }
        else
        {   
            Move value = new Move();
            value.score = 999;
            foreach (Vector2 x in playablePlace)
            {
                int[,] tempBoard = new int[3, 3];
                for (int i = 0; i < 3; i++)
                {
                    for (int y = 0; y < 3; y++)
                    {
                        tempBoard[i, y] = currentBoard[i, y];
                    }

                }
                tempBoard[(int)x.x, (int)x.y] = 1;
                Move aTemp = GetMeBestMove(tempBoard, depth - 1, true);
               
                if (value.score >= aTemp.score)
                {
                    aTemp.place = x;
                    value = aTemp;
                }
               
            }
            return value;
        }
    }
    public void SetBoardPiece(int x, int y,int code)
    {
        board[x, y]=code;
    }
    List<Vector2> FindAvailablePositions(int[,] stateOfboard)
    {
        List<Vector2> temp = new List<Vector2>();
        for(int x = 0; x < 3; x++)
        {
            for(int y = 0; y < 3; y++)
            {
                if (stateOfboard[x, y] == 0)
                {
                    temp.Add(new Vector2(x, y));
                }
            }
        }
      
        return temp;
    }

    int CheckScore(int[,] board)
    {   for (int x = 1; x < 2; x++)
        {
            for (int y = 1; y < 2; y++)
            {
                int centerX = x;
                int centerY = y;
                //Dikey kontroller---------------------------------------------------------
                if (board[centerX - 1, centerY + 1] == board[centerX - 1, centerY] &&
                    board[centerX - 1, centerY - 1] == board[centerX - 1, centerY + 1] &&
                    board[centerX - 1, centerY + 1] != 0)
                {
                    if (board[centerX - 1, centerY + 1] == 1)
                    {
                        return -1;
                    }
                    else
                    { 
                        return 1;
                    }
                }

                if (board[centerX, centerY + 1] == board[centerX, centerY] &&
                    board[centerX, centerY] == board[centerX, centerY - 1] &&
                    board[centerX, centerY] != 0)
                {
                    if (board[centerX, centerY] == 1)
                    {
                        return -1;
                    }
                    else
                    {
                        return 1;
                    }
                }

                if (board[centerX + 1, centerY + 1] == board[centerX + 1, centerY] &&
                    board[centerX + 1, centerY] == board[centerX + 1, centerY - 1] &&
                    board[centerX + 1, centerY] != 0)
                {
                    if (board[centerX + 1, centerY] == 1)
                    {
                        return -1;
                    }
                    else
                    {
                        return 1;
                    }
                }

                //---------------------------------------------------------------------
                //Yatay kontroller-----------------------------------------------------
                if (board[centerX - 1, centerY] == board[centerX, centerY] &&
                    board[centerX, centerY] == board[centerX + 1, centerY] &&
                    board[centerX, centerY] != 0)
                {
                    if (board[centerX, centerY] == 1)
                    {
                        return -1;
                    }
                    else
                    {
                        return 1;
                    }
                }

                if (board[centerX - 1, centerY + 1] == board[centerX, centerY + 1] &&
                    board[centerX, centerY + 1] == board[centerX + 1, centerY + 1] &&
                    board[centerX, centerY + 1] != 0)
                {
                    if (board[centerX, centerY + 1] == 1)
                    {
                        return -1;
                    }
                    else
                    {
                        return 1;
                    }
                }

                if (board[centerX - 1, centerY - 1] == board[centerX, centerY - 1] &&
                    board[centerX, centerY - 1] == board[centerX + 1, centerY - 1] &&
                    board[centerX, centerY - 1] != 0)
                {
                    if (board[centerX, centerY - 1] == 1)
                    {
                        return -1;
                    }
                    else
                    {
                        return 1;
                    }
                }

                //--------------------------------------------------------------------
                //Carpraz Kontroller
                if (board[centerX - 1, centerY + 1] == board[centerX, centerY] &&
                    board[centerX, centerY] == board[centerX + 1, centerY - 1] &&
                    board[centerX, centerY] != 0)
                {
                    if (board[centerX, centerY] == 1)
                    {
                        return -1;
                    }
                    else
                    {
                        return 1;
                    }
                }

                if (board[centerX + 1, centerY + 1] == board[centerX, centerY] &&
                    board[centerX, centerY] == board[centerX - 1, centerY - 1] &&
                    board[centerX, centerY] != 0)
                {
                    if (board[centerX, centerY] == 1)
                    {
                        return -1;
                    }
                    else
                    {
                        return 1;
                    }
                }
            }
        }
        return 0;
    }

    void UpdateStatisticText(int checkCount,int lookDepth)
    {   
        float endTime = timer;
        statisticText.text = checkCount.ToString() + " move checked \n" + " Ai look at :" + (maxDepth - lookDepth).ToString()+" depth\n";
    }
}
