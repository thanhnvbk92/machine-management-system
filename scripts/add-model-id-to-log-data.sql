-- Script thêm cột model_id vào bảng log_data
-- Thêm cột model_id liên kết với bảng models

USE hse_pm_db;

-- Thêm cột model_id vào bảng log_data
ALTER TABLE log_data 
ADD COLUMN model_id INT DEFAULT NULL AFTER station_id;

-- Thêm foreign key constraint
ALTER TABLE log_data 
ADD CONSTRAINT log_data_model_fk 
FOREIGN KEY (model_id) REFERENCES models(ID);

-- Thêm index cho performance
ALTER TABLE log_data 
ADD KEY idx_model_time (model_id, start_time);

-- Kiểm tra cấu trúc bảng sau khi thêm
DESCRIBE log_data;

SELECT 'Đã thêm cột model_id vào bảng log_data thành công!' as message;