using UnityEngine;

public class PositionAni : MonoBehaviour
{
    public Vector3 From;
    public Vector3 To;
    public GameObject InitStateObj;
    public GameObject GoalStateObj;

    private TweenPosition posAni;
    private int AniState = 0;       //动画的状态，0：初始状态；1：展开状态
    
	void Start ()
    {
        posAni = GetComponent<TweenPosition>();
        if (posAni == null)
        {
            return;
        }

        posAni.from = From;
        posAni.to = To;
        posAni.SetOnFinished(AniPlayOver);
    }

    public void OnBtnCLick()
    {
        if (AniState == 0)
        {
            posAni.from = From;
            posAni.to = To;
        }
        else
        {
            posAni.from = To;
            posAni.to = From;
        }
        posAni.ResetToBeginning();
        posAni.PlayForward();
    }

    private void AniPlayOver()
    {
        AniState = AniState == 0 ? 1 : 0;
        InitStateObj.SetActive(AniState == 0);
        GoalStateObj.SetActive(AniState == 1);
    }
}
