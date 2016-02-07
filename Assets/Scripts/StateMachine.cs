using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    #region Initialization

    public virtual void InitializeStateMachine ()
    {
    }

    private void Awake ()
    {
        stateSets.Add( ( int ) DefaultStates.Default , new StateSet(
            new IEnumeratorFactory( Default_Enter ) ,
            new IEnumeratorFactory( Default_Update ) ,
            new IEnumeratorFactory( Default_Exit ) )
            );
        stateSets.Add( ( int ) DefaultStates.Disabled , new StateSet(
            new IEnumeratorFactory( Disabled_Enter ) ,
            new IEnumeratorFactory( Disabled_Update ) ,
            new IEnumeratorFactory( Disabled_Exit ) )
            );
        stateSets.Add( ( int ) DefaultStates.Paused , new StateSet(
            new IEnumeratorFactory( Paused_Enter ) ,
            new IEnumeratorFactory( Paused_Update ) ,
            new IEnumeratorFactory( Paused_Exit ) )
            );
        stateSets.Add( ( int ) DefaultStates.Playback , new StateSet(
            new IEnumeratorFactory( Playback_Enter ) ,
            new IEnumeratorFactory( Playback_Update ) ,
            new IEnumeratorFactory( Playback_Exit ) )
            );
        stateSets.Add( ( int ) DefaultStates.Initialize , new StateSet(
            new IEnumeratorFactory( Initialize_Enter ) ,
            new IEnumeratorFactory( Initialize_Update ) ,
            new IEnumeratorFactory( Initialize_Exit ) )
            );
        stateSets.Add( ( int ) DefaultStates.Detonate , new StateSet(
            new IEnumeratorFactory( Detonate_Enter ) ,
            new IEnumeratorFactory( Detonate_Update ) ,
            new IEnumeratorFactory( Detonate_Exit ) )
            );
        stateSets.Add( ( int ) DefaultStates.Held , new StateSet(
            new IEnumeratorFactory( Held_Enter ) ,
            new IEnumeratorFactory( Held_Update ) ,
            new IEnumeratorFactory( Held_Exit ) )
            );
        stateSets.Add( ( int ) DefaultStates.Maintain , new StateSet(
            new IEnumeratorFactory( Maintain_Enter ) ,
            new IEnumeratorFactory( Maintain_Update ) ,
            new IEnumeratorFactory( Maintain_Exit ) )
            );
        stateSets.Add( ( int ) DefaultStates.MouseAndKeyboard , new StateSet(
            new IEnumeratorFactory( MouseAndKeyboard_Enter ) ,
            new IEnumeratorFactory( MouseAndKeyboard_Update ) ,
            new IEnumeratorFactory( MouseAndKeyboard_Exit ) )
            );
        stateSets.Add( ( int ) DefaultStates.Gamepad , new StateSet(
            new IEnumeratorFactory( Gamepad_Enter ) ,
            new IEnumeratorFactory( Gamepad_Update ) ,
            new IEnumeratorFactory( Gamepad_Exit ) )
            );
    }

    private void Start ()
    {
        InitializeStateMachine();
    }

    #endregion Initialization

    #region State machine

    #region State manipulation

    public void AddState ( int state )
    {
        if ( stateSets.ContainsKey( state ) )
        {
            if ( !_locked && !ContainsState( state ) )
            {
                currentStates.Add( state );
                StateSet stateSet = stateSets[ state ];
                StartCoroutine( StateHandler( state , stateSet.enter , stateSet.update , stateSet.exit ) );
            }
        }
    }

    public void AddStates ( int[] states )
    {
        if ( states != null )
        {
            for ( int i = 0 ; i < states.Length ; i++ )
                AddState( states[ i ] );
        }
    }

    public bool ContainsState ( int state )
    {
        return currentStates.Contains( state );
    }

    public void RemoveState ( int state )
    {
        if ( _locked && stateSets[ state ].locked )
            _locked = false;

        if ( !_locked && currentStates.Contains( state ) )
            currentStates.Remove( state );
    }

    public void RemoveStates ( int[] states )
    {
        if ( states != null )
        {
            for ( int i = 0 ; i < states.Length ; i++ )
                RemoveState( states[ i ] );
        }
    }

    public void RemoveStateByIndex ( int index )
    {
        if ( index >= 0 )
        {
            if ( _locked && stateSets[ currentStates[ index ] ].locked )
                _locked = false;

            if ( !_locked && currentStates.Count > index )
                currentStates.RemoveAt( index );
        }
    }

    public void AddState ( DefaultStates state )
    {
        AddState( ( int ) state );
    }

    public void AddStates ( DefaultStates[] states )
    {
        for ( int i = 0 ; i < states.Length ; i++ )
            AddState( ( int ) states[ i ] );
    }

    public bool ContainsState ( DefaultStates state )
    {
        return ContainsState( ( int ) state );
    }

    public void RemoveState ( DefaultStates state )
    {
        RemoveState( ( int ) state );
    }

    public void RemoveStates ( DefaultStates[] states )
    {
        for ( int i = 0 ; i < states.Length ; i++ )
            RemoveState( ( int ) states[ i ] );
    }

    public List<int> GetCurrentStates ()
    {
        return currentStates;
    }

    public int GetTopState ()
    {
        return currentStates[ stateCount - 1 ];
    }

    public int stateCount
    {
        get
        {
            return currentStates.Count;
        }
    }

    public void ClearStates ()
    {
        while ( stateCount > 0 )
            RemoveStateByIndex( 0 );
    }

    public bool IsLocked ()
    {
        return _locked;
    }

    #endregion State manipulation

    #region State processing

    private IEnumerator StateHandler ( int state , IEnumeratorFactory enter , IEnumeratorFactory update , IEnumeratorFactory exit )
    {
        IEnumerator enter_ = enter != null ? enter() : null;
        IEnumerator update_ = update != null ? update() : null;
        IEnumerator exit_ = exit != null ? exit() : null;

        if ( stateSets[ state ].locked )
            _locked = true;

        while ( enter_ != null && enter_.MoveNext() )
            yield return enter_.Current;

        while ( update_ != null && currentStates.Contains( state ) && update_.MoveNext() )
            yield return update_.Current;

        while ( currentStates.Contains( state ) )
            yield return null;

        while ( exit_ != null && exit_.MoveNext() )
            yield return exit_.Current;

        if ( stateSets[ state ].locked )
            _locked = false;
    }

    #endregion State processing

    #region Built-in transitions

    protected virtual IEnumerator Default_Enter ()
    {
        yield return null;
    }

    protected virtual IEnumerator Default_Update ()
    {
        yield return null;
    }

    protected virtual IEnumerator Default_Exit ()
    {
        yield return null;
    }

    protected virtual IEnumerator Disabled_Enter ()
    {
        yield return null;
    }

    protected virtual IEnumerator Disabled_Update ()
    {
        yield return null;
    }

    protected virtual IEnumerator Disabled_Exit ()
    {
        yield return null;
    }

    protected virtual IEnumerator Playback_Enter ()
    {
        yield return null;
    }

    protected virtual IEnumerator Playback_Update ()
    {
        yield return null;
    }

    protected virtual IEnumerator Playback_Exit ()
    {
        yield return null;
    }

    protected virtual IEnumerator Paused_Enter ()
    {
        yield return null;
    }

    protected virtual IEnumerator Paused_Update ()
    {
        yield return null;
    }

    protected virtual IEnumerator Paused_Exit ()
    {
        yield return null;
    }

    protected virtual IEnumerator Initialize_Enter ()
    {
        yield return null;
    }

    protected virtual IEnumerator Initialize_Update ()
    {
        yield return null;
    }

    protected virtual IEnumerator Initialize_Exit ()
    {
        yield return null;
    }

    protected virtual IEnumerator Detonate_Enter ()
    {
        yield return null;
    }

    protected virtual IEnumerator Detonate_Update ()
    {
        yield return null;
    }

    protected virtual IEnumerator Detonate_Exit ()
    {
        yield return null;
    }

    protected virtual IEnumerator Held_Enter ()
    {
        yield return null;
    }

    protected virtual IEnumerator Held_Update ()
    {
        yield return null;
    }

    protected virtual IEnumerator Held_Exit ()
    {
        yield return null;
    }

    protected virtual IEnumerator Maintain_Enter ()
    {
        yield return null;
    }

    protected virtual IEnumerator Maintain_Update ()
    {
        yield return null;
    }

    protected virtual IEnumerator Maintain_Exit ()
    {
        yield return null;
    }

    protected virtual IEnumerator MouseAndKeyboard_Enter ()
    {
        yield return null;
    }

    protected virtual IEnumerator MouseAndKeyboard_Update ()
    {
        yield return null;
    }

    protected virtual IEnumerator MouseAndKeyboard_Exit ()
    {
        yield return null;
    }

    protected virtual IEnumerator Gamepad_Enter ()
    {
        yield return null;
    }

    protected virtual IEnumerator Gamepad_Update ()
    {
        yield return null;
    }

    protected virtual IEnumerator Gamepad_Exit ()
    {
        yield return null;
    }

    protected virtual IEnumerator No_Op ()
    {
        yield return null;
    }

    protected static void NoOp ()
    {
    }

    #endregion Built-in transitions

    #region Classes And Structs

    public enum DefaultStates
    {
        Disabled = 0,
        Default = 1,
        Playback = 2,
        Paused = 3,
        Initialize = 4,
        Detonate = 5,
        Held = 6,
        Maintain = 7,
        MouseAndKeyboard = 8,
        Gamepad = 9
    }

    public struct StateSet
    {
        public IEnumeratorFactory enter;
        public IEnumeratorFactory update;
        public IEnumeratorFactory exit;
        public bool locked;

        public StateSet ( IEnumeratorFactory update )
        {
            this.enter = null;
            this.update = update;
            this.exit = null;
            locked = false;
        }

        public StateSet ( IEnumeratorFactory enter , IEnumeratorFactory update , IEnumeratorFactory exit )
        {
            this.enter = enter;
            this.update = update;
            this.exit = exit;
            locked = false;
        }

        public StateSet ( IEnumeratorFactory enter , IEnumeratorFactory update , IEnumeratorFactory exit , bool locked )
        {
            this.enter = enter;
            this.update = update;
            this.exit = exit;
            this.locked = locked;
        }
    }

    #endregion Classes And Structs

    #endregion State machine

    #region Getters and cached components

    public Rigidbody rigidbody
    {
        get
        {
            return _rigidbody ? _rigidbody : _rigidbody = gameObject.GetComponent<Rigidbody>();
        }
    }

    public Rigidbody2D rigidbody2D
    {
        get
        {
            if ( _rigidbody2d != null )
                return _rigidbody2d;
            else
            {
                _rigidbody2d = gameObject.GetComponent<Rigidbody2D>();
                return _rigidbody2d;
            }
        }
    }

    public LineRenderer lineRenderer
    {
        get
        {
            if ( _lineRenderer != null )
                return _lineRenderer;
            else
            {
                _lineRenderer = gameObject.GetComponent<LineRenderer>();
                return _lineRenderer;
            }
        }
        set
        {
            _lineRenderer = value;
        }
    }

    public SphereCollider sphereCollider
    {
        get
        {
            if ( _sphereCollider != null )
                return _sphereCollider;
            else
            {
                _sphereCollider = gameObject.GetComponent<SphereCollider>();
                return _sphereCollider;
            }
        }
    }

    public Renderer renderer
    {
        get
        {
            if ( _renderer != null )
                return _renderer;
            else
            {
                _renderer = gameObject.GetComponent<Renderer>();
                return _renderer;
            }
        }
    }

    public Material cMaterial
    {
        get
        {
            if ( _material == null )
                _material = renderer.material;

            return _material;
        }

        set
        {
            renderer.material = value;
        }
    }

    public BoxCollider boxCollider
    {
        get
        {
            if ( _boxCollider == null )
                _boxCollider = gameObject.GetComponent<BoxCollider>();

            return _boxCollider;
        }
    }

    public CircleCollider2D circleCollider2D
    {
        get
        {
            if ( _circleCollider2d == null )
                _circleCollider2d = gameObject.GetComponent<CircleCollider2D>();

            return _circleCollider2d;
        }
    }

    public BoxCollider2D boxCollider2D
    {
        get
        {
            if ( _boxCollider2d == null )
                _boxCollider2d = gameObject.GetComponent<BoxCollider2D>();

            return _boxCollider2d;
        }
    }

    #endregion Getters and cached components

    #region Public variables

    public delegate IEnumerator IEnumeratorFactory ();

    #endregion Public variables

    #region Protected variables

    protected Dictionary< int , StateSet > stateSets = new Dictionary< int , StateSet >();

    #endregion Protected variables

    #region Private variables

    protected List< int > currentStates = new List< int >();

    private Rigidbody _rigidbody;
    private Rigidbody2D _rigidbody2d;
    private TrailRenderer _trailRenderer;
    private LineRenderer _lineRenderer;
    private SphereCollider _sphereCollider;
    private BoxCollider _boxCollider;
    private CircleCollider2D _circleCollider2d;
    private BoxCollider2D _boxCollider2d;
    private Renderer _renderer;
    private Material _material;
    private Material _trailMaterial;
    private Rigidbody2D _rigidbody2D;

    private bool _locked = false;

    private float _trailStartWidth;
    private float _trailEndWidth;

    #endregion Private variables
}