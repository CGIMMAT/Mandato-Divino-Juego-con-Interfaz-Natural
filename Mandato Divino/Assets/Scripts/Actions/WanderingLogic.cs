using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WanderingLogic : MonoBehaviour
{
    public float moveSpeed = 0.5f;
    public Vector2 decisionTime = new Vector2(1,5);
    internal float decisionTimer = 0;
    internal Vector3[] moveDirection = new Vector3[] {Vector3.right, Vector3.left, Vector3.forward, Vector3.back, Vector3.zero};
    internal int currentDirection;

    void Start()
    {
        decisionTimer = Random.Range(decisionTime.x, decisionTime.y);
        ChooseDirection();
    }

    void Update()
    {
        transform.position += moveDirection[currentDirection] * Time.deltaTime * moveSpeed;

        if (decisionTimer > 0)
        {
            decisionTimer -= Time.deltaTime;
        }
        else
        {
            decisionTimer = Random.Range(decisionTime.x, decisionTime.y);
            ChooseDirection();
        }
    }

    void ChooseDirection()
    {
        currentDirection = Mathf.FloorToInt(Random.Range(0, moveDirection.Length));
    }
}