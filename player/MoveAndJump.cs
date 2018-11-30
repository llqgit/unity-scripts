using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAndJump : MonoBehaviour {

    [Tooltip ("速度")]
    public float speed = 10; // 速度
    [Tooltip ("跳跃力")]
    public float jumpForce = 10; // 跳跃力
    [Tooltip ("最大跳跃力")]
    public float maxJumpForce = 15; // 最大跳跃力
    [Tooltip ("蓄力速度")]
    public float jumpChargeSpeed = 10; // 蓄力速度
    [Tooltip ("N段跳")]
    public int jumpTimes = 1; // N段跳
    [Tooltip ("X锁")]
    public bool moveX = true;
    [Tooltip ("Y锁")]
    public bool moveY = true;

    public Transform body; // 身体
    public Transform shadow; // 阴影

    public enum State { ground, jumpStart, jumping, jumpEnd }

    private State state = State.ground;
    private float jumpHeight; // 当前跳跃的高度
    public float tempForce; // 当前跳跃的加速度
    public float tempMaxJumpForce; // 当前最大跳跃力
    private float shadowScaleX; // 记录阴影开始时的 X 值
    private float shadowScaleY; // 记录阴影开始时的 Y 值
    private float shadowAlpha; // 记录阴影开始时的 Alpha 值（SpriteRenderer）
    private SpriteRenderer shadowSpriteRenderer; // 阴影的 SpriteRenderer
    public int leftJumpTimes; // 缓存剩余跳跃次数

    // Use this for initialization
    void Start () {
        // 检查是否有默认的子对象（body & shadow）
        Transform childBody = this.transform.Find ("body");
        Transform childShadow = this.transform.Find ("shadow");
        if (this.body == null && childBody != null) {
            this.body = childBody;
        }
        if (this.shadow == null && childShadow != null) {
            this.shadow = childShadow;
        }
        // 缓存阴影初始缩放和透明度
        this.shadowScaleX = this.shadow.localScale.x;
        this.shadowScaleY = this.shadow.localScale.y;
        this.shadowSpriteRenderer = this.shadow.GetComponent<SpriteRenderer> ();
        this.shadowAlpha = this.shadowSpriteRenderer.color.a;
        this.jumpHeight = 0;
        this.tempMaxJumpForce = this.jumpForce;
        this.leftJumpTimes = this.jumpTimes;
    }

    // Update is called once per frame
    void Update () {
        this.move ();
        this.jump ();
    }

    void move () {
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
            }
        }
        // 更新坐标
        this.transform.Translate (new Vector3 (x1 * this.speed, y1 * this.speed, 0) * Time.deltaTime);
    }

    void jump () {
        this.chargeJump ();
        if (this.state == State.ground) {
            this.onGround ();
            // 重置阴影缩放
            this.shadow.localScale = new Vector3 (this.shadowScaleX, this.shadowScaleY, 0);
        } else if (this.state == State.jumping) {
            this.tempForce -= 9.8f * Time.deltaTime; // 模拟重力
            this.onJumping ();
            if (this.tempForce < 0) {
                this.onJumpFall ();
            }
            float height = this.jumpHeight + tempForce * Time.deltaTime;
            if (height <= 0) {
                this.onJumpEnd ();
            } else {
                this.jumpHeight = height;
            }
            this.body.localPosition = new Vector3 (this.body.localPosition.x, this.jumpHeight, 0);
            this.showShadowByDistance (this.jumpHeight);
        }
    }

    // set shadow scale and alpha by distance height
    void showShadowByDistance (float height) {
        float shadowScale = 1 / (1 + height);
        if (shadowScale < 0) {
            shadowScale = 0;
        }
        Color shadowColor = this.shadowSpriteRenderer.color;
        float alpha = this.shadowAlpha * shadowScale;
        this.shadowSpriteRenderer.color = new Color (shadowColor.r, shadowColor.g, shadowColor.b, alpha);
        this.shadow.localScale = new Vector3 (this.shadowScaleX, this.shadowScaleY, 0) * shadowScale;
    }

    // 蓄力跳跃
    void chargeJump () {
        if (Input.GetButton ("Jump")) {
            this.tempMaxJumpForce += Time.deltaTime * this.jumpChargeSpeed * this.jumpChargeSpeed;
            if (this.tempMaxJumpForce > this.maxJumpForce) {
                this.tempMaxJumpForce = this.maxJumpForce;
            }
        }
    }

    // 检查并触发跳跃
    void checkJump () {
        // 处理多段跳
        if (Input.GetButtonUp ("Jump")) {
            this.onJumpStart ();
        }
    }

    // 在地上
    void onGround () {
        this.checkJump ();
    }
    // 开始跳跃
    void onJumpStart () {
        if (this.leftJumpTimes <= 0) {
            return;
        }
        this.state = State.jumping;
        this.leftJumpTimes -= 1;
        this.tempForce = this.tempMaxJumpForce; // 设置当前蓄力速度
        this.tempMaxJumpForce = this.jumpForce; // 重置蓄力速度
        this.body.localPosition += Vector3.up * Time.deltaTime; // 立即跳一小段距离，防止误判触地
    }
    // 跳跃中
    void onJumping () {
        this.checkJump ();
    }
    // 跳跃下落
    void onJumpFall () { }
    // 结束跳跃
    void onJumpEnd () {
        this.jumpHeight = 0;
        this.tempMaxJumpForce = this.jumpForce;
        this.leftJumpTimes = this.jumpTimes; // 重置多段跳的计数器
        this.state = State.ground;
    }
}