using System.Collections.Generic;

namespace Framework.Common.IO.Network
{

    public partial class NetworkManager : Template.Singleton<NetworkManager>
    {
        Dictionary<string, string> ServerList = new Dictionary<string, string>();
        public override void DoDestroy()
        {
        }

        public override void OnInitialize()
        {
        }
        public string GetServer(string key)
        {
            if (ServerList.TryGetValue(key, out string address) == false) return string.Empty;
            return address;
        }
        public void AddServer(string key, string address)
        {
            if (string.IsNullOrEmpty(address) == true)
            {
                Debug.LogError("Invalid Address: " + address);
                return;
            }
            if (string.IsNullOrEmpty(key) == true)
            {
                Debug.LogError("Invalid Key: " + key);
                return;
            }
            if (ServerList.ContainsKey(key))
            {
                Debug.LogError("Already Exist Server: " + key);
                return;
            }
            ServerList.Add(key, address);
        }
    }
}