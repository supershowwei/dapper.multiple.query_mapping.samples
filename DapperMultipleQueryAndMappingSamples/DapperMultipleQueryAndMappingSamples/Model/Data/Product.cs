namespace DapperMultipleQueryAndMappingSamples.Model.Data
{
    public abstract class Product
    //public class Product
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public int Price { get; set; }
    }
}