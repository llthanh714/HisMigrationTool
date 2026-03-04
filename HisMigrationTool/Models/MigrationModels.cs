namespace HisMigrationTool.Models
{
    // Class tổng (Aggregate Root) chứa toàn bộ dữ liệu của 1 đợt chuyển đổi
    public class VisitMigrationData
    {
        public TiepNhanDto TiepNhan { get; set; } = new();
        public PhieuChiDinhVaoVienDto PhieuChiDinh { get; set; } = new();
    }

    public class TiepNhanDto
    {
        public int id_Tiepnhan { get; set; }
        public string? id_DMDoituong { get; set; }
        public string? id_Nhanvien { get; set; }
        public string? id_Benhnhan { get; set; }
        public string? Hoten { get; set; }
        public string? Ten { get; set; }
        public string? Ngaythangnamsinh { get; set; }
        public int? Namsinh { get; set; }
        public string? Gioitinh { get; set; }
        public string? Diachidaydu { get; set; }
        public string? Phanloaikham { get; set; }
        public string? Trangthai { get; set; }
        public bool? Dathuphi { get; set; }
        public string? id_Quaytiepnhan { get; set; }
        public bool? Noitru { get; set; }
        public int? id_Hosonoitru { get; set; }
        public bool? Ngoaitru { get; set; }
        public string? Sonhapvien { get; set; }
        public DateTime? Ngaygiotiepnhan { get; set; }
        public int? id_Voucher { get; set; }
        public string? id_TKTamung { get; set; }
        public bool? Tamung { get; set; }
        public bool? DuyetBHYT { get; set; }
        public bool? Tiepnhantunoitru { get; set; }
        public string? Lydomienphi { get; set; }
        public bool? Thuephong { get; set; }
        public bool? BNThammy { get; set; }
        public int? id_benhnhanTK { get; set; }
        public int? id_SttBenhnhanBD { get; set; }
        public int? Songaydieutri_ngoaitru { get; set; }
        public int? Tuoi { get; set; }
        public string? Dienthoai { get; set; }
        public bool? KoKiemtraGiamdinh { get; set; }
        public int? id_DMGoi { get; set; }
        public string? id_ChiNhanh { get; set; }
        public DateTime? NgaygioThu { get; set; }
        public string? Mathe { get; set; }
        public bool? Yeucau_BS { get; set; }
        public bool? SudungBHBL { get; set; }
        public int? Khuvuc { get; set; }
        public bool? Xuatvien { get; set; }
        public string? XV_Trangthai { get; set; }
        public string? Diachidaydu_Noitru { get; set; }
        public string? id_KhoaPhongTN { get; set; }
        public bool? VIP { get; set; }
        public bool? DaCheckin { get; set; }
        public bool? Checkin_Noitru { get; set; }
    }

    // DTO cho bảng BV_Phieuchidinhvaovien
    public class PhieuChiDinhVaoVienDto
    {
        public int id_Phieuchidinhvaovien { get; set; }
        public string? Sonhapvien { get; set; }
        public string? Namluutru { get; set; }
        public int? id_Tiepnhan { get; set; }
        public string? id_Benhan_Phanloai { get; set; }
        public string? Ten_PhanloaiBA { get; set; }
        public string? id_Benhnhan { get; set; }
        public string? Lienhe_hoten { get; set; }
        public string? Lienhe_diachi { get; set; }
        public string? Lienhe_sodienthoai { get; set; }
        public string? id_Bacsi_Lambenhan { get; set; }
        public string? Hoten_Bacsi_Lambenhan { get; set; }
        public string? id_Nhanvien { get; set; }
        public string? Hoten_Nhanvien { get; set; }
        public string? id_DMPhongkham { get; set; }
        public string? Ten_Phongkham { get; set; }
        public DateTime? Ngaygio { get; set; }
        public string? Ngaygiodaydu { get; set; }
        public string? Quatrinhbenhly { get; set; }
        public string? Tiensubenh_Banthan { get; set; }
        public string? Tiensubenh_Giadinh { get; set; }
        public string? Toanthan { get; set; }
        public string? Cacbophan { get; set; }
        public string? KQ_Canlamsang { get; set; }
        public string? Daxuly { get; set; }
        public string? Chuy { get; set; }
        public string? Chovaodieutritaikhoa { get; set; }
        public string? id_Khoadieutri { get; set; }
        public string? ICD_Chandoannoichuyenden { get; set; }
        public string? Chandoannoichuyenden { get; set; }
        public string? ICD_ChandoanKKB_CC { get; set; }
        public string? ChandoanKKB_CC { get; set; }
        public string? Lydovaovien { get; set; }
        public string? Para { get; set; }
        public string? Tiensusanphukhoa { get; set; }
        public string? Khamchuyenkhoa { get; set; }
        public string? Lienhe_Namsinh { get; set; }
        public string? Lienhe_Quanhe { get; set; }
        public string? Tinhtrang_BHYT { get; set; }
        public bool? BN_KygiayBHYT { get; set; }
        public string? Danhgia_Tutu { get; set; }
    }

    public class MigrationResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}