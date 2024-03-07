using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MoveWall : MonoBehaviour
{
    private Vector3 _firstPosition = new Vector3(0.0f, 0.0f, 0.0f);
    private bool _canMove = true;

    // 振動用変数
    private Tweener _shakeTweener;
    private float duration = 0.5f;
    private float strength = 10.0f;
    private int vibrato = 10;
    private float randomness = 45.0f;
    private bool fadeOut = true;

    // 回転用変数
    private readonly float MAX_rotate = 10.0f;
    private float _floatAngleX = 0.0f;
    private float _floatAngleZ = 0.0f;
    private float _rotationSpeed = 20.0f;


    /// <summary>
    /// 箱を揺らす
    /// </summary>
    public void ShakeWalls()
    {
        if (!_canMove) return;
        // 揺らしている間に座標が変わらないように、一時的にフラグを変える
        _canMove = false;
        _firstPosition = this.transform.position;
        // 前の処理が残って入れば初期位置に戻す
        if (_shakeTweener != null)
        {
            _shakeTweener.Kill();
            gameObject.transform.position = _firstPosition;
        }

        // 揺らす
        _shakeTweener = this.transform.DOShakePosition(duration, strength, vibrato, randomness, fadeOut);
        _canMove = true;
    }

    /// <summary>
    /// 箱の回転
    /// </summary>
    /// <param name="isright"></param>
    public void RotateWallX(bool isright)
    {
        if (!_canMove) return;
        float angle = _rotationSpeed * Time.deltaTime;
        // 壁の中心座標
        var wallPos = new Vector3(0,0,450);
        if (isright)
        {
            _floatAngleX += angle;
            if(_floatAngleX <= MAX_rotate)
            {
                this.transform.RotateAround(wallPos, Vector3.right, angle);
            }
            else
            {
                _floatAngleX = MAX_rotate;
            }
        }
        else if (!isright)
        {
            if (!_canMove) return;
            _floatAngleX -= angle;
            if(-MAX_rotate <= _floatAngleX)
            {
                this.transform.RotateAround(wallPos, Vector3.left, angle);
            }
            else
            {
                _floatAngleX = -MAX_rotate;
            }
        }
    }

    public void RotateWallZ(bool isright)
    {
        if (!_canMove) return;
        float angle = _rotationSpeed * Time.deltaTime;

        // 壁の中心座標
        var wallPos = new Vector3(0, 0, 450);
        if (isright)
        {
            _floatAngleZ += angle;
            if (_floatAngleZ <= MAX_rotate)
            {
                this.transform.RotateAround(wallPos, Vector3.forward, angle);
            }
            else
            {
                _floatAngleZ = MAX_rotate;
            }
        }
        else if (!isright)
        {
            _floatAngleZ -= angle;
            if (-MAX_rotate <= _floatAngleZ)
            {
                this.transform.RotateAround(wallPos, Vector3.back, angle);
            }
            else
            {
                _floatAngleZ = -MAX_rotate;
            }
        }
    }

    /// <summary>
    /// 箱を動かせるかのフラグを受け取る
    /// </summary>
    /// <param name="canMove"></param>
    public void SetCanFallFlag(bool canMove)
    {
        _canMove = canMove;
    }

    /// <summary>
    /// 箱の座標、角度の初期化
    /// </summary>
    public void InitializeWall()
    {
        this.transform.position = Vector3.zero;
        this.transform.rotation = Quaternion.Euler(0, 0, 0);
    }
}
