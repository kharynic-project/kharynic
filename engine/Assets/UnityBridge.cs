namespace org.kharynic
{
    public class UnityBridge : UnityEngine.MonoBehaviour
    {
        [UnityEngine.RuntimeInitializeOnLoadMethod]
        public static void Main()
        {
            var gameObject = new UnityEngine.GameObject(nameof(UnityBridge));
            gameObject.AddComponent<UnityBridge>();
        }

        private void Start()
        {
            Engine.Instance.Main(System.Environment.GetCommandLineArgs());
        }

        private void OnDestroy()
        {
            Engine.Instance.Dispose();
        }
    }
}
