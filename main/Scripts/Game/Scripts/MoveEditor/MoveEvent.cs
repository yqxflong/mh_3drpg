public class MoveEvent
{
	// comes from MoveEditor
	public MoveEditor.MoveAnimationEvent myAnimEvent;
	public bool required = false;
	public bool playOnce = false;
	public bool persistent = false;
	public float endFrame = -1.0f;

	// run-time tracking
	public int numTimesPlayed;

	public MoveEvent(MoveEditor.MoveAnimationEvent ee, bool isRequired, bool isPlayOnce, bool isPersistent, float endingFrame)
	{
		myAnimEvent = ee;
		required = isRequired;
		playOnce = isPlayOnce;
		persistent = isPersistent;
		endFrame = endingFrame;
	}
	
	public void Reset()
	{
		numTimesPlayed = 0;
	}
}

