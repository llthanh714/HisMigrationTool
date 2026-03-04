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
            _oldHisConn = config.GetConnectionString("OldHis") ?? "";
            _newHisConn = config.GetConnectionString("NewHis") ?? "";
        }

        public async Task<TiepNhanDto?> SearchVisitAsync(string visitId)
        {
            using var connection = new SqlConnection(_oldHisConn);

            // Bỏ FOR JSON PATH, trả về tập kết quả dạng cột (columnar) bình thường
            string sql = @"
                SELECT
                    0 AS id_Tiepnhan,
                    N'BHYT' AS id_DMDoituong,
                    N'PC-00750' AS id_Nhanvien,
                    p.mrn AS id_Benhnhan,
                    p.firstname AS Hoten,
                    REVERSE(LEFT(REVERSE(p.firstname), CHARINDEX(' ', REVERSE(p.firstname) + ' ') - 1)) AS Ten,
                    CONVERT(VARCHAR(24), p.dateofbirth, 103) AS Ngaythangnamsinh,
                    YEAR(p.dateofbirth) AS Namsinh,
                    rv_gender.valuedescription AS Gioitinh,
                    CONCAT_WS(', ', pa.address, pa.area, pa.state) AS Diachidaydu,
                    N'Bình_thường' AS Phanloaikham,
                    N'Đã_khám' AS Trangthai,
                    CAST(0 AS BIT) AS Dathuphi,
                    N'' AS id_Quaytiepnhan, 
                    CAST(1 AS BIT) AS Noitru,
                    0 AS id_Hosonoitru,
                    CAST(0 AS BIT) AS Ngoaitru,
                    N'' AS Sonhapvien,
                    DATEADD(HOUR, 7, pv.createdat) AS Ngaygiotiepnhan,
                    0 AS id_Voucher,
                    N'' AS id_TKTamung,
                    CAST(0 AS BIT) AS Tamung,
                    CAST(0 AS BIT) AS DuyetBHYT,
                    CAST(1 AS BIT) AS Tiepnhantunoitru,
                    N'' AS Lydomienphi,
                    CAST(0 AS BIT) AS Thuephong,
                    CAST(0 AS BIT) AS BNThammy,
                    0 AS id_benhnhanTK,
                    0 AS id_SttBenhnhanBD,
                    0 AS Songaydieutri_ngoaitru,
                    (2026 - YEAR(p.dateofbirth)) AS Tuoi,
                    pc.mobilephone AS Dienthoai,
                    CAST(0 AS BIT) AS KoKiemtraGiamdinh,
                    0 AS id_DMGoi,
                    N'02' AS id_ChiNhanh,
                    NULL AS NgaygioThu,
                    N'' AS Mathe,
                    CAST(0 AS BIT) AS Yeucau_BS,
                    CAST(0 AS BIT) AS SudungBHBL,
                    0 AS Khuvuc,
                    CAST(0 AS BIT) AS Xuatvien,
                    N'' AS XV_Trangthai,
                    CONCAT_WS(', ', pa.address, pa.area, pa.state) AS Diachidaydu_Noitru,
                    N'NT_CAPCUU' AS id_KhoaPhongTN,
                    CAST(0 AS BIT) AS VIP,
                    CAST(0 AS BIT) AS DaCheckin,
                    CAST(1 AS BIT) AS Checkin_Noitru
                FROM
                    ArcusAirSql.dbo.patients AS p
                    JOIN ArcusAirSql.dbo.organisations AS o ON p.orguid = o.id
                    JOIN ArcusAirSql.dbo.referencevalues AS rv_gender ON p.genderuid = rv_gender.id
                    JOIN ArcusAirSql.dbo.patients_address AS pa ON p.id = pa.patients_id
                    JOIN ArcusAirSql.dbo.patients_contact AS pc ON p.id = pc.patients_id
                    JOIN ArcusAirSql.dbo.patientvisits AS pv ON pv.patientuid = p.id
                WHERE
                    pv.visitid = @VisitId
                    AND o.code = 'PC02'";

            // Dapper sẽ tự động map các cột trả về vào các Property của class TiepNhanDto
            return await connection.QueryFirstOrDefaultAsync<TiepNhanDto>(sql, new { VisitId = visitId });
        }

        public async Task<MigrationResult> MigrateVisitAsync(TiepNhanDto data)
        {
            using var connection = new SqlConnection(_newHisConn);
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                // Dùng Dapper truyền object 'data' trực tiếp. 
                // Dapper tự động map các biến @TenBien với Property tương ứng trong TiepNhanDto
                string sqlInsert = @"
                    INSERT INTO BV_Tiepnhan (
                        id_DMDoituong, id_Nhanvien, id_Benhnhan, Hoten, Ten, Ngaythangnamsinh, Namsinh, 
                        Gioitinh, Diachidaydu, Phanloaikham, Trangthai, Dathuphi, id_Quaytiepnhan, 
                        Noitru, id_Hosonoitru, Ngoaitru, Sonhapvien, Ngaygiotiepnhan, id_Voucher, 
                        id_TKTamung, Tamung, DuyetBHYT, Tiepnhantunoitru, Lydomienphi, Thuephong, 
                        BNThammy, id_benhnhanTK, id_SttBenhnhanBD, Songaydieutri_ngoaitru, Tuoi, 
                        Dienthoai, KoKiemtraGiamdinh, id_DMGoi, id_ChiNhanh, NgaygioThu, Mathe, 
                        Yeucau_BS, SudungBHBL, Khuvuc, Xuatvien, XV_Trangthai, Diachidaydu_Noitru, 
                        id_KhoaPhongTN, VIP, DaCheckin, Checkin_Noitru
                    )
                    VALUES (
                        @id_DMDoituong, @id_Nhanvien, @id_Benhnhan, @Hoten, @Ten, @Ngaythangnamsinh, @Namsinh, 
                        @Gioitinh, @Diachidaydu, @Phanloaikham, @Trangthai, @Dathuphi, @id_Quaytiepnhan, 
                        @Noitru, @id_Hosonoitru, @Ngoaitru, @Sonhapvien, @Ngaygiotiepnhan, @id_Voucher, 
                        @id_TKTamung, @Tamung, @DuyetBHYT, @Tiepnhantunoitru, @Lydomienphi, @Thuephong, 
                        @BNThammy, @id_benhnhanTK, @id_SttBenhnhanBD, @Songaydieutri_ngoaitru, @Tuoi, 
                        @Dienthoai, @KoKiemtraGiamdinh, @id_DMGoi, @id_ChiNhanh, @NgaygioThu, @Mathe, 
                        @Yeucau_BS, @SudungBHBL, @Khuvuc, @Xuatvien, @XV_Trangthai, @Diachidaydu_Noitru, 
                        @id_KhoaPhongTN, @VIP, @DaCheckin, @Checkin_Noitru
                    );
                    SELECT CAST(SCOPE_IDENTITY() as int);";

                // Thực thi câu lệnh Insert và lấy về ID tự tăng (id_Tiepnhan)
                int newId = await connection.QuerySingleAsync<int>(sqlInsert, data, transaction);

                // Nếu có các bảng khác cần insert kèm (VD: BV_ChiDinh, BV_KhamBenh...), 
                // bạn có thể lấy `newId` này để làm khóa ngoại truyền tiếp vào các câu Insert phía dưới.
                // data.id_Tiepnhan = newId;

                transaction.Commit();
                return new MigrationResult
                {
                    IsSuccess = true,
                    Message = $"Đồng bộ thành công! Tạo Lượt tiếp nhận với ID mới: {newId}"
                };
            }
            catch (Exception ex)
            {
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