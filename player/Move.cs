using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour {

    [Tooltip ("速度")]
    public float speed = 10; // 速度
    [Tooltip ("X锁")]
    public bool moveX = true;
    [Tooltip ("Y锁")]
    public bool moveY = true;

    // Use this for initialization
    void Start () {

    }

    // Update is called once per frame
    void Update () {
        float dx = this.moveX ? Input.GetAxis ("Horizontal") : 0;
        float dy = this.moveY ? Input.GetAxis ("Vertical") : 0;

        this.transform.Translate (new Vector3 (dx * this.speed, dy * this.speed, 0) * Time.deltaTime);
    }
}