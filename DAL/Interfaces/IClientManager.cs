using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DAL.Entities;

namespace DAL.Interfaces
{
    public interface IClientManager : IDisposable
    {
        void Create(ClientProfile item);
        ClientProfile Find(string id);
        void Remove(ClientProfile item); 
        List<ClientProfile> GetAll();
    }
}
