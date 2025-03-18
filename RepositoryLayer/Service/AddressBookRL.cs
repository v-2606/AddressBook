using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RepositoryLayer.Context;
using RepositoryLayer.Entity;
using RepositoryLayer;
using RepositoryLayer.Interface;
using RepositoryLayer.Service;

namespace RepositoryLayer
{
    public class AddressBookRL : IAddressBookRL
    {
        private readonly UserContext _context;

        public AddressBookRL(UserContext context)
        {
            _context = context;
        }

        public bool AddContact(AddressBookEntity contact)
        {
            
            _context.AddressBookEntries.Add(contact);
            _context.SaveChanges();
            return true;
        }

        public List<AddressBookEntity> GetAllContacts(int userId)
        {
            return _context.AddressBookEntries
                           .Where(contact => contact.UserId == userId) 
                           .ToList();
        }

        public AddressBookEntity GetContactById(int contactId, int userId)
        {
            return _context.AddressBookEntries
                           .FirstOrDefault(c => c.Id == contactId && c.UserId == userId);
        }


        public bool UpdateContact(int contactId, AddressBookEntity updatedContact, int userId)
        {
            
            var existingContact = _context.AddressBookEntries
                                          .FirstOrDefault(c => c.Id == contactId && c.UserId == userId);

            if (existingContact != null)
            {
               
                existingContact.Name = updatedContact.Name;
                existingContact.Email = updatedContact.Email;
                existingContact.Phone = updatedContact.Phone;

                _context.SaveChanges(); 
                return true;
            }
            return false; 
        }




        public bool DeleteContact(int contactId, int userId)
        {
          
            var contact = _context.AddressBookEntries
                                  .FirstOrDefault(c => c.Id == contactId && c.UserId == userId);

            if (contact != null)
            {
                _context.AddressBookEntries.Remove(contact); 
                _context.SaveChanges(); 
                return true;
            }
            return false; 
        }

    }
}

