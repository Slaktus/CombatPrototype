using System.Collections;
using UnityEngine;

public class Actor : StateMachine
{
    #region Initialization

    private void Start ()
    {
        Initialize();
        //Overrides StateMachine
    }

    public virtual void Initialize ()
    {
    }

    public virtual void Terminate ()
    {
    }

    #endregion Initialization

    #region Updates

    private void LateUpdate ()
    {
        //This WAS overridden in BeamProjectile
        //Playback();
    }

    #endregion Updates

    #region Collision detection

    public void CollisionEnter2D ( Collision2D collision )
    {
        StartCoroutine( CollisionEnter2D_Coroutine( collision ) );
    }

    public void CollisionStay2D ( Collision2D collision )
    {
        StartCoroutine( CollisionStay2D_Coroutine( collision ) );
    }

    public void CollisionExit2D ( Collision2D collision )
    {
        StartCoroutine( CollisionExit2D_Coroutine( collision ) );
    }

    public void TriggerEnter2D ( Collider2D collider )
    {
        StartCoroutine( TriggerEnter2D_Coroutine( collider ) );
    }

    public void TriggerStay2D ( Collider2D collider )
    {
        StartCoroutine( TriggerStay2D_Coroutine( collider ) );
    }

    public void TriggerExit2D ( Collider2D collider )
    {
        StartCoroutine( TriggerExit2D_Coroutine( collider ) );
    }

    protected virtual IEnumerator CollisionEnter2D_Coroutine ( Collision2D collision )
    {
        yield return null;
    }

    protected virtual IEnumerator CollisionStay2D_Coroutine ( Collision2D collision )
    {
        yield return null;
    }

    protected virtual IEnumerator CollisionExit2D_Coroutine ( Collision2D collision )
    {
        yield return null;
    }

    protected virtual IEnumerator TriggerEnter2D_Coroutine ( Collider2D collider )
    {
        yield return null;
    }

    protected virtual IEnumerator TriggerStay2D_Coroutine ( Collider2D collider )
    {
        yield return null;
    }

    protected virtual IEnumerator TriggerExit2D_Coroutine ( Collider2D collider )
    {
        yield return null;
    }

    #endregion Collision detection

    #region Public variables

    [ Header("Actor Type") ]
    [ Header("ACTOR VARIABLES") ]
    [ Header("Trail Settings") ]
    public bool hasTrail;

    public bool useBezierTrail = true;

    [ Header("Trail Timing") ]
    public bool waitForTrail;

    [ Header("Actor Renderers") ]
    public new LineRenderer lineRenderer;

    [ Header("Actor Circle Collider 2D") ]
    public new CircleCollider2D circleCollider2D;

    [ Header("Collider Probes") ]
    public ColliderProbe innerProbe;

    public ColliderProbe outerProbe;

    [ HideInInspector ]
    public bool despawning = false;

    [ HideInInspector ]
    public float prevSize = 0f;

    public float size { get; set; }

    #endregion Public variables

    #region Protected variables

    protected Vector2 accumulatedVelocity;
    protected Vector2 accumulatedForce;
    protected Vector2 accumulatedImpulse;
    protected float trailRendererPauseTime = 0f;

    #endregion Protected variables
}