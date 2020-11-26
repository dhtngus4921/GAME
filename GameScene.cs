using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Coroutine
 * - 엄청 오래 걸리는 작업을 잠시 끊거나
 * - 원하는 타이밍에 함수를 잠시 stop/복원 하는 경우
 * - return ->원하는 타입으로 가능 (class 함수도 가능)
 * - 함수의 상태를 저장/복원 하는 기능 
 */

public class GameScene : BaseScene
{
    protected override void Init()
    {
        base.Init();

        SceneType = Define.Scene.Game;

        Managers.UI.ShowSceneUI<UI_Inven>();
    }

    public override void Clear()
    {
        
    }
}
