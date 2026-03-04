using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Dapper;
using HisMigrationTool.Models;

namespace HisMigrationTool.Services
{
    public class MigrationWebService
    {
        private readonly string _oldHisConn;
        private readonly string _newHisConn;

        public MigrationWebService(IConfiguration config)
        {
            // Lấy chuỗi kết nối từ appsettings.json
            _oldHisConn = config.GetConnectionString("OldHis");
            _newHisConn = config.GetConnectionString("NewHis");
        }

        public async Task<PatientMigrationDto> SearchPatientAsync(string patientCode)
        {
            using var connection = new SqlConnection(_oldHisConn);

            // Query lấy thông tin lượt khám mới nhất của bệnh nhân
            string sql = @"
                SELECT
                    p.id AS PatientUid,
                    p.mrn AS MRN,
                    p.firstname AS FullName,
                    p.dateofbirth AS DateOfBirth,
                    rv_gender.valuedescription AS Gender,
                    p.nationalid AS NationalID,
                    rv_nationality.valuedescription AS Nationality,
                    rv_race.valuedescription AS Race,
                    rv_occupation.valuedescription AS Occupation,
                    rv_bloodgroup.valuedescription AS BloodGroup,
                    rv_rhfactor.valuedescription AS RhFactor,
                    rv_religion.valuedescription AS Religion,
                    p.maritalstatusuid AS MaritalStatus,
                    rv_patienttype.valuedescription AS PatientType,
                    p.isvip AS IsVIP,
                    p.registereddate AS RegisteredDate,
                    p.ismcreg AS IsMCReg,
                    p.passportno AS PassportNo,
                    o.name AS OrganisationName,
                    o.code AS OrgCode,
                    MIN(pa.address) AS Address,
                    MIN(pa.id) AS AreaId,
                    MIN(pa.area) AS Area,
                    MIN(pa.city) AS City,
                    MIN(pa.state) AS State,
                    MIN(pa.country) AS Country, 
                    MIN(pa.zipcode) AS ZipCode,
                    MIN(pc.mobilephone) AS MobilePhone,
                    MIN(pc.emailid) AS EmailID,
                    MIN(pc.homephone) AS HomePhone,
                    MAX(p.modifiedat) AS ModifiedAt
                FROM
                    ArcusAirSql.dbo.patients AS p
                    LEFT JOIN ArcusAirSql.dbo.organisations AS o ON p.orguid = o.id
                    LEFT JOIN ArcusAirSql.dbo.referencevalues AS rv_gender ON p.genderuid = rv_gender.id
                    LEFT JOIN ArcusAirSql.dbo.referencevalues AS rv_nationality ON p.nationalityuid = rv_nationality.id
                    LEFT JOIN ArcusAirSql.dbo.referencevalues AS rv_race ON p.raceuid = rv_race.id
                    LEFT JOIN ArcusAirSql.dbo.referencevalues AS rv_occupation ON p.occupationuid = rv_occupation.id
                    LEFT JOIN ArcusAirSql.dbo.referencevalues AS rv_bloodgroup ON p.bloodgroupuid = rv_bloodgroup.id
                    LEFT JOIN ArcusAirSql.dbo.referencevalues AS rv_rhfactor ON p.rhfactoruid = rv_rhfactor.id
                    LEFT JOIN ArcusAirSql.dbo.referencevalues AS rv_religion ON p.religionuid = rv_religion.id
                    LEFT JOIN ArcusAirSql.dbo.referencevalues AS rv_maritalstatus ON p.maritalstatusuid = rv_maritalstatus.id
                    LEFT JOIN ArcusAirSql.dbo.referencevalues AS rv_patienttype ON p.patienttypeuid = rv_patienttype.id
                    LEFT JOIN ArcusAirSql.dbo.patients_address AS pa ON p.id = pa.patients_id
                    LEFT JOIN ArcusAirSql.dbo.patients_contact AS pc ON p.id = pc.patients_id
                WHERE
                    p.mrn = @PatientCode
                GROUP BY
                    p.id,
                    p.mrn,
                    p.firstname,
                    p.middlename,
                    p.lastname,
                    p.dateofbirth,
                    rv_gender.valuedescription,
                    p.nationalid,
                    p.maritalstatusuid,
                    rv_nationality.valuedescription,
                    rv_race.valuedescription,
                    rv_occupation.valuedescription,
                    rv_bloodgroup.valuedescription,
                    rv_rhfactor.valuedescription,
                    rv_religion.valuedescription,
                    rv_maritalstatus.valuedescription,
                    rv_patienttype.valuedescription,
                    p.isvip,
                    p.registereddate,
                    p.ismcreg,
                    p.passportno,
                    o.name,
                    o.code";

            return await connection.QueryFirstOrDefaultAsync<PatientMigrationDto>(sql, new { PatientCode = patientCode });
        }

        public async Task<MigrationResult> MigratePatientAsync(PatientMigrationDto p)
        {
            using var connection = new SqlConnection(_newHisConn);
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                //// 1. Kiểm tra tồn tại bệnh nhân ở DB mới
                //string sqlCheck = "SELECT TOP 1 id_Benhnhan FROM BV_Benhnhan WHERE MaBenhnhan = @MaBenhNhan";
                //int? existingId = await connection.QueryFirstOrDefaultAsync<int?>(sqlCheck, new { p.MaBenhNhan }, transaction);

                //if (!existingId.HasValue)
                //{
                //    return new MigrationResult
                //    {
                //        IsSuccess = false,
                //        Message = $"Không tìm thấy Bệnh nhân '{p.MaBenhNhan}' ở DB Mới. Hãy đảm bảo danh mục BN đã được đồng bộ."
                //    };
                //}

                //p.New_id_Benhnhan = existingId.Value;

                //// 2. Insert các bảng theo thứ tự, sử dụng SCOPE_IDENTITY() để lấy ID tự tăng
                //string sqlTiepNhan = "INSERT INTO BV_Tiepnhan (id_Benhnhan, ThoiGianTiepNhan) VALUES (@New_id_Benhnhan, @ThoiGianVaoVien); SELECT CAST(SCOPE_IDENTITY() as int);";
                //p.New_id_Tiepnhan = await connection.QuerySingleAsync<int>(sqlTiepNhan, p, transaction);

                //await connection.ExecuteAsync("INSERT INTO BV_Tiepnhan_Diachi (id_Tiepnhan, id_Benhnhan, Diachi_Daydu) VALUES (@New_id_Tiepnhan, @New_id_Benhnhan, @DiaChi);", p, transaction);
                //await connection.ExecuteAsync("INSERT INTO BV_Tiepnhan_Luutru (id_Tiepnhan, id_Benhnhan, ThoiGianVao) VALUES (@New_id_Tiepnhan, @New_id_Benhnhan, @ThoiGianVaoVien);", p, transaction);
                //await connection.ExecuteAsync("INSERT INTO BV_Tiepnhan_Phanloaibenh (id_Tiepnhan, id_Benhnhan, MaICD, TenBenh) VALUES (@New_id_Tiepnhan, @New_id_Benhnhan, @MaICD, @TenBenh);", p, transaction);
                //await connection.ExecuteAsync("INSERT INTO BV_Tiepnhan_Thuoctinh (id_Tiepnhan, id_Benhnhan, MaBHYT) VALUES (@New_id_Tiepnhan, @New_id_Benhnhan, @MaBHYT);", p, transaction);

                //string sqlPhieuCD = "INSERT INTO BV_Phieuchidinhvaovien (id_Tiepnhan, id_Benhnhan, Ngaychidinh) VALUES (@New_id_Tiepnhan, @New_id_Benhnhan, @ThoiGianVaoVien); SELECT CAST(SCOPE_IDENTITY() as int);";
                //p.New_id_Phieuchidinh = await connection.QuerySingleAsync<int>(sqlPhieuCD, p, transaction);

                //await connection.ExecuteAsync("INSERT INTO BV_Sonhapvien (id_Tiepnhan, id_Benhnhan, Ngaynhapvien) VALUES (@New_id_Tiepnhan, @New_id_Benhnhan, @ThoiGianVaoVien);", p, transaction);

                //string sqlBenhAn = "INSERT INTO BV_BenhAn_Ngoaitru (id_Tiepnhan, id_Benhnhan, Ngaytao) VALUES (@New_id_Tiepnhan, @New_id_Benhnhan, @ThoiGianVaoVien); SELECT CAST(SCOPE_IDENTITY() as int);";
                //p.New_id_BenhAn = await connection.QuerySingleAsync<int>(sqlBenhAn, p, transaction);

                //await connection.ExecuteAsync("INSERT INTO BV_Noitru_Benhnhantaikhoa (id_Tiepnhan, id_Benhnhan, MaKhoa, ThoiGianVaoKhoa) VALUES (@New_id_Tiepnhan, @New_id_Benhnhan, @MaKhoaPhong, @ThoiGianVaoVien);", p, transaction);

                //// 3. Logic riêng cho trẻ sơ sinh (nhỏ hơn 30 ngày tuổi)
                //if (p.NgaySinh.HasValue && (DateTime.Now - p.NgaySinh.Value).TotalDays <= 30)
                //{
                //    await connection.ExecuteAsync("INSERT INTO BV_HosonoitruBV_Tresosinh (id_Tiepnhan, id_Benhnhan) VALUES (@New_id_Tiepnhan, @New_id_Benhnhan);", p, transaction);
                //}

                //// Nếu mọi thứ trơn tru -> Commit
                //transaction.Commit();
                return new MigrationResult
                {
                    IsSuccess = true,
                    // Message = $"Đồng bộ thành công! ID Bệnh nhân: {p.New_id_Benhnhan} | ID Lượt tiếp nhận: {p.New_id_Tiepnhan}"
                };
            }
            catch (Exception ex)
            {
                // Lỗi ở bất kỳ bảng nào -> Hủy bỏ (Rollback) tất cả
                transaction.Rollback();
                return new MigrationResult
                {
                    IsSuccess = false,
                    Message = $"Lỗi hệ thống khi Insert: {ex.Message}"
                };
            }
        }
    }
}