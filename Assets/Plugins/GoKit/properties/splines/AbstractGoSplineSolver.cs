using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractGoSplineSolver
{
    protected List<Vector3> _nodes;
    public List<Vector3> nodes { get { return _nodes; } }
    protected float _pathLength;
    public float pathLength { get { return _pathLength; } }

    // how many subdivisions should we divide each segment into? higher values take longer to build and lookup but
    // result in closer to actual constant velocity
    protected int totalSubdivisionsPerNodeForLookupTable = 5;

    //protected Dictionary<float, float> _segmentTimeForDistance; // holds data in the form [time:distance] as a lookup table
    protected List< float > _segmentTime;

    protected List< float > _segmentDistance;

    private int totalSubdivisions = 0;

    // the default implementation breaks the spline down into segments and approximates distance by adding up
    // the length of each segment
    public virtual void buildPath ()
    {
        totalSubdivisions = _nodes.Count * totalSubdivisionsPerNodeForLookupTable;
        _pathLength = 0;
        float timePerSlice = 1f / totalSubdivisions;

        // we dont care about the first node for distances because they are always t:0 and len:0
        //_segmentTimeForDistance = new Dictionary<float , float>( totalSubdivisions );
        _segmentTime = new List<float>( totalSubdivisions );
        _segmentDistance = new List<float>( totalSubdivisions );
        Vector3 lastPoint = getPoint( 0 );
        Vector3 currentPoint;

        // skip the first node and wrap 1 extra node
        for ( var i = 1 ; i < totalSubdivisions + 1 ; i++ )
        {
            // what is the current time along the path?
            float currentTime = timePerSlice * i;

            currentPoint = getPoint( currentTime );
            _pathLength += Vector3.Distance( currentPoint , lastPoint );
            lastPoint = currentPoint;

            //_segmentTimeForDistance.Add( currentTime , _pathLength );
            _segmentTime.Add( currentTime );
            _segmentDistance.Add( _pathLength );
        }
    }

    public abstract void closePath ();

    // gets the raw point not taking into account constant speed. used for drawing gizmos
    public abstract Vector3 getPoint ( float t );

    // gets the point taking in to account constant speed. the default implementation approximates the length of the spline
    // by walking it and calculating the distance between each node
    public virtual Vector3 getPointOnPath ( float t )
    {
        // we know exactly how far along the path we want to be from the passed in t
        float targetDistance = _pathLength * t;

        // store the previous and next nodes in our lookup table
        float previousNodeTime = 0f;
        float previousNodeLength = 0f;
        float nextNodeTime = 0f;
        float nextNodeLength = 0f;
        float prevDist = 0f;

        int approxStart = ( int ) ( t * totalSubdivisions );

        // loop through all the values in our lookup table and find the two nodes our targetDistance falls between
        for ( int i = approxStart ; totalSubdivisions > i ; i++ )
        {
            if ( _segmentDistance[ i ] >= targetDistance )
            {
                nextNodeTime = _segmentTime[ i ];
                nextNodeLength = _segmentDistance[ i ];
                previousNodeLength = previousNodeTime > 0f ? prevDist : 0f;
                break;
            }

            previousNodeTime = _segmentTime[ i ];
            prevDist = _segmentDistance[ i ];
        }

        // translate the values from the lookup table estimating the arc length between our known nodes from the lookup table
        float segmentTime = nextNodeTime - previousNodeTime;
        float segmentLength = nextNodeLength - previousNodeLength;
        float distanceIntoSegment = targetDistance - previousNodeLength;

        t = segmentLength > 0f ? previousNodeTime + ( distanceIntoSegment / segmentLength ) * segmentTime : previousNodeTime;
        //t = previousNodeTime + ( distanceIntoSegment / segmentLength ) * segmentTime;

        return getPoint( t );
    }

    public void reverseNodes ()
    {
        _nodes.Reverse();
    }

    public virtual void drawGizmos ()
    { }
}