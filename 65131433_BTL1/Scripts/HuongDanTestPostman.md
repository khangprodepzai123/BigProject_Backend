# Hướng dẫn test API trên Postman - CHI TIẾT

## ⚠️ QUAN TRỌNG: Cách set Header Authorization trong Postman

### Cách 1: Sử dụng tab "Headers" (KHUYẾN NGHỊ)

1. Mở Postman
2. Tạo request mới: `GET http://localhost:5237/api/benhan/toathuoc-hientai`
3. Vào tab **"Headers"**
4. Thêm header mới:
   - **Key:** `Authorization`
   - **Value:** `Bearer YOUR_TOKEN_HERE`
   - ⚠️ **LƯU Ý:** Phải có khoảng trắng giữa "Bearer" và token!
   - ⚠️ **KHÔNG** có dấu ngoặc kép!

### Cách 2: Sử dụng tab "Authorization"

1. Vào tab **"Authorization"**
2. Chọn **Type:** `Bearer Token`
3. Paste token vào ô **Token** (KHÔNG cần gõ "Bearer", Postman tự thêm)

---

## Bước 1: Lấy JWT Token

### Request: Đăng nhập
- **Method:** `POST`
- **URL:** `http://localhost:5237/api/auth/login`
- **Tab Headers:**
  ```
  Content-Type: application/json
  ```
- **Tab Body:**
  - Chọn **raw** và **JSON**
  - Nhập:
  ```json
  {
    "tenDangNhap": "bn002",
    "matKhau": "123456"
  }
  ```
- **Response:** Copy toàn bộ token từ `data.token` (không có dấu ngoặc kép)

**Ví dụ response:**
```json
{
  "success": true,
  "message": "Đăng nhập thành công",
  "data": {
    "maTk": "TK002",
    "tenDangNhap": "bn002",
    "maBn": "BN002",
    "hoTen": "Trần Thị B",
    "diemTichLuy": 0,
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJtYVRrIjoiVEswMDIiLCJ0ZW5EYW5nT WhhcCI6ImJuMDAyIiwibmJmIjoxNzM1MzI4MDAwLCJleHAiOjE3MzU1ODcyMDAsImlzcyI6IlBob25nS2hhbU9ubGluZSIsImF1ZCI6IlBob25nS2hhbU9ubGluZVVzZXJzIn0.xxxxx"
  }
}
```

**Copy token:** `eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJtYVRrIjoiVEswMDIiLCJ0ZW5EYW5nT WhhcCI6ImJuMDAyIiwibmJmIjoxNzM1MzI4MDAwLCJleHAiOjE3MzU1ODcyMDAsImlzcyI6IlBob25nS2hhbU9ubGluZSIsImF1ZCI6IlBob25nS2hhbU9ubGluZVVzZXJzIn0.xxxxx`

---

## Bước 2: Test API Toa thuốc hiện tại

### Request: Lấy toa thuốc hiện tại
- **Method:** `GET`
- **URL:** `http://localhost:5237/api/benhan/toathuoc-hientai`
- **Tab Headers:**
  ```
  Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJtYVRrIjoiVEswMDIiLCJ0ZW5EYW5nT WhhcCI6ImJuMDAyIiwibmJmIjoxNzM1MzI4MDAwLCJleHAiOjE3MzU1ODcyMDAsImlzcyI6IlBob25nS2hhbU9ubGluZSIsImF1ZCI6IlBob25nS2hhbU9ubGluZVVzZXJzIn0.xxxxx
  ```
  ⚠️ **LƯU Ý:** 
  - Phải có khoảng trắng giữa "Bearer" và token
  - KHÔNG có dấu ngoặc kép
  - Token phải là token thật từ Bước 1

---

## Các lỗi thường gặp:

### ❌ Lỗi: "Token không hợp lệ"
**Nguyên nhân:**
1. Header Authorization không đúng format
2. Thiếu "Bearer " prefix
3. Có dấu ngoặc kép trong token
4. Token đã hết hạn

**Cách sửa:**
- Kiểm tra lại header: `Authorization: Bearer YOUR_TOKEN` (có khoảng trắng)
- Lấy token mới bằng cách đăng nhập lại

### ❌ Lỗi: "Token không hợp lệ hoặc hết hạn"
**Nguyên nhân:** Token đã hết hạn hoặc không đúng format

**Cách sửa:** Đăng nhập lại để lấy token mới

### ❌ Lỗi: "Tài khoản chưa liên kết với bệnh nhân"
**Nguyên nhân:** Tài khoản chưa có `MaBn`

**Cách sửa:** Chạy script `InsertTaiKhoanChoBN002.sql` để liên kết tài khoản với BN002

---

## Kiểm tra dữ liệu:

Chạy script SQL: `CheckToaThuocHienTai.sql` để kiểm tra:
- Tài khoản có tồn tại và đã liên kết chưa
- Có bệnh án chưa
- Bệnh án có toa thuốc chưa

