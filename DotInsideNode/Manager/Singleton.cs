
namespace DotInsideNode
{
    public class Singleton<T> where T : new()
    {
        static T __instance = new T();
        public static T Instance
        {
            get => __instance;
            set => __instance = value;
        }
    }
}
