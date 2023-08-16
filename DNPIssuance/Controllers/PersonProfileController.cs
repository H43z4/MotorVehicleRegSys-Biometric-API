using Inquiry.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.ViewModels.Identity;
using Models.ViewModels.VehicleRegistration.Core;
using SharedLib.APIs;
using SharedLib.Common;
using SharedLib.Security;

namespace Inquiry.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.JWT_BEARER_TOKEN_STATELESS)]
    public class PersonProfileController : Controller
    {
        private readonly IPersonProfileService _personService;
        public VwUser User
        {
            get
            {
                return (VwUser)this.Request.HttpContext.Items["User"];
            }
        }
        public PersonProfileController(IPersonProfileService service)
        {
            this._personService = service;
        }
        [HttpGet(Name = "GetOwnersDropDowns")]
        public async Task<ApiResponse> GetOwnersDropDowns()
        {
            var ds = await this._personService.GetOwnersDropDowns();

            var data = new
            {
                OwnerTaxGroups = ds.Tables[1],
                Countries = ds.Tables[2],
                Districts = ds.Tables[3],
                businessSector = ds.Tables[4],
                businessStatus = ds.Tables[5],
                businessType = ds.Tables[6],
                tehsils = ds.Tables[7],
                addressArea = ds.Tables[8],
                PostOffice = ds.Tables[9]
            };

            return ApiResponse.GetApiResponse(ApiResponseType.SUCCESS, data, Constants.RECORD_FOUND_MESSAGE);
        }
        [HttpGet(Name = "GetOwnerByCNIC")]
        public async Task<ApiResponse> GetOwnerByCNIC(string cnic)
        {
            var ds = await this._personService.GetPersonByCNIC(cnic);

            var person = SharedLib.Common.Extentions.ToList<VwPerson>(ds.Tables[0]).FirstOrDefault();

            if (person is not null)
            {
                person.Addresses = SharedLib.Common.Extentions.ToList<VwAddress>(ds.Tables[1]).ToList();
                person.PhoneNumbers = SharedLib.Common.Extentions.ToList<VwPhoneNumber>(ds.Tables[2]).ToList();
            }

            var data = new { person };

            var status = person is not null ? Constants.RECORD_FOUND_MESSAGE : Constants.NOT_FOUND_MESSAGE;

            return ApiResponse.GetApiResponse(ApiResponseType.SUCCESS, data, status);
        }
    }
}
