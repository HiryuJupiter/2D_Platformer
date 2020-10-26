using UnityEngine;

public class GameInput : MonoBehaviour
{
    public static float MoveX       { get; private set; }
    public static float MoveY       { get; private set; }
    public static bool JumpBtnDown  { get; private set; }
    public static bool JumpBtn      { get; private set; }
    public static bool JumpBtnUp    { get; private set; }
    public static bool PressedLeft  => MoveX < -0.1f;
    public static bool PressedRight => MoveX >  0.1f;
    public static bool PressedDown  => MoveY < -0.1f;
    public static bool PressedUp    => MoveY <  0.1f;

    void Update()
    {
        DirectionInputUpdate();
        ActionInputUpdate();
    }

    //Sets the movement values based on inputs
    void DirectionInputUpdate ()
    {
        //LEFT - RIGHT
        if (Input.GetKey(KeyScheme.Left))
        {
            MoveX = -1f;
        }
        else if (Input.GetKey(KeyScheme.Right))
        {
            MoveX = 1f;
        }
        else
        {
            MoveX = 0f;
        }

        //UP - DOWN
        if (Input.GetKey(KeyScheme.Up))
        {
            MoveY = 1f;
        }
        else if (Input.GetKey(KeyScheme.Down))
        {
            MoveY = -1f;
        }
        else
        {
            MoveY = 0f;
        }
    }

    //Detects jump key press status
    void ActionInputUpdate ()
    {
        JumpBtnDown = Input.GetKeyDown(KeyScheme.Jump);
        JumpBtn     = Input.GetKey(KeyScheme.Jump);
        JumpBtnUp   = Input.GetKeyUp(KeyScheme.Jump);
    }

    //private void OnGUI()
    //{
    //    GUI.Label(new Rect(20, 20, 500, 20), "UP " + KeyScheme.Up);
    //    GUI.Label(new Rect(20, 40, 500, 20), "DOWN " + KeyScheme.Down);
    //    GUI.Label(new Rect(20, 60, 500, 20), "Left " + KeyScheme.Left);
    //    GUI.Label(new Rect(20, 80, 500, 20), "Right " + KeyScheme.Right);
    //    GUI.Label(new Rect(20, 110, 500, 20), "MoveX " + MoveX);
    //    GUI.Label(new Rect(20, 130, 500, 20), "MoveY " + MoveY);
    //}
}