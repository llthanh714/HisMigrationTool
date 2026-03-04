using Dapper;
using HisMigrationTool.Models;
using Microsoft.Data.SqlClient;

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

        // 1. Thêm tham số Action<string> để đẩy log về giao diện
        public async Task<VisitMigrationData?> SearchVisitAsync(string visitId, Action<string>? onProgress = null)
        {
            using var connection = new SqlConnection(_oldHisConn);

            onProgress?.Invoke($"[1/3] Đang kết nối tới DB ArcusAir để tìm VisitID: {visitId}...");

            // Gom 2 câu lệnh truy vấn lại với nhau (cách nhau bởi dấu chấm phẩy ;)
            string sql = @"
                -- TRUY VẤN 1: Dữ liệu Tiếp nhận
                SELECT
                    0 AS id_Tiepnhan,
                    N'BHYT' AS id_DMDoituong,
                    u.code AS id_Nhanvien,
                    REPLACE(p.mrn, 'PC', 'PC-') AS id_Benhnhan,
                    p.firstname AS Hoten,
                    REVERSE(LEFT(REVERSE(p.firstname), CHARINDEX(' ', REVERSE(p.firstname) + ' ') - 1)) AS Ten,
                    CONVERT(VARCHAR(24), p.dateofbirth, 103) AS Ngaythangnamsinh,
                    YEAR(p.dateofbirth) AS Namsinh,
                    rv_gender.valuedescription AS Gioitinh,
                    CONCAT_WS(', ', pa.address, pa.area, pa.state) AS Diachidaydu,
                    N'Bình_thường' AS Phanloaikham,
                    N'Đã_khám' AS Trangthai,
                    CAST(0 AS BIT) AS Dathuphi,
                    N'' AS id_Quaytiepnhan, -- Nội trú không cần thông tin quầy
                    CAST(1 AS BIT) AS Noitru,
                    0 AS id_Hosonoitru,
                    CAST(0 AS BIT) AS Ngoaitru,
                    pv.admissioncode AS Sonhapvien,
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
                    JOIN ArcusAirSql.dbo.users AS u ON pv.admittedby = u.id
                WHERE
                    pv.visitid = @VisitId
                    AND o.code = 'PC02';

                -- TRUY VẤN 2: Dữ liệu Phiếu chỉ định vào viện
                SELECT
                    0 AS id_Phieuchidinhvaovien,
                    pv.admissioncode AS Sonhapvien,
                    N'2026' AS Namluutru,
                    0 AS id_Tiepnhan,
                    N'' AS id_Benhan_Phanloai,
                    N'' AS Ten_PhanloaiBA,
                    REPLACE(p.mrn, 'PC', 'PC-') AS id_Benhnhan,
                    N'' AS Lienhe_hoten,
                    N'' AS Lienhe_diachi,
                    N'' AS Lienhe_sodienthoai,
                    prcu.code AS id_Bacsi_Lambenhan,
                    prcu.name AS Hoten_Bacsi_Lambenhan,
                    u.code AS id_Nhanvien,
                    u.name AS Hoten_Nhanvien,
                    N'NT_CAPCUU' AS id_DMPhongkham,
                    N'PHÒNG CẤP CỨU' AS Ten_Phongkham,
                    DATEADD(HOUR, 7, pv.admittedon) AS Ngaygio,
                    N'' AS Ngaygiodaydu,
                    N'' AS Quatrinhbenhly,
                    N'' AS Tiensubenh_Banthan,
                    N'' AS Tiensubenh_Giadinh,
                    N'' AS Toanthan,
                    N'' AS Cacbophan,
                    N'' AS KQ_Canlamsang,
                    N'' AS Daxuly,
                    N'' AS Chuy,
                    mp.TenKhoaHIS AS Chovaodieutritaikhoa,
                    mp.MaKhoaHIS AS id_Khoadieutri,
                    N'' AS ICD_Chandoannoichuyenden,
                    N'' AS Chandoannoichuyenden,
                    N'' AS ICD_ChandoanKKB_CC,
                    N'' AS ChandoanKKB_CC, -- Bổ sung,
                    N'' AS Lydovaovien,
                    N'' AS Para,
                    N'' AS Tiensusanphukhoa,
                    N'' AS Khamchuyenkhoa,
                    N'' AS Lienhe_Namsinh,
                    N'' AS Lienhe_Quanhe,
                    N'Có_thẻ_BHYT' AS Tinhtrang_BHYT,
                    CAST(1 AS BIT) AS BN_KygiayBHYT,
                    N'' AS Danhgia_Tutu
                FROM
                    ArcusAirSql.dbo.patients AS p
                    JOIN ArcusAirSql.dbo.organisations AS o ON p.orguid = o.id
                    JOIN ArcusAirSql.dbo.patientvisits AS pv ON pv.patientuid = p.id
                    JOIN ArcusAirSql.dbo.users AS u ON pv.admittedby = u.id
                    JOIN ArcusAirSql.dbo.patientvisits_visitcareproviders AS prc ON prc.patientvisits_id = pv.id AND prc.isprimarycareprovider = 1
                    JOIN ArcusAirSql.dbo.users AS prcu ON prc.careprovideruid = prcu.id
                    JOIN PhuongChauDW.dbo.FactPatientVisits AS dwpv ON pv.visitid = dwpv.VisitId AND dwpv.OrganizationCode = o.code
                    LEFT JOIN ArcusAirSql.dbo.__MappingDept AS mp ON dwpv.DepartmentName = mp.TenKhoaAA
                WHERE
                    pv.visitid = @VisitId
                    AND o.code = 'PC02';
            ";

            using var multi = await connection.QueryMultipleAsync(sql, new { VisitId = visitId });

            // 2. Kiểm tra số dòng chặt chẽ cho Tiếp Nhận
            onProgress?.Invoke("[2/3] Đang lấy và xác thực thông tin Tiếp nhận...");
            var tiepNhanList = (await multi.ReadAsync<TiepNhanDto>()).ToList();

            if (tiepNhanList.Count == 0)
                throw new Exception($"Không tìm thấy dữ liệu Tiếp nhận cho VisitID '{visitId}'.");
            if (tiepNhanList.Count > 1)
                throw new Exception($"Lỗi NGHIÊM TRỌNG: Phát hiện {tiepNhanList.Count} dòng dữ liệu Tiếp nhận cho VisitID '{visitId}'. Yêu cầu chỉ được phép có đúng 1 dòng.");

            var tiepNhanData = tiepNhanList.First();

            // 3. Kiểm tra số dòng chặt chẽ cho Phiếu chỉ định
            onProgress?.Invoke("[3/3] Đang lấy và xác thực thông tin Phiếu chỉ định vào viện...");
            var phieuChiDinhList = (await multi.ReadAsync<PhieuChiDinhVaoVienDto>()).ToList();

            if (phieuChiDinhList.Count == 0)
                throw new Exception($"Không tìm thấy dữ liệu Phiếu chỉ định vào viện cho VisitID '{visitId}'.");
            if (phieuChiDinhList.Count > 1)
                throw new Exception($"Lỗi NGHIÊM TRỌNG: Phát hiện {phieuChiDinhList.Count} dòng dữ liệu Phiếu vào viện. Yêu cầu chỉ được phép có đúng 1 dòng.");

            var phieuChiDinhData = phieuChiDinhList.First();

            onProgress?.Invoke("✔ Hoàn tất quá trình lấy và xác thực dữ liệu từ HIS Cũ.");

            return new VisitMigrationData
            {
                TiepNhan = tiepNhanData,
                PhieuChiDinh = phieuChiDinhData
            };
        }

        public async Task<MigrationResult> MigrateVisitAsync(VisitMigrationData data, Action<string>? onProgress = null)
        {
            using var connection = new SqlConnection(_newHisConn);

            onProgress?.Invoke("Đang mở kết nối tới hệ thống HIS Mới...");
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                // ==========================================
                // BƯỚC 1: INSERT TIẾP NHẬN
                // ==========================================
                onProgress?.Invoke(">> Bắt đầu cập nhật bảng [BV_Tiepnhan]...");
                string sqlInsertTiepNhan = @"
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

                int newTiepNhanId = await connection.QuerySingleAsync<int>(sqlInsertTiepNhan, data.TiepNhan, transaction);
                onProgress?.Invoke($"   -> Tạo thành công ID Tiếp nhận mới: {newTiepNhanId}");

                // ==========================================
                // BƯỚC 2: INSERT CÁC BẢN CON CỦA TIẾP NHẬN
                // ==========================================
                onProgress?.Invoke(">> Bắt đầu cập nhật các bảng liên quan: Địa chỉ, Lưu trú, Thuộc tính...");
                string sqlInsertDiaChi = "INSERT INTO BV_Tiepnhan_Diachi (id_Tiepnhan) VALUES (@id_Tiepnhan);";
                await connection.ExecuteAsync(sqlInsertDiaChi, new { id_Tiepnhan = newTiepNhanId }, transaction);

                string sqlInsertLuuTru = @"INSERT INTO BV_Tiepnhan_Luutru (id_Tiepnhan) VALUES (@id_Tiepnhan);";
                await connection.ExecuteAsync(sqlInsertLuuTru, new { id_Tiepnhan = newTiepNhanId }, transaction);

                string sqlInsertThuocTinh = @"INSERT INTO BV_Tiepnhan_Thuoctinh (id_Tiepnhan) VALUES (@id_Tiepnhan);";
                await connection.ExecuteAsync(sqlInsertThuocTinh, new { id_Tiepnhan = newTiepNhanId }, transaction);

                // ==========================================
                // BƯỚC 3: INSERT SỐ VÀO VIỆN
                // ==========================================
                onProgress?.Invoke(">> Đang cập nhật Sổ nhập viện [BV_Sonhapvien]...");
                string sqlInsertSoNhaoVien = @"
                    INSERT INTO BV_Sonhapvien (Maso, Namluutru, Sohientai, Trangthai, Ngaygiocap, id_Nhanvien, id_Khoaphong, id_Tiepnhan) 
                    VALUES (@Maso, 2026, @Sohientai, N'Sử_dụng', @Ngaygio, @Nhanvien, N'NT_CAPCUU', @id_Tiepnhan);";

                await connection.ExecuteAsync(sqlInsertSoNhaoVien,
                    new
                    {
                        Maso = data.TiepNhan.Sonhapvien,
                        Sohientai = Convert.ToInt32(data.TiepNhan.Sonhapvien),
                        Ngaygio = data.TiepNhan.Ngaygiotiepnhan,
                        Nhanvien = data.TiepNhan.id_Nhanvien,
                        id_Tiepnhan = newTiepNhanId
                    }, transaction);

                // ==========================================
                // BƯỚC 4: INSERT PHIẾU CHỈ ĐỊNH VÀO VIỆN
                // ==========================================
                onProgress?.Invoke(">> Đang cập nhật Phiếu chỉ định vào viện [BV_Phieuchidinhvaovien]...");
                data.PhieuChiDinh.id_Tiepnhan = newTiepNhanId;

                if (data.PhieuChiDinh.Ngaygio is DateTime dt)
                {
                    data.PhieuChiDinh.Ngaygiodaydu = $"Vào lúc {dt.Hour} giờ {dt.Minute} phút, ngày {dt:dd} tháng {dt:MM} năm {dt:yyyy}";
                }
                else
                {
                    data.PhieuChiDinh.Ngaygiodaydu = string.Empty;
                }

                string sqlInsertPhieu = @"
                    INSERT INTO BV_Phieuchidinhvaovien (
                        Sonhapvien, Namluutru, id_Tiepnhan, id_Benhan_Phanloai, Ten_PhanloaiBA, id_Benhnhan, 
                        Lienhe_hoten, Lienhe_diachi, Lienhe_sodienthoai, id_Bacsi_Lambenhan, Hoten_Bacsi_Lambenhan, 
                        id_Nhanvien, Hoten_Nhanvien, id_DMPhongkham, Ten_Phongkham, Ngaygio, Ngaygiodaydu, 
                        Quatrinhbenhly, Tiensubenh_Banthan, Tiensubenh_Giadinh, Toanthan, Cacbophan, KQ_Canlamsang, 
                        Daxuly, Chuy, Chovaodieutritaikhoa, id_Khoadieutri, ICD_Chandoannoichuyenden, 
                        Chandoannoichuyenden, ICD_ChandoanKKB_CC, ChandoanKKB_CC, Lydovaovien, Para, 
                        Tiensusanphukhoa, Khamchuyenkhoa, Lienhe_Namsinh, Lienhe_Quanhe, Tinhtrang_BHYT, 
                        BN_KygiayBHYT, Danhgia_Tutu
                    ) 
                    VALUES (
                        @Sonhapvien, @Namluutru, @id_Tiepnhan, @id_Benhan_Phanloai, @Ten_PhanloaiBA, @id_Benhnhan, 
                        @Lienhe_hoten, @Lienhe_diachi, @Lienhe_sodienthoai, @id_Bacsi_Lambenhan, @Hoten_Bacsi_Lambenhan, 
                        @id_Nhanvien, @Hoten_Nhanvien, @id_DMPhongkham, @Ten_Phongkham, @Ngaygio, @Ngaygiodaydu, 
                        @Quatrinhbenhly, @Tiensubenh_Banthan, @Tiensubenh_Giadinh, @Toanthan, @Cacbophan, @KQ_Canlamsang, 
                        @Daxuly, @Chuy, @Chovaodieutritaikhoa, @id_Khoadieutri, @ICD_Chandoannoichuyenden, 
                        @Chandoannoichuyenden, @ICD_ChandoanKKB_CC, @ChandoanKKB_CC, @Lydovaovien, @Para, 
                        @Tiensusanphukhoa, @Khamchuyenkhoa, @Lienhe_Namsinh, @Lienhe_Quanhe, @Tinhtrang_BHYT, 
                        @BN_KygiayBHYT, @Danhgia_Tutu
                    )";

                await connection.ExecuteAsync(sqlInsertPhieu, data.PhieuChiDinh, transaction);

                onProgress?.Invoke("✔ Mọi bảng đã được xử lý. Đang hoàn tất lưu trữ (Commit)...");
                transaction.Commit();

                return new MigrationResult
                {
                    IsSuccess = true,
                    Message = $"Đồng bộ thành công! Lượt tiếp nhận: {newTiepNhanId} & Phiếu vào viện đã được tạo."
                };
            }
            catch (Exception ex)
            {
                onProgress?.Invoke($"✖ PHÁT HIỆN LỖI: {ex.Message}");
                onProgress?.Invoke("↺ Đang hủy bỏ tất cả thay đổi (Rollback) để bảo vệ dữ liệu...");
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