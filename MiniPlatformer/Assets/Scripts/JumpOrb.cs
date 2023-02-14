using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpOrb : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            PlayerMovement playerMovement = collision.gameObject.GetComponent<PlayerMovement>();
            playerMovement.AddMidAirJumps(1);
            this.gameObject.SetActive(false);
        }
    }
}
