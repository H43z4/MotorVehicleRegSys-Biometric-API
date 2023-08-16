using Inquiry.Services;
using Inquiry.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.ViewModels.Identity;
using SharedLib.APIs;
using SharedLib.Common;
using SharedLib.Security;
using Models.ViewModels.VehicleRegistration.Core;
using System;
using DCIssuance.ViewModels;
using System.Data;
using Inquiry.Models.udt;

namespace Inquiry.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.JWT_BEARER_TOKEN_STATELESS)]
    public class DNPIssuanceController : ControllerBase
    {
        private readonly IDNPIssuanceService _service;
        public VwUser User
        {
            get
            {
                return (VwUser)this.Request.HttpContext.Items["User"];
            }
        }

        public DNPIssuanceController(IDNPIssuanceService service)
        {
            this._service = service;
        }


        [HttpGet(Name = "GetApplicationsAgainstRegNo")]
        public async Task<ApiResponse> GetApplicationsAgainstRegNo(string registrationNo)
        {
            this._service.VwUser = this.User;

            var ds = await this._service.GetApplicationsAgainstRegNo(registrationNo);

            if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
            {
                return ApiResponse.GetApiResponse(ApiResponseType.FAILED, null, Constants.NOT_FOUND_MESSAGE);
            }

            var owner = SharedLib.Common.Extentions.ToList<OwnerApplicationInfo>(ds.Tables[0]).FirstOrDefault();
            if (owner is not null)
            {
                owner.Persons = SharedLib.Common.Extentions.ToList<ImPerson>(ds.Tables[1]);
                owner.Business = SharedLib.Common.Extentions.ToList<ImBusiness>(ds.Tables[2]).FirstOrDefault();
                foreach (var item in owner.Persons)
                {
                    if (item is not null)
                    {
                        item.PhoneNumbers = SharedLib.Common.Extentions.ToList<ImPhoneNumber>(ds.Tables[3]).Where(x=>x.PersonId == item.PersonId).ToList();
                    }
                }

                owner.Vehicle = SharedLib.Common.Extentions.ToList<ImVehicle>(ds.Tables[4]).FirstOrDefault();
                owner.Application = SharedLib.Common.Extentions.ToList<ImApplicationHist>(ds.Tables[5]).ToList();
                var Vehicle = SharedLib.Common.Extentions.ToList<ImVehicle>(ds.Tables[4]).FirstOrDefault();

            }


            return ApiResponse.GetApiResponse(ApiResponseType.SUCCESS, owner, Constants.RECORD_FOUND_MESSAGE);
        }
        
        [HttpGet(Name = "GetAssesmentDNPIssuance")]
        public async Task<ApiResponse> GetAssesmentDNPIssuance(long applicationId, long assesmentBaseId)
        {
            this._service.VwUser = this.User;

            var ds = await this._service.GetAssesmentDNPIssuance(applicationId, assesmentBaseId, this.User.UserId);

            var apiResponseType = ApiResponseType.SUCCESS;
            var msg = Constants.DATA_SAVED_MESSAGE;

            if (ds.Tables[0].Rows[0][0].ToString() == "1")
            {
                apiResponseType = ApiResponseType.FAILED;
                msg = Constants.DATA_NOT_SAVED_MESSAGE + " Error: " + ds.Tables[0].Rows[0][1].ToString();
            }
            else
            {
                apiResponseType = ApiResponseType.SUCCESS;
                msg = Constants.DATA_SAVED_MESSAGE;
            }

            return ApiResponse.GetApiResponse(apiResponseType, ds, msg);
        }
        [HttpGet(Name = "GetDNPApplicationDetail")]
        public async Task<ApiResponse> GetDNPApplicationDetail(long applicationId)
        {
            this._service.VwUser = this.User;

            var ds = await this._service.GetApplicationDetail(applicationId);

            if (ds.Tables.Count == 0)
            {
                return ApiResponse.GetApiResponse(ApiResponseType.FAILED, null, Constants.NOT_FOUND_MESSAGE);
            }

            var Owner = ds.Tables[0].ToList<VwTempOwner_v1>().FirstOrDefault();

            if (Owner is not null)
            {
                var addresses = ds.Tables[2].ToList<VwAddress>();
                var phoneNumbers = ds.Tables[3].ToList<VwPhoneNumber>();

                if (Owner.OwnerTypeId == 1)
                {
                    Owner.Persons = ds.Tables[1].ToList<VwPerson>();
                    Owner.Persons.ForEach(x =>
                    {
                        x.Addresses = new List<VwAddress>();
                        x.PhoneNumbers = new List<VwPhoneNumber>();

                        x.Addresses.AddRange(addresses.Where(add => add.PersonId == x.PersonId));
                        x.PhoneNumbers.AddRange(phoneNumbers.Where(add => add.PersonId == x.PersonId));
                    });
                }
                else if (Owner.OwnerTypeId == 2)
                {
                    Owner.Business = ds.Tables[1].ToList<VwBusiness>().FirstOrDefault();
                    Owner.Business.Addresses = new List<VwAddress>();
                    Owner.Business.PhoneNumbers = new List<VwPhoneNumber>();

                    Owner.Business.Addresses.AddRange(addresses.Where(add => add.BusinessId == Owner.Business.BusinessId));
                    Owner.Business.PhoneNumbers.AddRange(phoneNumbers.Where(add => add.BusinessId == Owner.Business.BusinessId));
                }

                VwVehicle Vehicle = ds.Tables[4].ToList<VwVehicle>().FirstOrDefault();

                var applicationHistory = ds.Tables[5].ToList<ImApplicationHist>();

                var application = ds.Tables[6].ToList<VwApplication>().FirstOrDefault();
                var remarks = ds.Tables[7].ToList<VwRemarks>();
                var assesment = ds.Tables[8];
                var validBusinessEventId = ds.Tables[9].AsEnumerable().Select(x => x.Field<Int64>(0)).ToArray();
                var data = new
                {
                    Owner,
                    Vehicle,
                    applicationHistory,
                    application,
                    remarks,
                    assesment,
                    validBusinessEventId = validBusinessEventId,
                    roleId = this.User.UserRoles.FirstOrDefault().RoleId
                };

                return ApiResponse.GetApiResponse(ApiResponseType.SUCCESS, data, Constants.RECORD_FOUND_MESSAGE);
            }

            return ApiResponse.GetApiResponse(ApiResponseType.FAILED, null, Constants.NOT_FOUND_MESSAGE);
        }
        [HttpGet(Name = "GetChallanDetail")]
        public async Task<ApiResponse> GetChallanDetail(long applicationId)
        {
            this._service.VwUser = this.User;

            var ds = await this._service.GetChallanDetail(applicationId);

            if (ds.Tables.Count == 0)
            {
                return ApiResponse.GetApiResponse(ApiResponseType.FAILED, null, Constants.NOT_FOUND_MESSAGE);
            }

            var challanDetails = ds.Tables[10];

            return ApiResponse.GetApiResponse(ApiResponseType.SUCCESS, challanDetails, Constants.NOT_FOUND_MESSAGE);
        }


        [HttpPost(Name = "AddDNPIssuance")]
        public async Task<ApiResponse> AddDNPIssuance(ImDuplicateNumberPlate imDuplicateCard)
        {
            this._service.VwUser = this.User;

            var ds = await this._service.SaveDNPIssuance(imDuplicateCard, this.User.UserId);

            var apiResponseType = ApiResponseType.SUCCESS;
            var msg = Constants.DATA_SAVED_MESSAGE;

            if (ds.Tables[0].Rows[0][0].ToString() == "1")
            {
                apiResponseType = ApiResponseType.FAILED;
                msg = Constants.DATA_NOT_SAVED_MESSAGE + " Error: " + ds.Tables[0].Rows[0][1].ToString();
            }
            else
            {
                apiResponseType = ApiResponseType.SUCCESS;
                msg = Constants.DATA_SAVED_MESSAGE;
            }

            return ApiResponse.GetApiResponse(apiResponseType, ds, msg);
        }

        [HttpPost(Name = "GenerateChallan")]
        public async Task<ApiResponse> GenerateChallan(ImGenerateChallan imGenerateChallan)
        {
            this._service.VwUser = this.User;

            var ds = await this._service.GenerateChallan(imGenerateChallan, this.User.UserId);

            var apiResponseType = ApiResponseType.SUCCESS;
            var msg = Constants.DATA_SAVED_MESSAGE;

            if (ds.Tables[1].Rows[0][0].ToString() == "0")
            {
                apiResponseType = ApiResponseType.SUCCESS;
                msg = Constants.DATA_SAVED_MESSAGE;
            }

            return ApiResponse.GetApiResponse(apiResponseType, ds, msg);
        }
        [HttpPost]
        public async Task<ApiResponse> SaveApplicationPhase(VwApplicationPhaseChange vwApplicationPhaseChange)
        {
            this._service.VwUser = this.User;

            var ds = await this._service.SaveApplicationPhase(vwApplicationPhaseChange);

            var apiResponseType = ApiResponseType.SUCCESS;
            var msg = Constants.DATA_SAVED_MESSAGE;

            if (ds.Tables[0].Rows[0][0].ToString() == "0")
            {
                apiResponseType = ApiResponseType.SUCCESS;
                msg = Constants.DATA_SAVED_MESSAGE;
            }
            else
            {
                apiResponseType = ApiResponseType.FAILED;
                msg = ds.Tables[0].Rows[0][1].ToString();
            }

            return ApiResponse.GetApiResponse(apiResponseType, null, msg);
        }
    }
}
