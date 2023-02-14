using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashOrb : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            PlayerMovement playerMovement = collision.gameObject.GetComponent<PlayerMovement>();
            playerMovement.AddMidAirDashs(1);
            this.gameObject.SetActive(false);
        }
    }
}
