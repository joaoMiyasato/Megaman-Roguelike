using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundaryManager : MonoBehaviour
{
    private BoxCollider2D managerBox;
    private Transform player;
    private GameObject boundary;

    void Start()
    {
        managerBox = GetComponent<BoxCollider2D>();
        player = GameObject.Find("Player").transform;
        boundary = this.transform.Find("Boundary").gameObject;
    }

    void Update()
    {
        ManageBoundary();
    }

    void ManageBoundary()
    {
        if( managerBox.bounds.min.x < player.position.x && player.position.x < managerBox.bounds.max.x &&
            managerBox.bounds.min.y < player.position.y && player.position.y < managerBox.bounds.max.y)
        {
            boundary.SetActive(true);
        }
        else
        {
            boundary.SetActive(false);
        }
    }


    [Header("Gizmos")]
    public bool ActivateGizmos;
    private void OnDrawGizmos()
    {
        if (ActivateGizmos)
        {
            Gizmos.color = Color.green;
            if(this.transform.parent != null)
            {
                Gizmos.DrawWireCube(this.transform.position, new Vector3(this.transform.localScale.x * this.transform.parent.localScale.x, this.transform.localScale.y * this.transform.parent.localScale.y, this.transform.localScale.z * this.transform.parent.localScale.z));
            }
            else
            {
                Gizmos.DrawWireCube(this.transform.position, this.transform.localScale);
            }
        }
    }
} //(this.transform.GetComponentInParent<Transform>().localScale)
