using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoginScene : BaseScene
{
    /*
     * 특정 scene에서 다른 scene으로 이동할 경우
     * - Build Settings가 필요하다.
     * - SceneManager.LoadScene("a");
     * 
     */
    protected override void Init()
    {
        base.Init();

        SceneType = Define.Scene.Login;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Managers.Scene.LoadScene(Define.Scene.Game);
        }
    }

    public override void Clear()
    {
        Debug.Log("LoginScene clear!");
    }
}
