using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RepositoryLayer.Entity;


namespace RepositoryLayer.Interface
{
public interface IAddressBookRL
    {

        bool AddContact(AddressBookEntity contact);
        List<AddressBookEntity> GetAllContacts(int userId);
        AddressBookEntity GetContactById(int contactId,int id);
        bool UpdateContact(int id, AddressBookEntity updatedContact, int userId);
        bool DeleteContact(int contactId, int id);
    }
}
