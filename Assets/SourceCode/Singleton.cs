public class CSingleton<T> where T : new()
{
    private static T m_pInstance;

    public static T Instance
    {
        get
        {
            if (m_pInstance == null)
            {
                m_pInstance = new T();
            }
            return m_pInstance;
        }
    }

    public virtual void Initialize() {}
    public virtual void Destroy() {}

}