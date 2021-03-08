using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMain : MonoBehaviour {

    public Character_Base CharacterBase { get; private set; }
    public PlayerMovementHandler PlayerMovementHandler { get; private set; }
    
    public Rigidbody2D PlayerRigidbody2D { get; private set; }

    private void Awake() {
        CharacterBase = GetComponent<Character_Base>();
        PlayerMovementHandler = GetComponent<PlayerMovementHandler>();

        PlayerRigidbody2D = GetComponent<Rigidbody2D>();
    }

}
