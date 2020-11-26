using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * Scene Manager
 * - 게임 시작 시 캐릭터의 정보를 받아와서 출력해주는 것이 일반적인 상황 
 * 
 * 
 */
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    float _speed = 10.0f;
 
    Vector3 _destPos;

    void Start()
    {
        Managers.Input.MouseAction -= OnMouseClicked;
        Managers.Input.MouseAction += OnMouseClicked;
    }
    public enum PlayerState
    {
        Die, 
        Moving,
        Idle,
    }
    PlayerState _state = PlayerState.Idle;

    void UpdateDie()
    {
        //아무것도 못함
    }

    void UpdateMoving()
    {
        Vector3 dir = _destPos - transform.position;
        if (dir.magnitude < 0.0001f)  //정확하게 0이 나오지 않을 경우가 많기 때문에 작은 값 이용
        {
            _state = PlayerState.Idle; //도착한 상태
        }
        else
        {
            float moveDist = Mathf.Clamp(_speed * Time.deltaTime, 0, dir.magnitude);
            transform.position += dir.normalized * moveDist; //좀 더 가야하는 상태
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 20 * Time.deltaTime);
        }

        Animator anim = GetComponent<Animator>();
        anim.SetFloat("speed", _speed);
    }
    void UpdateIdle()
    {
        Animator anim = GetComponent<Animator>();

        anim.SetFloat("speed", 0);
    }

    void Update()
    {
        switch (_state)
        {
            case PlayerState.Die:
                UpdateDie();
                break;
            case PlayerState.Idle:
                UpdateIdle();
                break;
            case PlayerState.Moving:
                UpdateMoving();
                break;
        }
    }

    void OnMouseClicked(Define.MouseEvent evt)
    {
        if (_state == PlayerState.Die)
            return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(Camera.main.transform.position, ray.direction * 100.0f, Color.red, 1.0f);

        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, 100.0f, LayerMask.GetMask("Wall")))
        {
            _destPos = hit.point;
            _state = PlayerState.Moving;
        }
    }
}
