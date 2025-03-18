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
using System.Text.Json;

namespace RepositoryLayer
{
    public class AddressBookRL : IAddressBookRL
    {
        private readonly UserContext _context;
        private readonly RedisCacheService _redisCache;
        private readonly ILogger<AddressBookRL> _logger;

        public AddressBookRL(UserContext context, RedisCacheService redisCache, ILogger<AddressBookRL> logger)
        {
            _context = context;
            _redisCache = redisCache;
            _logger = logger;
        }

        public bool AddContact(AddressBookEntity contact)
        {
            
            _context.AddressBookEntries.Add(contact);
            _context.SaveChanges();


            _redisCache.RemoveData($"contact_list_{contact.UserId}");
            _logger.LogInformation($"Cache invalidated for user {contact.UserId} contact list.");

            return true;
        }

        
        public List<AddressBookEntity> GetAllContacts(int userId)
        {
            string cacheKey = $"contact_list_{userId}";

            var cachedData = _redisCache.GetData(cacheKey);
            if (!string.IsNullOrEmpty(cachedData))
            {
                
                return JsonSerializer.Deserialize<List<AddressBookEntity>>(cachedData)!;
            }

          
            var contacts = _context.AddressBookEntries.Where(c => c.UserId == userId).ToList();

            if (contacts.Count > 0)
            {
                _redisCache.SetData(cacheKey, JsonSerializer.Serialize(contacts), TimeSpan.FromMinutes(10));
                _logger.LogInformation($"Contacts for User {userId} stored in Redis Cache.");
            }

            return contacts;
        }


        public AddressBookEntity GetContactById(int contactId, int userId)
        {
            string cacheKey = $"contact_{contactId}";

            var cachedData = _redisCache.GetData(cacheKey);
            if (!string.IsNullOrEmpty(cachedData))
            {
                _logger.LogInformation($"Fetched Contact ID {contactId} from Redis Cache.");
                return JsonSerializer.Deserialize<AddressBookEntity>(cachedData)!;
            }

            _logger.LogWarning($"Cache miss Fetching Contact ID {contactId} from Database.");
            var contact = _context.AddressBookEntries.FirstOrDefault(c => c.Id == contactId && c.UserId == userId);

            if (contact != null)
            {
                _redisCache.SetData(cacheKey, JsonSerializer.Serialize(contact), TimeSpan.FromMinutes(10));
                _logger.LogInformation($"Contact ID {contactId} stored in Redis Cache.");
            }

            return contact!;
        }


        public bool UpdateContact(int contactId, AddressBookEntity updatedContact, int userId)
        {
            _logger.LogInformation($"Update request received for Contact ID: {contactId}");

            var existingContact = _context.AddressBookEntries.FirstOrDefault(c => c.Id == contactId && c.UserId == userId);

            if (existingContact != null)
            {
                existingContact.Name = updatedContact.Name;
                existingContact.Email = updatedContact.Email;
                existingContact.Phone = updatedContact.Phone;

                _context.SaveChanges();
                _logger.LogInformation($"Contact ID {contactId} updated successfully.");

                // Cache Invalidate
                _redisCache.RemoveData($"contact_list_{userId}");
                _redisCache.RemoveData($"contact_{contactId}");
                _logger.LogInformation($"Cache invalidated for Contact ID {contactId} and user {userId} contacts list.");

                return true;
            }

            return false;
        }




        public bool DeleteContact(int contactId, int userId)
        {
            _logger.LogInformation($"Delete request received for Contact ID: {contactId}");

            var contact = _context.AddressBookEntries.FirstOrDefault(c => c.Id == contactId && c.UserId == userId);

            if (contact != null)
            {
                _context.AddressBookEntries.Remove(contact);
                _context.SaveChanges();
                _logger.LogInformation($"Contact ID {contactId} deleted successfully from Database.");

                // Cache Invalidate
                _redisCache.RemoveData($"contact_list_{userId}");
                _redisCache.RemoveData($"contact_{contactId}");
                _logger.LogInformation($"Cache invalidated for Contact ID {contactId} and user {userId} contacts list.");

                return true;
            }

            return false;
        }
    }

}


