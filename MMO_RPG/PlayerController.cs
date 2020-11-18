using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * ##raycasting = 3d 표면을 이용하는 코드에 주로 사용 (collider와 함께 사용)
 * -> raycasting을 이용하여 plane에 레이저가 닿으면 해당 지점으로 플레이어 이동
 * -> 공격시 플레이어가 맞으면 0> 공격하는 물체의 레이저가 닿으면 플레이어 rp 감소 
 * -> 카메라와 플레이어 사이에 장애물이 레이저에 감지되면 카메라 확대하여 플레이어가 보이게 이용
 * 
 * ##연산 개수가 증가한다면(물체증가) 과부하 가능성 있음
 * -> layer 사용 시 raycasting 연산을 사용하고 싶은 객체만 골라서 사용 가능 
 * -> 최적화 과정 .. -> 영역을 나눠서 연산 
 * 방법 1) 비트 사용 int mask = (1 >> 8) , 8번째 layer에 raycasting 연산을 할 객체 저장 
 * 방법 2) LayerMask기능 사용
 * 
 * ##Tag 
 * - 객체 대한 추가적인 정보 저장 (태그를 활용한 코드도 있으니 저장해서 활용)
 * - 지정 안해서 오류 남 (Null Reference Exception)
 * 
 * 
 */
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    float _speed = 10.0f;
    
    bool _moveToDest = false;
    Vector3 _destPos;

    void Start()
    {
        Managers.Input.KeyAction -= OnKeyBoard; //다른 구독 시 해제하고 실행
        Managers.Input.KeyAction += OnKeyBoard; //input매니저에게 키가 눌리면 함수 실행 지시
        Managers.Input.MouseAction -= OnMouseClicked;
        Managers.Input.MouseAction += OnMouseClicked;
    }

    void Update()
    {
        if (_moveToDest)
        {
            Vector3 dir = _destPos - transform.position;
            if(dir.magnitude < 0.0001f)  //정확하게 0이 나오지 않을 경우가 많기 때문에 작은 값 이용
            {
                _moveToDest = false; //도착한 상태
            }
            else
            {
                float moveDist = Mathf.Clamp(_speed * Time.deltaTime, 0, dir.magnitude);
                transform.position += dir.normalized * moveDist; //좀 더 가야하는 상태
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 20 * Time.deltaTime);
            }
        }
        if (_moveToDest)
        {
            Animator anim = GetComponent<Animator>();
            anim.Play("RUN");
        }
        else
        {
            Animator anim = GetComponent<Animator>();
            anim.Play("Wait");
        }
    }
    void OnKeyBoard()
    {
        if (Input.GetKey(KeyCode.W)) //원하는 방향을 보며 이동(lookrotation), 블렌딩(slerp)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.forward), 0.2f);
            transform.position += Vector3.forward * Time.deltaTime * _speed;
        }

        if (Input.GetKey(KeyCode.S))
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.back), 0.2f);
            transform.position += Vector3.back * Time.deltaTime * _speed;
        }

        if (Input.GetKey(KeyCode.A))
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.left), 0.2f);
            transform.position += Vector3.left * Time.deltaTime * _speed;
        }

        if (Input.GetKey(KeyCode.D))
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.right), 0.2f);
            transform.position += Vector3.right * Time.deltaTime * _speed;
        }
        _moveToDest = false;

    }

    void OnMouseClicked(Define.MouseEvent evt)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(Camera.main.transform.position, ray.direction * 100.0f, Color.red, 1.0f);

        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, 100.0f, LayerMask.GetMask("Wall")))
        {
            _destPos = hit.point;
            _moveToDest = true;
        }
    }
}
