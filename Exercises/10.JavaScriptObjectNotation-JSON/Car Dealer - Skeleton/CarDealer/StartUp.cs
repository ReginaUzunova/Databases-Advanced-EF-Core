using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using CarDealer.Data;
using CarDealer.DTO.Import;
using CarDealer.Models;
using Newtonsoft.Json;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            var context = new CarDealerContext();

            //Mapper.Initialize(cfg => cfg.AddProfile(new CarDealerProfile()));

            //09
            //var suppliersJson = File.ReadAllText(@"D:\DatabasesAdvanced-EF-February2019\10.JavaScriptObjectNotation-JSON\Car Dealer - Skeleton\CarDealer\Datasets\suppliers.json");
            //var result = ImportSuppliers(context, suppliersJson);

            //10
            //var partsJson = File.ReadAllText(@"D:\DatabasesAdvanced-EF-February2019\10.JavaScriptObjectNotation-JSON\Car Dealer - Skeleton\CarDealer\Datasets\parts.json");
            //var result = ImportParts(context, partsJson);

            var carsJson = File.ReadAllText(@"D:\DatabasesAdvanced-EF-February2019\10.JavaScriptObjectNotation-JSON\Car Dealer - Skeleton\CarDealer\Datasets\cars.json");
            var result = ImportCars(context, carsJson);
        }

        public static string ImportSuppliers(CarDealerContext context, string inputJson)
        {
            Supplier[] suppliers = JsonConvert.DeserializeObject<Supplier[]>(inputJson);

            context.Suppliers.AddRange(suppliers);
            context.SaveChanges();

            return $"Successfully imported {suppliers.Count()}.";
        }

        public static string ImportParts(CarDealerContext context, string inputJson)
        {
            Part[] parts = JsonConvert.DeserializeObject<Part[]>(inputJson);
            List<Part> correctParts = new List<Part>();

            foreach (var item in parts)
            {
                if (context.Suppliers.Any(s => s.Id == item.SupplierId))
                {
                    correctParts.Add(item);
                }
            }

            context.Parts.AddRange(correctParts);
            context.SaveChanges();

            return $"Successfully imported {correctParts.Count()}.";
        }

        public static string ImportCars(CarDealerContext context, string inputJson)
        {
            var cars = JsonConvert.DeserializeObject<CarDto[]>(inputJson);
            var partCars = new List<PartCar>();


            foreach (var car in cars)
            {
                var currentCar = new Car()
                {
                    Make = car.Make,
                    Model = car.Model,
                    TravelledDistance = car.TravelledDistance
                };

                context.Cars.Add(currentCar);

                int carId = context.Cars
                    .Where(x => x.Make == car.Make
                        && x.Model == car.Model
                        && x.TravelledDistance == car.TravelledDistance)
                    .Select(y => y.Id)
                    .FirstOrDefault();

                foreach (var partId in car.PartsId)
                {
                    var currentPart = new PartCar()
                    {
                        Car = currentCar,
                        Part = context.Parts.FirstOrDefault(x => x.Id == partId)
                    };
                   
                    
                }


                context.SaveChanges();
            }




            return $"Successfully imported {partCars.Count()}.";


        }
    }
}