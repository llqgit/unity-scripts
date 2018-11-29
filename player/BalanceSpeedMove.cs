using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalanceSpeedMove : MonoBehaviour {

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

        float x1 = dx;
        float y1 = dy;
        // 和谐掉斜向移动时，速度变快的问题
        if (dx != 0 && dy != 0) {
            float distance = Mathf.Abs (Mathf.Sqrt (dx * dx + dy * dy));
            if (distance > 1) {
                float a = Mathf.Abs (dy / dx);
                x1 = Mathf.Sqrt (1 / (a * a + 1));
                y1 = a * x1;
                if (dx < 0) {
                    x1 *= -1;
                }
                if (dy < 0) {
                    y1 *= -1;
                }
                Debug.Log ("x1:" + x1);
                Debug.Log ("y1:" + y1);
            }
        }

        this.transform.Translate (new Vector3 (x1 * this.speed, y1 * this.speed, 0) * Time.deltaTime);
    }
}