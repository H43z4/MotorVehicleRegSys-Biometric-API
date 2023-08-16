using DNPIssuance.ViewModels;
using Database;
using Models.ViewModels.Identity;
using SharedLib.Interfaces;
using System.Data;
using Microsoft.Data.SqlClient;
using SharedLib.Common;
using System.Runtime.InteropServices;
using DCIssuance.ViewModels;
using System.Runtime.CompilerServices;
using Models.ViewModels.VehicleRegistration.Core;

namespace DNPIssuance.Services
{
    public interface IDCIssuanceService : ICurrentUser
    {
        Task<DataSet> GetApplicationsAgainstRegNo(string registrationNo);
        Task<DataSet> GetApplicationDetail(long applicationId);
        Task<DataSet> GetChallanDetail(long applicationId);
        Task<DataSet> SaveDCIssuance(ImDuplicateCard imDuplicateCard, long UserId);
        Task<DataSet> SaveApplicationPhase(VwApplicationPhaseChange vwApplicationPhaseChange);
        Task<DataSet> GetAssesmentDCIssuance(long applicationId, long assesmentBaseId, long UserId);
        Task<DataSet> GenerateChallan(ImGenerateChallan imGenerateChallan, long UserId);
    }

    public class DCIssuanceService : IDCIssuanceService
    {
        readonly IDBHelper dbHelper;
        public VwUser VwUser { get; set; }
        public DCIssuanceService(IDBHelper dbHelper)
        {
            this.dbHelper = dbHelper;
        }


        #region public-Methods

        public async Task<DataSet> GetApplicationsAgainstRegNo(string registrationNo)
        {
            Dictionary<string, object> paramDict = new Dictionary<string, object>();

            paramDict.Add("@VehicleRegNo", registrationNo);

            var ds = await this.dbHelper.GetDataSetByStoredProcedure("[CardPrinting].[GetCardInfo]", paramDict);

            return ds;
        }

        public async Task<DataSet> GetApplicationDetail(long applicationId)
        {
            Dictionary<string, object> paramDict = new Dictionary<string, object>();

            paramDict.Add("@ApplicationId", applicationId);
            paramDict.Add("@UserId", this.VwUser.UserId);
            var ds = await this.dbHelper.GetDataSetByStoredProcedure("[CardPrinting].[GetApplicationDetail]", paramDict);

            return ds;
        }

        public async Task<DataSet> GetChallanDetail(long applicationId)
        {
            Dictionary<string, object> paramDict = new Dictionary<string, object>();

            paramDict.Add("@ApplicationId", applicationId);
            var ds = await this.dbHelper.GetDataSetByStoredProcedure("[Core].[GetChallanDetail]", paramDict);
            return ds;
        }

        public async Task<DataSet> GetAssesmentDCIssuance(long applicationId, long assesmentBaseId, long UserId)
        {
            Dictionary<string, object> paramDict = new Dictionary<string, object>();

            paramDict.Add("@ApplicationId", applicationId);
            paramDict.Add("@AssessmentBaseId", assesmentBaseId);
            paramDict.Add("@UserId", UserId);

            var ds = await this.dbHelper.GetDataSetByStoredProcedure("[CardPrinting].[GetAssessment]", paramDict);

            return ds;
        }
        public async Task<DataSet> SaveDCIssuance(ImDuplicateCard imDuplicateCard, long UserId)
        {
            Dictionary<string, object> paramDict = new Dictionary<string, object>();
            paramDict.Add("@VehicleId", imDuplicateCard.VehicleId);
            //paramDict.Add("@Remarks", imDuplicateCard.Remarks != null ? imDuplicateCard.Remarks : DBNull.Value);
            paramDict.Add("@UserId", UserId);

            var ds = await this.dbHelper.GetDataSetByStoredProcedure("[CardPrinting].[SaveAppForDR]", paramDict);

            return ds;
        }
        public async Task<DataSet> SaveApplicationPhase(VwApplicationPhaseChange vwApplicationPhaseChange)
        {
            Dictionary<string, object> paramDict = new Dictionary<string, object>();
            paramDict.Add("@ApplicationId", vwApplicationPhaseChange.ApplicationId);
            paramDict.Add("@BusinessEventId", vwApplicationPhaseChange.BusinessEventId);
            paramDict.Add("@UserId", this.VwUser.UserId);
            paramDict.Add("@Remarks", vwApplicationPhaseChange.RemarksStatement);

            var ds = await this.dbHelper.GetDataSetByStoredProcedure("[CardPrinting].[SaveApplicationPhase]", paramDict);

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

            var ds = await this.dbHelper.GetDataSetByStoredProcedure("[CardPrinting].[GenerateChallan]", paramDict);

            return ds;
        }
        
        #endregion
    }
}
