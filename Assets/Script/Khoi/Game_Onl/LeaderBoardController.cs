using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderBoardController : MonoBehaviour
{
    public GameObject rowPrefab;
    public Transform contentPanelParent;

    public static LeaderBoardController Instance;

    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        UpdateLeaderboard();
    }
    public void UpdateLeaderboard()
    {
        foreach (Transform child in contentPanelParent)
        {
            Destroy(child.gameObject);

        }

        var players = FindObjectsOfType<PlayerMoving>();

        System.Array.Sort(players,(a,b) => b.name.CompareTo(a.name));

        foreach (var player in players)
        {
            var row = Instantiate(rowPrefab,contentPanelParent);
            //var rowController = row.GetComponent<LeaderBoardController>();
            //if (rowController != null)
            //{
            //    rowController.SetData(player.PlayerName.ToString(),row);
            //}    
            var texts = row.GetComponentsInChildren<TMPro.TextMeshProUGUI>();
            texts[0].text = player.PlayerName.ToString();
            texts[1].text = "0";
        }
    }

    
}
