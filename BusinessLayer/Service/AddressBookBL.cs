using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BusinessLayer.Interface;
using ModelLayer.DTO;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;

namespace BusinessLayer.Service
{
    public class AddressBookBL : IAddressBookBL
    {

        private readonly IAddressBookRL _addressBookRL;
        private readonly IMapper _mapper;
        public AddressBookBL(IAddressBookRL addressBookRL, IMapper mapper)
        {
            _addressBookRL = addressBookRL;
            _mapper = mapper;
        }

        public bool AddContact(AddressBookDTO addressBookDTO , int userId)
        {
            var entity = _mapper.Map<AddressBookEntity>(addressBookDTO);
            entity.UserId = userId;
            return _addressBookRL.AddContact( entity);
        }

        public List<AddressBookDTO> GetAllContacts(int userId)
        {
            var entityList = _addressBookRL.GetAllContacts( userId);
            return _mapper.Map<List<AddressBookDTO>>(entityList);
        }


        public AddressBookDTO GetContactById(int contactId, int userId)
        {
            var entity = _addressBookRL.GetContactById(contactId, userId);
            return _mapper.Map<AddressBookDTO>(entity);
        }

        public bool UpdateContact(int contactId, AddressBookDTO addressBookDTO ,int userId)
        {
            var entity = _mapper.Map<AddressBookEntity>(addressBookDTO);
            return _addressBookRL.UpdateContact(contactId, entity, userId);
        }


        public bool DeleteContact(int contactId, int userId)
        {
            return _addressBookRL.DeleteContact(contactId, userId);
        }

    }
}