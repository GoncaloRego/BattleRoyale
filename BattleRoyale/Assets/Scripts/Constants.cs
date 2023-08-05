using UnityEngine;
using UnityEngine.InputSystem;

namespace Constants
{
    public static class PlayerAnimatorParameters
    {
        public const string Speed = "Speed";
        public const string IsCrouching = "IsCrouching";
        public const string CrouchIdle = "CrouchIdle";
        public const string CrouchToStand = "CrouchToStand";
        public const string Jump = "Jump";
    }

    public static class PlayerControls
    {
        public const KeyCode Key_Jump = KeyCode.Space;
        public const KeyCode Key_Crouch = KeyCode.LeftControl;
    }

    public static class Tags
    {
        public const string Ground = "Ground";
    }

    public static class Animations
    {
        public const string Jump = "Jump";
    }
}
