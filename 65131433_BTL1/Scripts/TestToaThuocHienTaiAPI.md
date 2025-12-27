# Hướng dẫn test API Toa thuốc hiện tại trên Postman

## Bước 1: Lấy JWT Token

### Request 1: Đăng nhập để lấy token
- **Method:** `POST`
- **URL:** `http://localhost:5237/api/auth/login`
- **Headers:**
  ```
  Content-Type: application/json
  ```
- **Body (JSON):**
  ```json
  {
    "tenDangNhap": "bn002",
    "matKhau": "123456"
  }
  ```
- **Response:** Copy `token` từ field `data.token`

## Bước 2: Test API Toa thuốc hiện tại

### Request 2: Lấy toa thuốc hiện tại
- **Method:** `GET`
- **URL:** `http://localhost:5237/api/benhan/toathuoc-hientai`
- **Headers:**
  ```
  Authorization: Bearer YOUR_TOKEN_HERE
  Content-Type: application/json
  ```
  (Thay `YOUR_TOKEN_HERE` bằng token từ Bước 1)

## Response mong đợi:

### Nếu thành công (có toa thuốc):
```json
{
  "success": true,
  "message": "Lấy toa thuốc hiện tại thành công",
  "data": {
    "maBenhAn": "BA001",
    "maKham": "KB001",
    "ngayKham": "2024-01-15",
    "ngayLuu": "2024-01-15 10:30:00",
    "bacSi": "Bác sĩ Nguyễn Văn C",
    "chuanDoan": "Cảm cúm thông thường",
    "toaThuoc": [
      {
        "maThuoc": "T001",
        "tenThuoc": "Paracetamol 500mg",
        "soLuong": 20,
        "lieuDung": "1 viên/lần, 3 lần/ngày",
        "cachDung": "Uống sau ăn",
        "soLanUongMoiNgay": 3
      }
    ]
  }
}
```

### Nếu chưa có toa thuốc:
```json
{
  "success": true,
  "message": "Chưa có toa thuốc nào",
  "data": null
}
```

### Nếu chưa liên kết với bệnh nhân:
```json
{
  "success": true,
  "message": "Tài khoản chưa liên kết với bệnh nhân",
  "data": null
}
```

## Lưu ý:
- Đảm bảo web đang chạy trên `http://localhost:5237`
- Token có thời hạn 30 ngày (theo cấu hình)
- Tài khoản phải đã liên kết với bệnh nhân (có MaBn)
- Bệnh nhân phải có ít nhất 1 bệnh án đã được lưu (trong bảng BenhAn)

