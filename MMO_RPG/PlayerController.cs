using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * state패턴 
 * state 별로 상태 함수 저장 
 * - 상태이동에 따른 정의를 함수로 저장(애니매이터의 화살표 역할)
 * 
 * KeyFrame Animation
 * - 게임에 의존적인 애니매이션
 * ex) 새가 날라가는 경우, 게임 월드에서 돌아다니는 경로는 정의하는 게 아님 
 *     인게임 위치, 날아다니는 경로를 애니매이션으로 지정
 *     즉, 객체에 지정해주는게 아닌 게임 전체에 경로 저장해서 경로대로 움직이게 지정
 * - 수동으로 애니매이션 녹화하고 움직이게 함 
 * 
 * UI
 * # 앵커
 * - 모바일 화면에서 보이는 객체의 보습은 기기마다 다름 "화면 크기에 따른 객체 출력"
 * - 활성화 Rect transform을 부모로 가지고 있어야 활성화 됨 (비율이 개입을 한다는 말) 
 * - 4개의 점이 하나의 모서리에 대응함 (객체)
 * - 앵커 하나의 점 <-> 부모: 비율로 연산 
 * - 앵커 하나의 점 <-> 객체: 고정거리
 * - 화면 크기에 따른 비율을 계산하여 고정거리를 활용해 객체를 화면의 비율에 맞게 출력해줌 
 * 
 * # 버튼 
 * - 게임 내 팝업 창 등을 ui로 지정하여 프리팹으로 두고 사용 가능 
 * 
 * # Sort Order
 * - 팝업 중첩 시 사용
 * - 팝업 시스템 이용 시 화면에 뜨는 ui는 2개다. (더 위에 뜨는 팝업을 정할 시 사용)
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

        Managers.Resource.Instantiate("UI/UI_Button");
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
