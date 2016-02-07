using System.Collections;
using UnityEngine;

public class Button : UIBase
{
    #region Initialize

    protected override void InitializeUI ()
    {
        stateSets.Add( ( int ) UIStates.Click , new StateSet( new IEnumeratorFactory( Click_Enter ) , new IEnumeratorFactory( Click_Update ) , new IEnumeratorFactory( Click_Exit ) ) );
        stateSets.Add( ( int ) UIStates.Hover , new StateSet( new IEnumeratorFactory( Hover_Enter ) , new IEnumeratorFactory( Hover_Update ) , new IEnumeratorFactory( Hover_Exit ) ) );
        InitializeButton();
    }

    protected virtual void InitializeButton ()
    {
    }

    #endregion Initialize

    #region Hover

    protected virtual IEnumerator Hover_Enter ()
    {
        yield return null;
    }

    protected virtual IEnumerator Hover_Update ()
    {
        yield return null;
    }

    protected virtual IEnumerator Hover_Exit ()
    {
        yield return null;
    }

    #endregion Hover

    #region Click

    protected virtual IEnumerator Click_Enter ()
    {
        yield return null;
    }

    protected virtual IEnumerator Click_Update ()
    {
        yield return null;
    }

    protected virtual IEnumerator Click_Exit ()
    {
        yield return null;
    }

    #endregion Click

    #region Variables

    public enum ButtonType
    {
        Play = 0,
        Modes = 1,
        Options = 3,
        Discover = 4,
        Practice = 5,
        Perform = 6,
        Discover_Option = 7,
        Practice_Option = 8,
        Perform_Option = 9,
    }

    public ButtonType buttonType;

    [ HideInInspector ]
    public Vector3 initialScale;

    [ HideInInspector ]
    public Vector3 initialPosition = Vector3.zero;

    [ HideInInspector ]
    public Quaternion initialRotation = Quaternion.identity;

    #endregion Variables
}