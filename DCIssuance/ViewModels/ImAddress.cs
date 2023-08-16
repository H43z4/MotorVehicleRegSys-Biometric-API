using Models.ViewModels.VehicleRegistration.Core;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DNPIssuance.ViewModels
{
    public class ImAddress:CommonFeature
    {
        public long AddressId { get; set; }
        public string AddressDescription { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; } 
        public long DistrictId { get; set; } 
        public string District { get; set; }
        public long AddressTypeId { get; set; }
    }
}
