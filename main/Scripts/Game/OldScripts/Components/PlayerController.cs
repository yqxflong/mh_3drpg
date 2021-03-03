using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Íæ¼Ò¿Ø¼þ
/// </summary>
public class PlayerController : Controller
{
    public DynamicMonoILR HotfixController;

    public static System.Action<Hashtable> onCollisionOpen;
    public static System.Action<string> onCollisionExit;
    public static Collider CurNpcCollision;

    private EB.Sparx.Player _replicationPlayer;
    public EB.Sparx.Player ReplicationPlayer
    {
        set {
            _replicationPlayer = value;
        }
        get
        {
            return _replicationPlayer;
        }
    }

    public bool _isLocalPlayer = false;
    public bool IsLocal
    {
        get
        {
            return PlayerManager.IsLocalPlayer(gameObject);
        }
    }
    
    private eGender _characterGender;
    public eGender Gender
    {
        set {
            _characterGender = value;
        }

        get
        {
            return _characterGender;
        }
    }
    
    private GameObject _pixie;
    public GameObject Pixie
    {
        get
        {
            return _pixie;
        }
    }

    public long playerUid
    {
        get; set;
    }
    
    public static void LocalPlayerEnableNavigation()
    {
        PlayerController controller = PlayerManager.LocalPlayerController();
        if (null != controller)
        {
            SelectionLogic selection = controller.GetComponent<SelectionLogic>();
            if (null != selection)
            {
                selection.EnablePlayerSelectionControls();
            }
        }
    }
    
    public static void LocalPlayerDisableNavigation()
    {
        PlayerController controller = PlayerManager.LocalPlayerController();
        if (null != controller)
        {
            CharacterTargetingComponent characterTargetingComp = controller.gameObject.GetComponent<CharacterTargetingComponent>();
            if (null != characterTargetingComp)
            {
                characterTargetingComp.SetMovementTarget(Vector3 .zero ,true);
            }

            SelectionLogic selection = controller.GetComponent<SelectionLogic>();
            if (null != selection)
            {
                selection.DisablePlayerSelectionControls();
            }
        }
    }

    new void Awake()
    {
        base.Awake();
        HotfixController = GetComponent<DynamicMonoILR>();
        if(HotfixController!=null) HotfixController.ILRObjInit();
    }
    
    public void Destroy()
    {
        HotfixController.OnHandleMessage("Destroy", null);
    }

    public void OnTriggerEnter(Collider collision)
    {
        HotfixController.OnHandleMessage("OnTriggerEnter", collision);
    }

    public void OnTriggerExit(Collider collision)
    {
        HotfixController.OnHandleMessage("OnTriggerExit", collision);
	}
}
