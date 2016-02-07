using UnityEngine;

public class UIBase : StateMachine
{
    #region Initialization

    public override void InitializeStateMachine ()
    {
        InitializeUI();
    }

    protected virtual void InitializeUI ()
    {
    }

    #endregion Initialization

    #region State manipulation

    public void AddState ( UIStates newState )
    {
        if ( gameObject.activeInHierarchy )
            AddState( ( int ) newState );
    }

    public void AddStates ( UIStates[] newStates )
    {
        for ( int i = 0 ; i < newStates.Length ; i++ )
            AddState( ( int ) newStates[ i ] );
    }

    public bool ContainsState ( UIStates state )
    {
        return ContainsState( ( int ) state );
    }

    public void RemoveState ( UIStates state )
    {
        RemoveState( ( int ) state );
    }

    public void RemoveStates ( UIStates[] newStates )
    {
        for ( int i = 0 ; i < newStates.Length ; i++ )
            RemoveState( ( int ) newStates[ i ] );
    }

    #endregion State manipulation

    #region Utilities

    protected Vector2 LineIntersectionPoint ( Vector2 positionA , Vector2 directionA , Vector2 positionB , Vector2 directionB )
    {
        // Get A,B,C of first line - points : ps1 to pe1
        float a1 = directionA.y - positionA.y;
        float b1 = positionA.x - directionA.x;
        float c1 = a1 * positionA.x + b1 * positionA.y;

        // Get A,B,C of second line - points : ps2 to pe2
        float a2 = directionB.y - positionB.y;
        float b2 = positionB.x - directionB.x;
        float c2 = a2 * positionB.x + b2 * positionB.y;

        // Get delta and check if the lines are parallel
        float delta = a1 * b2 - a2 * b1;

        // now return the Vector2 intersection point
        if ( delta != 0 )
            return new Vector2(
            ( b2 * c1 - b1 * c2 ) / delta ,
            ( a1 * c2 - a2 * c1 ) / delta
            );
        //unless parallel
        else
            return Vector2.zero;
    }

    #endregion Utilities

    #region Variables

    public enum UIStates
    {
        Title = 10,
        Modes = 11,
        Hover = 12,
        Click = 13,
        Left_Prompt = 14,
        Right_Prompt = 15,
        Modes_SubMenu = 16,
        Multiplier = 17,
        Weapon = 18,
        Chain = 19,
        New_Goal = 20,
        Completed_Goal = 21,
        Pause = 22,
        Start_Cycle = 23,
        Sweep = 24
    }

    #endregion Variables
}