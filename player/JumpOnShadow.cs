using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 【带阴影的跳跃】
/// 支持阴影缩放和阴影的透明度变化
/// </summary>
public class JumpOnShadow : MonoBehaviour {
    public float force = 10; // 跳跃力
    public Transform body; // 身体
    public Transform shadow; // 阴影

    public enum State { ground, jump }

    private State state = State.ground;
    private float jumpHeight; // 当前跳跃的高度
    private float tempForce; // 当前跳跃的加速度
    private float shadowScaleX; // 记录阴影开始时的 X 值
    private float shadowScaleY; // 记录阴影开始时的 Y 值
    private float shadowAlpha; // 记录阴影开始时的 Alpha 值（SpriteRenderer）
    private SpriteRenderer shadowSpriteRenderer; // 阴影的 SpriteRenderer

    // Use this for initialization
    void Start () {
        this.shadowScaleX = this.shadow.localScale.x;
        this.shadowScaleY = this.shadow.localScale.y;
        this.shadowSpriteRenderer = this.shadow.GetComponent<SpriteRenderer> ();
        this.shadowAlpha = this.shadowSpriteRenderer.color.a;
        this.jumpHeight = 0;
    }
    // Update is called once per frame
    void LateUpdate () {
        if (this.state == State.ground) {
            float isJump = Input.GetAxis ("Jump");
            if (isJump > 0) {
                this.state = State.jump;
                this.tempForce = this.force;
                this.body.localPosition += Vector3.up * Time.deltaTime;
            }
            // reset shadow scale
            this.shadow.localScale = new Vector3 (this.shadowScaleX, this.shadowScaleY, 0);
        } else if (this.state == State.jump) {
            tempForce -= 9.8f * Time.deltaTime;
            float height = this.jumpHeight + tempForce * Time.deltaTime;
            if (height <= 0) {
                this.jumpHeight = 0;
                this.state = State.ground;
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
}