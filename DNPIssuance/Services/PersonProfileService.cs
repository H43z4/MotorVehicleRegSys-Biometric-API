using Database;
using Models.ViewModels.Identity;
using SharedLib.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Microsoft.Data.SqlClient;
using SharedLib.Common;
using Models.ViewModels.VehicleRegistration.Core;

namespace Inquiry.Services
{
    public interface IPersonProfileService : ICurrentUser
    {
        Task<DataSet> GetOwnersDropDowns();
        Task<DataSet> GetPersonByCNIC(string cnic);
    }
    public class PersonProfileService:IPersonProfileService
    {
        readonly IDBHelper dbHelper;
        public VwUser VwUser { get; set; }

        public PersonProfileService(IDBHelper dbHelper)
        {
            this.dbHelper = dbHelper;
        }
        public async Task<DataSet> GetOwnersDropDowns()
        {
            var ds = await this.dbHelper.GetDataSetByStoredProcedure("[Setup].[GetOwnersDropDowns]",null);

            return ds;
        }
        public async Task<DataSet> GetPersonByCNIC(string cnic)
        {
            Dictionary<string, object> paramDict = new Dictionary<string, object>();

            paramDict.Add("@PersonId", DBNull.Value);
            paramDict.Add("@CNIC", cnic);

            var ds = await this.dbHelper.GetDataSetByStoredProcedure("[Core].[GetPerson]", paramDict);

            return ds;
        }
    }
}
