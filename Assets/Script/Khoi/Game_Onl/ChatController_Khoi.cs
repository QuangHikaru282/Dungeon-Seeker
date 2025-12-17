using System.Collections;
using System.Collections.Generic;
using Fusion;
using TMPro;
using UnityEngine;

public class ChatController_Khoi : NetworkBehaviour
{
    public TMP_InputField inputMessage;
    public TMP_Text chatDisplay;

    public void OnSandChat()
    {
        var message = inputMessage.text;
        if (!string.IsNullOrEmpty(message))
        {
            Debug.Log("Object null? " + (Object == null));
            Debug.Log("Has InputAuthority? " + (Object != null && Object.HasInputAuthority));

            RPC_SendChatMessage(message, Object.InputAuthority.PlayerId);
            inputMessage.text = string.Empty;
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_SendChatMessage(string message, int senderID)
    {
        var line = $"<b>Player {senderID}:</b> {message}\n";
        chatDisplay.text += line ;
    }
    public void GetAllPlayers()
    {
        foreach (var player in Runner.ActivePlayers)
        {
            
        }
    }

    public void StartPrivateChat(int targetPlayerId)
    {

    }
}

//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class AudioManager_Khoi : MonoBehaviour
//{
//    public static AudioManager_Khoi Instance { get; private set; }
//    public AudioClip shotClip;

//    void Awake()
//    {
//        if (Instance == null)
//        {
//            Instance = this;
//            DontDestroyOnLoad(gameObject);
//        }
//        else
//        {
//            Destroy(gameObject);
//        }
//    }

//    public void PlayShotSound(AudioClip clip, Vector3 position, float volume)
//    {
//        AudioSource.PlayClipAtPoint(clip, position, volume);
//    }
//}
