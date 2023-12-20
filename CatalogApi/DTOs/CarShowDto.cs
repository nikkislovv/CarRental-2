namespace CatalogApi.DTOs
{
    public class CarShowDto
    {
        public Guid Id { get; set; }
        public string VehicleNumber { get; set; }
        public string Title { get; set; }
        public string Model { get; set; }
        public DateTime ReleaseYear { get; set; }
        public string Color { get; set; }
        public decimal RentPrice { get; set; }
        public bool IsAvailable { get; set; }
        public string Image { get; set; }
    }
}
