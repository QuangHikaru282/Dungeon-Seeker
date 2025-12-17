//using System.Collections;
//using System.Collections.Generic;
//using PlayFab.ClientModels;
//using PlayFab;
//using UnityEngine;
//using System;
//using Random = UnityEngine.Random;
//using UnityEditor.PackageManager;

//[Serializable]
//public class PlayerData_Khoi
//{
//    public string playerName;
//    public int coins;
//    public int level;

//}
//public class PlayfabManager : MonoBehaviour
//{
//    public void Start()
//    {

//        var request = new LoginWithCustomIDRequest
//        {
//            CustomId = $"GettingStartedGuide{Random.Range(1, 10000)}",
//            CreateAccount = true
//        };
//        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);
//    }
//    public void Update()
//    {
//        if (Input.GetKey(KeyCode.P))
//        {
//            SaveDataToPlayFab();
//        }
//    }
//    public void SaveDataToPlayFab()
//    {
//        var playerData = new PlayerData_Khoi
//        {
//            playerName = "PlayerOne",
//            coins = 150,
//            level = 5
//        };
//        var request = new UpdateUserDataRequest
//        {
//            Data = new System.Collections.Generic.Dictionary<string, string>
//            {
//                { "PlayerData", JsonUtility.ToJson(playerData) }
//            }
//        };
//        PlayFabClientAPI.UpdateUserData(request, OnDataSaveSuccess, OnDataSaveFailure);

//    }

//    private void OnDataSaveSuccess(UpdateUserDataResult result)
//    {
//        Debug.Log("Player data saved successfully to PlayFab");
//    }
//    private void OnDataSaveFailure(PlayFabError error)
//    {
//        Debug.Log("Failed to save player date to PlayFab.");
//        Debug.LogError(error.GenerateErrorReport());
//    }
//    private void OnLoginSuccess(LoginResult result)
//    {
//        Debug.Log("Congratulations, you made your first successful API call!");
//    }

//    private void OnLoginFailure(PlayFabError error)
//    {
//        Debug.LogWarning("Something went wrong with your first API call.  :(");
//        Debug.LogError("Here's some debug information:");
//        Debug.LogError(error.GenerateErrorReport());
//    }
//}
