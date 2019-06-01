using AutoMapper;
using CarDealer.Data;
using CarDealer.Dtos.Import;
using CarDealer.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            Mapper.Initialize(x =>
            {
                x.AddProfile<CarDealerProfile>();
            });

            var suppliersXml = File.ReadAllText(@"D:\DatabasesAdvanced-EF-February2019\11.ExtensibleMarkupLanguage-XML\CarDealer - Skeleton\CarDealer\Datasets\suppliers.xml");
            var partsXml = File.ReadAllText(@"D:\DatabasesAdvanced-EF-February2019\11.ExtensibleMarkupLanguage-XML\CarDealer - Skeleton\CarDealer\Datasets\parts.xml");
            var carsXml = File.ReadAllText(@"D:\DatabasesAdvanced-EF-February2019\11.ExtensibleMarkupLanguage-XML\CarDealer - Skeleton\CarDealer\Datasets\cars.xml");

            using (CarDealerContext context = new CarDealerContext())
            {
                //09
                //var suppliers = ImportSuppliers(context, suppliersXml);

                //10
                //var parts = ImportParts(context, partsXml);

                //11
                var cars = ImportCars(context, carsXml);
            }
        }

        public static string ImportSuppliers(CarDealerContext context, string inputXml)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportSupplierDto[]),
                new XmlRootAttribute("Suppliers"));

            var suppliersDto = (ImportSupplierDto[])xmlSerializer.Deserialize(new StringReader(inputXml));

            var suppliers = new List<Supplier>();

            foreach (var supplierDto in suppliersDto)
            {
                var supplier = Mapper.Map<Supplier>(supplierDto);
                suppliers.Add(supplier);
            }

            context.Suppliers.AddRange(suppliers);
            context.SaveChanges();

            return $"Successfully imported {suppliers.Count}";
        }

        public static string ImportParts(CarDealerContext context, string inputXml)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportPartDto[]),
                new XmlRootAttribute("Parts"));

            var partsDto = (ImportPartDto[])xmlSerializer.Deserialize(new StringReader(inputXml));

            var parts = new List<Part>();

            foreach (var partDto in partsDto)
            {
                var supplier = context.Suppliers.Find(partDto.SupplierId);

                if (supplier == null)
                {
                    continue;
                }

                var part = Mapper.Map<Part>(partDto);
                parts.Add(part);
            }

            context.Parts.AddRange(parts);
            context.SaveChanges();

            return $"Successfully imported {parts.Count}";
        }

        public static string ImportCars(CarDealerContext context, string inputXml)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportCarDto[]),
                new XmlRootAttribute("Cars"));

            var carsDto = (ImportCarDto[])xmlSerializer.Deserialize(new StringReader(inputXml));

            var cars = new List<Car>();

            foreach (var carDto in carsDto)
            {
                var car = Mapper.Map<ImportCarDto, Car>(carDto);
                cars.Add(car);

                var partIds = carDto
                    .CarParts
                    .Distinct()
                    .ToList();

                if (partIds == null)
                {
                    continue;
                }

                foreach (var partId in partIds)
                {
                    var part = Mapper.Map<ImportCarPartDto, Part>(partId);

                    var current = new PartCar()
                    {
                        Car = car,
                        PartId = partId.PartId
                    };

                    car.PartCars.Add(current);
                }

                context.Cars.AddRange(cars);
                context.SaveChanges();
            }

            

            return $"Successfully imported {cars.Count}";
        }
    }
}