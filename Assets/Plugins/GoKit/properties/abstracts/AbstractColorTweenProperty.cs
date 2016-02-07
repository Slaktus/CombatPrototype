using UnityEngine;

/// <summary>
/// base class for any color tweens (MaterialColor and ColorTween)
/// </summary>
public abstract class AbstractColorTweenProperty : AbstractTweenProperty
{
    protected Renderer _target;

    protected Color _originalEndValue;
    protected Color _startValue;
    protected Color _endValue;
    protected Color _diffValue;

    public AbstractColorTweenProperty ( Color endValue , bool isRelative ) : base( isRelative )
    {
        _originalEndValue = endValue;
    }

    public override bool validateTarget ( object target )
    {
        Renderer test = target as Renderer;
        return test != null;
    }

    public override void init ( GoTween owner )
    {
        // setup our target before initting
        if ( owner.target is Renderer )
            _target = ( Renderer ) owner.target;

        base.init( owner );
    }

    public override void prepareForUse ()
    {
        if ( _isRelative && !_ownerTween.isFrom )
            _diffValue = _endValue;
        else
            _diffValue = _endValue - _startValue;
    }
}