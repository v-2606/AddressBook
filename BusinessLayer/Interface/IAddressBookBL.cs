﻿using System;
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
        bool AddContact(AddressBookDTO addressBookDTO);
        List<AddressBookDTO> GetAllContacts();
        AddressBookDTO GetContactById(int id);
        bool UpdateContact(int id, AddressBookDTO addressBookDTO);
        bool DeleteContact(int id);
     
    }
}
