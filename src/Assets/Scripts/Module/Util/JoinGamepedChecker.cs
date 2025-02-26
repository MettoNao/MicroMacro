using UnityEngine;

public class JoinGamepedChecker
{
    public bool CheckJoinGameped()
    {
        string[] joystickNames = Input.GetJoystickNames();

        if (joystickNames.Length > 0)
        {
            // 接続されているジョイスティックを確認
            bool isGamepadConnected = false;

            foreach (string name in joystickNames)
            {
                if (!string.IsNullOrEmpty(name))
                {
                    isGamepadConnected = true;
                    break;
                }
            }

            if (isGamepadConnected)
            {
               // Debug.Log("ゲームパッドが接続されています。");
            }
            else
            {
                Debug.Log("ジョイスティックは検出されましたが、名前が空です。");
            }

            return isGamepadConnected;
        }
        else
        {
            Debug.Log("ゲームパッドは接続されていません。");

            return false;
        }
    }
}
