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

        public async Task<VisitMigrationData?> SearchVisitAsync(string visitId, Action<string>? onProgress = null)
        {
            using var connection = new SqlConnection(_oldHisConn);

            onProgress?.Invoke($"[1/6] Đang kết nối tới DB ArcusAir để tìm VisitID: {visitId}...");

            string sql = @"
                -- TRUY VẤN 1: Dữ liệu Tiếp nhận
                SELECT
                    0 AS id_Tiepnhan, N'BHYT' AS id_DMDoituong, u.code AS id_Nhanvien, REPLACE(p.mrn, 'PC', 'PC-') AS id_Benhnhan,
                    p.firstname AS Hoten, REVERSE(LEFT(REVERSE(p.firstname), CHARINDEX(' ', REVERSE(p.firstname) + ' ') - 1)) AS Ten,
                    CONVERT(VARCHAR(24), p.dateofbirth, 103) AS Ngaythangnamsinh, YEAR(p.dateofbirth) AS Namsinh,
                    rv_gender.valuedescription AS Gioitinh, CONCAT_WS(', ', pa.address, pa.area, pa.state) AS Diachidaydu,
                    N'Bình_thường' AS Phanloaikham, N'Đã_khám' AS Trangthai, CAST(0 AS BIT) AS Dathuphi, N'' AS id_Quaytiepnhan,
                    CAST(1 AS BIT) AS Noitru, 0 AS id_Hosonoitru, CAST(0 AS BIT) AS Ngoaitru, pv.admissioncode AS Sonhapvien,
                    DATEADD(HOUR, 7, pv.createdat) AS Ngaygiotiepnhan, 0 AS id_Voucher, N'' AS id_TKTamung, CAST(0 AS BIT) AS Tamung,
                    CAST(0 AS BIT) AS DuyetBHYT, CAST(1 AS BIT) AS Tiepnhantunoitru, N'' AS Lydomienphi, CAST(0 AS BIT) AS Thuephong,
                    CAST(0 AS BIT) AS BNThammy, 0 AS id_benhnhanTK, 0 AS id_SttBenhnhanBD, 0 AS Songaydieutri_ngoaitru,
                    (2026 - YEAR(p.dateofbirth)) AS Tuoi, pc.mobilephone AS Dienthoai, CAST(0 AS BIT) AS KoKiemtraGiamdinh,
                    0 AS id_DMGoi, N'02' AS id_ChiNhanh, NULL AS NgaygioThu, N'' AS Mathe, CAST(0 AS BIT) AS Yeucau_BS,
                    CAST(0 AS BIT) AS SudungBHBL, 0 AS Khuvuc, CAST(0 AS BIT) AS Xuatvien, N'' AS XV_Trangthai,
                    CONCAT_WS(', ', pa.address, pa.area, pa.state) AS Diachidaydu_Noitru, N'NT_CAPCUU' AS id_KhoaPhongTN,
                    CAST(0 AS BIT) AS VIP, CAST(0 AS BIT) AS DaCheckin, CAST(1 AS BIT) AS Checkin_Noitru
                FROM ArcusAirSql.dbo.patients AS p
                JOIN ArcusAirSql.dbo.organisations AS o ON p.orguid = o.id
                JOIN ArcusAirSql.dbo.referencevalues AS rv_gender ON p.genderuid = rv_gender.id
                JOIN ArcusAirSql.dbo.patients_address AS pa ON p.id = pa.patients_id
                JOIN ArcusAirSql.dbo.patients_contact AS pc ON p.id = pc.patients_id
                JOIN ArcusAirSql.dbo.patientvisits AS pv ON pv.patientuid = p.id
                JOIN ArcusAirSql.dbo.users AS u ON pv.admittedby = u.id
                WHERE pv.visitid = @VisitId AND o.code = 'PC02';

                -- TRUY VẤN 2: Dữ liệu Phiếu chỉ định vào viện
                SELECT
                    0 AS id_Phieuchidinhvaovien, pv.admissioncode AS Sonhapvien, N'2026' AS Namluutru, 0 AS id_Tiepnhan,
                    N'' AS id_Benhan_Phanloai, N'' AS Ten_PhanloaiBA, REPLACE(p.mrn, 'PC', 'PC-') AS id_Benhnhan,
                    N'' AS Lienhe_hoten, N'' AS Lienhe_diachi, N'' AS Lienhe_sodienthoai, prcu.code AS id_Bacsi_Lambenhan,
                    prcu.name AS Hoten_Bacsi_Lambenhan, u.code AS id_Nhanvien, u.name AS Hoten_Nhanvien, N'NT_CAPCUU' AS id_DMPhongkham,
                    N'PHÒNG CẤP CỨU' AS Ten_Phongkham, DATEADD(HOUR, 7, pv.admittedon) AS Ngaygio, N'' AS Ngaygiodaydu,
                    N'' AS Quatrinhbenhly, N'' AS Tiensubenh_Banthan, N'' AS Tiensubenh_Giadinh, N'' AS Toanthan, N'' AS Cacbophan,
                    N'' AS KQ_Canlamsang, N'' AS Daxuly, N'' AS Chuy, mp.TenKhoaHIS AS Chovaodieutritaikhoa, mp.MaKhoaHIS AS id_Khoadieutri,
                    N'' AS ICD_Chandoannoichuyenden, N'' AS Chandoannoichuyenden, N'' AS ICD_ChandoanKKB_CC, N'' AS ChandoanKKB_CC, 
                    N'' AS Lydovaovien, N'' AS Para, N'' AS Tiensusanphukhoa, N'' AS Khamchuyenkhoa, N'' AS Lienhe_Namsinh,
                    N'' AS Lienhe_Quanhe, N'Có_thẻ_BHYT' AS Tinhtrang_BHYT, CAST(1 AS BIT) AS BN_KygiayBHYT, N'' AS Danhgia_Tutu
                FROM ArcusAirSql.dbo.patients AS p
                JOIN ArcusAirSql.dbo.organisations AS o ON p.orguid = o.id
                JOIN ArcusAirSql.dbo.patientvisits AS pv ON pv.patientuid = p.id
                JOIN ArcusAirSql.dbo.users AS u ON pv.admittedby = u.id
                LEFT JOIN ArcusAirSql.dbo.patientvisits_visitcareproviders AS prc ON prc.patientvisits_id = pv.id AND prc.isprimarycareprovider = 1
                LEFT JOIN ArcusAirSql.dbo.users AS prcu ON prc.careprovideruid = prcu.id
                JOIN PhuongChauDW.dbo.FactPatientVisits AS dwpv ON pv.visitid = dwpv.VisitId AND dwpv.OrganizationCode = o.code
                LEFT JOIN ArcusAirSql.dbo.__MappingDept AS mp ON dwpv.DepartmentName = mp.TenKhoaAA
                WHERE pv.visitid = @VisitId AND o.code = 'PC02';

                -- TRUY VẤN 3: Dữ liệu Bệnh nhân tại khoa (BV_Noitru_Benhnhantaikhoa)
                SELECT
                    0 AS id_Taikhoa, N'' AS id_Khoa, N'' AS Tenkhoa, DATEADD(HOUR, 7, pv.admittedon) AS Ngaygio,
                    pv.admissioncode AS Sonhapvien, REPLACE(p.mrn, 'PC', 'PC-') AS id_Benhnhan, p.firstname AS Hoten,
                    CONVERT(VARCHAR(24), p.dateofbirth, 103) AS Ngaythangnamsinh, CAST(YEAR(p.dateofbirth) AS NVARCHAR(4)) AS Namsinh,
                    CAST((2026 - YEAR(p.dateofbirth)) AS NVARCHAR(50)) AS Tuoi, rv_gender.valuedescription AS Gioitinh,
                    N'Tại_khoa' AS Trangthai, u.code AS id_Nhanvienget, u.name AS Hoten_Nhanvienget, N'2026' AS NamNhapvien,
                    0 AS id_Tiepnhan, N'' AS id_KhoaChuyenden, N'' AS KhoaChuyenden, NULL AS Ngaygio_Chuyen, N'' AS Giochuyenmo,
                    N'' AS Xutri, N'' AS Hientai, N'' AS Bacsi_Truc, N'' AS YeucauThuho
                FROM ArcusAirSql.dbo.patients AS p
                JOIN ArcusAirSql.dbo.organisations AS o ON p.orguid = o.id
                JOIN ArcusAirSql.dbo.referencevalues AS rv_gender ON p.genderuid = rv_gender.id
                JOIN ArcusAirSql.dbo.patientvisits AS pv ON pv.patientuid = p.id
                JOIN ArcusAirSql.dbo.users AS u ON pv.admittedby = u.id
                WHERE pv.visitid = @VisitId AND o.code = 'PC02';

                -- TRUY VẤN 4: Dữ liệu Bệnh án Ngoại trú (BV_BenhAn_Ngoaitru)
                SELECT
                    0 AS id_Benhan_Ngoaitru, 0 AS STT_Sokhambenh, N'' AS id_Benhan_Phanloai_Khambenh, N'2026' AS Namluutru,
                    0 AS id_Hosonoitru, REPLACE(p.mrn, 'PC', 'PC-') AS id_Benhnhan, 0 AS id_Tiepnhan, N'BHYT' AS id_DMDoituong,
                    prcu.code AS id_Bacsi, prcu.name AS Hoten_Bacsi, DATEADD(HOUR, 7, pv.admittedon) AS Ngaygio, N'' AS Ngaygiokhamdaydu,
                    0 AS id_DuyetBHYT, mp.MaKhoaHIS AS id_DMPhongkham, mp.TenKhoaHIS AS Ten_Khoaphong, N'' AS Quatrinhbenhly,
                    N'' AS Tiensubenh_Banthan, N'' AS Tiensubenh_Giadinh, N'' AS Cacbophan, N'' AS Lydovaovien, N'' AS KQ_Canlamsang,
                    N'' AS id_ICD_Chandoan, N'' AS Chandoan, N'' AS Denghi, N'' AS Xutri, N'' AS Ngaytaikham, N'' AS Dientienbenh,
                    N'' AS Toanthan, N'' AS MasoChungchi, N'' AS dmdc_Id_Khoa, N'' AS dmdc_Ten_Khoa, 0 AS id_Lanmangthai,
                    N'' AS Chidinhdichvu, N'' AS Chidinhdieutri, N'' AS Toathuoc, N'' AS Ketluan, 0 AS id_Benhnhanphongkham,
                    NULL AS Dieutri_Tungay, NULL AS Dieutri_Denngay, N'' AS Phuongphap_Dieutri, N'' AS Tinhtrang_Ravien,
                    0 AS Loai_BenhAn, N'' AS Phanloai_Capcuu, N'' AS Phanloai_Ravien, N'' AS Huongdieutri, CAST(0 AS BIT) AS QuenSo,
                    N'' AS Danhgia_tenga, CAST(0 AS BIT) AS Benh_phuctap, N'' AS Danhgia_tutu, u.code AS id_nguoilap, u.name AS ten_nguoilap,
                    CAST(0 AS BIT) AS duyetphieu, NULL AS ngayduyet, 0 AS id_BA_Phuctap, N'' AS chiphi_taikham
                FROM ArcusAirSql.dbo.patients AS p
                JOIN ArcusAirSql.dbo.organisations AS o ON p.orguid = o.id
                JOIN ArcusAirSql.dbo.patientvisits AS pv ON pv.patientuid = p.id
                JOIN ArcusAirSql.dbo.users AS u ON pv.admittedby = u.id
                LEFT JOIN ArcusAirSql.dbo.patientvisits_visitcareproviders AS prc ON prc.patientvisits_id = pv.id AND prc.isprimarycareprovider = 1
                LEFT JOIN ArcusAirSql.dbo.users AS prcu ON prc.careprovideruid = prcu.id
                JOIN PhuongChauDW.dbo.FactPatientVisits AS dwpv ON pv.visitid = dwpv.VisitId AND dwpv.OrganizationCode = o.code
                LEFT JOIN ArcusAirSql.dbo.__MappingDept AS mp ON dwpv.DepartmentName = mp.TenKhoaAA
                WHERE pv.visitid = @VisitId AND o.code = 'PC02';

                -- TRUY VẤN 5: BỔ SUNG Dữ liệu Hồ sơ nội trú (BV_Hosonoitru)
                SELECT
                    0 AS id_Hosonoitru, 0 AS id_Phieuchidinhvaovien, N'' AS Soluutru, pv.admissioncode AS Sonhapvien,
                    N'2026' AS Namluutru, N'' AS Mayte, 0 AS id_Tiepnhan, N'' AS id_Benhan_Phanloai, N'' AS Ten_PhanloaiBA,
                    REPLACE(p.mrn, 'PC', 'PC-') AS id_Benhnhan, p.firstname AS Hoten, CONVERT(VARCHAR(24), p.dateofbirth, 103) AS Ngaythangnamsinh,
                    CAST(YEAR(p.dateofbirth) AS NVARCHAR(4)) AS Namsinh, CAST((2026 - YEAR(p.dateofbirth)) AS NVARCHAR(50)) AS Tuoi,
                    rv_gender.valuedescription AS Gioitinh, pc.mobilephone AS Dienthoaididong, N'' AS id_DMQuoctich, N'' AS Quoctich,
                    N'' AS id_DMNghenghiep, N'' AS Nghenghiep, N'' AS id_DMDantoc, N'' AS Dantoc,
                    CONCAT_WS(', ', pa.address, pa.area, pa.state) AS Tamtru_diachi, N'' AS Tamtru_xaphuong, N'' AS Tamtru_idXaphuong,
                    N'' AS Tamtru_quanhuyen, N'' AS Tamtru_idQuanhuyen, N'' AS Tamtru_tinhthanh, N'' AS Tamtru_idTinhthanh, N'' AS Noilamviec,
                    N'BHYT' AS id_DMDoituong, N'BHYT' AS Doituong, 0 AS id_DuyetBHYT, N'' AS SotheBHYT, CAST(0 AS BIT) AS DuyetBHYT,
                    N'' AS NgayhethanBHYT, N'' AS DiachiBHYT, N'' AS Lienhe_hoten, N'' AS Lienhe_diachi, N'' AS Lienhe_sodienthoai,
                    prcu.code AS id_Bacsi_Lambenhan, prcu.name AS Hoten_Bacsi_Lambenhan, u.code AS id_Nhanvien, u.name AS Hoten_Nhanvien,
                    mp.MaKhoaHIS AS id_DMPhongkham, mp.TenKhoaHIS AS Ten_Phongkham, DATEADD(HOUR, 7, pv.createdat) AS Ngaygio, N'' AS Ngaygiodaydu,
                    DATEADD(HOUR, 7, pv.admittedon) AS Vaovien_Ngaygiovaovien, N'' AS Vaovien_Ngaygiovaoviendaydu, N'' AS Tructiepvao,
                    N'' AS Noigioithieu, 1 AS Vaoviendobenhnaylanthu, mp.TenKhoaHIS AS Vaokhoa, mp.MaKhoaHIS AS Vaokhoa_idKhoaphong,
                    DATEADD(HOUR, 7, pv.admittedon) AS Vaokhoa_Ngaygiovaokhoa, N'' AS Vaokhoa_Ngaygiovaokhoadaydu, 0 AS Vaokhoa_Songaydieutri,
                    N'' AS Chuyenden, N'' AS Chuyenvien, NULL AS Ravien_Ngaygioravien, N'' AS Ravien_Ngaygioraviendaydu, N'' AS Ravien_Phanloai,
                    0 AS Tongsongaydieutri, N'' AS Chandoan_khivaokhoadieutri, N'' AS ICD_Chandoan_khivaokhoadieutri, N'' AS Chandoan_Lucvaokhoa_Lydo,
                    N'' AS Chandoan_Lucvaokhoa_Phanloai, N'' AS ICD_Chandoannoichuyenden, N'' AS Chandoannoichuyenden, N'' AS ICD_ChandoanKKB_CC,
                    N'' AS ChandoanKKB_CC, N'' AS Chandoan_Lucvaode, N'' AS ICD_Chandoan_Lucvaode, CAST(0 AS BIT) AS Chandoan_phauthuatsausinh,
                    CAST(0 AS BIT) AS Chandoan_thuthuatsausinh, CAST(0 AS BIT) AS Chandoan_taibien, CAST(0 AS BIT) AS Chandoan_bienchung,
                    N'' AS Chandoan_Ravien_Benhchinh, N'' AS ICD_Chandoan_Ravien_Benhchinh, N'' AS Chandoan_Ravien_Benhkemtheo,
                    N'' AS ICD_Chandoan_Ravien_Benhkemtheo, NULL AS Ngayde_Mode, N'' AS Ngoithai, N'' AS Cachthucde, CAST(0 AS BIT) AS Cachthucde_Bienchung,
                    N'' AS Cachthucde_Kiemsoattucung, CAST(0 AS BIT) AS Cachthucde_Taibien, CAST(0 AS BIT) AS Cachthucde_TB_BC_Dogayme,
                    N'' AS Cachthucde_TB_BC_Dokhac, CAST(0 AS BIT) AS Cachthucde_TB_BC_Donhiemkhuan, CAST(0 AS BIT) AS Cachthucde_TB_BC_Dophauthuat,
                    N'' AS Tinhhinhphauthuat, N'' AS Chandoan_Sauphauthuat, N'' AS ICD_Chandoan_Sauphauthuat, N'' AS Chandoan_Truocphauthuat,
                    N'' AS ICD_Chandoan_Truocphauthuat, N'' AS Phuongphapphauthuat, 0 AS Tongsongaydieutri_Sauphauthuat, 0 AS Tongsolanphauthuat,
                    N'' AS Ketquadieutri, N'' AS Giaiphaubenh, NULL AS Tinhhinhtuvong_Ngay, N'' AS Tinhhinhtuvong_phanloai, N'' AS Tinhhinhtuvong_Lydo,
                    CAST(0 AS BIT) AS Tinhhinhtuvong_Trong24giovaovien, CAST(0 AS BIT) AS Tinhhinhtuvong_Ngoai24giovaovien, CAST(0 AS BIT) AS Tinhhinhtuvong_Sau24giovaovien,
                    N'' AS Nguyennhanchinhtuvong, N'' AS ICD_Nguyennhanchinhtuvong, CAST(0 AS BIT) AS Khamnghiemtuthi, N'' AS ICD_Chandoangiaiphaututhi,
                    N'' AS Chandoangiaiphaututhi, CAST(0 AS BIT) AS Khivaokhoadieutri_TB_BC_Dogayme, N'' AS Khivaokhoadieutri_TB_BC_Dokhac,
                    CAST(0 AS BIT) AS Khivaokhoadieutri_TB_BC_Donhiemkhuan, CAST(0 AS BIT) AS Khivaokhoadieutri_TB_BC_Dophauthuat, CAST(0 AS BIT) AS Xuatvien,
                    mp.TenKhoaHIS AS Ten_Khoahienhanh, mp.MaKhoaHIS AS id_Khoahienhanh, N'' AS id_Giuongbenh, N'' AS Tengiuongbenh,
                    CAST(0 AS BIT) AS Dathanhtoanvienphi, 0 AS id_Toaxuatvien, 0 AS id_Taikham, N'' AS Ngayhentaikham, CAST(0 AS BIT) AS Hosophu,
                    N'' AS Id_Me, CAST(0 AS BIT) AS Cohosophu, N'' AS id_Khoaxuatvien, N'' AS Tenkhoaxuatvien, CAST(0 AS BIT) AS KhongsudungDV,
                    N'' AS dmdc_Id_Khoa, NULL AS Cannang, CAST(0 AS BIT) AS KhoaXuly, N'' AS id_NVKhoaXuly, CAST(0 AS BIT) AS Danhan_HSBA, NULL AS Thoigiannhan_HSBA
                FROM ArcusAirSql.dbo.patients AS p
                JOIN ArcusAirSql.dbo.organisations AS o ON p.orguid = o.id
                JOIN ArcusAirSql.dbo.referencevalues AS rv_gender ON p.genderuid = rv_gender.id
                JOIN ArcusAirSql.dbo.patients_address AS pa ON p.id = pa.patients_id
                JOIN ArcusAirSql.dbo.patients_contact AS pc ON p.id = pc.patients_id
                JOIN ArcusAirSql.dbo.patientvisits AS pv ON pv.patientuid = p.id
                JOIN ArcusAirSql.dbo.users AS u ON pv.admittedby = u.id
                LEFT JOIN ArcusAirSql.dbo.patientvisits_visitcareproviders AS prc ON prc.patientvisits_id = pv.id AND prc.isprimarycareprovider = 1
                LEFT JOIN ArcusAirSql.dbo.users AS prcu ON prc.careprovideruid = prcu.id
                JOIN PhuongChauDW.dbo.FactPatientVisits AS dwpv ON pv.visitid = dwpv.VisitId AND dwpv.OrganizationCode = o.code
                LEFT JOIN ArcusAirSql.dbo.__MappingDept AS mp ON dwpv.DepartmentName = mp.TenKhoaAA
                WHERE pv.visitid = @VisitId AND o.code = 'PC02';
            ";

            using var multi = await connection.QueryMultipleAsync(sql, new { VisitId = visitId });

            onProgress?.Invoke("[2/6] Đang lấy và xác thực thông tin Tiếp nhận...");
            var tiepNhanList = (await multi.ReadAsync<TiepNhanDto>()).ToList();
            if (tiepNhanList.Count == 0) throw new Exception($"Không tìm thấy dữ liệu Tiếp nhận cho VisitID '{visitId}'.");
            var tiepNhanData = tiepNhanList.First();

            onProgress?.Invoke("[3/6] Đang lấy và xác thực thông tin Phiếu chỉ định vào viện...");
            var phieuChiDinhList = (await multi.ReadAsync<PhieuChiDinhVaoVienDto>()).ToList();
            if (phieuChiDinhList.Count == 0) throw new Exception($"Không tìm thấy dữ liệu Phiếu chỉ định vào viện cho VisitID '{visitId}'.");
            var phieuChiDinhData = phieuChiDinhList.First();

            onProgress?.Invoke("[4/6] Đang lấy và xác thực thông tin Bệnh nhân tại khoa...");
            var bnTaiKhoaList = (await multi.ReadAsync<BenhNhanTaiKhoaDto>()).ToList();
            if (bnTaiKhoaList.Count == 0) throw new Exception($"Không tìm thấy dữ liệu Bệnh nhân tại khoa cho VisitID '{visitId}'.");
            var bnTaiKhoaData = bnTaiKhoaList.First();

            onProgress?.Invoke("[5/6] Đang lấy và xác thực thông tin Bệnh án ngoại trú...");
            var baNgoaiTruList = (await multi.ReadAsync<BenhAnNgoaiTruDto>()).ToList();
            var baNgoaiTruData = baNgoaiTruList.FirstOrDefault() ?? new BenhAnNgoaiTruDto();

            onProgress?.Invoke("[6/6] Đang lấy và xác thực thông tin Hồ sơ nội trú...");
            var hsNoiTruList = (await multi.ReadAsync<HoSoNoiTruDto>()).ToList();
            var hsNoiTruData = hsNoiTruList.FirstOrDefault() ?? new HoSoNoiTruDto();

            onProgress?.Invoke("✔ Hoàn tất quá trình lấy và xác thực dữ liệu từ HIS Cũ.");

            return new VisitMigrationData
            {
                TiepNhan = tiepNhanData,
                PhieuChiDinh = phieuChiDinhData,
                BenhNhanTaiKhoa = bnTaiKhoaData,
                BenhAnNgoaiTru = baNgoaiTruData,
                HoSoNoiTru = hsNoiTruData
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
                // BƯỚC 1: INSERT TIẾP NHẬN
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

                // BƯỚC 2: INSERT CÁC BẢN CON CỦA TIẾP NHẬN
                onProgress?.Invoke(">> Bắt đầu cập nhật các bảng liên quan: Địa chỉ, Lưu trú, Thuộc tính...");
                await connection.ExecuteAsync("INSERT INTO BV_Tiepnhan_Diachi (id_Tiepnhan) VALUES (@id_Tiepnhan);", new { id_Tiepnhan = newTiepNhanId }, transaction);
                await connection.ExecuteAsync("INSERT INTO BV_Tiepnhan_Luutru (id_Tiepnhan) VALUES (@id_Tiepnhan);", new { id_Tiepnhan = newTiepNhanId }, transaction);
                await connection.ExecuteAsync("INSERT INTO BV_Tiepnhan_Thuoctinh (id_Tiepnhan) VALUES (@id_Tiepnhan);", new { id_Tiepnhan = newTiepNhanId }, transaction);

                // BƯỚC 3: INSERT SỐ VÀO VIỆN
                onProgress?.Invoke(">> Đang cập nhật Sổ nhập viện [BV_Sonhapvien]...");
                string sqlInsertSoNhaoVien = @"
                    INSERT INTO BV_Sonhapvien (Maso, Namluutru, Sohientai, Trangthai, Ngaygiocap, id_Nhanvien, id_Khoaphong, id_Tiepnhan) 
                    VALUES (@Maso, 2026, @Sohientai, N'Sử_dụng', @Ngaygio, @Nhanvien, N'NT_CAPCUU', @id_Tiepnhan);";

                await connection.ExecuteAsync(sqlInsertSoNhaoVien, new
                {
                    Maso = data.TiepNhan.Sonhapvien,
                    Sohientai = Convert.ToInt32(data.TiepNhan.Sonhapvien),
                    Ngaygio = data.TiepNhan.Ngaygiotiepnhan,
                    Nhanvien = data.TiepNhan.id_Nhanvien,
                    id_Tiepnhan = newTiepNhanId
                }, transaction);

                // BƯỚC 4: INSERT PHIẾU CHỈ ĐỊNH VÀO VIỆN
                onProgress?.Invoke(">> Đang cập nhật Phiếu chỉ định vào viện [BV_Phieuchidinhvaovien]...");
                data.PhieuChiDinh.id_Tiepnhan = newTiepNhanId;

                if (data.PhieuChiDinh.Ngaygio is DateTime dt)
                    data.PhieuChiDinh.Ngaygiodaydu = $"Vào lúc {dt.Hour} giờ {dt.Minute} phút, ngày {dt:dd} tháng {dt:MM} năm {dt:yyyy}";

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
                    );
                    SELECT CAST(SCOPE_IDENTITY() as int);";

                int newPhieuId = await connection.QuerySingleAsync<int>(sqlInsertPhieu, data.PhieuChiDinh, transaction);

                // BƯỚC 5: INSERT BỆNH NHÂN TẠI KHOA
                onProgress?.Invoke(">> Đang cập nhật Bệnh nhân tại khoa [BV_Noitru_Benhnhantaikhoa]...");
                data.BenhNhanTaiKhoa.id_Tiepnhan = newTiepNhanId;
                data.BenhNhanTaiKhoa.id_Khoa = data.PhieuChiDinh.id_Khoadieutri;
                data.BenhNhanTaiKhoa.Tenkhoa = data.PhieuChiDinh.Chovaodieutritaikhoa;

                string sqlInsertTaiKhoa = @"
                    INSERT INTO BV_Noitru_Benhnhantaikhoa (
                        id_Khoa, Tenkhoa, Ngaygio, Sonhapvien, id_Benhnhan, Hoten, Ngaythangnamsinh, Namsinh, Tuoi, Gioitinh, Trangthai, 
                        id_Nhanvienget, Hoten_Nhanvienget, NamNhapvien, id_Tiepnhan, id_KhoaChuyenden, KhoaChuyenden, Ngaygio_Chuyen, Giochuyenmo, 
                        Xutri, Hientai, Bacsi_Truc, YeucauThuho
                    ) VALUES (
                        @id_Khoa, @Tenkhoa, @Ngaygio, @Sonhapvien, @id_Benhnhan, @Hoten, @Ngaythangnamsinh, @Namsinh, @Tuoi, @Gioitinh, @Trangthai, 
                        @id_Nhanvienget, @Hoten_Nhanvienget, @NamNhapvien, @id_Tiepnhan, @id_KhoaChuyenden, @KhoaChuyenden, @Ngaygio_Chuyen, @Giochuyenmo, 
                        @Xutri, @Hientai, @Bacsi_Truc, @YeucauThuho
                    )";
                await connection.ExecuteAsync(sqlInsertTaiKhoa, data.BenhNhanTaiKhoa, transaction);

                // BƯỚC 6: INSERT BỆNH ÁN NGOẠI TRÚ
                if (!string.IsNullOrEmpty(data.BenhAnNgoaiTru.id_Benhnhan))
                {
                    onProgress?.Invoke(">> Đang cập nhật Bệnh án Ngoại trú [BV_BenhAn_Ngoaitru]...");
                    data.BenhAnNgoaiTru.id_Tiepnhan = newTiepNhanId;

                    if (data.BenhAnNgoaiTru.Ngaygio is DateTime dtNgoaitru)
                        data.BenhAnNgoaiTru.Ngaygiokhamdaydu = $"Khám lúc {dtNgoaitru.Hour} giờ {dtNgoaitru.Minute} phút, ngày {dtNgoaitru:dd} tháng {dtNgoaitru:MM} năm {dtNgoaitru:yyyy}";

                    string sqlInsertBaNgoaitru = @"
                        INSERT INTO BV_BenhAn_Ngoaitru (
                            STT_Sokhambenh, id_Benhan_Phanloai_Khambenh, Namluutru, id_Hosonoitru, id_Benhnhan, id_Tiepnhan, id_DMDoituong, id_Bacsi, Hoten_Bacsi, 
                            Ngaygio, Ngaygiokhamdaydu, id_DuyetBHYT, id_DMPhongkham, Ten_Khoaphong, Quatrinhbenhly, Tiensubenh_Banthan, Tiensubenh_Giadinh, Cacbophan, 
                            Lydovaovien, KQ_Canlamsang, id_ICD_Chandoan, Chandoan, Denghi, Xutri, Ngaytaikham, Dientienbenh, Toanthan, MasoChungchi, dmdc_Id_Khoa, 
                            dmdc_Ten_Khoa, id_Lanmangthai, Chidinhdichvu, Chidinhdieutri, Toathuoc, Ketluan, id_Benhnhanphongkham, Dieutri_Tungay, Dieutri_Denngay, 
                            Phuongphap_Dieutri, Tinhtrang_Ravien, Loai_BenhAn, Phanloai_Capcuu, Phanloai_Ravien, Huongdieutri, QuenSo, Danhgia_tenga, Benh_phuctap, 
                            Danhgia_tutu, id_nguoilap, ten_nguoilap, duyetphieu, ngayduyet, id_BA_Phuctap, chiphi_taikham
                        ) VALUES (
                            @STT_Sokhambenh, @id_Benhan_Phanloai_Khambenh, @Namluutru, @id_Hosonoitru, @id_Benhnhan, @id_Tiepnhan, @id_DMDoituong, @id_Bacsi, @Hoten_Bacsi, 
                            @Ngaygio, @Ngaygiokhamdaydu, @id_DuyetBHYT, @id_DMPhongkham, @Ten_Khoaphong, @Quatrinhbenhly, @Tiensubenh_Banthan, @Tiensubenh_Giadinh, @Cacbophan, 
                            @Lydovaovien, @KQ_Canlamsang, @id_ICD_Chandoan, @Chandoan, @Denghi, @Xutri, @Ngaytaikham, @Dientienbenh, @Toanthan, @MasoChungchi, @dmdc_Id_Khoa, 
                            @dmdc_Ten_Khoa, @id_Lanmangthai, @Chidinhdichvu, @Chidinhdieutri, @Toathuoc, @Ketluan, @id_Benhnhanphongkham, @Dieutri_Tungay, @Dieutri_Denngay, 
                            @Phuongphap_Dieutri, @Tinhtrang_Ravien, @Loai_BenhAn, @Phanloai_Capcuu, @Phanloai_Ravien, @Huongdieutri, @QuenSo, @Danhgia_tenga, @Benh_phuctap, 
                            @Danhgia_tutu, @id_nguoilap, @ten_nguoilap, @duyetphieu, @ngayduyet, @id_BA_Phuctap, @chiphi_taikham
                        )";
                    await connection.ExecuteAsync(sqlInsertBaNgoaitru, data.BenhAnNgoaiTru, transaction);
                }

                // BƯỚC 7: INSERT HỒ SƠ NỘI TRÚ 
                // Sử dụng QuerySingleAsync để lấy ID sinh ra
                int newHoSoNoiTruId = 0;
                if (!string.IsNullOrEmpty(data.HoSoNoiTru.id_Benhnhan))
                {
                    onProgress?.Invoke(">> Đang cập nhật Hồ sơ nội trú [BV_Hosonoitru]...");
                    data.HoSoNoiTru.id_Tiepnhan = newTiepNhanId;
                    data.HoSoNoiTru.id_Phieuchidinhvaovien = newPhieuId;

                    if (data.HoSoNoiTru.Ngaygio is DateTime dtHsnt)
                        data.HoSoNoiTru.Ngaygiodaydu = $"Vào lúc {dtHsnt.Hour} giờ {dtHsnt.Minute} phút, ngày {dtHsnt:dd} tháng {dtHsnt:MM} năm {dtHsnt:yyyy}";

                    if (data.HoSoNoiTru.Vaovien_Ngaygiovaovien is DateTime dtVaovien)
                        data.HoSoNoiTru.Vaovien_Ngaygiovaoviendaydu = $"Vào lúc {dtVaovien.Hour} giờ {dtVaovien.Minute} phút, ngày {dtVaovien:dd} tháng {dtVaovien:MM} năm {dtVaovien:yyyy}";

                    if (data.HoSoNoiTru.Vaokhoa_Ngaygiovaokhoa is DateTime dtVaokhoa)
                        data.HoSoNoiTru.Vaokhoa_Ngaygiovaokhoadaydu = $"Vào lúc {dtVaokhoa.Hour} giờ {dtVaokhoa.Minute} phút, ngày {dtVaokhoa:dd} tháng {dtVaokhoa:MM} năm {dtVaokhoa:yyyy}";

                    string sqlInsertHoSoNoiTru = @"
                        INSERT INTO BV_Hosonoitru (
                            id_Phieuchidinhvaovien, Soluutru, Sonhapvien, Namluutru, Mayte, id_Tiepnhan, id_Benhan_Phanloai, Ten_PhanloaiBA, id_Benhnhan, Hoten, Ngaythangnamsinh, Namsinh, Tuoi, Gioitinh, Dienthoaididong, id_DMQuoctich, Quoctich, id_DMNghenghiep, Nghenghiep, id_DMDantoc, Dantoc, Tamtru_diachi, Tamtru_xaphuong, Tamtru_idXaphuong, Tamtru_quanhuyen, Tamtru_idQuanhuyen, Tamtru_tinhthanh, Tamtru_idTinhthanh, Noilamviec, id_DMDoituong, Doituong, id_DuyetBHYT, SotheBHYT, DuyetBHYT, NgayhethanBHYT, DiachiBHYT, Lienhe_hoten, Lienhe_diachi, Lienhe_sodienthoai, id_Bacsi_Lambenhan, Hoten_Bacsi_Lambenhan, id_Nhanvien, Hoten_Nhanvien, id_DMPhongkham, Ten_Phongkham, Ngaygio, Ngaygiodaydu, Vaovien_Ngaygiovaovien, Vaovien_Ngaygiovaoviendaydu, Tructiepvao, Noigioithieu, Vaoviendobenhnaylanthu, Vaokhoa, Vaokhoa_idKhoaphong, Vaokhoa_Ngaygiovaokhoa, Vaokhoa_Ngaygiovaokhoadaydu, Vaokhoa_Songaydieutri, Chuyenden, Chuyenvien, Ravien_Ngaygioravien, Ravien_Ngaygioraviendaydu, Ravien_Phanloai, Tongsongaydieutri, Chandoan_khivaokhoadieutri, ICD_Chandoan_khivaokhoadieutri, Chandoan_Lucvaokhoa_Lydo, Chandoan_Lucvaokhoa_Phanloai, ICD_Chandoannoichuyenden, Chandoannoichuyenden, ICD_ChandoanKKB_CC, ChandoanKKB_CC, Chandoan_Lucvaode, ICD_Chandoan_Lucvaode, Chandoan_phauthuatsausinh, Chandoan_thuthuatsausinh, Chandoan_taibien, Chandoan_bienchung, Chandoan_Ravien_Benhchinh, ICD_Chandoan_Ravien_Benhchinh, Chandoan_Ravien_Benhkemtheo, ICD_Chandoan_Ravien_Benhkemtheo, Ngayde_Mode, Ngoithai, Cachthucde, Cachthucde_Bienchung, Cachthucde_Kiemsoattucung, Cachthucde_Taibien, Cachthucde_TB_BC_Dogayme, Cachthucde_TB_BC_Dokhac, Cachthucde_TB_BC_Donhiemkhuan, Cachthucde_TB_BC_Dophauthuat, Tinhhinhphauthuat, Chandoan_Sauphauthuat, ICD_Chandoan_Sauphauthuat, Chandoan_Truocphauthuat, ICD_Chandoan_Truocphauthuat, Phuongphapphauthuat, Tongsongaydieutri_Sauphauthuat, Tongsolanphauthuat, Ketquadieutri, Giaiphaubenh, Tinhhinhtuvong_Ngay, Tinhhinhtuvong_phanloai, Tinhhinhtuvong_Lydo, Tinhhinhtuvong_Trong24giovaovien, Tinhhinhtuvong_Ngoai24giovaovien, Tinhhinhtuvong_Sau24giovaovien, Nguyennhanchinhtuvong, ICD_Nguyennhanchinhtuvong, Khamnghiemtuthi, ICD_Chandoangiaiphaututhi, Chandoangiaiphaututhi, Khivaokhoadieutri_TB_BC_Dogayme, Khivaokhoadieutri_TB_BC_Dokhac, Khivaokhoadieutri_TB_BC_Donhiemkhuan, Khivaokhoadieutri_TB_BC_Dophauthuat, Xuatvien, Ten_Khoahienhanh, id_Khoahienhanh, id_Giuongbenh, Tengiuongbenh, Dathanhtoanvienphi, id_Toaxuatvien, id_Taikham, Ngayhentaikham, Hosophu, Id_Me, Cohosophu, id_Khoaxuatvien, Tenkhoaxuatvien, KhongsudungDV, dmdc_Id_Khoa, Cannang, KhoaXuly, id_NVKhoaXuly, Danhan_HSBA, Thoigiannhan_HSBA
                        ) VALUES (
                            @id_Phieuchidinhvaovien, @Soluutru, @Sonhapvien, @Namluutru, @Mayte, @id_Tiepnhan, @id_Benhan_Phanloai, @Ten_PhanloaiBA, @id_Benhnhan, @Hoten, @Ngaythangnamsinh, @Namsinh, @Tuoi, @Gioitinh, @Dienthoaididong, @id_DMQuoctich, @Quoctich, @id_DMNghenghiep, @Nghenghiep, @id_DMDantoc, @Dantoc, @Tamtru_diachi, @Tamtru_xaphuong, @Tamtru_idXaphuong, @Tamtru_quanhuyen, @Tamtru_idQuanhuyen, @Tamtru_tinhthanh, @Tamtru_idTinhthanh, @Noilamviec, @id_DMDoituong, @Doituong, @id_DuyetBHYT, @SotheBHYT, @DuyetBHYT, @NgayhethanBHYT, @DiachiBHYT, @Lienhe_hoten, @Lienhe_diachi, @Lienhe_sodienthoai, @id_Bacsi_Lambenhan, @Hoten_Bacsi_Lambenhan, @id_Nhanvien, @Hoten_Nhanvien, @id_DMPhongkham, @Ten_Phongkham, @Ngaygio, @Ngaygiodaydu, @Vaovien_Ngaygiovaovien, @Vaovien_Ngaygiovaoviendaydu, @Tructiepvao, @Noigioithieu, @Vaoviendobenhnaylanthu, @Vaokhoa, @Vaokhoa_idKhoaphong, @Vaokhoa_Ngaygiovaokhoa, @Vaokhoa_Ngaygiovaokhoadaydu, @Vaokhoa_Songaydieutri, @Chuyenden, @Chuyenvien, @Ravien_Ngaygioravien, @Ravien_Ngaygioraviendaydu, @Ravien_Phanloai, @Tongsongaydieutri, @Chandoan_khivaokhoadieutri, @ICD_Chandoan_khivaokhoadieutri, @Chandoan_Lucvaokhoa_Lydo, @Chandoan_Lucvaokhoa_Phanloai, @ICD_Chandoannoichuyenden, @Chandoannoichuyenden, @ICD_ChandoanKKB_CC, @ChandoanKKB_CC, @Chandoan_Lucvaode, @ICD_Chandoan_Lucvaode, @Chandoan_phauthuatsausinh, @Chandoan_thuthuatsausinh, @Chandoan_taibien, @Chandoan_bienchung, @Chandoan_Ravien_Benhchinh, @ICD_Chandoan_Ravien_Benhchinh, @Chandoan_Ravien_Benhkemtheo, @ICD_Chandoan_Ravien_Benhkemtheo, @Ngayde_Mode, @Ngoithai, @Cachthucde, @Cachthucde_Bienchung, @Cachthucde_Kiemsoattucung, @Cachthucde_Taibien, @Cachthucde_TB_BC_Dogayme, @Cachthucde_TB_BC_Dokhac, @Cachthucde_TB_BC_Donhiemkhuan, @Cachthucde_TB_BC_Dophauthuat, @Tinhhinhphauthuat, @Chandoan_Sauphauthuat, @ICD_Chandoan_Sauphauthuat, @Chandoan_Truocphauthuat, @ICD_Chandoan_Truocphauthuat, @Phuongphapphauthuat, @Tongsongaydieutri_Sauphauthuat, @Tongsolanphauthuat, @Ketquadieutri, @Giaiphaubenh, @Tinhhinhtuvong_Ngay, @Tinhhinhtuvong_phanloai, @Tinhhinhtuvong_Lydo, @Tinhhinhtuvong_Trong24giovaovien, @Tinhhinhtuvong_Ngoai24giovaovien, @Tinhhinhtuvong_Sau24giovaovien, @Nguyennhanchinhtuvong, @ICD_Nguyennhanchinhtuvong, @Khamnghiemtuthi, @ICD_Chandoangiaiphaututhi, @Chandoangiaiphaututhi, @Khivaokhoadieutri_TB_BC_Dogayme, @Khivaokhoadieutri_TB_BC_Dokhac, @Khivaokhoadieutri_TB_BC_Donhiemkhuan, @Khivaokhoadieutri_TB_BC_Dophauthuat, @Xuatvien, @Ten_Khoahienhanh, @id_Khoahienhanh, @id_Giuongbenh, @Tengiuongbenh, @Dathanhtoanvienphi, @id_Toaxuatvien, @id_Taikham, @Ngayhentaikham, @Hosophu, @Id_Me, @Cohosophu, @id_Khoaxuatvien, @Tenkhoaxuatvien, @KhongsudungDV, @dmdc_Id_Khoa, @Cannang, @KhoaXuly, @id_NVKhoaXuly, @Danhan_HSBA, @Thoigiannhan_HSBA
                        );
                        SELECT CAST(SCOPE_IDENTITY() as int);";

                    newHoSoNoiTruId = await connection.QuerySingleAsync<int>(sqlInsertHoSoNoiTru, data.HoSoNoiTru, transaction);
                    onProgress?.Invoke($"   -> Tạo thành công ID Hồ sơ nội trú mới: {newHoSoNoiTruId}");
                }

                // =========================================================================
                // BƯỚC 8: CẬP NHẬT NGƯỢC ID_HOSONOITRU CHO TIẾP NHẬN & BỆNH ÁN NGOẠI TRÚ
                // =========================================================================
                if (newHoSoNoiTruId > 0)
                {
                    onProgress?.Invoke($">> Cập nhật ngược ID_Hosonoitru ({newHoSoNoiTruId}) cho bảng Tiếp nhận và Bệnh án ngoại trú...");

                    // 8.1 Cập nhật bảng BV_Tiepnhan
                    string sqlUpdateTiepNhan = "UPDATE BV_Tiepnhan SET id_Hosonoitru = @id_Hosonoitru WHERE id_Tiepnhan = @id_Tiepnhan;";
                    await connection.ExecuteAsync(sqlUpdateTiepNhan, new { id_Hosonoitru = newHoSoNoiTruId, id_Tiepnhan = newTiepNhanId }, transaction);

                    // 8.2 Cập nhật bảng BV_BenhAn_Ngoaitru (chỉ cập nhật nếu đã insert ở bước 6)
                    if (!string.IsNullOrEmpty(data.BenhAnNgoaiTru.id_Benhnhan))
                    {
                        string sqlUpdateBaNgoaiTru = "UPDATE BV_BenhAn_Ngoaitru SET id_Hosonoitru = @id_Hosonoitru WHERE id_Tiepnhan = @id_Tiepnhan;";
                        await connection.ExecuteAsync(sqlUpdateBaNgoaiTru, new { id_Hosonoitru = newHoSoNoiTruId, id_Tiepnhan = newTiepNhanId }, transaction);
                    }
                }

                onProgress?.Invoke("✔ Mọi bảng đã được xử lý. Đang hoàn tất lưu trữ (Commit)...");
                transaction.Commit();

                return new MigrationResult
                {
                    IsSuccess = true,
                    Message = $"Đồng bộ thành công! Lượt tiếp nhận: {newTiepNhanId} & Hồ sơ nội trú: {newHoSoNoiTruId}."
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