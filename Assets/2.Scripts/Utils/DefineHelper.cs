using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefineHelper : MonoBehaviour
{
    #region [Manager Info]
    public enum _eSceneIndex
    {
        none,
        MainScene,
        IngameScene
    }

    public enum eIngameState
    {
        none = 0,
        COUNT,
        PLAY,
        END,
        RESULT,

        state_Count
    }
    #endregion

    #region [UI State Info]


    public enum eBgmType
    {
        Main = 0,
        Ingame,

        max_count
    }
    public enum eFxType
    {
        Click_tock = 0,
        Counting_Nor,
        Failed,
        Answer,
        StoneBreak,
        Swap,
        TreeBreak
    }

    public enum eUIblocktype
    {
        RedCandy = 0,
        BlueCandy,
        YellowCandy,
        GreenCandy,
        Cookie,
        SweetStar,
        Hole,

        max_count
    }

    public enum eUIRowblocktype
    {
        RedRowCandy = 0,
        BlueRowCandy,
        YellowRowCandy,
        GreenRowCandy,

        max_count
    }
    public enum eUIColblocktype
    {
        RedColCandy = 0,
        BlueColCandy,
        YellowColCandy,
        GreenColCandy,

        max_count
    }

    public enum eUIBoomblocktype
    {
        RedBoomCandy = 0,
        BlueBoomCandy,
        YellowBoomCandy,
        GreenBoomCandy,

        max_count
    }
    public enum eUIwindowtype
    {
        LoaddingWnd = 0,
        ResultWnd,
        OptionWnd,

        max_count
    }
    public enum eDirection
    {
        None = 0,
        Left,
        Right,
        Up,
        Down,

        max_count
    }
    #endregion
}
