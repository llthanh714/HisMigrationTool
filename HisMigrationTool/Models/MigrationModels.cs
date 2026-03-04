namespace HisMigrationTool.Models
{
    // Class tổng (Aggregate Root) chứa toàn bộ dữ liệu của 1 đợt chuyển đổi
    public class VisitMigrationData
    {
        public TiepNhanDto TiepNhan { get; set; } = new();
        public PhieuChiDinhVaoVienDto PhieuChiDinh { get; set; } = new();
        public BenhNhanTaiKhoaDto BenhNhanTaiKhoa { get; set; } = new();
        public BenhAnNgoaiTruDto BenhAnNgoaiTru { get; set; } = new();
        public HoSoNoiTruDto HoSoNoiTru { get; set; } = new();
        public List<TreSoSinhDto> DanhSachTreSoSinh { get; set; } = new();
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

    public class BenhNhanTaiKhoaDto
    {
        public int id_Taikhoa { get; set; }
        public string? id_Khoa { get; set; }
        public string? Tenkhoa { get; set; }
        public DateTime? Ngaygio { get; set; }
        public string? Sonhapvien { get; set; }
        public string? id_Benhnhan { get; set; }
        public string? Hoten { get; set; }
        public string? Ngaythangnamsinh { get; set; }
        public string? Namsinh { get; set; }
        public string? Tuoi { get; set; }
        public string? Gioitinh { get; set; }
        public string? Trangthai { get; set; }
        public string? id_Nhanvienget { get; set; }
        public string? Hoten_Nhanvienget { get; set; }
        public string? NamNhapvien { get; set; }
        public int? id_Tiepnhan { get; set; }
        public string? id_KhoaChuyenden { get; set; }
        public string? KhoaChuyenden { get; set; }
        public DateTime? Ngaygio_Chuyen { get; set; }
        public string? Giochuyenmo { get; set; }
        public string? Xutri { get; set; }
        public string? Hientai { get; set; }
        public string? Bacsi_Truc { get; set; }
        public string? YeucauThuho { get; set; }
    }

    public class BenhAnNgoaiTruDto
    {
        public int id_Benhan_Ngoaitru { get; set; }
        public int? STT_Sokhambenh { get; set; }
        public string? id_Benhan_Phanloai_Khambenh { get; set; }
        public string? Namluutru { get; set; }
        public int? id_Hosonoitru { get; set; }
        public string? id_Benhnhan { get; set; }
        public int? id_Tiepnhan { get; set; }
        public string? id_DMDoituong { get; set; }
        public string? id_Bacsi { get; set; }
        public string? Hoten_Bacsi { get; set; }
        public DateTime? Ngaygio { get; set; }
        public string? Ngaygiokhamdaydu { get; set; }
        public int? id_DuyetBHYT { get; set; }
        public string? id_DMPhongkham { get; set; }
        public string? Ten_Khoaphong { get; set; }
        public string? Quatrinhbenhly { get; set; }
        public string? Tiensubenh_Banthan { get; set; }
        public string? Tiensubenh_Giadinh { get; set; }
        public string? Cacbophan { get; set; }
        public string? Lydovaovien { get; set; }
        public string? KQ_Canlamsang { get; set; }
        public string? id_ICD_Chandoan { get; set; }
        public string? Chandoan { get; set; }
        public string? Denghi { get; set; }
        public string? Xutri { get; set; }
        public string? Ngaytaikham { get; set; }
        public string? Dientienbenh { get; set; }
        public string? Toanthan { get; set; }
        public string? MasoChungchi { get; set; }
        public string? dmdc_Id_Khoa { get; set; }
        public string? dmdc_Ten_Khoa { get; set; }
        public int? id_Lanmangthai { get; set; }
        public string? Chidinhdichvu { get; set; }
        public string? Chidinhdieutri { get; set; }
        public string? Toathuoc { get; set; }
        public string? Ketluan { get; set; }
        public int? id_Benhnhanphongkham { get; set; }
        public DateTime? Dieutri_Tungay { get; set; }
        public DateTime? Dieutri_Denngay { get; set; }
        public string? Phuongphap_Dieutri { get; set; }
        public string? Tinhtrang_Ravien { get; set; }
        public int? Loai_BenhAn { get; set; }
        public string? Phanloai_Capcuu { get; set; }
        public string? Phanloai_Ravien { get; set; }
        public string? Huongdieutri { get; set; }
        public bool? QuenSo { get; set; }
        public string? Danhgia_tenga { get; set; }
        public bool? Benh_phuctap { get; set; }
        public string? Danhgia_tutu { get; set; }
        public string? id_nguoilap { get; set; }
        public string? ten_nguoilap { get; set; }
        public bool? duyetphieu { get; set; }
        public DateTime? ngayduyet { get; set; }
        public int? id_BA_Phuctap { get; set; }
        public string? chiphi_taikham { get; set; }
    }

    public class HoSoNoiTruDto
    {
        public int id_Hosonoitru { get; set; }
        public int? id_Phieuchidinhvaovien { get; set; }
        public string? Soluutru { get; set; }
        public string? Sonhapvien { get; set; }
        public string? Namluutru { get; set; }
        public string? Mayte { get; set; }
        public int? id_Tiepnhan { get; set; }
        public string? id_Benhan_Phanloai { get; set; }
        public string? Ten_PhanloaiBA { get; set; }
        public string? id_Benhnhan { get; set; }
        public string? Hoten { get; set; }
        public string? Ngaythangnamsinh { get; set; }
        public string? Namsinh { get; set; }
        public string? Tuoi { get; set; }
        public string? Gioitinh { get; set; }
        public string? Dienthoaididong { get; set; }
        public string? id_DMQuoctich { get; set; }
        public string? Quoctich { get; set; }
        public string? id_DMNghenghiep { get; set; }
        public string? Nghenghiep { get; set; }
        public string? id_DMDantoc { get; set; }
        public string? Dantoc { get; set; }
        public string? Tamtru_diachi { get; set; }
        public string? Tamtru_xaphuong { get; set; }
        public string? Tamtru_idXaphuong { get; set; }
        public string? Tamtru_quanhuyen { get; set; }
        public string? Tamtru_idQuanhuyen { get; set; }
        public string? Tamtru_tinhthanh { get; set; }
        public string? Tamtru_idTinhthanh { get; set; }
        public string? Noilamviec { get; set; }
        public string? id_DMDoituong { get; set; }
        public string? Doituong { get; set; }
        public int? id_DuyetBHYT { get; set; }
        public string? SotheBHYT { get; set; }
        public bool? DuyetBHYT { get; set; }
        public string? NgayhethanBHYT { get; set; }
        public string? DiachiBHYT { get; set; }
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
        public DateTime? Vaovien_Ngaygiovaovien { get; set; }
        public string? Vaovien_Ngaygiovaoviendaydu { get; set; }
        public string? Tructiepvao { get; set; }
        public string? Noigioithieu { get; set; }
        public int? Vaoviendobenhnaylanthu { get; set; }
        public string? Vaokhoa { get; set; }
        public string? Vaokhoa_idKhoaphong { get; set; }
        public DateTime? Vaokhoa_Ngaygiovaokhoa { get; set; }
        public string? Vaokhoa_Ngaygiovaokhoadaydu { get; set; }
        public int? Vaokhoa_Songaydieutri { get; set; }
        public string? Chuyenden { get; set; }
        public string? Chuyenvien { get; set; }
        public DateTime? Ravien_Ngaygioravien { get; set; }
        public string? Ravien_Ngaygioraviendaydu { get; set; }
        public string? Ravien_Phanloai { get; set; }
        public int? Tongsongaydieutri { get; set; }
        public string? Chandoan_khivaokhoadieutri { get; set; }
        public string? ICD_Chandoan_khivaokhoadieutri { get; set; }
        public string? Chandoan_Lucvaokhoa_Lydo { get; set; }
        public string? Chandoan_Lucvaokhoa_Phanloai { get; set; }
        public string? ICD_Chandoannoichuyenden { get; set; }
        public string? Chandoannoichuyenden { get; set; }
        public string? ICD_ChandoanKKB_CC { get; set; }
        public string? ChandoanKKB_CC { get; set; }
        public string? Chandoan_Lucvaode { get; set; }
        public string? ICD_Chandoan_Lucvaode { get; set; }
        public bool? Chandoan_phauthuatsausinh { get; set; }
        public bool? Chandoan_thuthuatsausinh { get; set; }
        public bool? Chandoan_taibien { get; set; }
        public bool? Chandoan_bienchung { get; set; }
        public string? Chandoan_Ravien_Benhchinh { get; set; }
        public string? ICD_Chandoan_Ravien_Benhchinh { get; set; }
        public string? Chandoan_Ravien_Benhkemtheo { get; set; }
        public string? ICD_Chandoan_Ravien_Benhkemtheo { get; set; }
        public DateTime? Ngayde_Mode { get; set; }
        public string? Ngoithai { get; set; }
        public string? Cachthucde { get; set; }
        public bool? Cachthucde_Bienchung { get; set; }
        public string? Cachthucde_Kiemsoattucung { get; set; }
        public bool? Cachthucde_Taibien { get; set; }
        public bool? Cachthucde_TB_BC_Dogayme { get; set; }
        public string? Cachthucde_TB_BC_Dokhac { get; set; }
        public bool? Cachthucde_TB_BC_Donhiemkhuan { get; set; }
        public bool? Cachthucde_TB_BC_Dophauthuat { get; set; }
        public string? Tinhhinhphauthuat { get; set; }
        public string? Chandoan_Sauphauthuat { get; set; }
        public string? ICD_Chandoan_Sauphauthuat { get; set; }
        public string? Chandoan_Truocphauthuat { get; set; }
        public string? ICD_Chandoan_Truocphauthuat { get; set; }
        public string? Phuongphapphauthuat { get; set; }
        public int? Tongsongaydieutri_Sauphauthuat { get; set; }
        public int? Tongsolanphauthuat { get; set; }
        public string? Ketquadieutri { get; set; }
        public string? Giaiphaubenh { get; set; }
        public DateTime? Tinhhinhtuvong_Ngay { get; set; }
        public string? Tinhhinhtuvong_phanloai { get; set; }
        public string? Tinhhinhtuvong_Lydo { get; set; }
        public bool? Tinhhinhtuvong_Trong24giovaovien { get; set; }
        public bool? Tinhhinhtuvong_Ngoai24giovaovien { get; set; }
        public bool? Tinhhinhtuvong_Sau24giovaovien { get; set; }
        public string? Nguyennhanchinhtuvong { get; set; }
        public string? ICD_Nguyennhanchinhtuvong { get; set; }
        public bool? Khamnghiemtuthi { get; set; }
        public string? ICD_Chandoangiaiphaututhi { get; set; }
        public string? Chandoangiaiphaututhi { get; set; }
        public bool? Khivaokhoadieutri_TB_BC_Dogayme { get; set; }
        public string? Khivaokhoadieutri_TB_BC_Dokhac { get; set; }
        public bool? Khivaokhoadieutri_TB_BC_Donhiemkhuan { get; set; }
        public bool? Khivaokhoadieutri_TB_BC_Dophauthuat { get; set; }
        public bool? Xuatvien { get; set; }
        public string? Ten_Khoahienhanh { get; set; }
        public string? id_Khoahienhanh { get; set; }
        public string? id_Giuongbenh { get; set; }
        public string? Tengiuongbenh { get; set; }
        public bool? Dathanhtoanvienphi { get; set; }
        public int? id_Toaxuatvien { get; set; }
        public int? id_Taikham { get; set; }
        public string? Ngayhentaikham { get; set; }
        public bool? Hosophu { get; set; }
        public string? Id_Me { get; set; }
        public bool? Cohosophu { get; set; }
        public string? id_Khoaxuatvien { get; set; }
        public string? Tenkhoaxuatvien { get; set; }
        public bool? KhongsudungDV { get; set; }
        public string? dmdc_Id_Khoa { get; set; }
        public double? Cannang { get; set; }
        public bool? KhoaXuly { get; set; }
        public string? id_NVKhoaXuly { get; set; }
        public bool? Danhan_HSBA { get; set; }
        public DateTime? Thoigiannhan_HSBA { get; set; }
    }

    public class TreSoSinhDto
    {
        public int id_Tresosinh { get; set; }
        public int? id_Hosonoitru_Me { get; set; }
        public string? Sonhapvien_Me { get; set; }
        public int? Stt_Tatcatre { get; set; }
        public string? Hotenbe { get; set; }
        public string? Gioitinh { get; set; }
        public string? Tinhtrang { get; set; }
        public string? Ditat { get; set; }
        public string? Cannang { get; set; }
        public string? Cao { get; set; }
        public string? Vongdau { get; set; }
        public string? Deluc_Gio { get; set; }
        public string? Deluc_Phut { get; set; }
        public string? Deluc_Ngay { get; set; }
        public bool? Cohaumon { get; set; }
        public string? Cuthetatbamsinh { get; set; }
        public string? Tinhtrangtresosinhsaukhide { get; set; }
        public string? Xulyvaketqua { get; set; }
        public string? Apgar1phut { get; set; }
        public string? Apgar5phut { get; set; }
        public string? Apgar10phut { get; set; }
        public bool? Vevoime { get; set; }
        public DateTime? Ngaygio { get; set; }
        public int? Magiaychungsinh { get; set; }
        public bool? Conhananhbe { get; set; }
        public string? id_Yta { get; set; }
        public string? Hoten_Yta { get; set; }
        public string? id_BSTruc { get; set; }
        public string? Hoten_BSTruc { get; set; }
        public string? id_Benhnhan_be { get; set; }
        public int? Quyenso { get; set; }
        public string? Hotenme_NND { get; set; }
        public string? Namsinh_Me { get; set; }
        public string? Diachithuongtru { get; set; }
        public string? SoCMND_Hochieu { get; set; }
        public string? Dantoc { get; set; }
        public int? Solansinh { get; set; }
        public int? Soconhiensong { get; set; }
        public int? Socontronglansinhnay { get; set; }
        public string? Dudinhdattenbe { get; set; }
        public string? Id_Nhanviendode { get; set; }
        public string? Hoten_Nhanviendode { get; set; }
        public string? Nam { get; set; }
        public string? Machucdanh { get; set; }
        public DateTime? Ngaycap_SoCMND_Hochieu { get; set; }
        public string? Noicap_SoCMND_Hochieu { get; set; }
        public string? Ghichu { get; set; }
        public string? id_Benhnhan_Me { get; set; }
        public string? CachSanh { get; set; }
        public int? id_TheodoiTuoithai { get; set; }
        public bool? Dagui_SMS { get; set; }
        public int? Thai_Tuan { get; set; }
        public int? Thai_Ngay { get; set; }
        public string? Hoten_Cha { get; set; }
        public string? MasoBHXH_Me { get; set; }
        public string? Thutruong_Donvi { get; set; }
        public int? id_Tiepnhan { get; set; }
        public DateTime? NgayGioIn { get; set; }
        public string? Quatrinhmangthai { get; set; }
        public string? Ma_The_Tam { get; set; }
        public string? ChandoanVV { get; set; }
        public string? LydoVV { get; set; }
        public bool? CaplaiGCS { get; set; }
        public string? TinhtrangSK { get; set; }
    }

    public class MigrationResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}