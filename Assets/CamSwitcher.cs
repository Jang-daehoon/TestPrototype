using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class CamSwitcher : MonoBehaviour
{
    public Animator camAnim;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            camAnim.SetBool("ViewHallWay", true);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            camAnim.SetBool("ViewHallWay", false);
        }
    }
}
