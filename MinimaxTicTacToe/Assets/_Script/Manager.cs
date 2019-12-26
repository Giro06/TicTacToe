using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class Move
{
   public Vector2 place;
   public int score;
   public int depth;
}

public class Manager : MonoBehaviour
{
    int[,] board = new int[5, 5];
    Slot[,] slots = new Slot[5, 5];
    int count = 0;
    public GameObject slot;
    public int maxDepth;
    public int maxSearchCount;

    private int checkedTableCount;
    private int lookingDepth;
    public TMP_Text statisticText;
    private float timer = 0;

    private bool gameOver;

    private LineRenderer _lineRenderer;

    public GameObject win;
    public GameObject lose;
    public GameObject draw;
    private void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        Vector2 startPosition = new Vector2(-6, -6);
        for (int i = 0; i < 5; i++)
        {
            for (int y = 0; y < 5; y++)
            {
                GameObject temp = Instantiate(slot, startPosition + new Vector2(y * 3, i * 3), Quaternion.identity);
                temp.GetComponent<Slot>().SetIndex(i, y);
                slots[i, y] = temp.GetComponent<Slot>();
            }
        }

        for (int i = 0; i < 5; i++)
        {
            for (int y = 0; y < 5; y++)
            {
                board[i, y] = 0;
            }
        }
        win.SetActive(false);
        lose.SetActive(false);
        draw.SetActive(false);
        _lineRenderer.enabled = false;
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
        for (int i = 0; i < 5; i++)
        {
            for (int y = 0; y < 5; y++)
            {
                board[i, y] = 0;
                slots[i, y].SetShape(0);
            }
        }
        foreach (Slot x in slots)
        {
            x.GetComponent<BoxCollider2D>().enabled = true;
        }
        win.SetActive(false);
        lose.SetActive(false);
        draw.SetActive(false);
        _lineRenderer.enabled = false;
        _lineRenderer.positionCount = 0;
        timer = 0;
        statisticText.text = "";
    }

    public void ActivateAi()
    {
        int i = CheckScore(board);
        if (FindAvailablePositions(board).Count > 0 && i == 0)
        {
            StartCoroutine(PlayAi(board, maxDepth));
        }
        else
        {
            GameOver();

        }

    }

    IEnumerator PlayAi(int[,] board, int maxDepth)
    {
        yield return new WaitForSeconds(0);
        checkedTableCount = 0;
        lookingDepth = 0;
        Move playHere = GetMeBestMove(board, maxDepth, true);
        UpdateStatisticText(checkedTableCount, lookingDepth);
        Debug.Log("Place to play :" + playHere.place + " Score of this move: " + playHere.score);
        SetBoardPiece((int) playHere.place.x, (int) playHere.place.y, 2);
        slots[(int) playHere.place.x, (int) playHere.place.y].SetShape(2);
        int i = CheckScore(board);
        if (i != 0)
        {
            GameOver();
        }
    }

    Move GetMeBestMove(int[,] currentBoard, int depth, bool maximizingPlayer)
    {
        checkedTableCount++;
        Move temp = new Move();
        temp.depth = depth;
        int scoreOfCurrentBoard = CheckScore(currentBoard);
        if (depth == 0)
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

        List<Vector2> playablePlace = FindAvailablePositions(currentBoard);
        if (playablePlace.Count == 0)
        {
            temp.score = scoreOfCurrentBoard;
            lookingDepth = depth;
            return temp;
        }

        if (maximizingPlayer) //Play for ai
        {

            Move value = new Move();
            value.score = -999;
            foreach (Vector2 x in playablePlace)
            {
                int[,] tempBoard = new int[5, 5];
                for (int i = 0; i < 5; i++)
                {
                    for (int y = 0; y < 5; y++)
                    {
                        tempBoard[i, y] = currentBoard[i, y];
                    }

                }

                tempBoard[(int) x.x, (int) x.y] = 2;
                Move aTemp = GetMeBestMove(tempBoard, depth - 1, false);

                if (value.score <= aTemp.score)
                {
                    if (value.score < aTemp.score)
                    {
                        aTemp.place = x;
                        value = aTemp;

                    }
                    else if (value.score == aTemp.score)
                    {
                        if (value.depth < aTemp.depth)
                        {
                            aTemp.place = x;
                            value = aTemp;

                        }
                    }

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
                int[,] tempBoard = new int[5, 5];
                for (int i = 0; i < 5; i++)
                {
                    for (int y = 0; y < 5; y++)
                    {
                        tempBoard[i, y] = currentBoard[i, y];
                    }

                }

                tempBoard[(int) x.x, (int) x.y] = 1;
                Move aTemp = GetMeBestMove(tempBoard, depth - 1, true);
                if (value.score >= aTemp.score)
                {
                    if (value.score > aTemp.score)
                    {
                        aTemp.place = x;
                        value = aTemp;

                    }
                    else if (value.score == aTemp.score)
                    {
                        if (value.depth < aTemp.depth)
                        {
                            aTemp.place = x;
                            value = aTemp;
                        }
                    }

                }


            }

            return value;
        }
    }

    public void SetBoardPiece(int x, int y, int code)
    {
        board[x, y] = code;
    }

    List<Vector2> FindAvailablePositions(int[,] stateOfboard)
    {
        List<Vector2> temp = new List<Vector2>();
        for (int x = 0; x < 5; x++)
        {
            for (int y = 0; y < 5; y++)
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
    {
        if (board[0, 3] == board[1, 2] && board[1, 2] == board[2, 1] && board[2, 1] == board[3, 0] && board[0, 3] != 0)
        {
            if (board[0, 3] == 1)
            {
                return -1;
            }

            if (board[0, 3] == 2)
            {
                return 1;
            }
        }

        if (board[1, 0] == board[2, 1] && board[2, 1] == board[3, 2] && board[3, 2] == board[4, 3] && board[1, 0] != 0)
        {
            if (board[1, 0] == 1)
            {
                return -1;
            }

            if (board[1, 0] == 2)
            {
                return 1;
            }
        }

        if (board[0, 1] == board[1, 2] && board[1, 2] == board[2, 3] && board[2, 3] == board[3, 4] && board[0, 1] != 0)
        {
            if (board[0, 1] == 1)
            {
                return -1;
            }

            if (board[0, 1] == 2)
            {
                return 1;
            }
        }

        if (board[1, 4] == board[2, 3] && board[2, 3] == board[3, 2] && board[3, 2] == board[4, 1] && board[1, 4] != 0)
        {
            if (board[1, 4] == 1)
            {
                return -1;
            }

            if (board[1, 4] == 2)
            {
                return 1;
            }
        }

        if (board[1, 1] == board[2, 2] && board[2, 2] == board[3, 3] && board[3, 3] == board[4, 4] && board[1, 1] != 0)
        {
            if (board[1, 1] == 1)
            {
                return -1;
            }

            if (board[1, 1] == 2)
            {
                return 1;
            }
        }

        if (board[0, 0] == board[1, 1] && board[1, 1] == board[2, 2] && board[2, 2] == board[3, 3] && board[0, 0] != 0)
        {
            if (board[0, 0] == 1)
            {
                return -1;
            }

            if (board[0, 0] == 2)
            {
                return 1;
            }
        }

        if (board[0, 4] == board[1, 3] && board[1, 3] == board[2, 2] && board[2, 2] == board[3, 1] && board[0, 4] != 0)
        {
            if (board[0, 4] == 1)
            {
                return -1;
            }

            if (board[0, 4] == 2)
            {
                return 1;
            }
        }

        if (board[1, 3] == board[2, 2] && board[2, 2] == board[3, 1] && board[3, 1] == board[4, 0] && board[1, 3] != 0)
        {
            if (board[1, 3] == 1)
            {
                return -1;
            }

            if (board[1, 3] == 2)
            {
                return 1;
            }
        }

        //Dikey check
        for (int x = 0; x < 5; x++)
        {
            if (board[x, 0] == board[x, 1] && board[x, 1] == board[x, 2] && board[x, 2] == board[x, 3] &&
                board[x, 0] != 0)
            {
                if (board[x, 0] == 1)
                {
                    return -1;
                }

                if (board[x, 0] == 2)
                {
                    return 1;
                }
            }

            if (board[x, 4] == board[x, 3] && board[x, 3] == board[x, 2] && board[x, 2] == board[x, 1] &&
                board[x, 4] != 0)
            {
                if (board[x, 4] == 1)
                {
                    return -1;
                }

                if (board[x, 4] == 2)
                {
                    return 1;
                }
            }
        }

        for (int y = 0; y < 5; y++)
        {
            if (board[0, y] == board[1, y] && board[1, y] == board[2, y] && board[2, y] == board[3, y] &&
                board[0, y] != 0)
            {
                if (board[0, y] == 1)
                {
                    return -1;
                }

                if (board[0, y] == 2)
                {
                    return 1;
                }
            }

            if (board[4, y] == board[3, y] && board[3, y] == board[2, y] && board[2, y] == board[1, y] &&
                board[4, y] != 0)
            {
                if (board[4, y] == 1)
                {
                    return -1;
                }

                if (board[4, y] == 2)
                {
                    return 1;
                }
            }
        }


        return 0;
    }

    void UpdateStatisticText(int checkCount, int lookDepth)
    {
        float endTime = timer;
        statisticText.text = checkCount.ToString() + " move checked \n" + " Ai look at :" +
                             (maxDepth - lookDepth).ToString() + " depth\n";
    }

    void GameOver()
    {
        int score = CheckScore(board);
        DrawLine(board);
        gameOver = true;
        if (score == 1)
        {
            lose.SetActive(true);
        }
        else if(score == -1)
        {
            win.SetActive(true);
        }
        else
        {
            draw.SetActive(true);
        }
        foreach (Slot x in slots)
        {
            x.GetComponent<BoxCollider2D>().enabled = false;
        }
        
    }

    void DrawLine(int[,] board)
    {  _lineRenderer.enabled = true;
        _lineRenderer.positionCount = 2;
        if (board[0, 3] == board[1, 2] && board[1, 2] == board[2, 1] && board[2, 1] == board[3, 0] && board[0, 3] != 0)
        {
           _lineRenderer.SetPosition(0,slots[0,3].transform.position);  
           _lineRenderer.SetPosition(1,slots[3,0].transform.position);  
        }

        if (board[1, 0] == board[2, 1] && board[2, 1] == board[3, 2] && board[3, 2] == board[4, 3] && board[1, 0] != 0)
        {
            _lineRenderer.SetPosition(0,slots[1,0].transform.position);  
            _lineRenderer.SetPosition(1,slots[4,3].transform.position);  
        }

        if (board[0, 1] == board[1, 2] && board[1, 2] == board[2, 3] && board[2, 3] == board[3, 4] && board[0, 1] != 0)
        {
            _lineRenderer.SetPosition(0,slots[0,1].transform.position);  
            _lineRenderer.SetPosition(1,slots[3,4].transform.position);  
        }

        if (board[1, 4] == board[2, 3] && board[2, 3] == board[3, 2] && board[3, 2] == board[4, 1] && board[1, 4] != 0)
        {
            _lineRenderer.SetPosition(0,slots[1,4].transform.position);  
            _lineRenderer.SetPosition(1,slots[4,1].transform.position);  
        }

        if (board[1, 1] == board[2, 2] && board[2, 2] == board[3, 3] && board[3, 3] == board[4, 4] && board[1, 1] != 0)
        {
            _lineRenderer.SetPosition(0,slots[1,1].transform.position);  
            _lineRenderer.SetPosition(1,slots[4,4].transform.position);  
        }

        if (board[0, 0] == board[1, 1] && board[1, 1] == board[2, 2] && board[2, 2] == board[3, 3] && board[0, 0] != 0)
        {
            _lineRenderer.SetPosition(0,slots[0,0].transform.position);  
            _lineRenderer.SetPosition(1,slots[3,3].transform.position);  
        }

        if (board[0, 4] == board[1, 3] && board[1, 3] == board[2, 2] && board[2, 2] == board[3, 1] && board[0, 4] != 0)
        {
            _lineRenderer.SetPosition(0,slots[0,4].transform.position);  
            _lineRenderer.SetPosition(1,slots[3,1].transform.position);  
        }

        if (board[1, 3] == board[2, 2] && board[2, 2] == board[3, 1] && board[3, 1] == board[4, 0] && board[1, 3] != 0)
        {
            _lineRenderer.SetPosition(0,slots[1,3].transform.position);  
            _lineRenderer.SetPosition(1,slots[4,0].transform.position);  
        }

            //Dikey check
            for (int x = 0; x < 5; x++)
            {
                if (board[x, 0] == board[x, 1] && board[x, 1] == board[x, 2] && board[x, 2] == board[x, 3] &&
                    board[x, 0] != 0)
                {
                    _lineRenderer.SetPosition(0,slots[x,0].transform.position);  
                    _lineRenderer.SetPosition(1,slots[x,3].transform.position);  
                }

                if (board[x, 4] == board[x, 3] && board[x, 3] == board[x, 2] && board[x, 2] == board[x, 1] &&
                    board[x, 4] != 0)
                {
                    _lineRenderer.SetPosition(0,slots[x,4].transform.position);  
                    _lineRenderer.SetPosition(1,slots[x,1].transform.position);  
                }
            }

            for (int y = 0; y < 5; y++)
            {
                if (board[0, y] == board[1, y] && board[1, y] == board[2, y] && board[2, y] == board[3, y] &&
                    board[0, y] != 0)
                {
                    _lineRenderer.SetPosition(0,slots[0,y].transform.position);  
                    _lineRenderer.SetPosition(1,slots[3,y].transform.position);  
                }

                if (board[4, y] == board[3, y] && board[3, y] == board[2, y] && board[2, y] == board[1, y] &&
                    board[4, y] != 0)
                {
                    _lineRenderer.SetPosition(0,slots[4,y].transform.position);  
                    _lineRenderer.SetPosition(1,slots[1,y].transform.position);  
                }
            }
    } 
}

