using UnityEngine;

namespace org.kharynic.unity
{
    public class FreeMotionController : MonoBehaviour
    {
        public float RotationMultiplier = 4;
        public float MoveMultiplier = 0.2f;
        public float FastMoveMultiplier = 3f;
        
        private void Update()
        {
            transform.rotation = Quaternion.Euler(
                transform.eulerAngles.x - Input.GetAxis(DefaultVirtualAxes.MouseY) * RotationMultiplier, 
                transform.eulerAngles.y + Input.GetAxis(DefaultVirtualAxes.MouseX) * RotationMultiplier, 
                0);
            var multiplier = Input.GetKey(KeyCode.LeftShift) ? FastMoveMultiplier : MoveMultiplier;
            transform.position += (transform.forward * Input.GetAxis(DefaultVirtualAxes.Vertical) + 
                                   transform.right * Input.GetAxis(DefaultVirtualAxes.Horizontal)) * 
                                  multiplier;
        }
    }
}
