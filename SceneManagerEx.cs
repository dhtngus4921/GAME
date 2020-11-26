using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerEx
{
    public BaseScene CurrentScene { get { return GameObject.FindObjectOfType<BaseScene>(); } }
   
    public void LoadScene(Define.Scene type)
    {
        Managers.Clear(); //현재 사용 scene을 날려주고
        SceneManager.LoadScene(GetSceneName(type)); //다음 scene으로 이동
    }

    //reflection으로 추출하고자 하는 scene type 추출
    string GetSceneName(Define.Scene type)
    {
        string name = System.Enum.GetName(typeof(Define.Scene), type);
        return name;
    }

    public void Clear()
    {
        CurrentScene.Clear();
    }
}
