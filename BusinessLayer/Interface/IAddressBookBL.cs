using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelLayer.DTO;
using RepositoryLayer.Entity;

namespace BusinessLayer.Interface
{
    public interface IAddressBookBL
    {
        bool AddContact( AddressBookDTO addressBookDTO, int userId);
        List<AddressBookDTO> GetAllContacts(int userId);
        AddressBookDTO GetContactById(int contactId,int id);
        bool UpdateContact(int id, AddressBookDTO addressBookDTO, int userId);
        bool DeleteContact(int contactId, int id);
     
    }
}
