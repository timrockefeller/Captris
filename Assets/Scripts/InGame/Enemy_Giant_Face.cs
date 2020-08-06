using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Periodicitily Jumping & 
/// </summary>
public class Enemy_Giant_Face : MonoBehaviour
{

    public bool facingWall;

    private void OnTriggerStay(Collider other)
    {

        if (other.tag == "Terrain" || other.tag == "Piece")
        {
            facingWall = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Terrain" || other.tag == "Piece")
        {
            facingWall = false;
        }
    }
}
