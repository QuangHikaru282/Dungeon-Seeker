using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using TMPro;

public class PlayerMoving : NetworkBehaviour
{
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _jumpForce = 10f;
    public Rigidbody2D rb;
    public PlayerDisplayName playerDisplayName;
    public float speed;

    //public TextMeshProUGUI playerNameText;

    [Networked] 
    public NetworkString<_16> PlayerName {  get; set; }

    //public void OnNameChanged()
    //{
    //    playerNameText.text = PlayerName.ToString();
    //}

    public override void Spawned()
    {
        rb = GetComponent<Rigidbody2D>();
        //if (Object.HasInputAuthority)
        //{
        PlayerName = PlayerPrefs.GetString("PlayerName", "Player");
        //    PlayerName = name;
        //}
        
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_InfoPlauerJoinedGame(string name)
    {

    }
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_RequestPickItem()
    {

    }
    public override void Render()
    {
        if (IsFading)
        {
            if (Object.HasInputAuthority)
            {
                SetAlpha(0.3f);
            }
            else
            {
                SetAlpha(0f);
            }
        }
        else
        {
            SetAlpha(1f);
        } 
    }
    [Networked] public NetworkBool IsFading { get; set; }

    public void CastFadingSkill()
    {
        RPC_CastFadingSkill(Object.InputAuthority.PlayerId,true);
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_CastFadingSkill(int playerId, bool state)
    {
        //IsFading = true;
        foreach(var player in FindObjectsOfType<PlayerMoving>())
        {
            if (player.Object.InputAuthority.PlayerId==playerId )
            {
                player.IsFading = state;
                break;
            }    
        }
    }
    private void SetAlpha(float alpha)
    {
        var spriteRenderer = GetComponent<SpriteRenderer>();
        var color = spriteRenderer.color;
        color.a = alpha;
        spriteRenderer.color = color;
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_PlayShotSound()
    {
        AudioManager_Khoi.Instance.PlayShotSound(
            AudioManager_Khoi.Instance.shotClip,
            transform.position, 1f);
    }
    public override void FixedUpdateNetwork()
    {
        if (Object.HasInputAuthority)
        {
            Vector2 input = Vector2.zero;
            if (Input.GetKey(KeyCode.W))
            {
                input.y += 1;
            }
            if (Input.GetKey(KeyCode.S))
            {
                input.y -= 1;
            }
            if (Input.GetKey(KeyCode.A))
            {
                input.x -= 1;
            }
            if (Input.GetKey(KeyCode.D))
            {
                input.x += 1;
            }
            if (Input.GetKey(KeyCode.U))
            {
                CastFadingSkill();
            }
            if (Input.GetKey(KeyCode.Y))
            {
                RPC_PlayShotSound();
            }
            rb.velocity = input.normalized * speed;
        }
    }
}