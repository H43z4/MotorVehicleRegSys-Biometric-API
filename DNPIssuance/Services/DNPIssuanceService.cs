using Inquiry.ViewModels;
using Database;
using Models.ViewModels.Identity;
using SharedLib.Interfaces;
using System.Data;
using Microsoft.Data.SqlClient;
using SharedLib.Common;
using System.Runtime.InteropServices;
using DCIssuance.ViewModels;
using Models.ViewModels.VehicleRegistration.Core;

namespace Inquiry.Services
{
    public interface IDNPIssuanceService : ICurrentUser
    {
        Task<DataSet> GetApplicationsAgainstRegNo(string registrationNo);
        Task<DataSet> GetApplicationDetail(long applicationId);

        Task<DataSet> GetChallanDetail(long applicationId);
        Task<DataSet> SaveDNPIssuance(ImDuplicateNumberPlate imDuplicateNumberPlate, long UserId);
        Task<DataSet> SaveApplicationPhase(VwApplicationPhaseChange vwApplicationPhaseChange);

        Task<DataSet> GetAssesmentDNPIssuance(long applicationId, long assesmentBaseId, long UserId);

        Task<DataSet> GenerateChallan(ImGenerateChallan imGenerateChallan, long UserId);
    }

    public class DNPIssuanceService : IDNPIssuanceService
    {
        readonly IDBHelper dbHelper;
        public VwUser VwUser { get; set; }
        public DNPIssuanceService(IDBHelper dbHelper)
        {
            this.dbHelper = dbHelper;
        }


        #region public-Methods

        public async Task<DataSet> GetApplicationsAgainstRegNo(string registrationNo)
        {
            Dictionary<string, object> paramDict = new Dictionary<string, object>();

            paramDict.Add("@VehicleRegNo", registrationNo);

            var ds = await this.dbHelper.GetDataSetByStoredProcedure("[NPPrinting].[GetNPInfo]", paramDict);

            return ds;
        }
        public async Task<DataSet> GetAssesmentDNPIssuance(long applicationId, long assesmentBaseId, long UserId)
        {
            Dictionary<string, object> paramDict = new Dictionary<string, object>();

            paramDict.Add("@ApplicationId", applicationId);
            paramDict.Add("@AssessmentBaseId", assesmentBaseId);
            paramDict.Add("@UserId", UserId);

            var ds = await this.dbHelper.GetDataSetByStoredProcedure("[NPPrinting].[GetAssessment]", paramDict);

            return ds;
        }
        public async Task<DataSet> GetApplicationDetail(long applicationId)
        {
            Dictionary<string, object> paramDict = new Dictionary<string, object>();

            paramDict.Add("@ApplicationId", applicationId);
            paramDict.Add("@UserId", this.VwUser.UserId);
            var ds = await this.dbHelper.GetDataSetByStoredProcedure("[NPPrinting].[GetApplicationDetail]", paramDict);

            return ds;
        }
        public async Task<DataSet> GetChallanDetail(long applicationId)
        {
            Dictionary<string, object> paramDict = new Dictionary<string, object>();

            paramDict.Add("@ApplicationId", applicationId);
            var ds = await this.dbHelper.GetDataSetByStoredProcedure("[Core].[GetChallanDetail]", paramDict);
            return ds;
        }

        public async Task<DataSet> SaveDNPIssuance(ImDuplicateNumberPlate imDuplicateNumberPlate, long UserId)
        {
            Dictionary<string, object> paramDict = new Dictionary<string, object>();
            paramDict.Add("@VehicleId", imDuplicateNumberPlate.VehicleId);
            //paramDict.Add("@Remarks", imDuplicateCard.Remarks != null ? imDuplicateCard.Remarks : DBNull.Value);
            paramDict.Add("@UserId", UserId);

            var ds = await this.dbHelper.GetDataSetByStoredProcedure("[NPPrinting].[SaveAppForNP]", paramDict);

            return ds;
        }
        public async Task<DataSet> SaveApplicationPhase(VwApplicationPhaseChange vwApplicationPhaseChange)
        {
            Dictionary<string, object> paramDict = new Dictionary<string, object>();
            paramDict.Add("@ApplicationId", vwApplicationPhaseChange.ApplicationId);
            paramDict.Add("@BusinessEventId", vwApplicationPhaseChange.BusinessEventId);
            paramDict.Add("@UserId", this.VwUser.UserId);
            paramDict.Add("@Remarks", vwApplicationPhaseChange.RemarksStatement);

            var ds = await this.dbHelper.GetDataSetByStoredProcedure("[NPPrinting].[SaveApplicationPhase]", paramDict);

            return ds;
        }
        public async Task<DataSet> GenerateChallan(ImGenerateChallan imGenerateChallan, long UserId)
        {
            Dictionary<string, object> paramDict = new Dictionary<string, object>();
            paramDict.Add("@ApplicationId", imGenerateChallan.ApplicationId);
            paramDict.Add("@AssessmentBaseId", imGenerateChallan.AssesmentBaseId);
            paramDict.Add("@VehicleId", imGenerateChallan.VehicleId);
            paramDict.Add("@Remarks", imGenerateChallan.Remarks != null ? imGenerateChallan.Remarks : DBNull.Value);
            paramDict.Add("@UserId", UserId);

            var ds = await this.dbHelper.GetDataSetByStoredProcedure("[NPPrinting].[GenerateChallan]", paramDict);

            return ds;
        }
        
        #endregion
    }
}
