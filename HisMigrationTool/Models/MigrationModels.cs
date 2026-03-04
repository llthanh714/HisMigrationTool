using System;

namespace HisMigrationTool.Models
{
    // Đối tượng map trực tiếp với các cột từ câu lệnh SELECT
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

    public class MigrationResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}