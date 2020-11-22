using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class BaseScene : MonoBehaviour
{
    public Define.Scene SceneType { get; protected set; } = Define.Scene.Unknown; //get은 public set은 protected

    //Awake() - 컴포넌트를 끈 상태에서도 작동하고 싶다면, 최상위에 넣어주어도 잘 작동
    void Awake()
    {
        Init();
    }

    //prefab eventsystem 불러오기/찾기, 없다면 새로 만들어줌
    protected virtual void Init() 
    {
        Object obj = GameObject.FindObjectOfType(typeof(EventSystem));  
        if (obj == null)
            Managers.Resource.Instantiate("UI/EventSystem").name = "@EventSystem";  
    }

    public abstract void Clear();
}
