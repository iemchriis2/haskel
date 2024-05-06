using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XRPlayer : MonoBehaviour
{

    [SerializeField]
    private Transform _rigTransform, _headTransform;
    // Start is called before the first frame update
    void Start()
    {
        MainMenu.GameStartEvent.Subscribe(this, EV_GameStart);
        NameConfirmationMenu.RequestEvent.Subscribe(this, EV_DoneLookingAtScoreOutput);
    }





    private void EV_DoneLookingAtScoreOutput()
    {
        Fade.FadeToBlack(() => {
            Teleport(TeleportTransform.GetTransformFromName("MAIN_AREA").position);
        });
    }

    private void EV_GameStart()
    {
       // _isInGame = _canInteract = _canTeleport = true;

        Fade.FadeToBlack(() => {
            Teleport(TeleportTransform.GetTransformFromName("GAME_AREA").position);
        });
    }

    private void Teleport(Vector3 position)
    {
        Vector3 offset = _rigTransform.position - _headTransform.position;
        offset = new Vector3(offset.x, 0, offset.z);
        _rigTransform.position = position + offset;
    }

}
