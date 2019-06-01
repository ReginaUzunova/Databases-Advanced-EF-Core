using System;
using System.Collections.Generic;
using System.Text;

namespace CarDealer.DTO.Import
{
    public class CarDto
    {
        public CarDto()
        {
            this.PartsId = new List<int>();
        }

        public string Make { get; set; }

        public string Model { get; set; }

        public long TravelledDistance { get; set; }

        public List<int> PartsId { get; set; }
    }
}
