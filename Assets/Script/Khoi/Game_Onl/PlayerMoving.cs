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
            
            rb.velocity = input.normalized * speed;
        }
    }
}