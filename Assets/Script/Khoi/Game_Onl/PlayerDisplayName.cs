using System.Collections;
using System.Collections.Generic;
using Fusion;
using TMPro;
using UnityEngine;

public class PlayerDisplayName : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI _nameText;
    
    private PlayerMoving _playerController;

    public override void Spawned()
    {
        _playerController = GetComponent<PlayerMoving>();
    }
    public override void Render()
    {
        _nameText.text = _playerController.PlayerName.ToString();
    }
}
