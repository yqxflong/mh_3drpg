namespace _HotfixScripts.Utils
{
    public interface IHotfixUpdate
    {
        void Update();
    }
    
    public interface IHotfixLateUpdate
    {
        void LateUpdate();
    }
    
    public interface IHotfixFixedUpdate
    {
        void FixedUpdate();
    }
}