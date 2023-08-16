namespace DNPIssuance.ViewModels
{
    public class ImVehicle
    {
        public long VehicleId { get; set; }
        public string VehicleClass { get; set; }
        public string VehicleBodyConvention { get; set; }
        public string VehicleBodyType { get; set; }
        public string VehicleMake { get; set; }
        public string VehicleMaker { get; set; }
        public long ManufacturingYear { get; set; }
        public long NoOfCylinder { get; set; }
        public long EngineSize { get; set; }
        public string ChasisNo { get; set; }
        public string EngineNo { get; set; }
        public long SeatingCapacity { get; set; }
        public string VehicleFuelType { get; set; }
        public string VehicleColor { get; set; }
        public long Price { get; set; }
        public string VehiclePurchaseType { get; set; }
        public DateTime PurchaseDate { get; set; }
        public DateTime TaxPaidUpto { get; set; }
    }
}
