using UnityEngine;

public class ColliderProbe : MonoBehaviour
{
    private void OnCollisionEnter2D ( Collision2D collision )
    { actor.CollisionEnter2D( collision ); }

    private void OnCollisionStay2D ( Collision2D collision )
    { actor.CollisionStay2D( collision ); }

    private void OnCollisionExit2D ( Collision2D collision )
    { actor.CollisionExit2D( collision ); }

    private void OnTriggerEnter2D ( Collider2D collider )
    { actor.TriggerEnter2D( collider ); }

    private void OnTriggerStay2D ( Collider2D collider )
    { actor.TriggerStay2D( collider ); }

    private void OnTriggerExit2D ( Collider2D collider )
    { actor.TriggerExit2D( collider ); }

    [SerializeField]
    private Actor actor;
}