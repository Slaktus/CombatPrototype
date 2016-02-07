using System.Collections;
using UnityEngine;

public class CombatAvatar : Actor
{
    public override void Initialize ()
    {
        AddState( DefaultStates.MouseAndKeyboard );
    }

    protected override IEnumerator MouseAndKeyboard_Update ()
    {
        float chargeTimer = 0;
        float movementSpeed = 0f;
        bool chargeEffect = false;
        bool leftDoubleClick = false;
        float leftClickTime = 0f;
        Vector3 direction = Vector3.zero;
        Vector3 moveDirection = Vector3.zero;

        while ( ContainsState( DefaultStates.MouseAndKeyboard ) )
        {
            Vector3 mouseScreenPos = Input.mousePosition;
            Ray ray = Camera.main.ScreenPointToRay( mouseScreenPos );
            RaycastHit hit;
            Physics.Raycast( ray , out hit );

            if ( hit.transform != null && Vector3.Distance( transform.position , hit.point ) > 1 )
            {
                direction = ( hit.point - transform.position );
                direction.y = 0;
                direction.Normalize();
            }

            //Cursor orientation
            moveCursor.localRotation = Quaternion.Lerp( moveCursor.localRotation , Quaternion.LookRotation( direction ) , moveCursorRotationSpeed * Time.deltaTime );

            if ( !_chargePunch && !_chargeStomp && !_chargePush && !_blink )
            {
                //Punch
                if ( Input.GetMouseButton( 1 ) )
                {
                    if ( Input.GetMouseButtonDown( 1 ) )
                    {
                        moveDirection = direction;

                        if ( maxMovementSpeed > movementSpeed )
                            movementSpeed += punchSpeedIncrement;

                        body.localRotation = Quaternion.LookRotation( direction );
                        StartCoroutine( Punch_Coroutine( direction ) );
                    }
                    else if ( chargeTimer < chargeTimeThreshold )
                        chargeTimer += Time.deltaTime;
                    else if ( chargeTimer >= chargeTimeThreshold && !chargeEffect )
                    {
                        chargeEffect = true;
                        Go.to( body.GetChild( 0 ).GetComponent<MeshRenderer>() , 0.2f , new GoTweenConfig().materialColor( Color.red ).setIterations( -1 , GoLoopType.PingPong ) );
                    }
                }
                else if ( Input.GetMouseButtonUp( 1 ) )
                {
                    chargeEffect = false;
                    Go.killAllTweensWithTarget( body.GetChild( 0 ).GetComponent<MeshRenderer>() );

                    //Charge attack
                    if ( chargeTimer >= chargeTimeThreshold )
                    {
                        body.GetChild( 0 ).GetComponent<MeshRenderer>().material.color = Color.red;
                        moveDirection = direction;

                        if ( Input.GetMouseButton( 0 ) )
                        {
                            //Push
                            if ( maxMovementSpeed > movementSpeed )
                                movementSpeed += chargePushSpeedIncrement;

                            body.localRotation = Quaternion.LookRotation( direction );

                            StartCoroutine( ChargePush_Coroutine( direction ) );
                        }
                        else
                        {
                            //Uppercut
                            if ( maxMovementSpeed > movementSpeed )
                                movementSpeed += chargePunchSpeedIncrement;

                            body.localRotation = Quaternion.LookRotation( direction );

                            StartCoroutine( ChargePunch_Coroutine( direction ) );
                        }
                    }
                    else
                        body.GetChild( 0 ).GetComponent<MeshRenderer>().material.color = new Color( 62f / 255f , 1f , 0 );

                    chargeTimer = 0;
                }

                if ( leftDoubleClick )
                    leftClickTime += Time.deltaTime;

                if ( leftDoubleClick && leftClickTime >= doubleClickTime )
                {
                    leftClickTime = 0;
                    leftDoubleClick = false;
                }

                //Movement
                if ( Input.GetMouseButton( 0 ) )
                {
                    if ( Input.GetMouseButtonDown( 0 ) )
                    {
                        //Blink
                        if ( leftDoubleClick )
                        {
                            if ( chargeTimer > chargeTimeThreshold )
                                body.GetChild( 0 ).GetComponent<MeshRenderer>().material.color = Color.red;

                            body.localRotation = Quaternion.LookRotation( direction );
                            moveDirection = direction;
                            Go.killAllTweensWithTarget( body );
                            leftClickTime = 0;
                            leftDoubleClick = false;
                            StartCoroutine( Blink_Coroutine( direction , hit.point , chargeTimer > chargeTimeThreshold ) );
                            chargeTimer = 0;
                        }

                        leftDoubleClick = true;
                    }

                    moveDirection = direction;
                    body.localRotation = Quaternion.Lerp( body.localRotation , Quaternion.LookRotation( moveDirection ) , bodyRotationSpeed * Time.deltaTime );

                    if ( minMovementSpeed > movementSpeed )
                        movementSpeed = minMovementSpeed;

                    if ( chargeTimer > chargeTimeThreshold )
                    {
                        if ( chargeMaxMovementSpeed > movementSpeed )
                            movementSpeed *= movementSpeedIncrement;
                        else
                            movementSpeed = chargeMaxMovementSpeed;
                    }
                    else
                    {
                        if ( maxMovementSpeed > movementSpeed )
                            movementSpeed *= movementSpeedIncrement;
                        else
                            movementSpeed = maxMovementSpeed;
                    }
                }
                else
                {
                    if ( movementSpeed > minMovementSpeed )
                        movementSpeed *= movementSpeedDecrement;
                    else
                        movementSpeed = 0;
                }
            }
            else
            {
                if ( movementSpeed > minMovementSpeed )
                    movementSpeed *= movementSpeedDecrement;
                else
                    movementSpeed = 0;
            }

            if ( movementSpeed > 0 )
            {
                Vector3 movePos = rigidbody.position + ( moveDirection * movementSpeed * Time.deltaTime );
                movePos.y = 0.5f;
                rigidbody.MovePosition( movePos );
            }

            yield return null;
        }
    }

    private IEnumerator Blink_Coroutine ( Vector3 direction , Vector3 mousePos , bool charge )
    {
        _blink = true;
        Vector3 pos = transform.position;
        pos.y = 0;
        mousePos.y = 0;
        Vector3 bodyScale = body.localScale;
        float yPos = transform.position.y;

        Go.to( body , 0.25f , new GoTweenConfig().scale( Vector3.one * 0.25f ).setEaseType( GoEaseType.BackIn ) );

        yield return new WaitForSeconds( 0.25f );
        Vector3 newPos = mousePos;
        newPos.y = yPos;

        Go.to( transform , 0.5f , new GoTweenConfig().position( newPos ).setEaseType( GoEaseType.BackInOut ) );

        yield return new WaitForSeconds( 0.5f );
        transform.position = newPos;

        Go.to( body , 0.25f , new GoTweenConfig().scale( bodyScale ).setEaseType( GoEaseType.BackOut ) );

        if ( charge )
        {
            GameObject punch = GameObject.CreatePrimitive( PrimitiveType.Cube );
            punch.GetComponent<MeshRenderer>().material = attackMaterial;
            punch.transform.position = transform.position;
            punch.transform.localScale = Vector3.zero;

            GoTweenChain chain = new GoTweenChain();
            GoTween growTween = Go.to( punch.transform , chargeBlinkFXTime * 0.25f , new GoTweenConfig().scale( Vector3.one * 3 ).setEaseType( GoEaseType.BackOut ) );
            GoTween shrinkTween = Go.to( punch.transform , chargeBlinkFXTime * 0.75f , new GoTweenConfig().scale( Vector3.zero ).setEaseType( GoEaseType.ExpoIn ) );
            chain.append( growTween );
            chain.append( shrinkTween );
            chain.play();

            yield return new WaitForSeconds( chargeBlinkFXTime );
            yield return new WaitForSeconds( blinkRecoverTime );
        }
        else
            yield return new WaitForSeconds( 0.25f );

        Go.killAllTweensWithTarget( body.GetChild( 0 ).GetComponent<MeshRenderer>() );
        Go.to( body.GetChild( 0 ).GetComponent<MeshRenderer>() , 0.2f , new GoTweenConfig().materialColor( new Color( 62f / 255f , 1f , 0 ) ) );
        _blink = false;
    }

    private IEnumerator Punch_Coroutine ( Vector3 direction )
    {
        GameObject punch = GameObject.CreatePrimitive( PrimitiveType.Cube );
        punch.GetComponent<MeshRenderer>().material = attackMaterial;
        punch.transform.position = transform.position + ( ( _right ? body.TransformDirection( Vector3.right ) : body.TransformDirection( Vector3.left ) ) * 0.35f );
        punch.transform.localScale = Vector3.one * 0.5f;
        float speed = punchFXSpeed;
        bool tweenPlaying = true;
        _right = !_right;

        Vector3 dirToPunch = body.transform.TransformDirection( Vector3.forward ) + ( ( _right ? body.TransformDirection( Vector3.right ) : body.TransformDirection( Vector3.left ) ) * 0.25f );
        dirToPunch.y = 0;
        dirToPunch.Normalize();
        Quaternion rot = Quaternion.LookRotation( dirToPunch );

        GoTweenFlow flow = new GoTweenFlow();
        GoTween punchTween = Go.to( punch.transform , punchFXDuration , new GoTweenConfig().scale( Vector3.zero ).setEaseType( GoEaseType.ExpoIn ).onComplete( c => tweenPlaying = false ) );
        flow.insert( 0 , punchTween );

        Go.killAllTweensWithTarget( body );
        GoTween rotationTween = new GoTween( body , punchFXDuration * 0.4f , new GoTweenConfig().rotation( rot ).setEaseType( GoEaseType.ExpoOut ).setIterations( 2 , GoLoopType.PingPong ) );
        flow.insert( 0 , rotationTween );
        flow.play();

        while ( tweenPlaying )
        {
            punch.transform.position += direction * speed * Time.deltaTime;
            speed *= punchFXSpeedIncrement;
            yield return null;
        }

        Destroy( punch );
    }

    private IEnumerator ChargePunch_Coroutine ( Vector3 direction )
    {
        _chargePunch = true;
        GameObject punch = GameObject.CreatePrimitive( PrimitiveType.Cube );
        punch.GetComponent<MeshRenderer>().material = attackMaterial;
        punch.transform.position = transform.position + direction;
        punch.transform.localScale = Vector3.one * 0.75f;
        float speed = chargePunchFXSpeed;
        bool tweenPlaying = true;
        bool atTop = false;
        bool falling = false;
        float time = 0;

        Vector3 dirToPunch = direction + ( Vector3.up * 0.25f );
        dirToPunch.Normalize();
        Quaternion rot = Quaternion.LookRotation( dirToPunch );

        GoTweenFlow flow = new GoTweenFlow();
        GoTween punchTween = Go.to( punch.transform , chargePunchFXDuration , new GoTweenConfig().scale( Vector3.zero ).setEaseType( GoEaseType.ExpoIn ).onComplete( c => tweenPlaying = false ) );
        flow.insert( 0 , punchTween );

        Go.killAllTweensWithTarget( body );
        GoTween rotationTween = new GoTween( body , chargePunchFXDuration * 0.3f , new GoTweenConfig().rotation( rot ).setEaseType( GoEaseType.ExpoOut ).setIterations( 2 , GoLoopType.PingPong ) );
        GoTween jumpTween = new GoTween( body , chargePunchFXDuration * 0.6f , new GoTweenConfig().localPosition( Vector3.up * 1.5f , true ).setEaseType( GoEaseType.BackOut ) );
        GoTween landTween = new GoTween( body , chargePunchFXDuration * 0.15f , new GoTweenConfig().localPosition( Vector3.down *  1.5f , true ).setEaseType( GoEaseType.QuadIn ).onBegin( c => falling = true ) );
        flow.insert( 0 , rotationTween );
        flow.insert( 0 , jumpTween );
        flow.insert( chargePunchFXDuration * 0.85f , landTween );
        flow.play();

        while ( tweenPlaying )
        {
            time += Time.deltaTime;
            punch.transform.position += Vector3.up * speed * Time.deltaTime;
            speed *= chargePunchFXSpeedIncrement;

            if ( time > chargePunchFXDuration * 0.5f )
                atTop = true;

            if ( !_chargeStomp && atTop && !falling && Input.GetMouseButtonDown( 1 ) )
            {
                Go.killAllTweensWithTarget( body );
                StartCoroutine( ChargeStomp_Coroutine( direction ) );
            }

            yield return null;
        }

        yield return new WaitForSeconds( chargePunchRecoverTime );

        if ( !_chargeStomp )
        {
            Go.to( body.GetChild( 0 ).GetComponent<MeshRenderer>() , 0.2f , new GoTweenConfig().materialColor( new Color( 62f / 255f , 1f , 0 ) ) );
            body.transform.localPosition = Vector3.zero;
        }

        Destroy( punch );
        _chargePunch = false;
    }

    private IEnumerator ChargeStomp_Coroutine ( Vector3 direction )
    {
        _chargeStomp = true;
        bool effect = false;
        GoTweenChain chain = new GoTweenChain();
        GameObject stomp = GameObject.CreatePrimitive( PrimitiveType.Cube );
        stomp.GetComponent<MeshRenderer>().material = attackMaterial;
        stomp.transform.localScale = Vector3.zero;

        Go.to( body , chargeStompFXDuration * 0.3f , new GoTweenConfig().localPosition( Vector3.up * 0.5f , true ).setEaseType( GoEaseType.BackOut ) );
        Go.to( body , chargePunchFXDuration * 0.3f , new GoTweenConfig().rotation( Quaternion.LookRotation( direction ) ) );

        yield return new WaitForSeconds( chargeStompFXDuration * 0.3f );

        Go.to( body , chargeStompFXDuration * 0.7f , new GoTweenConfig().localPosition( Vector3.zero ).setEaseType( GoEaseType.BackOut ) );
        float time = 0;

        while ( chargeStompFXDuration * 0.7f > time )
        {
            if ( !effect && time > chargeStompFXDuration * 0.2f )
            {
                effect = true;
                stomp.transform.position = transform.position + ( Vector3.down * 0.25f );

                GoTween effectGrow = new GoTween( stomp.transform , 0.2f , new GoTweenConfig().scale( new Vector3( 3 , 0.5f , 3 ) ).setEaseType( GoEaseType.BackOut ) );
                GoTween effectShrink = new GoTween( stomp.transform , 1f , new GoTweenConfig().scale( new Vector3( 3 , 0 , 3 ) ).setEaseType( GoEaseType.ExpoIn ) );
                chain.append( effectGrow );
                chain.append( effectShrink );
                chain.play();
            }

            time += Time.deltaTime;
            yield return null;
        }

        yield return chain.waitForCompletion();
        Destroy( stomp );

        yield return new WaitForSeconds( chargeStompRecoverTime );
        _chargeStomp = false;
        Go.to( body.GetChild( 0 ).GetComponent<MeshRenderer>() , 0.2f , new GoTweenConfig().materialColor( new Color( 62f / 255f , 1f , 0 ) ) );
    }

    private IEnumerator ChargePush_Coroutine ( Vector3 direction )
    {
        _chargePush = true;
        GameObject punch = GameObject.CreatePrimitive( PrimitiveType.Cube );
        punch.GetComponent<MeshRenderer>().material = attackMaterial;
        punch.transform.position = transform.position + direction;
        punch.transform.localScale = Vector3.one;
        float speed = chargePushFXSpeed;
        bool tweenPlaying = true;
        float time = 0;

        Vector3 dirToPush = direction + ( Vector3.down * 0.25f );
        dirToPush.Normalize();
        Quaternion rot = Quaternion.LookRotation( dirToPush );

        GoTweenFlow flow = new GoTweenFlow();
        GoTween punchTween = Go.to( punch.transform , chargePushFXDuration , new GoTweenConfig().scale( Vector3.zero ).setEaseType( GoEaseType.ExpoIn ).onComplete( c => tweenPlaying = false ) );
        flow.insert( 0 , punchTween );

        Go.killAllTweensWithTarget( body );
        GoTween rotationTween = new GoTween( body , chargePushFXDuration * 0.15f , new GoTweenConfig().rotation( rot ).setEaseType( GoEaseType.ExpoOut ).setIterations( 2 , GoLoopType.PingPong ) );
        flow.insert( 0 , rotationTween );
        flow.play();

        while ( tweenPlaying )
        {
            time += Time.deltaTime;
            punch.transform.position += direction * speed * Time.deltaTime;
            speed *= chargePunchFXSpeedIncrement;
            yield return null;
        }

        yield return new WaitForSeconds( chargePushRecoverTime );
        Go.to( body.GetChild( 0 ).GetComponent<MeshRenderer>() , 0.2f , new GoTweenConfig().materialColor( new Color( 62f / 255f , 1f , 0 ) ) );
        body.transform.localPosition = Vector3.zero;

        Destroy( punch );
        _chargePush = false;
    }

    public Material attackMaterial;

    public float doubleClickTime = 0.2f;
    public float chargeBlinkFXTime = 1f;
    public float blinkRecoverTime = 0.2f;
    public float chargePushRecoverTime = 0.3f;
    public float chargePushFXDuration = 0.2f;
    public float chargePushFXSpeed = 10;
    public float chargePushFXSpeedIncrement = 0.8f;
    public float  chargePushSpeedIncrement = 7;
    public float chargeStompFXDuration = 0.5f;
    public float chargeStompRecoverTime = 0.2f;

    public float chargePunchFXDuration = 0.2f;
    public float chargePunchFXSpeed = 10;
    public float chargePunchFXSpeedIncrement = 0.8f;
    public float chargePunchTopTime = 0.2f;
    public float chargePunchRecoverTime = 0.2f;

    public float punchFXDuration = 0.2f;
    public float punchFXSpeed = 0.8f;
    public float punchFXSpeedIncrement = 10;
    public float chargePunchSpeedIncrement = 3;
    public float punchSpeedIncrement = 1;
    public float chargeTimeThreshold = 1;

    public float maxMovementSpeed = 5;
    public float minMovementSpeed = 0.25f;
    public float movementSpeedIncrement = 1.5f;
    public float movementSpeedDecrement = 0.9f;
    public float chargeMaxMovementSpeed = 6;

    public float bodyRotationSpeed = 5;
    public float moveCursorRotationSpeed = 10;

    public Transform body;
    public Transform moveCursor;

    private bool _right = false;
    private bool _chargePunch = false;
    private bool _chargeStomp = false;
    private bool _chargePush = false;
    private bool _blink = false;
}