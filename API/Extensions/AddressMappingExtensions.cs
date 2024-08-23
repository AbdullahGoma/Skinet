using API.Dtos;
using Core.Entities;

namespace API.Extensions
{
    public static class AddressMappingExtensions
    {
        public static AddressDto? ToDto(this Address? address)
        {
            return address == null
                ? null
                : new AddressDto
            {
                Line1 = address.Line1,
                Line2 = address.Line2,
                City = address.City,
                State = address.State,
                Country = address.Country,
                PostalCode = address.PostalCode,
            };
        }

        public static Address ToEntity(this AddressDto dto)
        {
            return dto == null
                ? throw new ArgumentNullException(nameof(dto))
                : new Address
            {
                Line1 = dto.Line1,
                Line2 = dto.Line2,
                City = dto.City,
                State = dto.State,
                Country = dto.Country,
                PostalCode = dto.PostalCode,
            };
        }

        public static void UpdateFromDto(this Address address, AddressDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            if (address == null) throw new ArgumentNullException(nameof(address));

            address.Line1 = dto.Line1;
            address.Line2 = dto.Line2;
            address.City = dto.City;
            address.State = dto.State;
            address.Country = dto.Country;
            address.PostalCode = dto.PostalCode;
        }
    }
}