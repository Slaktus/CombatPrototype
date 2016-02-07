using UnityEngine;

public class MaterialColorTweenProperty : AbstractColorTweenProperty
{
    private MaterialPropertyBlock _matPropBlock = new MaterialPropertyBlock();

    public MaterialColorTweenProperty ( Color endValue , bool isRelative = false ) : base( endValue , isRelative )
    {
    }

    #region Object overrides

    public override int GetHashCode ()
    {
        return base.GetHashCode();
    }

    public override bool Equals ( object obj )
    {
        // start with a base check and then compare our endvalues
        if ( base.Equals( obj ) )
            return _endValue == ( ( MaterialColorTweenProperty ) obj )._endValue;

        return false;
    }

    #endregion Object overrides

    public override void prepareForUse ()
    {
        _endValue = _originalEndValue;

        // if this is a from tween we need to swap the start and end values
        if ( _ownerTween.isFrom )
        {
            _startValue = _endValue;
            _endValue = _target.material.color;
        }
        else
            _startValue = _target.material.color;

        base.prepareForUse();
    }

    public override void tick ( float totalElapsedTime )
    {
        float easedTime = _easeFunction( totalElapsedTime, 0, 1, _ownerTween.duration );
        _target.material.color = GoTweenUtils.unclampedColorLerp( _startValue , _diffValue , easedTime );
    }
}