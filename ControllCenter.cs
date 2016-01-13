using UnityEngine;
using System.Collections;
public class ControllCenter : MonoBehaviour
{
    public GameObject LoseObj;
    public GameObject WinObj;
    public UINumberManager mUINumberManager;
    public UIScore mUIScore;
    public GameData mGameData;
    private bool canControll = false;
    private bool isGameOver = false;
    // Use this for initialization
    public IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
        NewGame();
    }

    public void NewGame()
    {
        isGameOver = false;
        LoseObj.SetActive(false);
        WinObj.SetActive(false);
        mGameData = new GameData();
        mGameData.Init();
        mUIScore.SetScore(mGameData.CurScore, mGameData.BestScore);
        mUINumberManager.InitNumbers();
        for (int i = 0; i < GameData.StartShowNumberCount; i++)
            mUINumberManager.ShowNextNumber();
        canControll = true;
    }

    #region 移动
    void Update()
    {
        if (!isGameOver&&canControll)
        {
            KeyBoardUpdate();
            TouchUpdate();
        }
    }
    void KeyBoardUpdate()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            UpMove();
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            LeftMove();
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            RightMove();
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            DownMove();
        }
    }
    void TouchUpdate()
    {
        if(Input.touchCount>0&&Input.touchCount<2)
        {
            Touch  touch= Input.GetTouch(0);

            Vector2 deltatouch = touch.deltaPosition;
            if (deltatouch.magnitude /mGameData.ScreenDpi > GameData.DeltaDrag)
            {
                if (Mathf.Abs(deltatouch.x) > Mathf.Abs(deltatouch.y))
                {
                    if (deltatouch.x > 0)
                    {
                        RightMove();
                    }
                    else
                    {
                        LeftMove();
                    }
                }
                else
                    if (Mathf.Abs(deltatouch.x) < Mathf.Abs(deltatouch.y))
                    {
                        if (deltatouch.y > 0)
                        {
                            UpMove();
                        }
                        else
                        {
                            DownMove();
                        }
                    }
            }
            

        }
    }

    public void UpMove()
    {
        StartMove(MoveNormal.Up);
    }
    public void DownMove()
    {
        StartMove(MoveNormal.Down);
    }
    public void RightMove()
    {
        StartMove(MoveNormal.Right);
    }
    public void LeftMove()
    {
        StartMove(MoveNormal.Left);
    }

    private void StartMove(MoveNormal nor)
    {
        canControll = false;
        //Debug.Log("Move "+nor.ToString());
        if (mUINumberManager.Move(nor))
            StartCoroutine(Moving());
        else
            canControll = true;
    }
    private IEnumerator Moving()
    {
        canControll = false;
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(GameData.MoveTime);
        yield return new WaitForEndOfFrame();

        mGameData.CurScore += mUINumberManager.CurMergeScore;
        mUIScore.ShowAddScore(mUINumberManager.CurMergeScore);
        mUIScore.SetScore(mGameData.CurScore, mGameData.BestScore);
        if (mUINumberManager.CurMergeMaxNumber >= GameData.MaxNum)
        {
            yield return new WaitForSeconds(1f);
            WinObj.SetActive(true);
            isGameOver = true;
            yield break;
        }
        mUINumberManager.ShowNextNumber();
        if (mUINumberManager.OnMapNumbers.Count==GameData.MapSize*GameData.MapSize&&!mUINumberManager.CheckCanMove())
        {
            yield return new WaitForSeconds(1f);
            LoseObj.SetActive(true);
            isGameOver = true;
            yield break;
        }
        canControll = true;
    }



    #endregion
}
