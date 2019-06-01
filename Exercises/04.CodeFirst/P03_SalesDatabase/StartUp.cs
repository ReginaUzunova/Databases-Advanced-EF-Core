using P03_SalesDatabase.Data;
using P03_SalesDatabase.Data.Models;
using System;

namespace P03_SalesDatabase
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            using (var db = new SalesContext())
            {
                var store = new Store
                {
                    Name = "Magazin"
                };

                db.Stores.Add(store);

                db.SaveChanges();
            }
        }
    }
}
