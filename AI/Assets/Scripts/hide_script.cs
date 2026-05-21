using UnityEngine;

public class HidingSpot : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        PlayerHide playerHide = other.GetComponent<PlayerHide>();

        if (playerHide != null)
        {
            playerHide.isHidden = true;
            Debug.Log("Player is hidden");
        }
    }

    void OnTriggerExit(Collider other)
    {
        PlayerHide playerHide = other.GetComponent<PlayerHide>();

        if (playerHide != null)
        {
            playerHide.isHidden = false;
            Debug.Log("Player is no longer hidden");
        }
    }
}
