
namespace LT.Component.EmailAuth
{
    public class Account
    {
        private POPClient m_popClient = new POPClient();

        public bool Authentic(string account, string password, string host, int port)
        {
            m_popClient.Disconnect();
            if (m_popClient.Connect(host, port))
            {
                if (m_popClient.Authenticate(account, password))
                {
                    return true;
                }
                return false;
            }
            return false;
        }

        ~Account()
        {
            m_popClient.Disconnect();
        }
    }
}
