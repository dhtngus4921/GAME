using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * prefab = 붕어빵틀 
 * nested prefab = 붕어빵틀들을 합친 붕어빵틀 
 * 
 * 충돌 - collision(물리), Trigger(충돌 판단 확인)
 * 
 * Raycasting 
 *  - 필요성(2d 화면 내에서 3d를 구현할 때 특정 객체를 선택할 수 있는가?)
 *  - 카메라를 기준으로 2d 좌표를 누르면 레이저 쏘기 시작, collider에 닿으면 좌표 추출
 *  - Physics.Raycast
 *  - 물체가 플레이어를 막고 있을 때 레이저를 활용해 중간 물체가 인식되면 카메라 위치 변경 (물체보다 더 가까이)
 * 
 * 투영 
 *  - local <-> world <-> viewport <-> screen(화면)
 *  
 */
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    float _speed = 10.0f;

    void Start()
    {
        Managers.Input.KeyAction -= OnKeyBoard; //다른 구독 시 해제하고 실행
        Managers.Input.KeyAction += OnKeyBoard; //input매니저에게 키가 눌리면 함수 실행 지시
    }

    void Update()
    {
       

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

    }
}
