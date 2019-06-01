using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using CarDealer.DTO.Import;
using CarDealer.Models;

namespace CarDealer
{
    public class CarDealerProfile : Profile
    {
        public CarDealerProfile()
        {
            this.CreateMap<Car, CarDto>()
                .ForMember(x => x.PartsId, y => y.MapFrom(s => s.PartCars.Select(p => p.PartId)));

        }
    }
}
