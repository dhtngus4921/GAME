using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 벡터 - 위치, 방향
 * 방향 -> 이동시 사용 (a->b이동시, b에서 a를 뺀 값만큼 이동, vector-)
 * 방향벡터(거리(크기), 실제 방향(x, y, z)) -> vector 함수 사용
 */
public class PlayerController : MonoBehaviour
{
    [SerializeField] //public으로 하지 않아도 사용 가능
    float _speed = 10.0f;


    void Start()
    {
        
    }
    //월드좌표(게임세상), 로컬좌표(캐릭터세상) -> 변환하며 사용 
    float _yAngle = 0.0f;
    void Update()
    {
        _yAngle += Time.deltaTime * _speed;

        //절대 회전값
        //transform.eulerAngles = new Vector3(0.0f, _yAngle, 0.0f);

        //+-delta -> 특정 축을 기준으로 회전
        //transform.Rotate(new Vector3(0.0f, Time.deltaTime * _speed, 0.0f));

        //transform.rotation = Quaternion.Euler(new Vector3(0.0f, _yAngle, 0.0f));

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
