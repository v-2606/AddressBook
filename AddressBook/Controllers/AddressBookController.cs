using AutoMapper;
using BusinessLayer.Interface;
using BusinessLayer.Service;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.DTO;
using ModelLayer.Model;
using RepositoryLayer.Entity;

namespace AddressBook.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class AddressBookController : ControllerBase
    {

        private readonly IAddressBookBL _addressBookBL;
        private readonly IMapper _mapper;
        private readonly IValidator<AddressBookDTO> _validator;

        public AddressBookController(IAddressBookBL addressBookBL, IMapper mapper, IValidator<AddressBookDTO> validator)
        {
            _addressBookBL = addressBookBL;
            _mapper = mapper;
            _validator = validator;
        }

        [Authorize]
        [HttpPost("AddContact")]
        public IActionResult AddContact([FromBody] AddressBookDTO addressBookDTO)
        {
            
            var userId = int.Parse(User.FindFirst("UserId").Value);

            
            var validationResult = _validator.Validate(addressBookDTO);
            if (!validationResult.IsValid)
            {
                return BadRequest(new ResponseModel<object>
                {
                    Success = false,
                    Message = "Validation failed",
                    Data = validationResult.Errors
                });
            }

           
            var result = _addressBookBL.AddContact(addressBookDTO, userId);

            return Ok(new ResponseModel<bool>
            {
                Success = result,
                Message = result ? "Contact added successfully" : "Failed to add contact",
                Data = result
            });
        }


        [Authorize]

        [HttpGet("GetAllContacts")]
        public IActionResult GetAllContacts()
        {
            var userId = int.Parse(User.FindFirst("UserId").Value);
            var data = _addressBookBL.GetAllContacts(userId);
            return Ok(new ResponseModel<List<AddressBookDTO>>
            {
                Success = true,
                Message = "Contacts fetched successfully",
                Data = data
            });
        }

        [Authorize]
        [HttpGet("GetContactById/{id}")]
        public IActionResult GetContactById(int id)
        {
            var userId = int.Parse(User.FindFirst("UserId").Value); 
            var data = _addressBookBL.GetContactById(id, userId);

            return Ok(new ResponseModel<AddressBookDTO>
            {
                Success = data != null,
                Message = data != null ? "Contact found" : "Contact not found",
                Data = data
            });
        }


        [Authorize]
        [HttpPut("UpdateContact/{id}")]
        public IActionResult UpdateContact(int id, [FromBody] AddressBookDTO addressBookDTO)
        {
          
            var userId = int.Parse(User.FindFirst("UserId").Value);

            
            var validationResult = _validator.Validate(addressBookDTO);
            if (!validationResult.IsValid)
            {
                return BadRequest(new ResponseModel<object>
                {
                    Success = false,
                    Message = "Validation failed",
                    Data = validationResult.Errors
                });
            }

          
            var result = _addressBookBL.UpdateContact(id, addressBookDTO, userId);
            return Ok(new ResponseModel<bool>
            {
                Success = result,
                Message = result ? "Contact updated successfully" : "Failed to update contact"
            });
        }



        [Authorize]
        [HttpDelete("DeleteContact/{id}")]
        public IActionResult DeleteContact(int id)
        {
          
            var userId = int.Parse(User.FindFirst("UserId").Value);

            var result = _addressBookBL.DeleteContact(id, userId);
            return Ok(new ResponseModel<bool>
            {
                Success = result,
                Message = result ? "Contact deleted successfully" : "Failed to delete contact"
            });
        }

    }

}





